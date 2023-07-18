using System.Data.Common;
using System.Data;
using Microsoft.Data.Sqlite;

namespace Norlysical2.Models
{
    public class EmployeeQuery
    {
        public AppDb Db { get; }

        public EmployeeQuery(AppDb db)
        {
            Db = db;
        }

        public async Task<Employee> FindEmployeeAsync(int id)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT Id, firstName, lastName, BirthDate, MainOfficeId FROM employees WHERE Id = @id;";
            cmd.Parameters.Add(new SqliteParameter
            {
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = id,
            });
            var result = await ReadEmployeesAsync(await cmd.ExecuteReaderAsync());

            return result.Count > 0 ? result[0] : null;
        }

        public async Task<List<Employee>> AllEmployeesAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT Id, firstName, lastName, BirthDate, MainOfficeId FROM employees ORDER BY Id;";
            return await ReadEmployeesAsync(await cmd.ExecuteReaderAsync());
        }

        private async Task<List<Employee>> ReadEmployeesAsync(DbDataReader reader)
        {
            var employees = new List<Employee>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new Employee(Db)
                    {
                        Id = reader.GetInt32(0),
                        FirstName = reader.GetString(1),
                        LastName = reader.GetString(2),
                        BirthDate = reader.GetDateTime(3),
                        MainOfficeId = reader.GetInt32(4)
                    };
                    employees.Add(post);
                }
            }
            return employees;
        }
    }
}
