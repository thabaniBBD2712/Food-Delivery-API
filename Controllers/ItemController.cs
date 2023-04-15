using FoodDeliveryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using System.Linq.Expressions;

namespace FoodDeliveryAPI.Controllers
{
    [Route("api/FoodDeliver.com/v1/[controller]")]
    public class ItemController : Controller
    {

        SqlConnection _connection;

        // Query Strings
        private static readonly string GET_ALL_ITEMS = "SELECT * FROM [ItemView]";
        private static readonly string GET_ITEMS_FROM_RESTAURANT = "SELECT * FROM [ItemView] IV WHERE IV.[restaurantId] = 1";

        private static readonly string GET_CATEGORY = "SELECT * FROM [ItemCategory] WHERE itemCategoryId = @categoryId";
        private static readonly string GET_STATUS = "SELECT * FROM [ItemStatus] WHERE itemStatusId = @statusId";
        private static readonly string GET_INFO = "SELECT * FROM [ItemInformation] WHERE ItemInformation = @infoId";


        [HttpGet]
        public ActionResult GetAllItems()
        {
            List<Item> items = new();
            using (SqlCommand command = new(GET_ALL_ITEMS, _connection))
            {
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    items.Add(new Item(reader));
                }
            }
            return Ok(items);
        }

        [HttpGet("Restaurant/{id}")]
        public ActionResult GetItemsFromRestaurant(int id)
        {
            List<Item> items = new();
            using (SqlCommand command = new(GET_ITEMS_FROM_RESTAURANT, _connection))
            {
                command.Parameters.Add("restaurant_id", SqlDbType.Int).Value = id;
                using SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    items.Add(new Item(reader));
                }
            }
            return Ok(items);
        }

        private int persistItemCategory(int id, string categoryName)
        {
            if (id == -1)
            {
                // perform insert
                using (SqlCommand command = new("", _connection))
                {
                    int rows_affected = command.ExecuteNonQuery();
                }

                // retrieve id added
                using (SqlCommand command = new("", _connection))
                {
                    int rows_affected = command.ExecuteNonQuery();
                }

                return id;
            } else
            {
                // perform update
                using (SqlCommand command = new("", _connection)){
                    int rows_affected = command.ExecuteNonQuery();
                }

                return id;
            }
        }

        public ItemController(IConfiguration _configuration)
        {
            _connection = new SqlConnection(_configuration.GetConnectionString("FoodDB"));
            _connection.Open();
        }

        ~ItemController()
        {
            _connection.Close();
        }
    }
}
