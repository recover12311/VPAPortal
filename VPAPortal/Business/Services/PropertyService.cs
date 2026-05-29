using Microsoft.EntityFrameworkCore;
using VPAPortal.Business.Services.Interfaces;
using VPAPortal.Data.Models;

namespace VPAPortal.Data.Services
{
    /// <summary>
    /// Реалізація сервісу майна.
    /// Вся логіка роботи з БД зосереджена тут.
    /// </summary>
    public class PropertyService : IPropertyService
    {
        private readonly ApplicationDbContext _db;

        public PropertyService(ApplicationDbContext db)
        {
            _db = db;
        }

        // ════════════════════════════════════════════════════════════════════
        // ТИПИ МАЙНА
        // ════════════════════════════════════════════════════════════════════

        /// <inheritdoc />
        public async Task<List<PropertyType>> GetPropertyTypesAsync()
        {
            return await _db.PropertyTypes
                .OrderBy(t => t.SortOrder)
                .ThenBy(t => t.Name)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task AddPropertyTypeAsync(string name, int maxCurrentOrder)
        {
            _db.PropertyTypes.Add(new PropertyType
            {
                Name = name.Trim(),
                SortOrder = maxCurrentOrder + 1
            });

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task SavePropertyTypeAsync(int typeId, string name)
        {
            var pt = await _db.PropertyTypes.FindAsync(typeId);
            if (pt == null) return;

            pt.Name = name.Trim();

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task<bool> DeletePropertyTypeAsync(int typeId)
        {
            // Перевіряємо чи використовується тип
            var inUse = await _db.PropertyItems.AnyAsync(i => i.PropertyTypeId == typeId)
                     || await _db.CompanyPropertyItems.AnyAsync(i => i.PropertyTypeId == typeId);

            if (inUse) return false;

            var pt = await _db.PropertyTypes.FindAsync(typeId);
            if (pt != null)
            {
                _db.PropertyTypes.Remove(pt);
                await _db.SaveChangesAsync();
            }

            return true;
        }

        // ════════════════════════════════════════════════════════════════════
        // МАЙНО РОТИ
        // ════════════════════════════════════════════════════════════════════

        /// <inheritdoc />
        public async Task<List<CompanyPropertyItem>> GetCompanyItemsAsync(int companyId)
        {
            return await _db.CompanyPropertyItems
                .Where(i => i.CompanyId == companyId)
                .Include(i => i.PropertyType)
                .OrderBy(i => i.PropertyType.Name)
                .ThenBy(i => i.Name)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task AddCompanyItemAsync(
            int companyId,
            string name,
            int qty,
            int typeId,
            string createdBy)
        {
            var typeName = await GetTypeNameAsync(typeId);

            _db.CompanyPropertyItems.Add(new CompanyPropertyItem
            {
                CompanyId = companyId,
                Name = name.Trim(),
                Quantity = qty,
                PropertyTypeId = typeId,
                CreatedBy = createdBy,
                CreatedAt = DateTime.Now
            });

            await AddLogAsync(companyId, null, "Рота", "Додано",
                name.Trim(), $"К-сть: {qty}, Тип: {typeName}", createdBy);

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task SaveCompanyItemAsync(
            int itemId,
            string name,
            int qty,
            int typeId,
            string changedBy)
        {
            var item = await _db.CompanyPropertyItems.FindAsync(itemId);
            if (item == null) return;

            var typeName = await GetTypeNameAsync(typeId);

            await AddLogAsync(item.CompanyId, null, "Рота", "Змінено",
                item.Name, $"К-сть: {item.Quantity}→{qty}, Тип: {typeName}", changedBy);

            item.Name = name.Trim();
            item.Quantity = qty;
            item.PropertyTypeId = typeId;

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DeleteCompanyItemAsync(int itemId, string changedBy)
        {
            var item = await _db.CompanyPropertyItems.FindAsync(itemId);
            if (item == null) return;

            await AddLogAsync(item.CompanyId, null, "Рота", "Видалено",
                item.Name, $"К-сть: {item.Quantity}", changedBy);

            _db.CompanyPropertyItems.Remove(item);

            await _db.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // ПЕРЕДАЧА РОТИ → ЕКІПАЖ
        // ════════════════════════════════════════════════════════════════════

        /// <inheritdoc />
        public async Task<string> TransferToCrewAsync(
            int companyId,
            int crewId,
            int companyItemId,
            int qty,
            string changedBy)
        {
            var src = await _db.CompanyPropertyItems
                .Include(i => i.PropertyType)
                .FirstOrDefaultAsync(i => i.Id == companyItemId);

            if (src == null || src.Quantity < qty)
                return "Недостатня кількість у роти.";

            // Зменшуємо у роти
            src.Quantity -= qty;

            // Шукаємо існуючу позицію в екіпажі з таким же іменем та типом
            var dest = await _db.PropertyItems
                .FirstOrDefaultAsync(i =>
                    i.CrewId == crewId &&
                    i.Name == src.Name &&
                    i.PropertyTypeId == src.PropertyTypeId);

            if (dest != null)
            {
                dest.Quantity += qty;
            }
            else
            {
                _db.PropertyItems.Add(new PropertyItem
                {
                    CrewId = crewId,
                    Name = src.Name,
                    Quantity = qty,
                    PropertyTypeId = src.PropertyTypeId,
                    CreatedBy = changedBy,
                    CreatedAt = DateTime.Now
                });
            }

            // Отримуємо назву екіпажу для журналу
            var crew = await _db.Crews.FindAsync(crewId);
            var crewName = crew?.Name ?? "";

            await AddLogAsync(companyId, crewId, "Рота", "Передано",
                src.Name, $"Передано {qty} шт. екіпажу «{crewName}»", changedBy);

            await _db.SaveChangesAsync();

            return $"Передано {qty} шт. «{src.Name}» екіпажу «{crewName}».";
        }

        // ════════════════════════════════════════════════════════════════════
        // МАЙНО ЕКІПАЖУ
        // ════════════════════════════════════════════════════════════════════

        /// <inheritdoc />
        public async Task<List<PropertyItem>> GetCrewItemsAsync(int crewId)
        {
            return await _db.PropertyItems
                .Where(i => i.CrewId == crewId)
                .Include(i => i.PropertyType)
                .OrderBy(i => i.PropertyType.Name)
                .ThenBy(i => i.Name)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task AddCrewItemAsync(
            int crewId,
            int companyId,
            string name,
            int qty,
            int typeId,
            string crewName,
            string createdBy)
        {
            var typeName = await GetTypeNameAsync(typeId);

            _db.PropertyItems.Add(new PropertyItem
            {
                CrewId = crewId,
                Name = name.Trim(),
                Quantity = qty,
                PropertyTypeId = typeId,
                CreatedBy = createdBy,
                CreatedAt = DateTime.Now
            });

            await AddLogAsync(companyId, crewId, "Екіпаж", "Додано",
                name.Trim(),
                $"К-сть: {qty}, Тип: {typeName}, Екіпаж: {crewName}",
                createdBy);

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task SaveCrewItemAsync(
            int itemId,
            string name,
            int qty,
            int typeId,
            string changedBy)
        {
            var item = await _db.PropertyItems
                .Include(i => i.PropertyType)
                .FirstOrDefaultAsync(i => i.Id == itemId);

            if (item == null) return;

            // Отримуємо companyId через екіпаж
            var crew = await _db.Crews.FindAsync(item.CrewId);

            await AddLogAsync(
                crew?.CompanyId ?? 0,
                item.CrewId,
                "Екіпаж", "Змінено",
                item.Name,
                $"К-сть: {item.Quantity}→{qty}",
                changedBy);

            item.Name = name.Trim();
            item.Quantity = qty;
            item.PropertyTypeId = typeId;

            await _db.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task DeleteCrewItemAsync(
            int itemId,
            string crewName,
            string changedBy)
        {
            var item = await _db.PropertyItems.FindAsync(itemId);
            if (item == null) return;

            var crew = await _db.Crews.FindAsync(item.CrewId);

            await AddLogAsync(
                crew?.CompanyId ?? 0,
                item.CrewId,
                "Екіпаж", "Видалено",
                item.Name,
                $"К-сть: {item.Quantity}, Екіпаж: {crewName}",
                changedBy);

            _db.PropertyItems.Remove(item);

            await _db.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // НАКЛАДНІ
        // ════════════════════════════════════════════════════════════════════

        /// <inheritdoc />
        public async Task<List<PropertyInvoice>> GetInvoicesAsync(int companyId)
        {
            return await _db.PropertyInvoices
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
            _db.PropertyInvoices.Add(new PropertyInvoice
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
            var inv = await _db.PropertyInvoices.FindAsync(invoiceId);
            if (inv == null) return;

            // Видаляємо фізичний файл якщо він є
            if (!string.IsNullOrEmpty(inv.PhotoPath))
            {
                var filePath = Path.Combine(
                    webRootPath,
                    inv.PhotoPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            _db.PropertyInvoices.Remove(inv);
            await _db.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // ЖУРНАЛ
        // ════════════════════════════════════════════════════════════════════

        /// <inheritdoc />
        public async Task<List<PropertyLog>> GetLogsAsync(int companyId)
        {
            return await _db.PropertyLogs
                .Where(l => l.CompanyId == companyId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Внутрішній метод запису в журнал майна.
        /// crewId = null для операцій на рівні роти.
        /// </summary>
        private async Task AddLogAsync(
            int companyId,
            int? crewId,
            string level,
            string action,
            string itemName,
            string details,
            string createdBy)
        {
            _db.PropertyLogs.Add(new PropertyLog
            {
                CompanyId = companyId,
                CrewId = crewId,
                Level = level,
                Action = action,
                ItemName = itemName,
                Details = details,
                CreatedBy = createdBy,
                CreatedAt = DateTime.Now
            });

            await _db.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        // ПРИВАТНІ ДОПОМІЖНІ МЕТОДИ
        // ════════════════════════════════════════════════════════════════════

        /// <summary>Повертає назву типу майна за Id.</summary>
        private async Task<string> GetTypeNameAsync(int typeId)
        {
            var type = await _db.PropertyTypes.FindAsync(typeId);
            return type?.Name ?? "";
        }
    }
}