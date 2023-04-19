using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.DatabaseAccess;

namespace FoodDeliveryAPI.Controllers
{
    [Route("api/FoodDelivery.com/v1/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly SqlConnection _connection;

        public ItemController()
        {
            _connection = DBConnection.Instance.Connection;
        }

        // GET: retrieve all items from db
        [HttpGet]
        public IActionResult Get()
        {
            List<Item> items = new();
            using (SqlDataReader reader = new SqlCommand("SELECT * FROM Item", _connection)
                .ExecuteReader())
            {
                while (reader.Read())
                {
                    Item item = new Item
                    {
                        itemPrice = reader.GetDecimal("itemPrice"),
                        restaurantId = reader.GetInt32("restaurantId"),
                        itemStatusId = reader.GetInt32("itemStatusId"),
                        itemInformationId = reader.GetInt32("itemInformationId"),
                        itemId = reader.GetInt32("itemId"),
                    };

                    items.Add(item);
                }
            }
            return Ok(items);
        }

        // GET: retrieve single item by id from db
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            using (SqlCommand command = new SqlCommand("SELECT * FROM Item WHERE itemId = @itemId", _connection))
            {
                command.Parameters.AddWithValue("@itemId", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Ok(new Item
                        {
                            itemPrice = reader.GetDecimal("itemPrice"),
                            restaurantId = reader.GetInt32("restaurantId"),
                            itemStatusId = reader.GetInt32("itemStatusId"),
                            itemInformationId = reader.GetInt32("itemInformationId"),
                            itemId = reader.GetInt32("itemId"),
                        });
                    }
                    else
                    {
                        return NotFound($"Item with id {id} was not found");
                    }
                }
            }
        }

        // POST: add single item to db
        [HttpPost]
        public IActionResult Post([FromBody] Item item)
        {
            if (item == null)
            {
                return BadRequest();
            }
            using (SqlCommand command = new SqlCommand(
                "INSERT INTO Item (itemPrice, restaurantId, itemStatusId, itemInformationId) " +
                "VALUES (@itemPrice, @restaurantId, @itemStatusId, @itemInformationId)", _connection
            ))
            {

                command.Parameters.AddWithValue("@itemPrice", item.itemPrice);
                command.Parameters.AddWithValue("@restaurantId", item.restaurantId);
                command.Parameters.AddWithValue("@itemStatusId", item.itemStatusId);
                command.Parameters.AddWithValue("@itemInformationId", item.itemInformationId);
                command.ExecuteNonQuery();
            }
            return Ok("Item successfully posted");
        }

        // PUT: update single item in db by id 
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Item item)
        {
            if (item == null || item.itemId != id)
            {
                return BadRequest();
            }
            using (SqlCommand command = new SqlCommand(
                "UPDATE Item " +
                "SET itemInformationId = @itemInformationId, itemStatusId = @itemStatusId, restaurantId = @restaurantId, itemPrice = @itemPrice " +
                "WHERE itemId = @itemId", _connection
            ))
            {
                command.Parameters.AddWithValue("@itemInformationId", item.itemInformationId);
                command.Parameters.AddWithValue("@itemStatusId", item.itemStatusId);
                command.Parameters.AddWithValue("@restaurantId", item.restaurantId);
                command.Parameters.AddWithValue("@itemPrice", item.itemPrice);
                command.Parameters.AddWithValue("@itemId", id);
                if (command.ExecuteNonQuery() == 0)
                {
                    return NotFound($"Item with id {id} was not found");
                }
            }
            return Ok("Item successfully updated");
        }

        // DELETE: delete single item from db by id
        [HttpDelete("{id}", Name = "DeleteItem")]
        public IActionResult Delete(int id)
        {
            using (SqlCommand command = new SqlCommand(
                "DELETE FROM Item WHERE itemId = @itemId", _connection
            ))
            {
                command.Parameters.AddWithValue("@itemId", id);
                if (command.ExecuteNonQuery() == 0)
                {
                    return NotFound($"Item with id {id} was not found");
                }
            }
            return Ok("Item successfully deleted");
        }
    }

}