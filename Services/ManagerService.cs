using CsvHelper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.IO;
using Warehouse1.Data;
using Warehouse1.Models;
namespace Warehouse1.Services
{
    public class ManagerService
    {
        private readonly AppDbContext _context;

        public ManagerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task TrackPriceAsync(string productName, decimal price, int? externalTableId)
        {
            var history = new PriceHistory
            {
                ProductName = productName,
                Price = price,
                ExternalTableId = externalTableId,
                RecordedAt = DateTime.Now,
                Currency = "RUB",
                Source = "Manual_Manager"
            };

            _context.PriceHistories.Add(history);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PriceHistory>> GetPriceDynamicsAsync(string productName, DateTime? from = null, DateTime? to = null)
        {
            var query = _context.PriceHistories.AsQueryable();

            query = query.Where(p => p.ProductName == productName);

            if (from.HasValue) query = query.Where(p => p.RecordedAt >= from.Value);
            if (to.HasValue) query = query.Where(p => p.RecordedAt <= to.Value);

            return await query.OrderBy(p => p.RecordedAt).ToListAsync();
        }

        public void ExportStatsToCsv(List<PriceHistory> data, string filePath)
        {
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);


            var records = data.Select(x => new
            {
                Товар = x.ProductName,
                Цена = x.Price,
                Валюта = x.Currency,
                Дата = x.RecordedAt.ToString("yyyy-MM-dd HH:mm"),
                Источник = x.Source
            });

            csv.WriteRecords(records);
        }

    }
}
