using FoodDeliveryAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Data.SqlClient;
using System.Data;

namespace FoodDeliveryAPI.Controllers
{
    [Route("api/FoodDeliver.com/v1/[controller]")]
    public class ItemViewController : Controller
    {

        SqlConnection _connection;

        // Query Strings
        private static readonly string GET_ALL_ITEMS = "SELECT * FROM [ItemView]";
        private static readonly string GET_ITEMS_FROM_RESTAURANT = "SELECT * FROM [ItemView] IV WHERE IV.[restaurantId] = @restaurant_id";
        private static readonly string GET_SINGLE_ITEM = "SELECT * FROM [ItemView] IV where IV.[itemId] = @item_id";

        [HttpGet]
        public IEnumerable GetAllItemViews()
        {
            List<ItemView> items = new();
            using (SqlCommand command = new(GET_ALL_ITEMS, _connection))
            {
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    items.Add(new ItemView(ref reader));
                }
            }
            return items;
        }

        [HttpGet("Restaurant/{id}")]
        public IEnumerable GetItemViewsFromRestaurant(int id)
        {
            List<ItemView> items = new();
            using (SqlCommand command = new(GET_ITEMS_FROM_RESTAURANT, _connection))
            {
                command.Parameters.Add("restaurant_id", SqlDbType.Int).Value = id;
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    items.Add(new ItemView(ref reader));
                }
            }
            return items;
        }

        [HttpGet("{id}")]
        public IActionResult GetItemView(int id)
        {
            using (SqlCommand command = new(GET_SINGLE_ITEM, _connection))
            {
                command.Parameters.Add("item_id", SqlDbType.Int).Value = id;
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read()){
                    return Ok (new ItemView(ref reader));
                }
            }

            return BadRequest("No ItemView Found");
        }

        public ItemViewController(IConfiguration _configuration)
        {
            _connection = new SqlConnection(_configuration.GetConnectionString("FoodDB"));
            _connection.Open();
        }

        ~ItemViewController()
        {
            _connection.Close();
        }
    }
}
