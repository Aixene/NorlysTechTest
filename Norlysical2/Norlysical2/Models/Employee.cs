using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.Data.Sqlite;
using System.Data;

namespace Norlysical2.Models
{
    public class Employee
    {
        public int Id { get; set; } = 0;
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; } = new DateTime();
        public int MainOfficeId { get; set; }

        internal AppDb Db { get; set; }

        public Employee()
        {
        }

        internal Employee(AppDb db)
        {
            Db = db;
        }

        public async Task InsertAsync()
        {
            using var cmd = Db.Connection.CreateCommand();

            SqliteTransaction transaction = Db.Connection.BeginTransaction();
            cmd.Transaction = transaction;
            cmd.CommandText = @"INSERT INTO employees (firstName, lastName, birthdate, mainOfficeId) VALUES (@firstName, @lastName, @BirthDate, @MainOfficeId);";
            BindParams(cmd);
            await cmd.ExecuteNonQueryAsync();

            //Get last inserted rowid
            cmd.CommandText = @"SELECT last_insert_rowid();";
            var reader = await cmd.ExecuteReaderAsync();
            reader.ReadAsync();
            Id = reader.GetInt32(0);

            transaction.Commit();
        }

        public async Task UpdateAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"UPDATE `employees` SET `FirstName` = @firstName, `LastName` = @lastName, BirthDate = @BirthDate, MainOfficeId = @MainOfficeId WHERE `Id` = @id;";
            BindParams(cmd);
            BindId(cmd);
            try
            {
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }

        public async Task DeleteAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"DELETE FROM `employees` WHERE `Id` = @id;";
            BindId(cmd);
            await cmd.ExecuteNonQueryAsync();
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
                ParameterName = "@firstName",
                DbType = DbType.String,
                Value = FirstName,
            });
            cmd.Parameters.Add(new SqliteParameter
            {
                ParameterName = "@lastName",
                DbType = DbType.String,
                Value = LastName,
            });
            cmd.Parameters.Add(new SqliteParameter
            {
                ParameterName = "@BirthDate",
                DbType = DbType.DateTime,
                Value = BirthDate,
            });
            cmd.Parameters.Add(new SqliteParameter
            {
                ParameterName = "@MainOfficeId",
                DbType = DbType.Int32,
                Value = MainOfficeId,
            });
        }



        //validity methods

        public bool isValid()
        {

            if (HasValidFirstName() && HasValidLastName() && HasValidBirthDate())
            {
                return true;
            }

            return false;
        }

        public bool HasValidFirstName()
        {
            if (string.IsNullOrEmpty(FirstName))
            {
                return false;
            }
            foreach (char c in FirstName)
            {
                if (!Char.IsLetter(c) && !(c == ' '))
                {
                    return false;
                }
            }
            return true;
        }

        public bool HasValidLastName()
        {
            if (string.IsNullOrEmpty(LastName))
            {
                return false;
            }
            foreach (char c in LastName)
            {
                if (!Char.IsLetter(c))
                {
                    return false;
                }
            }
            return true;
        }

        public bool HasValidBirthDate()
        { 
            if ((BirthDate.Year+100)<DateTime.Now.Year || BirthDate.Year>DateTime.Now.Year-16)
            {
                return false;
            }
            return true;
        }

    }
}
