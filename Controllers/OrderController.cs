using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using FoodDeliveryAPI.Models;

namespace FoodDeliveryAPI.Controllers
{
    [Route("api/FoodDeliver.com/v1/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // get all orders 
        [HttpGet]
        public IActionResult Get()
        {

            List<Order> orders = new List<Order>();
            string connectionString = _configuration.GetConnectionString("FoodDeliveryDB");
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM Order", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Order order = new Order();

                            order.orderId = (int)reader["orderId"];
                            order.orderDate=(string)reader["orderDate"];
                            order.restaurantId = (int)reader["restaurantId"];
                            order.userId = (int)reader["userId"];
                            order.personnelId = (int)reader["personnelId"];
                            order.addressId = (int)reader["addressId"];
                            order.orderStatusId = (int)reader["orderStatusId"];    
                            
                            orders.Add(order);
                        }
                    }
                }
            }

            return Ok(orders);
        }
        //get specific Order 
        [HttpGet("{orderId}")]
        public ActionResult<Order> Get(int orderId)
        {
            Order order = new Order();
             string connectionString = _configuration.GetConnectionString("FoodDeliveryDB");
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT * FROM Order WHERE orderId = @orderId", connection);
                command.Parameters.AddWithValue("@orderId", orderId);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    order.orderId = (int)reader["orderId"];
                     order.orderDate=(string)reader["orderDate"];
                    order.restaurantId = (int)reader["restaurantId"];
                    order.userId = (int)reader["userId"];
                    order.personnelId = (int)reader["personnelId"];
                    order.addressId = (int)reader["addressId"];
                    order.orderStatusId = (int)reader["orderStatusId"];        
                }
                else
                {
                    return NotFound();
                }
                reader.Close();
            }
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
            string connectionString = _configuration.GetConnectionString("FoodDeliveryDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                DateTime currentDateTime = DateTime.Now;
                connection.Open();
                using (SqlCommand command = new SqlCommand( "INSERT INTO Orders (orderId, orderDate, restaurantId, userId, personnelId, addressId, orderStatusId)"+
                "VALUES (@orderId,@orderDate, @restaurantId, @userId, @personnelId, @addressId, @orderStatusId)", connection))
                {
                    command.Parameters.AddWithValue("@orderId", order.orderId);
                    command.Parameters.AddWithValue("@orderDate", currentDateTime);
                    command.Parameters.AddWithValue("@restaurantId", order.restaurantId);
                    command.Parameters.AddWithValue("@userId", order.userId);
                    command.Parameters.AddWithValue("@personnelId", order.personnelId);
                    command.Parameters.AddWithValue("@addressId", order.addressId);
                    command.Parameters.AddWithValue("@orderStatusId", order.orderStatusId);
                    command.ExecuteNonQuery();
                }
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
            string connectionString = _configuration.GetConnectionString("FoodDeliveryDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                 string query = @"UPDATE Orders
                       SET restaurantId = @restaurantId,
                           orderDate=@orderDate
                           userId = @userId,
                           personnelId = @personnelId,
                           addressId = @addressId,
                           orderStatusId = @orderStatusId
                       WHERE orderId = @orderId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@OrderId", id);
                    command.Parameters.AddWithValue("@orderDate", currentDateTime);
                    command.Parameters.AddWithValue("@restaurantId", order.restaurantId);
                    command.Parameters.AddWithValue("@userId", order.userId);
                    command.Parameters.AddWithValue("@personnelId", order.personnelId);
                    command.Parameters.AddWithValue("@addressId", order.addressId);
                    command.Parameters.AddWithValue("@orderStatusId", order.orderStatusId);
                    command.ExecuteNonQuery();
                    
                }
            }
            return Ok("Order Updated Successfully");
        }

         // DELETE api/<RestaurantController>/3
        [HttpDelete("{id}", Name = "DeleteOrder")]
        public IActionResult Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("FoodDeliveryDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("DELETE FROM Order WHERE oderID = @orderID", connection))
                {
                    command.Parameters.AddWithValue("@orderID", id);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        return NotFound(); 
                    }
                    else
                    {
                        return Ok("Restaurant Deleted Successfully");
                    }
                }
            }
        }
    }

    
    
}