using Microsoft.EntityFrameworkCore;
using VPAPortal.Business.Services.Interfaces;
using VPAPortal.Data;
using VPAPortal.Data.Models;

namespace VPAPortal.Business.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly ApplicationDbContext _db;

        public WarehouseService(ApplicationDbContext db) => _db = db;

        // ════════════════════════════════════════════════════════════════════
        // РОТИ
        // ════════════════════════════════════════════════════════════════════

        public async Task<List<Company>> GetCompaniesAsync() =>
            await _db.Companies.OrderBy(c => c.SortOrder).ToListAsync();

        public async Task<Company> AddCompanyAsync(string name, int maxCurrentOrder)
        {
            var company = new Company { Name = name.Trim(), SortOrder = maxCurrentOrder + 1 };
            _db.Companies.Add(company);
            await _db.SaveChangesAsync();
            return company;
        }

        public async Task DeleteCompanyAsync(int companyId)
        {
            var company = await _db.Companies.FindAsync(companyId);
            if (company == null) return;

            var crewIds = await _db.Crews
                .Where(c => c.CompanyId == companyId)
                .Select(c => c.Id)
                .ToListAsync();

            if (crewIds.Any())
            {
                var flightIds = await _db.Flights
                    .Where(f => crewIds.Contains(f.CrewId))
                    .Select(f => f.Id)
                    .ToListAsync();

                if (flightIds.Any())
                {
                    _db.FlightDrops.RemoveRange(
                        _db.FlightDrops.Where(d => flightIds.Contains(d.FlightId)));
                    _db.Flights.RemoveRange(
                        _db.Flights.Where(f => flightIds.Contains(f.Id)));
                }

                _db.DroneItems.RemoveRange(_db.DroneItems.Where(d => crewIds.Contains(d.CrewId)));
                _db.AmmoItems.RemoveRange(_db.AmmoItems.Where(a => crewIds.Contains(a.CrewId)));
                _db.CrewLogs.RemoveRange(_db.CrewLogs.Where(l => crewIds.Contains(l.CrewId)));
                _db.PropertyItems.RemoveRange(_db.PropertyItems.Where(p => crewIds.Contains(p.CrewId)));
                _db.Crews.RemoveRange(_db.Crews.Where(c => c.CompanyId == companyId));
            }

            _db.CompanyPropertyItems.RemoveRange(_db.CompanyPropertyItems.Where(p => p.CompanyId == companyId));
            _db.PropertyInvoices.RemoveRange(_db.PropertyInvoices.Where(i => i.CompanyId == companyId));
            _db.PropertyLogs.RemoveRange(_db.PropertyLogs.Where(l => l.CompanyId == companyId));
            _db.WarehouseDroneItems.RemoveRange(_db.WarehouseDroneItems.Where(d => d.CompanyId == companyId));
            _db.WarehouseAmmoItems.RemoveRange(_db.WarehouseAmmoItems.Where(a => a.CompanyId == companyId));
            _db.WarehouseInvoices.RemoveRange(_db.WarehouseInvoices.Where(i => i.CompanyId == companyId));
            _db.WarehouseLogs.RemoveRange(_db.WarehouseLogs.Where(l => l.CompanyId == companyId));
            _db.Companies.Remove(company);

            await _db.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // БЕЗПІЛОТНИКИ СКЛАДУ
        // ════════════════════════════════════════════════════════════════════

        public async Task<List<WarehouseDroneItem>> GetDronesAsync(int companyId) =>
            await _db.WarehouseDroneItems
                .Where(d => d.CompanyId == companyId)
                .OrderBy(d => d.Name)
                .ToListAsync();

        public async Task AddDroneAsync(int companyId, string name, int qty, string changedBy)
        {
            var existing = await _db.WarehouseDroneItems
                .FirstOrDefaultAsync(d => d.CompanyId == companyId &&
                                          d.Name.ToLower() == name.Trim().ToLower());
            if (existing != null)
            {
                AddWarehouseLog(companyId, "Дрон", existing.Name,
                    existing.Quantity, existing.Quantity + qty, "Додано", changedBy);
                existing.Quantity += qty;
            }
            else
            {
                var item = new WarehouseDroneItem { CompanyId = companyId, Name = name.Trim(), Quantity = qty };
                _db.WarehouseDroneItems.Add(item);
                AddWarehouseLog(companyId, "Дрон", name.Trim(), 0, qty, "Додано", changedBy);
            }
            await _db.SaveChangesAsync();
        }

        public async Task SaveDroneAsync(int itemId, string name, int qty, string changedBy)
        {
            var item = await _db.WarehouseDroneItems.FindAsync(itemId);
            if (item == null) return;

            AddWarehouseLog(item.CompanyId, "Дрон", item.Name, item.Quantity, qty, "Змінено", changedBy);
            item.Name = name.Trim();
            item.Quantity = qty;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteDroneAsync(int itemId, string changedBy)
        {
            var item = await _db.WarehouseDroneItems.FindAsync(itemId);
            if (item == null) return;

            AddWarehouseLog(item.CompanyId, "Дрон", item.Name, item.Quantity, 0, "Видалено", changedBy);
            _db.WarehouseDroneItems.Remove(item);
            await _db.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // БОЄКОМПЛЕКТ СКЛАДУ
        // ════════════════════════════════════════════════════════════════════

        public async Task<List<WarehouseAmmoItem>> GetAmmosAsync(int companyId) =>
            await _db.WarehouseAmmoItems
                .Where(a => a.CompanyId == companyId)
                .OrderBy(a => a.Name)
                .ToListAsync();

        public async Task AddAmmoAsync(int companyId, string name, int qty, string changedBy)
        {
            var existing = await _db.WarehouseAmmoItems
                .FirstOrDefaultAsync(a => a.CompanyId == companyId &&
                                          a.Name.ToLower() == name.Trim().ToLower());
            if (existing != null)
            {
                AddWarehouseLog(companyId, "Боєкомплект", existing.Name,
                    existing.Quantity, existing.Quantity + qty, "Додано", changedBy);
                existing.Quantity += qty;
            }
            else
            {
                var item = new WarehouseAmmoItem { CompanyId = companyId, Name = name.Trim(), Quantity = qty };
                _db.WarehouseAmmoItems.Add(item);
                AddWarehouseLog(companyId, "Боєкомплект", name.Trim(), 0, qty, "Додано", changedBy);
            }
            await _db.SaveChangesAsync();
        }

        public async Task SaveAmmoAsync(int itemId, string name, int qty, string changedBy)
        {
            var item = await _db.WarehouseAmmoItems.FindAsync(itemId);
            if (item == null) return;

            AddWarehouseLog(item.CompanyId, "Боєкомплект", item.Name, item.Quantity, qty, "Змінено", changedBy);
            item.Name = name.Trim();
            item.Quantity = qty;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAmmoAsync(int itemId, string changedBy)
        {
            var item = await _db.WarehouseAmmoItems.FindAsync(itemId);
            if (item == null) return;

            AddWarehouseLog(item.CompanyId, "Боєкомплект", item.Name, item.Quantity, 0, "Видалено", changedBy);
            _db.WarehouseAmmoItems.Remove(item);
            await _db.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // НАКЛАДНІ
        // ════════════════════════════════════════════════════════════════════

        public async Task<List<WarehouseInvoice>> GetInvoicesAsync(int companyId) =>
            await _db.WarehouseInvoices
                .Where(i => i.CompanyId == companyId)
                .OrderByDescending(i => i.Date)
                .ToListAsync();

        public async Task AddInvoiceAsync(int companyId, string invoiceNumber,
            DateOnly date, string? photoPath, string createdBy)
        {
            _db.WarehouseInvoices.Add(new WarehouseInvoice
            {
                CompanyId = companyId,
                InvoiceNumber = invoiceNumber.Trim(),
                Date = date,
                PhotoPath = photoPath,
                CreatedBy = createdBy,
                CreatedAt = DateTime.Now
            });
            await _db.SaveChangesAsync();
        }

        public async Task DeleteInvoiceAsync(int invoiceId, string webRootPath)
        {
            var inv = await _db.WarehouseInvoices.FindAsync(invoiceId);
            if (inv == null) return;

            DeleteFileIfExists(webRootPath, inv.PhotoPath);
            _db.WarehouseInvoices.Remove(inv);
            await _db.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // ЖУРНАЛ
        // ════════════════════════════════════════════════════════════════════

        public async Task<List<WarehouseLog>> GetLogsAsync(int companyId) =>
            await _db.WarehouseLogs
                .Where(l => l.CompanyId == companyId)
                .OrderByDescending(l => l.ChangedAt)
                .ToListAsync();

        // ════════════════════════════════════════════════════════════════════
        // ПРИВАТНІ ДОПОМІЖНІ МЕТОДИ
        // ════════════════════════════════════════════════════════════════════

        /// <summary>Додає запис у журнал складу (без SaveChanges — зберігається разом з основною операцією).</summary>
        private void AddWarehouseLog(int companyId, string itemType, string itemName,
            int qtyBefore, int qtyAfter, string action, string changedBy)
        {
            _db.WarehouseLogs.Add(new WarehouseLog
            {
                CompanyId = companyId,
                ItemType = itemType,
                ItemName = itemName,
                QuantityBefore = qtyBefore,
                QuantityAfter = qtyAfter,
                Action = action,
                ChangedBy = changedBy,
                ChangedAt = DateTime.Now
            });
        }

        private static void DeleteFileIfExists(string webRootPath, string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;
            var fullPath = Path.Combine(webRootPath,
                relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
