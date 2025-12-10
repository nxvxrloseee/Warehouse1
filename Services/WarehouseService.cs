using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Warehouse1.Data;
using Warehouse1.Models;


namespace Warehouse1.Services
{
    public class WarehouseService
    {
        private readonly AppDbContext _context;

        public WarehouseService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ExternalTable1>> GetVirtualWarehousesAsync()
        {
            return await _context.ExternalTables1.AsNoTracking().ToListAsync();
        }

        public async Task<DataTable> SearchProductAsync(int externalTableId, string? searchText)
        {
            var dt = new DataTable();
            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open) await connection.OpenAsync();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "ext.usp_SearchExternalTable";
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@ExternalTableId", externalTableId));
            var searchPattern = string.IsNullOrEmpty(searchText) ? null : $"%{searchText}%";
            cmd.Parameters.Add(new SqlParameter("@SearchText", (object?)searchPattern ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@PageSize", 500));

            using var reader = await cmd.ExecuteReaderAsync();
            dt.Load(reader);

            return dt;
        }

        public async Task<Dictionary<string, string>> GetPlatformLinksAsync(string productName)
        {
            var links = await _context.PlatformLinks.ToListAsync();
            var result = new Dictionary<string, string>();

            foreach (var link in links)
            {
                var url = link.UrlTemplate.Replace("{name}", System.Web.HttpUtility.UrlEncode(productName));
                result.Add(link.Name, url);
            }
            return result;
        }
    }
}
