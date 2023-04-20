using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.DatabaseAccess;
using FoodDeliveryAPI.Updaters;

namespace FoodDeliveryAPI.Controllers
{
    [Route("api/FoodDelivery.com/v1/[controller]")]
    [ApiController]
    public class ItemInformationController : ControllerBase
    {
        private readonly SqlConnection _connection;
        private readonly EventHandler<Updater.UpdateItemCategoryArgs> onUpdateItemCategory = (sender, eventArgs) =>
        {
            using (SqlCommand command = new SqlCommand(
                @"UPDATE ItemInformation
                SET itemCategoryId = @itemCategoryId
                WHERE itemInformationId = @itemInformationId", DBConnection.Instance.Connection
            ))
            {
                command.Parameters.AddWithValue("@itemCategoryId", eventArgs.ItemCategoryId);
                command.Parameters.AddWithValue("@itemInformationId", eventArgs.ParentId);
                command.ExecuteNonQuery();
            }
        };

        public ItemInformationController()
        {
            _connection = DBConnection.Instance.Connection;
            Updater.UpdateItemCategoryEvent += onUpdateItemCategory;
        }

        // GET: retrieve all itemInformations from db
        [HttpGet]
        public IActionResult Get()
        {
            List<ItemInformation> itemInformations = new();
            using (SqlDataReader reader = new SqlCommand("SELECT * FROM ItemInformation", _connection)
                .ExecuteReader())
            {
                while (reader.Read())
                {
                    ItemInformation itemInformation = new ItemInformation
                    {
                        itemInformationId = reader.GetInt32("itemInformationId"),
                        itemName = reader.GetString("itemName"),
                        itemDescription = reader.GetString("itemDescription"),
                        itemCategoryId = reader.GetInt32("itemCategoryId"),
                    };

                    itemInformations.Add(itemInformation);
                }
            }
            return Ok(itemInformations);
        }

        // GET: retrieve single itemInformation by id from db
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            using (SqlCommand command = new SqlCommand("SELECT * FROM ItemInformation WHERE itemInformationId = @itemInformationId", _connection))
            {
                command.Parameters.AddWithValue("@itemInformationId", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Ok(new ItemInformation
                        {
                            itemInformationId = reader.GetInt32("itemInformationId"),
                            itemName = reader.GetString("itemName"),
                            itemDescription = reader.GetString("itemDescription"),
                            itemCategoryId = reader.GetInt32("itemCategoryId"),
                        });
                    }
                    else
                    {
                        return NotFound($"ItemInformation with id {id} was not found");
                    }
                }
            }
        }

        // POST: add single itemInformation to db
        [HttpPost]
        public IActionResult Post([FromBody] ItemInformation itemInformation)
        {
            if (itemInformation == null)
            {
                return BadRequest();
            }
            using (SqlCommand command = new SqlCommand(
                "INSERT INTO ItemInformation (itemName, itemDescription, itemCategoryId) " +
                "VALUES (@itemName, @itemDescription, @itemCategoryId)", _connection
            ))
            {
                command.Parameters.AddWithValue("@itemName", itemInformation.itemName);
                command.Parameters.AddWithValue("@itemDescription", itemInformation.itemDescription);
                command.Parameters.AddWithValue("@itemCategoryId", itemInformation.itemCategoryId);
                command.ExecuteNonQuery();
            }
            return Ok("ItemInformation successfully posted");
        }

        // PUT: update single itemInformation in db by id 
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] ItemInformation itemInformation)
        {
            if (itemInformation == null || itemInformation.itemInformationId != id)
            {
                return BadRequest();
            }
            using (SqlCommand command = new SqlCommand(
                "UPDATE ItemInformation " +
                "SET itemName = @itemName, itemDescription = @itemDescription, itemCategoryId = @itemCategoryId " +
                "WHERE itemInformationId = @itemInformationId", _connection
            ))
            {
                command.Parameters.AddWithValue("@itemInformationId", itemInformation.itemInformationId);
                command.Parameters.AddWithValue("@itemName", itemInformation.itemName);
                command.Parameters.AddWithValue("@itemDescription", itemInformation.itemDescription);
                command.Parameters.AddWithValue("@itemCategoryId", itemInformation.itemCategoryId);
                if (command.ExecuteNonQuery() == 0)
                {
                    return NotFound($"ItemInformation with id {id} was not found");
                }
            }
            return Ok("ItemInformation successfully updated");
        }

        // DELETE: delete single itemInformation from db by id
        [HttpDelete("{id}", Name = "DeleteItemInformation")]
        public IActionResult Delete(int id)
        {
            using (SqlCommand command = new SqlCommand(
                "DELETE FROM ItemInformation WHERE itemInformationId = @itemInformationId", _connection
            ))
            {
                command.Parameters.AddWithValue("@itemInformationId", id);
                if (command.ExecuteNonQuery() == 0)
                {
                    return NotFound($"ItemInformation with id {id} was not found");
                }
            }
            return Ok("ItemInformation successfully deleted");
        }
    }

}