using FoodDeliveryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Data.SqlClient;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlTypes;
using Microsoft.VisualBasic;
using System.Reflection.Metadata.Ecma335;
using System.ComponentModel;

namespace FoodDeliveryAPI.Controllers
{
    [Route("api/FoodDeliver.com/v1/[controller]")]
    public class ItemController : Controller
    {

        SqlConnection _connection;

        // Query Strings
        private static readonly string GET_ALL_ITEMS = "SELECT * FROM [ItemView]";
        private static readonly string GET_ITEMS_FROM_RESTAURANT = "SELECT * FROM [ItemView] IV WHERE IV.[restaurantId] = @restaurant_id";
        private static readonly string GET_SINGLE_ITEM = "SELECT * FROM [ItemView] IV where IV.[itemId] = @item_id";

        private static readonly string INSERT_ITEM_INFORMATION =
            "INSERT INTO [ItemInformation] " +
            "(itemCategoryId, itemDescription, itemName) " +
            "VALUES " +
            "(@item_category_id, @item_description, @item_name)";

        private static readonly string UPDATE_ITEM_INFORMATION =
            "UPDATE [ItemInformation] " +
            "SET " +
                "itemCategoryId = @item_category_id, " +
                "itemDescription = @item_description, " +
                "itemName = @item_name " +
            "WHERE itemInformationID = @item_information_id;";

        private static readonly string INSERT_ITEM = 
            "INSERT INTO[Item] " +
            "(itemPrice, itemInformationId, itemStatusId, restaurantId) " +
            "VALUES " +
            "(@item_price, @item_information_id, @item_status_id, @restaurant_id)";

        private static readonly string UPDATE_ITEM =
            "UPDATE [Item] " +
            "SET " +
                "itemPrice = @item_price, " + 
                "itemInformationId = @item_information_id, " +
                "itemStatusId = @item_status_id " +
            "WHERE itemId = @item_id;";

        private static readonly string GET_INFORMATION_ID = "SELECT TOP(1) * FROM [ItemInformation]" +
            " WHERE ItemName = @item_name AND ItemDescription = @item_description" +
            " ORDER BY [ItemInformationId] DESC";

        private static readonly string DELETE_ITEM =
            "DELETE FROM Item WHERE itemId = @item_id";


        [HttpGet]
        public IEnumerable GetAllItems()
        {
            List<Item> items = new();
            using (SqlCommand command = new(GET_ALL_ITEMS, _connection))
            {
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    items.Add(new Item(ref reader));
                }
            }
            return items;
        }

        [HttpGet("Restaurant/{id}")]
        public IEnumerable GetItemsFromRestaurant(int id)
        {
            List<Item> items = new();
            using (SqlCommand command = new(GET_ITEMS_FROM_RESTAURANT, _connection))
            {
                command.Parameters.Add("restaurant_id", SqlDbType.Int).Value = id;
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    items.Add(new Item(ref reader));
                }
            }
            return items;
        }

        [HttpGet("{id}")]
        public IActionResult GetItem(int id)
        {
            using (SqlCommand command = new(GET_SINGLE_ITEM, _connection))
            {
                command.Parameters.Add("item_id", SqlDbType.Int).Value = id;
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read()){
                    return Ok (new Item(ref reader));
                }
            }

            return BadRequest("No Item Found");
        }


        private IActionResult PostInfo( ref Item item)
        {

            using (SqlCommand command = new(INSERT_ITEM_INFORMATION, _connection))
            {
                command.Parameters.AddWithValue("item_category_id", item.CategoryID);
                command.Parameters.AddWithValue("item_description", item.Description);
                command.Parameters.AddWithValue("item_name", item.Name);

                Console.WriteLine(command.Parameters.ToString());

                try
                {
                    command.ExecuteNonQuery();
                }
                catch
                {
                    return BadRequest("FAIL");
                }
            }

            Console.WriteLine("db updated");

            using (SqlCommand command = new(GET_INFORMATION_ID, _connection))
            {
                command.Parameters.AddWithValue("item_name", item.Name);
                command.Parameters.AddWithValue("item_description", item.Description);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    item.InformationID = reader.GetInt32(Item.INFORMATION_ID);
                    return Ok(item);
                }
                else
                {
                    return BadRequest("FAIL GET INFORMATION ID");
                }
            }
        }



        private IActionResult PutInfo( ref Item item)
        {
            // insert new info
            Console.WriteLine("Updating information");

            using (SqlCommand command = new(UPDATE_ITEM_INFORMATION, _connection))
            {
                command.Parameters.AddWithValue("item_category_id", item.CategoryID);
                command.Parameters.AddWithValue("item_description", item.Description);
                command.Parameters.AddWithValue("item_name", item.Name);
                command.Parameters.AddWithValue("item_information_id", item.InformationID);

                try
                {
                    command.ExecuteNonQuery();
                    return Ok(item);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }
        }

        [HttpPost("{Item}")]
        public IActionResult PostItem([FromBody]Item item)
        {

            ActionResult info_result =  (ActionResult)PostInfo(ref item);
            Console.WriteLine(info_result.ToString());


            using (SqlCommand command = new(INSERT_ITEM, _connection))
            {
                command.Parameters.AddWithValue("item_price", item.Price);
                command.Parameters.AddWithValue("item_information_id", item.InformationID);
                command.Parameters.AddWithValue("item_status_id", item.StatusID);
                command.Parameters.AddWithValue("restaurant_id", item.RestaurantID);
                try
                {
                    int rows_affected = command.ExecuteNonQuery();
                    return Ok(rows_affected);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }
            }

        }

        [HttpPut("{Item}")]

        public IActionResult PutItem([FromBody] Item item)
        {
            PutInfo(ref item);
            // perform update
            using (SqlCommand command = new(UPDATE_ITEM, _connection))
            {
                command.Parameters.AddWithValue("item_id", item.ID);
                command.Parameters.AddWithValue("item_price", item.Price);
                command.Parameters.AddWithValue("item_information_id", item.InformationID);
                command.Parameters.AddWithValue("item_status_id", item.StatusID);

                try
                {
                    int rows_affected = command.ExecuteNonQuery();
                    return Ok(rows_affected);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return BadRequest(e.Message);
                }
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteItem (int id)
        {
            using (SqlCommand command = new(DELETE_ITEM, _connection))
            {
                command.Parameters.AddWithValue("item_id", id);
                try
                {
                    int rows_affected = command.ExecuteNonQuery();
                    return(Ok(rows_affected));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return Ok(e.Message);
                }
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
