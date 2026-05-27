using Microsoft.EntityFrameworkCore;
using VPAPortal.Business.Services.Interfaces;
using VPAPortal.Common;
using VPAPortal.Data;
using VPAPortal.Data.Models;

namespace VPAPortal.Business.Services
{
    /// <summary>
    /// Реалізація сервісу вильотів.
    /// Вся логіка списання/повернення запасів зосереджена тут.
    /// </summary>
    public class FlightService : IFlightService
    {
        private readonly ApplicationDbContext _db;

        public FlightService(ApplicationDbContext db)
        {
            _db = db;
        }

        // ════════════════════════════════════════════════════════════════════
        // ДОВІДНИКИ
        // ════════════════════════════════════════════════════════════════════

        /// <inheritdoc />
        public async Task<List<DroneItem>> GetAvailableDronesAsync(int crewId)
        {
            return await _db.DroneItems
                .Where(d => d.CrewId == crewId && !d.IsBomber && !d.IsWing && d.Quantity > 0)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<DroneItem>> GetAvailableBombersAsync(int crewId)
        {
            // Бомбери показуємо незалежно від кількості — вони повертаються
            return await _db.DroneItems
                .Where(d => d.CrewId == crewId && d.IsBomber)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<DroneItem>> GetAvailableWingsAsync(int crewId)
        {
            // Крила також показуємо незалежно від кількості
            return await _db.DroneItems
                .Where(d => d.CrewId == crewId && d.IsWing)
                .OrderBy(d => d.Name)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<List<AmmoItem>> GetAvailableAmmosAsync(int crewId)
        {
            return await _db.AmmoItems
                .Where(a => a.CrewId == crewId && a.Quantity > 0)
                .OrderBy(a => a.Name)
                .ToListAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // ВИЛЬОТИ — ЧИТАННЯ
        // ════════════════════════════════════════════════════════════════════

        /// <inheritdoc />
        public async Task<List<Flight>> GetFlightsAsync(int crewId, DateOnly date)
        {
            return await _db.Flights
                .Where(f => f.CrewId == crewId && f.Date == date)
                .Include(f => f.DroneItem)
                .Include(f => f.Drops).ThenInclude(d => d.AmmoItem)
                .OrderBy(f => f.Time)
                .ToListAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // ВИЛЬОТИ — ДОДАВАННЯ
        // ════════════════════════════════════════════════════════════════════

        /// <inheritdoc />
        public async Task AddFpvFlightAsync(FpvFlightRequest req)
        {
            // Валідація наявності дрону та боєкомплекту
            var drone = await _db.DroneItems.FindAsync(req.DroneItemId)
                ?? throw new Exception("Безпілотник не знайдено.");

            if (drone.Quantity <= 0)
                throw new Exception("Немає дронів на складі.");

            var ammo = await _db.AmmoItems.FindAsync(req.AmmoItemId)
                ?? throw new Exception("Боєкомплект не знайдено.");

            if (ammo.Quantity <= 0)
                throw new Exception("Боєкомплект закінчився.");

            // Списуємо по одній одиниці
            drone.Quantity--;
            ammo.Quantity--;

            var flight = new Flight
            {
                CrewId = req.CrewId,
                Date = req.Date,
                Time = req.Time,
                Coordinates = req.Coordinates,
                Target = req.Target,
                Settlement = req.Settlement,
                DroneItemId = req.DroneItemId,
                Result = req.Result,
                TargetCount = req.TargetCount,
                DroneReturned = false,
                CreatedBy = req.CreatedBy,
                CreatedAt = DateTime.Now
            };

            // Один скид для FPV
            flight.Drops.Add(new FlightDrop
            {
                Coordinates = req.Coordinates,
                Target = req.Target,
                AmmoItemId = req.AmmoItemId,
                Result = req.Result,
                TargetCount = req.TargetCount,
                Settlement = req.Settlement,
                IsDelivery = false
            });

            _db.Flights.Add(flight);
            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task AddBomberFlightAsync(BomberFlightRequest req)
        {
            if (req.DroneItemId == 0)
                throw new Exception("Оберіть безпілотник.");

            var flight = new Flight
            {
                CrewId = req.CrewId,
                Date = req.Date,
                Time = req.Time,
                DroneItemId = req.DroneItemId,
                DroneReturned = req.DroneReturned,
                CreatedBy = req.CreatedBy,
                CreatedAt = DateTime.Now
            };

            if (req.MissionType == "attack")
            {
                // Фільтруємо скиди без координат
                var validDrops = req.Drops
                    .Where(d => !string.IsNullOrWhiteSpace(d.Coordinates))
                    .ToList();

                if (!validDrops.Any())
                    throw new Exception("Додайте хоча б один скид з координатами.");

                foreach (var drop in validDrops)
                {
                    if (drop.AmmoItemId == 0)
                        throw new Exception("Оберіть боєкомплект для кожного скиду.");

                    // Списуємо боєкомплект для кожного скиду
                    var ammo = await _db.AmmoItems.FindAsync(drop.AmmoItemId)
                        ?? throw new Exception("Боєкомплект не знайдено.");

                    if (ammo.Quantity <= 0)
                        throw new Exception("Боєкомплект закінчився.");

                    ammo.Quantity--;

                    flight.Drops.Add(new FlightDrop
                    {
                        Coordinates = drop.Coordinates,
                        Target = drop.Target,
                        AmmoItemId = drop.AmmoItemId,
                        Result = drop.Result,
                        TargetCount = drop.TargetCount,
                        Settlement = drop.Settlement,
                        DeliveryTime = drop.DropTime,
                        IsDelivery = false
                    });
                }

                // Координати вильоту беремо з першого скиду
                flight.Coordinates = flight.Drops.First().Coordinates;
                flight.Settlement = flight.Drops.First().Settlement;
            }
            else // delivery
            {
                var validDrops = req.Drops
                    .Where(d => !string.IsNullOrWhiteSpace(d.Coordinates))
                    .ToList();

                foreach (var drop in validDrops)
                {
                    flight.Drops.Add(new FlightDrop
                    {
                        Coordinates = drop.Coordinates,
                        Target = "",
                        AmmoItemId = null,
                        Result = drop.Result,
                        TargetCount = 0,
                        Settlement = drop.Settlement,
                        DeliveryTime = drop.DeliveryTime,
                        IsDelivery = true
                    });
                }

                flight.Coordinates = flight.Drops.FirstOrDefault()?.Coordinates ?? "";
                flight.Settlement = flight.Drops.FirstOrDefault()?.Settlement ?? "";
            }

            _db.Flights.Add(flight);
            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task AddWingFlightAsync(WingFlightRequest req)
        {
            if (req.DroneItemId == 0)
                throw new Exception("Оберіть безпілотник.");

            if (string.IsNullOrWhiteSpace(req.Settlement))
                throw new Exception("Введіть населений пункт.");

            // Крило не списує запаси автоматично
            var flight = new Flight
            {
                CrewId = req.CrewId,
                Date = req.Date,
                Time = req.Time,
                Coordinates = req.Coordinates,
                Settlement = req.Settlement,
                DroneItemId = req.DroneItemId,
                DroneReturned = req.DroneReturned,
                CreatedBy = req.CreatedBy,
                CreatedAt = DateTime.Now
            };

            _db.Flights.Add(flight);
            await _db.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // ВИЛЬОТИ — РЕДАГУВАННЯ / ВИДАЛЕННЯ
        // ════════════════════════════════════════════════════════════════════

        /// <inheritdoc />
        public async Task UpdateFlightAsync(
            int flightId,
            TimeOnly time,
            string coordinates,
            string target,
            FlightResult result,
            string settlement)
        {
            var flight = await _db.Flights.FindAsync(flightId)
                ?? throw new Exception("Виліт не знайдено.");

            flight.Time = time;
            flight.Coordinates = coordinates.Trim();
            flight.Target = target;
            flight.Result = result;
            flight.Settlement = settlement.Trim();

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DeleteFlightAsync(int flightId, CrewType crewType)
        {
            var flight = await _db.Flights
                .Include(f => f.DroneItem)
                .Include(f => f.Drops).ThenInclude(d => d.AmmoItem)
                .FirstOrDefaultAsync(f => f.Id == flightId);

            if (flight == null) return;

            // FPV — повертаємо дрон і боєкомплект
            if (crewType == CrewType.FPV)
            {
                if (flight.DroneItem != null)
                    flight.DroneItem.Quantity++;

                foreach (var drop in flight.Drops.Where(d => !d.IsDelivery))
                {
                    if (drop.AmmoItem != null)
                        drop.AmmoItem.Quantity++;
                }
            }

            // Бомбер — повертаємо тільки боєкомплект скидів (не дрон)
            if (crewType == CrewType.Бомбер)
            {
                foreach (var drop in flight.Drops.Where(d => !d.IsDelivery))
                {
                    if (drop.AmmoItem != null)
                        drop.AmmoItem.Quantity++;
                }
            }

            // Крило — нічого не повертаємо

            _db.Flights.Remove(flight);
            await _db.SaveChangesAsync();
        }
    }
}
