using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.DatabaseAccess;
using FoodDeliveryAPI.Updaters;

namespace FoodDeliveryAPI.Controllers
{
    [Route("api/FoodDeliver.com/v1/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly SqlConnection _connection;
        private readonly EventHandler<Updater.UpdateOrderStatusArgs> onUpdateOrderStatus = (sender, eventArgs) =>
        {
            using (SqlCommand command = new SqlCommand(
                @"UPDATE [Order]
                SET orderStatusId = @orderStatusId
                WHERE orderId = @orderId", DBConnection.Instance.Connection
            ))
            {
                command.Parameters.AddWithValue("@orderStatusId", eventArgs.OrderStatusId);
                command.Parameters.AddWithValue("@orderId", eventArgs.ParentId);
                command.ExecuteNonQuery();
            }
        };

        public OrderController()
        {
            _connection = DBConnection.Instance.Connection;
            Updater.UpdateOrderStatusEvent += onUpdateOrderStatus;
        }


        // get all orders 
        [HttpGet]
        public IActionResult Get()
        {

            List<Order> orders = new List<Order>();

            using (SqlCommand command = new SqlCommand("SELECT * FROM [Order]", _connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Order order = new Order();

                        order.orderId = (int)reader["orderId"];
                        order.orderDate = reader.GetDateTime("orderDate");
                        order.restaurantId = (int)reader["restaurantId"];
                        order.userId = (int)reader["userId"];
                        order.personeelId = reader.GetInt32("personeelId");
                        order.addressId = (int)reader["addressId"];
                        order.orderStatusId = (int)reader["orderStatusId"];

                        orders.Add(order);
                    }
                }
            }
            // }

            return Ok(orders);
        }
        //get specific Order 
        [HttpGet("{orderId}")]
        public ActionResult<Order> Get(int orderId)
        {
            Order order = new Order();
            SqlCommand command = new SqlCommand("SELECT * FROM [Order] WHERE orderId = @orderId", _connection);
            command.Parameters.AddWithValue("@orderId", orderId);

            SqlDataReader reader = command.ExecuteReader();
            if (reader.Read())
            {
                order.orderId = (int)reader["orderId"];
                order.orderDate = reader.GetDateTime("orderDate");

                order.restaurantId = (int)reader["restaurantId"];
                order.userId = (int)reader["userId"];
                order.personeelId = (int)reader["personeelId"];
                order.addressId = (int)reader["addressId"];
                order.orderStatusId = (int)reader["orderStatusId"];
            }
            else
            {
                return NotFound();
            }
            reader.Close();
            return order;
        }


        // POST api/<RestaurantController>
        [HttpPost]
        public IActionResult Post([FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest();
            }
            DateTime currentDateTime = DateTime.Now;
            using (SqlCommand command = new SqlCommand("INSERT INTO [Order] (orderDate, restaurantId, userId, personeelId, addressId, orderStatusId)" +
            "VALUES (@orderDate, @restaurantId, @userId, @personeelId, @addressId, @orderStatusId)", _connection))
            {
                command.Parameters.AddWithValue("@orderDate", currentDateTime);
                command.Parameters.AddWithValue("@restaurantId", order.restaurantId);
                command.Parameters.AddWithValue("@userId", order.userId);
                command.Parameters.AddWithValue("@personeelId", order.personeelId);
                command.Parameters.AddWithValue("@addressId", order.addressId);
                command.Parameters.AddWithValue("@orderStatusId", order.orderStatusId);
                command.ExecuteNonQuery();
            }

            return Ok();
        }

        [HttpPut("{id}", Name = "UpdateOrder")]
        public IActionResult Put(int id, [FromBody] Order order)
        {
            if (order == null || order.orderId != id)
            {
                return BadRequest();
            }
            DateTime currentDateTime = DateTime.Now;

            string query = @"UPDATE [Order]
                       SET restaurantId = @restaurantId,
                           orderDate = @orderDate,
                           userId = @userId,
                           personeelId = @personeelId,
                           addressId = @addressId,
                           orderStatusId = @orderStatusId
                       WHERE orderId = @orderId";

            using (SqlCommand command = new SqlCommand(query, _connection))
            {
                command.Parameters.AddWithValue("@orderId", id);
                command.Parameters.AddWithValue("@orderDate", currentDateTime);
                command.Parameters.AddWithValue("@restaurantId", order.restaurantId);
                command.Parameters.AddWithValue("@userId", order.userId);
                command.Parameters.AddWithValue("@personeelId", order.personeelId);
                command.Parameters.AddWithValue("@addressId", order.addressId);
                command.Parameters.AddWithValue("@orderStatusId", order.orderStatusId);
                command.ExecuteNonQuery();

            }

            return Ok("Order Updated Successfully");
        }
    }
}