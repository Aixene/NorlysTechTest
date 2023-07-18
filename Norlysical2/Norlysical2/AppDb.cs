using Microsoft.Data.Sqlite;

namespace Norlysical2
{
    public class AppDb : IDisposable
    {
        public SqliteConnection Connection { get; }

        public AppDb(string connectionString)
        {
            Connection = new SqliteConnection(connectionString);
        }

        public void Dispose() => Connection.Dispose();
    }
}
