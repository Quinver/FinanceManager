using System.Data.Common;
using FinanceManager.Models;
using Microsoft.EntityFrameworkCore;

namespace FinanceManager.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Expense> Expenses { get; set; }

        public async Task ClearDatabaseAsync()
        {
            var tableNames = await GetTableNamesAsync();

            foreach (var tableName in tableNames)
            {
                try
                {
                    var sqlCommand = $"DELETE FROM \"{tableName}\";";
                    await Database.ExecuteSqlRawAsync(sqlCommand);
                }
                catch (Exception ex)
                {
                    // Log exception details for debugging
                    Console.Error.WriteLine($"Error clearing table {tableName}: {ex.Message}");
                    // Optionally rethrow or handle as appropriate
                }
            }
        }
        private async Task<List<string>> GetTableNamesAsync()
        {
            var tableNames = new List<string>();
            var sqlQuery = "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public'";

            // Use a distinct variable name for the command
            using (DbCommand dbCommand = Database.GetDbConnection().CreateCommand())
            {
                dbCommand.CommandText = sqlQuery;
                Database.OpenConnection();

                using (DbDataReader reader = await dbCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        tableNames.Add(reader.GetString(0));
                    }
                }
            }
            return tableNames;
        }
    }
}