using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Warehouse1.Data;
using ClosedXML.Excel;
using System.Data;

namespace Warehouse1.Services
{
    public class AdminImportService
    {
        private readonly AppDbContext _context;

        public AdminImportService(AppDbContext context)
        {
            _context = context;
        }

        public async Task ImportExcelTableAsync(string filePath, string tableName, int userId)
        {
            using var workbook = new XLWorkbook(filePath);
            var worksheet = workbook.Worksheet(1);
            var firstRow = worksheet.Row(1);

            var sb = new System.Text.StringBuilder();
            sb.Append($"CREATE TABLE dbo.[{tableName}] (Id INT IDENTITY(1,1) PRIMARY KEY, ");

            var columns = new List<string>();
            foreach (var cell in firstRow.CellsUsed())
            {
                string colName = cell.GetString().Trim().Replace(" ", "_");
                columns.Add(colName);
                sb.Append($"[{colName}] NVARCHAR(MAX) NULL, ");
            }
            sb.Length -= 2;
            sb.Append(");");


            await _context.Database.ExecuteSqlRawAsync(sb.ToString());

            await _context.Database.ExecuteSqlInterpolatedAsync(
                $"EXEC ext.usp_RegisterExternalTable @SchemaName='dbo', @TableName={tableName}, @DisplayName={tableName}, @CreatedByUserId={userId}, @HasHeaderRow=1");

            var dt = new DataTable();
            foreach (var col in columns) dt.Columns.Add(col);

            foreach (var row in worksheet.RowsUsed().Skip(1))
            {
                var dataRow = dt.NewRow();
                int i = 0;
                foreach (var cell in row.Cells(1, columns.Count)) 
                {
                    dataRow[i] = cell.GetString();
                    i++;
                }
                dt.Rows.Add(dataRow);

            }

            var connectionString = _context.Database.GetConnectionString();
            using var bulkCopy = new SqlBulkCopy(connectionString);
            bulkCopy.DestinationTableName = $"dbo.[{tableName}]";

            foreach (var col in columns)
                bulkCopy.ColumnMappings.Add(col, col);

            await bulkCopy.WriteToServerAsync(dt);
        }

    }
}
