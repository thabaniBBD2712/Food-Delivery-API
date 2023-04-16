using System.Data.SqlClient;

namespace FoodDeliveryAPI.DatabaseAccess
{
    public sealed class DBConnection
    {
        private readonly SqlConnection _connection;
        public SqlConnection Connection
        {
            get { return _connection; }
        }

        private static readonly Lazy<DBConnection> lazy =
                new Lazy<DBConnection>(() => new DBConnection());
        public static DBConnection Instance { get { return lazy.Value; } }


        private DBConnection()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            string connectionString = config.GetConnectionString("FoodDeliveryDB");
            _connection = new SqlConnection("string");
            _connection.Open();
        }
    }
}