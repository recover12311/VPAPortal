using Microsoft.EntityFrameworkCore;
using VPAPortal.Business.Services.Interfaces;
using VPAPortal.Common;
using VPAPortal.Data.Models;

namespace VPAPortal.Data.Services
{
    /// <summary>
    /// Реалізація сервісу екіпажів.
    /// Вся логіка роботи з БД зосереджена тут.
    /// </summary>
    public class CrewService : ICrewService
    {
        private readonly ApplicationDbContext _db;

        public CrewService(ApplicationDbContext db)
        {
            _db = db;
        }

        // ════════════════════════════════════════════════════════════════════
        // ЕКІПАЖІ
        // ════════════════════════════════════════════════════════════════════

        /// <inheritdoc />
        public async Task<List<Crew>> GetCrewsAsync(int companyId)
        {
            return await _db.Crews
                .Where(c => c.CompanyId == companyId)
                .OrderBy(c => c.SortOrder)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Crew> AddCrewAsync(
            int companyId,
            string name,
            CrewType type,
            int maxCurrentOrder)
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

        /// <inheritdoc />
        public async Task DeleteCrewAsync(int crewId)
        {
            var crew = await _db.Crews.FindAsync(crewId);
            if (crew == null) return;

            // Видаляємо всі пов'язані дані екіпажу
            _db.DroneItems.RemoveRange(
                _db.DroneItems.Where(d => d.CrewId == crewId));

            _db.AmmoItems.RemoveRange(
                _db.AmmoItems.Where(a => a.CrewId == crewId));

            _db.CrewLogs.RemoveRange(
                _db.CrewLogs.Where(l => l.CrewId == crewId));

            // Видаляємо вильоти з їхніми скидами
            var flights = await _db.Flights
                .Include(f => f.Drops)
                .Where(f => f.CrewId == crewId)
                .ToListAsync();

            foreach (var flight in flights)
            {
                _db.FlightDrops.RemoveRange(flight.Drops);
            }

            _db.Flights.RemoveRange(flights);
            _db.Crews.Remove(crew);

            await _db.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // БЕЗПІЛОТНИКИ ЕКІПАЖУ
        // ════════════════════════════════════════════════════════════════════

        /// <inheritdoc />
        public async Task<List<DroneItem>> GetDroneItemsAsync(int crewId)
        {
            return await _db.DroneItems
                .Where(d => d.CrewId == crewId)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task AddDroneItemAsync(
            int crewId,
            string name,
            int qty,
            bool isBomber,
            bool isWing,
            string changedBy)
        {
            // Шукаємо існуючий запис з таким же іменем та типом
            var existing = await _db.DroneItems
                .FirstOrDefaultAsync(d =>
                    d.CrewId == crewId &&
                    d.IsBomber == isBomber &&
                    d.IsWing == isWing &&
                    d.Name.ToLower() == name.Trim().ToLower());

            if (existing != null)
            {
                // Збільшуємо кількість існуючого запису
                await AddLogAsync(crewId, "Дрон", existing.Name,
                    existing.Quantity, existing.Quantity + qty, "Додано", changedBy);

                existing.Quantity += qty;
            }
            else
            {
                // Створюємо новий запис
                var item = new DroneItem
                {
                    CrewId = crewId,
                    Name = name.Trim(),
                    Quantity = qty,
                    IsBomber = isBomber,
                    IsWing = isWing
                };

                _db.DroneItems.Add(item);
                await _db.SaveChangesAsync();

                await AddLogAsync(crewId, "Дрон", item.Name,
                    0, qty, "Додано", changedBy);
            }

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task SaveDroneItemAsync(
            int itemId,
            string name,
            int qty,
            string changedBy)
        {
            var item = await _db.DroneItems.FindAsync(itemId);
            if (item == null) return;

            await AddLogAsync(item.CrewId, "Дрон", item.Name,
                item.Quantity, qty, "Змінено", changedBy);

            item.Name = name.Trim();
            item.Quantity = qty;

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DeleteDroneItemAsync(int itemId, string changedBy)
        {
            var item = await _db.DroneItems.FindAsync(itemId);
            if (item == null) return;

            await AddLogAsync(item.CrewId, "Дрон", item.Name,
                item.Quantity, 0, "Видалено", changedBy);

            _db.DroneItems.Remove(item);

            await _db.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // БОЄКОМПЛЕКТ ЕКІПАЖУ
        // ════════════════════════════════════════════════════════════════════

        /// <inheritdoc />
        public async Task<List<AmmoItem>> GetAmmoItemsAsync(int crewId)
        {
            return await _db.AmmoItems
                .Where(a => a.CrewId == crewId)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task AddAmmoItemAsync(
            int crewId,
            string name,
            int qty,
            string changedBy)
        {
            var existing = await _db.AmmoItems
                .FirstOrDefaultAsync(a =>
                    a.CrewId == crewId &&
                    a.Name.ToLower() == name.Trim().ToLower());

            if (existing != null)
            {
                await AddLogAsync(crewId, "Боєкомплект", existing.Name,
                    existing.Quantity, existing.Quantity + qty, "Додано", changedBy);

                existing.Quantity += qty;
            }
            else
            {
                var item = new AmmoItem
                {
                    CrewId = crewId,
                    Name = name.Trim(),
                    Quantity = qty
                };

                _db.AmmoItems.Add(item);
                await _db.SaveChangesAsync();

                await AddLogAsync(crewId, "Боєкомплект", item.Name,
                    0, qty, "Додано", changedBy);
            }

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task SaveAmmoItemAsync(
            int itemId,
            string name,
            int qty,
            string changedBy)
        {
            var item = await _db.AmmoItems.FindAsync(itemId);
            if (item == null) return;

            await AddLogAsync(item.CrewId, "Боєкомплект", item.Name,
                item.Quantity, qty, "Змінено", changedBy);

            item.Name = name.Trim();
            item.Quantity = qty;

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DeleteAmmoItemAsync(int itemId, string changedBy)
        {
            var item = await _db.AmmoItems.FindAsync(itemId);
            if (item == null) return;

            await AddLogAsync(item.CrewId, "Боєкомплект", item.Name,
                item.Quantity, 0, "Видалено", changedBy);

            _db.AmmoItems.Remove(item);

            await _db.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // ПЕРЕДАЧА ЗІ СКЛАДУ
        // ════════════════════════════════════════════════════════════════════

        /// <inheritdoc />
        public async Task<List<WarehouseDroneItem>> GetWarehouseDronesAsync(int companyId)
        {
            return await _db.WarehouseDroneItems
                .Where(d => d.CompanyId == companyId && d.Quantity > 0)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<WarehouseAmmoItem>> GetWarehouseAmmosAsync(int companyId)
        {
            return await _db.WarehouseAmmoItems
                .Where(a => a.CompanyId == companyId && a.Quantity > 0)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<string> TransferDroneFromWarehouseAsync(
            int companyId,
            int crewId,
            int warehouseItemId,
            int qty,
            string transferType,
            string changedBy)
        {
            var wItem = await _db.WarehouseDroneItems.FindAsync(warehouseItemId);

            if (wItem == null || wItem.Quantity < qty)
                return "Недостатньо на складі.";

            bool isBomber = transferType == "bomber";
            bool isWing = transferType == "wing";

            // Шукаємо існуючий запис в екіпажі
            var crewItem = await _db.DroneItems
                .FirstOrDefaultAsync(d =>
                    d.CrewId == crewId &&
                    d.IsBomber == isBomber &&
                    d.IsWing == isWing &&
                    d.Name == wItem.Name);

            if (crewItem != null)
            {
                await AddLogAsync(crewId, "Дрон", crewItem.Name,
                    crewItem.Quantity, crewItem.Quantity + qty,
                    "Поповнено зі складу", changedBy);

                crewItem.Quantity += qty;
            }
            else
            {
                var newItem = new DroneItem
                {
                    CrewId = crewId,
                    Name = wItem.Name,
                    Quantity = qty,
                    IsBomber = isBomber,
                    IsWing = isWing
                };

                _db.DroneItems.Add(newItem);
                await _db.SaveChangesAsync();

                await AddLogAsync(crewId, "Дрон", wItem.Name,
                    0, qty, "Поповнено зі складу", changedBy);
            }

            // Зменшуємо кількість на складі
            wItem.Quantity -= qty;

            await _db.SaveChangesAsync();

            return $"Передано {qty} шт. '{wItem.Name}' екіпажу.";
        }

        /// <inheritdoc />
        public async Task<string> TransferAmmoFromWarehouseAsync(
            int companyId,
            int crewId,
            int warehouseItemId,
            int qty,
            string changedBy)
        {
            var wItem = await _db.WarehouseAmmoItems.FindAsync(warehouseItemId);

            if (wItem == null || wItem.Quantity < qty)
                return "Недостатньо на складі.";

            // Шукаємо існуючий запис в екіпажі
            var crewItem = await _db.AmmoItems
                .FirstOrDefaultAsync(a =>
                    a.CrewId == crewId &&
                    a.Name == wItem.Name);

            if (crewItem != null)
            {
                await AddLogAsync(crewId, "Боєкомплект", crewItem.Name,
                    crewItem.Quantity, crewItem.Quantity + qty,
                    "Поповнено зі складу", changedBy);

                crewItem.Quantity += qty;
            }
            else
            {
                var newItem = new AmmoItem
                {
                    CrewId = crewId,
                    Name = wItem.Name,
                    Quantity = qty
                };

                _db.AmmoItems.Add(newItem);
                await _db.SaveChangesAsync();

                await AddLogAsync(crewId, "Боєкомплект", wItem.Name,
                    0, qty, "Поповнено зі складу", changedBy);
            }

            // Зменшуємо кількість на складі
            wItem.Quantity -= qty;

            await _db.SaveChangesAsync();

            return $"Передано {qty} шт. '{wItem.Name}' екіпажу.";
        }

        // ════════════════════════════════════════════════════════════════════
        // ЖУРНАЛ
        // ════════════════════════════════════════════════════════════════════

        /// <inheritdoc />
        public async Task<List<CrewLog>> GetLogsAsync(int crewId)
        {
            return await _db.CrewLogs
                .Where(l => l.CrewId == crewId)
                .OrderByDescending(l => l.ChangedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Внутрішній метод запису в журнал екіпажу.
        /// Викликається перед кожною зміною даних.
        /// </summary>
        private async Task AddLogAsync(
            int crewId,
            string itemType,
            string itemName,
            int qtyBefore,
            int qtyAfter,
            string action,
            string changedBy)
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

            await _db.SaveChangesAsync();
        }
    }
}