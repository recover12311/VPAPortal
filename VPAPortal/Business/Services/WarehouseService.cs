using Microsoft.EntityFrameworkCore;
using VPAPortal.Business.Services.Interfaces;
using VPAPortal.Data;
using VPAPortal.Data.Models;

namespace VPAPortal.Business.Services
{
    /// <summary>
    /// Реалізація сервісу складу.
    /// Всі звернення до БД відбуваються тут — сторінки не знають про DbContext.
    /// </summary>
    public class WarehouseService : IWarehouseService
    {
        private readonly ApplicationDbContext _db;

        public WarehouseService(ApplicationDbContext db)
        {
            _db = db;
        }

        // ════════════════════════════════════════════════════════════════════
        // РОТИ
        // ════════════════════════════════════════════════════════════════════

        /// <inheritdoc />
        public async Task<List<Company>> GetCompaniesAsync()
        {
            return await _db.Companies
                .OrderBy(c => c.SortOrder)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Company> AddCompanyAsync(string name, int maxCurrentOrder)
        {
            var company = new Company
            {
                Name = name.Trim(),
                SortOrder = maxCurrentOrder + 1
            };

            _db.Companies.Add(company);
            await _db.SaveChangesAsync();

            return company;
        }

        /// <inheritdoc />
        public async Task DeleteCompanyAsync(int companyId)
        {
            var company = await _db.Companies.FindAsync(companyId);
            if (company == null) return;

            // Каскадне видалення всіх даних складу цієї роти
            _db.WarehouseDroneItems.RemoveRange(
                _db.WarehouseDroneItems.Where(d => d.CompanyId == companyId));

            _db.WarehouseAmmoItems.RemoveRange(
                _db.WarehouseAmmoItems.Where(a => a.CompanyId == companyId));

            _db.WarehouseInvoices.RemoveRange(
                _db.WarehouseInvoices.Where(i => i.CompanyId == companyId));

            _db.WarehouseLogs.RemoveRange(
                _db.WarehouseLogs.Where(l => l.CompanyId == companyId));

            _db.Companies.Remove(company);

            await _db.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // БЕЗПІЛОТНИКИ СКЛАДУ
        // ════════════════════════════════════════════════════════════════════

        /// <inheritdoc />
        public async Task<List<WarehouseDroneItem>> GetDronesAsync(int companyId)
        {
            return await _db.WarehouseDroneItems
                .Where(d => d.CompanyId == companyId)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task AddDroneAsync(int companyId, string name, int qty, string changedBy)
        {
            var existing = await _db.WarehouseDroneItems
                .FirstOrDefaultAsync(d =>
                    d.CompanyId == companyId &&
                    d.Name.ToLower() == name.Trim().ToLower());

            if (existing != null)
            {
                // Збільшуємо кількість існуючого запису
                await AddLogAsync(companyId, "Дрон", existing.Name,
                    existing.Quantity, existing.Quantity + qty, "Додано", changedBy);

                existing.Quantity += qty;
            }
            else
            {
                // Створюємо новий запис
                var item = new WarehouseDroneItem
                {
                    CompanyId = companyId,
                    Name = name.Trim(),
                    Quantity = qty
                };

                _db.WarehouseDroneItems.Add(item);
                await _db.SaveChangesAsync();

                await AddLogAsync(companyId, "Дрон", item.Name,
                    0, qty, "Додано", changedBy);
            }

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task SaveDroneAsync(int itemId, string name, int qty, string changedBy)
        {
            var item = await _db.WarehouseDroneItems.FindAsync(itemId);
            if (item == null) return;

            await AddLogAsync(item.CompanyId, "Дрон", item.Name,
                item.Quantity, qty, "Змінено", changedBy);

            item.Name = name.Trim();
            item.Quantity = qty;

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DeleteDroneAsync(int itemId, string changedBy)
        {
            var item = await _db.WarehouseDroneItems.FindAsync(itemId);
            if (item == null) return;

            await AddLogAsync(item.CompanyId, "Дрон", item.Name,
                item.Quantity, 0, "Видалено", changedBy);

            _db.WarehouseDroneItems.Remove(item);

            await _db.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // БОЄКОМПЛЕКТ СКЛАДУ
        // ════════════════════════════════════════════════════════════════════

        /// <inheritdoc />
        public async Task<List<WarehouseAmmoItem>> GetAmmosAsync(int companyId)
        {
            return await _db.WarehouseAmmoItems
                .Where(a => a.CompanyId == companyId)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task AddAmmoAsync(int companyId, string name, int qty, string changedBy)
        {
            var existing = await _db.WarehouseAmmoItems
                .FirstOrDefaultAsync(a =>
                    a.CompanyId == companyId &&
                    a.Name.ToLower() == name.Trim().ToLower());

            if (existing != null)
            {
                await AddLogAsync(companyId, "Боєкомплект", existing.Name,
                    existing.Quantity, existing.Quantity + qty, "Додано", changedBy);

                existing.Quantity += qty;
            }
            else
            {
                var item = new WarehouseAmmoItem
                {
                    CompanyId = companyId,
                    Name = name.Trim(),
                    Quantity = qty
                };

                _db.WarehouseAmmoItems.Add(item);
                await _db.SaveChangesAsync();

                await AddLogAsync(companyId, "Боєкомплект", item.Name,
                    0, qty, "Додано", changedBy);
            }

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task SaveAmmoAsync(int itemId, string name, int qty, string changedBy)
        {
            var item = await _db.WarehouseAmmoItems.FindAsync(itemId);
            if (item == null) return;

            await AddLogAsync(item.CompanyId, "Боєкомплект", item.Name,
                item.Quantity, qty, "Змінено", changedBy);

            item.Name = name.Trim();
            item.Quantity = qty;

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DeleteAmmoAsync(int itemId, string changedBy)
        {
            var item = await _db.WarehouseAmmoItems.FindAsync(itemId);
            if (item == null) return;

            await AddLogAsync(item.CompanyId, "Боєкомплект", item.Name,
                item.Quantity, 0, "Видалено", changedBy);

            _db.WarehouseAmmoItems.Remove(item);

            await _db.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // НАКЛАДНІ
        // ════════════════════════════════════════════════════════════════════

        /// <inheritdoc />
        public async Task<List<WarehouseInvoice>> GetInvoicesAsync(int companyId)
        {
            return await _db.WarehouseInvoices
                .Where(i => i.CompanyId == companyId)
                .OrderByDescending(i => i.Date)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task AddInvoiceAsync(
            int companyId,
            string invoiceNumber,
            DateOnly date,
            string? photoPath,
            string createdBy)
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

        /// <inheritdoc />
        public async Task DeleteInvoiceAsync(int invoiceId, string webRootPath)
        {
            var inv = await _db.WarehouseInvoices.FindAsync(invoiceId);
            if (inv == null) return;

            // Видаляємо фізичний файл, якщо він є
            if (!string.IsNullOrEmpty(inv.PhotoPath))
            {
                var filePath = Path.Combine(
                    webRootPath,
                    inv.PhotoPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            _db.WarehouseInvoices.Remove(inv);
            await _db.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // ЖУРНАЛ
        // ════════════════════════════════════════════════════════════════════

        /// <inheritdoc />
        public async Task<List<WarehouseLog>> GetLogsAsync(int companyId)
        {
            return await _db.WarehouseLogs
                .Where(l => l.CompanyId == companyId)
                .OrderByDescending(l => l.ChangedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Внутрішній метод для запису в журнал складу.
        /// Викликається з інших методів сервісу перед збереженням змін.
        /// </summary>
        private async Task AddLogAsync(
            int companyId,
            string itemType,
            string itemName,
            int qtyBefore,
            int qtyAfter,
            string action,
            string changedBy)
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

            await _db.SaveChangesAsync();
        }
    }
}
