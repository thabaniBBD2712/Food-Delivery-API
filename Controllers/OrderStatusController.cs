using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.DatabaseAccess;

namespace FoodDeliveryAPI.Controllers
{
    [Route("api/FoodDelivery.com/v1/[controller]")]
    [ApiController]
    public class OrderStatusController : ControllerBase
    {
        private readonly SqlConnection _connection;

        public OrderStatusController()
        {
            _connection = DBConnection.Instance.Connection;
        }

        // GET: retrieve all orderStatuses from db
        [HttpGet]
        public IActionResult Get()
        {
            List<OrderStatus> orderStatuses = new();
            using (SqlDataReader reader = new SqlCommand("SELECT * FROM OrderStatus", _connection)
                .ExecuteReader())
            {
                while (reader.Read())
                {
                    OrderStatus orderStatus = new OrderStatus
                    {
                        orderStatusId = reader.GetInt32("orderStatusId"),
                        orderStatusName = reader.GetString("orderStatusName")
                    };

                    orderStatuses.Add(orderStatus);
                }
            }
            return Ok(orderStatuses);
        }

        // GET: retrieve single orderStatus by id from db
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            using (SqlCommand command = new SqlCommand("SELECT * FROM OrderStatus WHERE orderStatusId = @orderStatusId", _connection))
            {
                command.Parameters.AddWithValue("@orderStatusId", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Ok(new OrderStatus
                        {
                            orderStatusId = reader.GetInt32("orderStatusId"),
                            orderStatusName = reader.GetString("orderStatusName")
                        });
                    }
                    else
                    {
                        return NotFound($"OrderStatus with id {id} was not found");
                    }
                }
            }
        }
    }
}