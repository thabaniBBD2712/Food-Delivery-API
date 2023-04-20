using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.DatabaseAccess;

namespace FoodDeliveryAPI.Controllers
{
    [Route("api/FoodDelivery.com/v1/[controller]")]
    [ApiController]
    public class ItemStatusController : ControllerBase
    {
        private readonly SqlConnection _connection;

        public ItemStatusController()
        {
            _connection = DBConnection.Instance.Connection;
        }

        // GET: retrieve all itemStatuses from db
        [HttpGet]
        public IActionResult Get()
        {
            List<ItemStatus> itemStatuses = new();
            using (SqlDataReader reader = new SqlCommand("SELECT * FROM ItemStatus", _connection)
                .ExecuteReader())
            {
                while (reader.Read())
                {
                    ItemStatus itemStatus = new ItemStatus
                    {
                        itemStatusId = reader.GetInt32("itemStatusId"),
                        itemStatusName = reader.GetString("itemStatusName")
                    };

                    itemStatuses.Add(itemStatus);
                }
            }
            return Ok(itemStatuses);
        }

        // GET: retrieve single itemStatus by id from db
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            using (SqlCommand command = new SqlCommand("SELECT * FROM ItemStatus WHERE itemStatusId = @itemStatusId", _connection))
            {
                command.Parameters.AddWithValue("@itemStatusId", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Ok(new ItemStatus
                        {
                            itemStatusId = reader.GetInt32("itemStatusId"),
                            itemStatusName = reader.GetString("itemStatusName")
                        });
                    }
                    else
                    {
                        return NotFound($"ItemStatus with id {id} was not found");
                    }
                }
            }
        }

        public static int GetIdByName(string name)
        {
            using (SqlCommand command = new SqlCommand(
                @"SELECT TOP (1) [itemStatusId]
                FROM [FoodDeliveryDB].[dbo].[ItemStatus] status
                WHERE status.itemStatusName = @name", DBConnection.Instance.Connection
            ))
            {
                command.Parameters.AddWithValue("@name", name);
                object result = command.ExecuteScalar();
                return result == null ? 0 : (Int32)result;
            }
        }
    }
}