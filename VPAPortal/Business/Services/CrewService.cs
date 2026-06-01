using Microsoft.EntityFrameworkCore;
using VPAPortal.Business.Services.Interfaces;
using VPAPortal.Common;
using VPAPortal.Data.Models;

namespace VPAPortal.Data.Services
{
    public class CrewService : ICrewService
    {
        private readonly ApplicationDbContext _db;

        public CrewService(ApplicationDbContext db) => _db = db;

        // ════════════════════════════════════════════════════════════════════
        // ЕКІПАЖІ
        // ════════════════════════════════════════════════════════════════════

        public async Task<List<Crew>> GetCrewsAsync(int companyId) =>
            await _db.Crews
                .Where(c => c.CompanyId == companyId)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();

        public async Task<Crew> AddCrewAsync(int companyId, string name, CrewType type, int maxCurrentOrder)
        {
            var crew = new Crew
            {
                CompanyId = companyId,
                Name = name.Trim(),
                Type = type,
                SortOrder = maxCurrentOrder + 1
            };
            _db.Crews.Add(crew);
            await _db.SaveChangesAsync();
            return crew;
        }

        public async Task DeleteCrewAsync(int crewId)
        {
            var crew = await _db.Crews.FindAsync(crewId);
            if (crew == null) return;

            var flightIds = await _db.Flights
                .Where(f => f.CrewId == crewId)
                .Select(f => f.Id)
                .ToListAsync();

            if (flightIds.Any())
            {
                _db.FlightDrops.RemoveRange(_db.FlightDrops.Where(d => flightIds.Contains(d.FlightId)));
                _db.Flights.RemoveRange(_db.Flights.Where(f => flightIds.Contains(f.Id)));
            }

            _db.DroneItems.RemoveRange(_db.DroneItems.Where(d => d.CrewId == crewId));
            _db.AmmoItems.RemoveRange(_db.AmmoItems.Where(a => a.CrewId == crewId));
            _db.CrewLogs.RemoveRange(_db.CrewLogs.Where(l => l.CrewId == crewId));
            _db.PropertyItems.RemoveRange(_db.PropertyItems.Where(p => p.CrewId == crewId));
            _db.Crews.Remove(crew);

            await _db.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // БЕЗПІЛОТНИКИ ЕКІПАЖУ
        // ════════════════════════════════════════════════════════════════════

        public async Task<List<DroneItem>> GetDroneItemsAsync(int crewId) =>
            await _db.DroneItems
                .Where(d => d.CrewId == crewId)
                .OrderBy(d => d.Name)
                .ToListAsync();

        public async Task AddDroneItemAsync(int crewId, string name, int qty,
            bool isBomber, bool isWing, bool isWingAttack, string changedBy)
        {
            var existing = await _db.DroneItems
                .FirstOrDefaultAsync(d => d.CrewId == crewId &&
                                          d.IsBomber == isBomber &&
                                          d.IsWing == isWing &&
                                          d.IsWingAttack == isWingAttack &&
                                          d.Name.ToLower() == name.Trim().ToLower());
            if (existing != null)
            {
                AddCrewLog(crewId, "Дрон", existing.Name,
                    existing.Quantity, existing.Quantity + qty, "Додано", changedBy);
                existing.Quantity += qty;
            }
            else
            {
                _db.DroneItems.Add(new DroneItem
                {
                    CrewId = crewId,
                    Name = name.Trim(),
                    Quantity = qty,
                    IsBomber = isBomber,
                    IsWing = isWing,
                    IsWingAttack = isWingAttack
                });
                AddCrewLog(crewId, "Дрон", name.Trim(), 0, qty, "Додано", changedBy);
            }
            await _db.SaveChangesAsync();
        }

        public async Task SaveDroneItemAsync(int itemId, string name, int qty, string changedBy)
        {
            var item = await _db.DroneItems.FindAsync(itemId);
            if (item == null) return;

            AddCrewLog(item.CrewId, "Дрон", item.Name, item.Quantity, qty, "Змінено", changedBy);
            item.Name = name.Trim();
            item.Quantity = qty;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteDroneItemAsync(int itemId, string changedBy)
        {
            var item = await _db.DroneItems.FindAsync(itemId);
            if (item == null) return;

            AddCrewLog(item.CrewId, "Дрон", item.Name, item.Quantity, 0, "Видалено", changedBy);
            _db.DroneItems.Remove(item);
            await _db.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // БОЄКОМПЛЕКТ ЕКІПАЖУ
        // ════════════════════════════════════════════════════════════════════

        public async Task<List<AmmoItem>> GetAmmoItemsAsync(int crewId) =>
            await _db.AmmoItems
                .Where(a => a.CrewId == crewId)
                .OrderBy(a => a.Name)
                .ToListAsync();

        public async Task AddAmmoItemAsync(int crewId, string name, int qty, string changedBy)
        {
            var existing = await _db.AmmoItems
                .FirstOrDefaultAsync(a => a.CrewId == crewId &&
                                          a.Name.ToLower() == name.Trim().ToLower());
            if (existing != null)
            {
                AddCrewLog(crewId, "Боєкомплект", existing.Name,
                    existing.Quantity, existing.Quantity + qty, "Додано", changedBy);
                existing.Quantity += qty;
            }
            else
            {
                _db.AmmoItems.Add(new AmmoItem { CrewId = crewId, Name = name.Trim(), Quantity = qty });
                AddCrewLog(crewId, "Боєкомплект", name.Trim(), 0, qty, "Додано", changedBy);
            }
            await _db.SaveChangesAsync();
        }

        public async Task SaveAmmoItemAsync(int itemId, string name, int qty, string changedBy)
        {
            var item = await _db.AmmoItems.FindAsync(itemId);
            if (item == null) return;

            AddCrewLog(item.CrewId, "Боєкомплект", item.Name, item.Quantity, qty, "Змінено", changedBy);
            item.Name = name.Trim();
            item.Quantity = qty;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAmmoItemAsync(int itemId, string changedBy)
        {
            var item = await _db.AmmoItems.FindAsync(itemId);
            if (item == null) return;

            AddCrewLog(item.CrewId, "Боєкомплект", item.Name, item.Quantity, 0, "Видалено", changedBy);
            _db.AmmoItems.Remove(item);
            await _db.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // ПЕРЕДАЧА ЗІ СКЛАДУ
        // ════════════════════════════════════════════════════════════════════

        public async Task<List<WarehouseDroneItem>> GetWarehouseDronesAsync(int companyId) =>
            await _db.WarehouseDroneItems
                .Where(d => d.CompanyId == companyId && d.Quantity > 0)
                .OrderBy(d => d.Name)
                .ToListAsync();

        public async Task<List<WarehouseAmmoItem>> GetWarehouseAmmosAsync(int companyId) =>
            await _db.WarehouseAmmoItems
                .Where(a => a.CompanyId == companyId && a.Quantity > 0)
                .OrderBy(a => a.Name)
                .ToListAsync();

        public async Task<string> TransferDroneFromWarehouseAsync(int companyId, int crewId,
            int warehouseItemId, int qty, string transferType, string changedBy)
        {
            var wItem = await _db.WarehouseDroneItems.FindAsync(warehouseItemId);
            if (wItem == null || wItem.Quantity < qty)
                return "Недостатньо на складі.";

            bool isBomber = transferType == "bomber";
            bool isWing = transferType == "wing";
            bool isWingAttack = transferType == "wing-attack";

            var crewItem = await _db.DroneItems
                .FirstOrDefaultAsync(d => d.CrewId == crewId &&
                                          d.IsBomber == isBomber &&
                                          d.IsWing == isWing &&
                                          d.IsWingAttack == isWingAttack &&
                                          d.Name == wItem.Name);
            if (crewItem != null)
            {
                AddCrewLog(crewId, "Дрон", crewItem.Name,
                    crewItem.Quantity, crewItem.Quantity + qty, "Поповнено зі складу", changedBy);
                crewItem.Quantity += qty;
            }
            else
            {
                _db.DroneItems.Add(new DroneItem
                {
                    CrewId = crewId,
                    Name = wItem.Name,
                    Quantity = qty,
                    IsBomber = isBomber,
                    IsWing = isWing,
                    IsWingAttack = isWingAttack
                });
                AddCrewLog(crewId, "Дрон", wItem.Name, 0, qty, "Поповнено зі складу", changedBy);
            }

            wItem.Quantity -= qty;
            await _db.SaveChangesAsync();
            return $"Передано {qty} шт. '{wItem.Name}' екіпажу.";
        }

        public async Task<string> TransferAmmoFromWarehouseAsync(int companyId, int crewId,
            int warehouseItemId, int qty, string changedBy)
        {
            var wItem = await _db.WarehouseAmmoItems.FindAsync(warehouseItemId);
            if (wItem == null || wItem.Quantity < qty)
                return "Недостатньо на складі.";

            var crewItem = await _db.AmmoItems
                .FirstOrDefaultAsync(a => a.CrewId == crewId && a.Name == wItem.Name);
            if (crewItem != null)
            {
                AddCrewLog(crewId, "Боєкомплект", crewItem.Name,
                    crewItem.Quantity, crewItem.Quantity + qty, "Поповнено зі складу", changedBy);
                crewItem.Quantity += qty;
            }
            else
            {
                _db.AmmoItems.Add(new AmmoItem { CrewId = crewId, Name = wItem.Name, Quantity = qty });
                AddCrewLog(crewId, "Боєкомплект", wItem.Name, 0, qty, "Поповнено зі складу", changedBy);
            }

            wItem.Quantity -= qty;
            await _db.SaveChangesAsync();
            return $"Передано {qty} шт. '{wItem.Name}' екіпажу.";
        }

        // ════════════════════════════════════════════════════════════════════
        // ЖУРНАЛ
        // ════════════════════════════════════════════════════════════════════

        public async Task<List<CrewLog>> GetLogsAsync(int crewId) =>
            await _db.CrewLogs
                .Where(l => l.CrewId == crewId)
                .OrderByDescending(l => l.ChangedAt)
                .ToListAsync();

        // ════════════════════════════════════════════════════════════════════
        // ПРИВАТНІ ДОПОМІЖНІ МЕТОДИ
        // ════════════════════════════════════════════════════════════════════

        /// <summary>Додає запис у журнал екіпажу (без SaveChanges — зберігається разом з основною операцією).</summary>
        private void AddCrewLog(int crewId, string itemType, string itemName,
            int qtyBefore, int qtyAfter, string action, string changedBy)
        {
            _db.CrewLogs.Add(new CrewLog
            {
                CrewId = crewId,
                ItemType = itemType,
                ItemName = itemName,
                QuantityBefore = qtyBefore,
                QuantityAfter = qtyAfter,
                Action = action,
                ChangedBy = changedBy,
                ChangedAt = DateTime.Now
            });
        }
    }
}
