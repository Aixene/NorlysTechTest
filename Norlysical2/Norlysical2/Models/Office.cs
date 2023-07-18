using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Hosting;
using System.Data;
using System.Reflection.PortableExecutable;

namespace Norlysical2.Models
{
    public class Office
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public int MaxOccupancy { get; set; }
        public int Occupancy { get; set; }

        internal AppDb Db { get; set; }

        public Office()
        {
        }

        internal Office(AppDb db)
        {
            Db = db;
        }

        private void BindId(SqliteCommand cmd)
        {
            cmd.Parameters.Add(new SqliteParameter
            {
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = Id,
            });
        }

        private void BindParams(SqliteCommand cmd)
        {
            cmd.Parameters.Add(new SqliteParameter
            {
                ParameterName = "@Location",
                DbType = DbType.String,
                Value = Location,
            });
            cmd.Parameters.Add(new SqliteParameter
            {
                ParameterName = "@MaxOccupancy",
                DbType = DbType.Int32,
                Value = MaxOccupancy,
            });
        }
    


        public async Task<bool> HasVacantSpace()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"select count(*) from employees where mainofficeid = @id;";
            cmd.Parameters.Add(new SqliteParameter
            {
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = Id,
            });

            var reader = await cmd.ExecuteReaderAsync();
            await reader.ReadAsync();
            return reader.GetInt32(0) < MaxOccupancy;
        }
    }
}
