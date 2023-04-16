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


        private static readonly string GET_CATEGORY = "SELECT * FROM [ItemCategory] WHERE itemCategoryId = @categoryId";
        private static readonly string GET_STATUS = "SELECT * FROM [ItemStatus] WHERE itemStatusId = @statusId";

        private static readonly string GET_INFO = "SELECT * FROM [ItemInformation] WHERE ItemInformation = @infoId"; 
        private static readonly string GET_INFORMATION_ID = "SELECT TOP(1) * FROM [ItemInformation]" +
            " WHERE ItemName = @item_name AND ItemDescription = @item_description" +
            " ORDER BY [ItemInformationId] DESC";


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
        public Item GetItem(int id)
        {
            using (SqlCommand command = new(GET_SINGLE_ITEM, _connection))
            {
                command.Parameters.Add("item_id", SqlDbType.Int).Value = id;
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read()){
                    return new Item(ref reader);
                }
            }

            return null;
        }

        private string PersistInformation([FromBody] ref Item item)
        {
            // insert new info

            if (item.InformationID == 0) // perform insert
            {
                Console.WriteLine("Inserting information");
                using (SqlCommand command = new(INSERT_ITEM_INFORMATION, _connection))
                {
                    command.Parameters.AddWithValue("item_category_id", item.CategoryID);
                    command.Parameters.AddWithValue("item_description", item.Description);
                    command.Parameters.AddWithValue("item_name", item.Name);

                    Console.WriteLine(command.ToString());

                    try
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Done");
                    }
                    catch
                    {
                        return "FAIL INSERT INFORMATION";
                    }
                }

                // get ID of new info

                using (SqlCommand command = new(GET_INFORMATION_ID, _connection))
                {
                    command.Parameters.AddWithValue("item_name", item.Name);
                    command.Parameters.AddWithValue("item_description", item.Name);
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.Read())
                    {
                        item.InformationID = reader.GetInt32(Item.INFORMATION_ID);
                        Console.WriteLine(item.InformationID);
                    }
                    else
                    {
                        return "FAIL GET INFORMATION ID";
                    }
                }
                
            } 

            else // perform update
            {
                Console.WriteLine("Updating information");

                using (SqlCommand command = new(UPDATE_ITEM_INFORMATION, _connection))
                {
                    command.Parameters.AddWithValue("item_category_id", item.CategoryID);
                    command.Parameters.AddWithValue("item_description", item.Description);
                    command.Parameters.AddWithValue("item_name", item.Name);
                    command.Parameters.AddWithValue("item_information_id", item.InformationID);

                    Console.WriteLine("infoID" + item.InformationID);

                    try
                    {
                        Console.WriteLine("start");
                        command.ExecuteNonQuery();
                        Console.WriteLine("done");
                    }
                    catch
                    {
                        return "FAIL UPDATE INFORMATION";
                    }
                }
            }
            return "OK";
        }


        [HttpPost("Persist/{Item}")]
        public string PersistItem([FromBody]Item item)
        {

            PersistInformation(ref item);

            if (item.ID == 0)
            {
                // perform insert
                

                // insert new item
                using (SqlCommand command = new(INSERT_ITEM, _connection))
                {
                    command.Parameters.AddWithValue("item_price", item.Price);
                    command.Parameters.AddWithValue("item_information_id", item.InformationID);
                    command.Parameters.AddWithValue("item_status_id", item.StatusID);
                    command.Parameters.AddWithValue("restaurant_id", item.RestaurantID);
                    try
                    {
                        int rows_affected = command.ExecuteNonQuery();
                        Console.WriteLine(rows_affected);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return "FAIL 2";
                    }
                }
            }
            else
            {
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
                        Console.WriteLine(rows_affected);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return "FAIL 2";
                    }
                }

            }


            return "OK";
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
