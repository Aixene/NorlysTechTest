using System.Data.Common;
using System.Data;
using Microsoft.Data.Sqlite;

namespace Norlysical2.Models
{
    public class OfficeQuery
    {
        public AppDb Db { get; }

        public OfficeQuery(AppDb db)
        {
            Db = db;
        }

        private string OfficeQueryString(bool IdWhereClause = false)
        {
            string qry = @"
                    select offices.id, offices.location, offices.maxOccupancy, count(employees.id) as Occupancy
                    from offices
                    left outer join employees on employees.mainOfficeId = offices.id";
            if (IdWhereClause) 
            {
                qry += @"
                    where offices.Id = @id";
            }
            qry += @"
                    group by offices.id, offices.location, offices.maxOccupancy
                    order by offices.id;";
            return qry;
        }

        public async Task<Office> FindOfficeAsync(int id)
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = OfficeQueryString(true);
            cmd.Parameters.Add(new SqliteParameter
            {
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = id,
            });
            try
            {
                var result = await ReadOfficesAsync(await cmd.ExecuteReaderAsync());
                return result.Count > 0 ? result[0] : null;
            }
            catch (Exception ex)
            { Console.WriteLine(ex.ToString());
                return null;
                    
                    }

            
        }

        //
        public async Task<List<Office>> AllOfficesAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = OfficeQueryString();
            return await ReadOfficesAsync(await cmd.ExecuteReaderAsync());
        }

        private async Task<List<Office>> ReadOfficesAsync(DbDataReader reader)
        {
            var offices = new List<Office>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new Office(Db)
                    {
                        Id = reader.GetInt32(0),
                        Location = reader.GetString(1),
                        MaxOccupancy = reader.GetInt32(2),
                        Occupancy = reader.GetInt32(3)
                    };
                    offices.Add(post);
                }
            }
            return offices;
        }
    }
}
