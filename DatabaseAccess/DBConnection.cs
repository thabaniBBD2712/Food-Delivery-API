using System.Data.SqlClient;

namespace FoodDeliveryAPI.DatabaseAccess
{
    public class DatabaseAccess
    {
        private readonly SqlConnection _connection;
        public SqlConnection Connection
        {
            get { return _connection; }
        }

        private static readonly Lazy<DatabaseAccess> lazy =
                new Lazy<DatabaseAccess>(() => new DatabaseAccess());
        public static DatabaseAccess Instance { get { return lazy.Value; } }


        private DatabaseAccess()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();
            string connectionString = config.GetConnectionString("FoodDeliveryDB");
            _connection = new SqlConnection("string");
        }
    }
}