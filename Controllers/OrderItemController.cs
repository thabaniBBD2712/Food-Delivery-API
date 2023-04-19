using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using FoodDeliveryAPI.Models;
using System.Threading.Channels;

namespace FoodDeliveryAPI.Controllers
{
    [Route("api/FoodDeliver.com/v1/[controller]")]
    [ApiController]
    public class OrderItemController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<OrderItemController> _logger;
        private readonly Channel<OrderItem> channel;

        private BasketController basket;

        public OrderItemController(ILogger<OrderItemController> logger, IConfiguration configuration, Channel<OrderItem> channel)
        {
            _logger = logger;
            _configuration = configuration;
            this.channel = channel;
            basket = new BasketController(configuration, channel);
            basket.OnCheckout += (sender, e) => this.checkout(e);
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<OrderItem> lstOrderItems = new List<OrderItem>();
            string connectionString = _configuration.GetConnectionString("FoodDB");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM OrderItem", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            OrderItem orderItem = new OrderItem
                            {
                                orderItemId = reader.GetInt32("orderItemId"),
                                orderItemQuantity = reader.GetInt32("orderItemQuantity"),
                                orderItemPrice = reader.GetDecimal("orderItemPrice"),
                                orderId = reader.GetInt32("orderId"),
                                itemInformationId = reader.GetInt32("itemInformationId")
                            };

                            lstOrderItems.Add(orderItem);
                        }
                    }
                }
            }
            return Ok(lstOrderItems);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            string connectionString = _configuration.GetConnectionString("FoodDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM OrderItem WHERE orderItemId = @orderItemId", connection))
                {
                    command.Parameters.AddWithValue("@orderItemId", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            OrderItem orderItem = new OrderItem
                            {
                                orderItemId = reader.GetInt32("orderItemId"),
                                orderItemQuantity = reader.GetInt32("orderItemQuantity"),
                                orderItemPrice = reader.GetDecimal("orderItemPrice"),
                                orderId = reader.GetInt32("orderId"),
                                itemInformationId = reader.GetInt32("itemInformationId")
                            };

                            return Ok(orderItem);
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] OrderItem orderItem)
        {
            if (orderItem == null)
            {
                return BadRequest();
            }
            string connectionString = _configuration.GetConnectionString("FoodDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("INSERT INTO OrderItem (orderItemQuantity, orderItemPrice, orderId, itemInformationId)" +
                    " VALUES (@orderItemQuantity, @orderItemPrice, @orderId, @itemInformationId)", connection))
                {
                    command.Parameters.AddWithValue("@orderItemQuantity", orderItem.orderItemQuantity);
                    command.Parameters.AddWithValue("@orderItemPrice", orderItem.orderItemPrice);
                    command.Parameters.AddWithValue("@orderId", orderItem.orderId);
                    command.Parameters.AddWithValue("@itemInformationId", orderItem.itemInformationId);
                    command.ExecuteNonQuery();
                }
            }
            return Ok();
        }

        [HttpPut("{id}", Name = "UpdateOrderItem")]
        public IActionResult Put(int id, [FromBody] OrderItem orderItem)
        {
            if (orderItem == null || orderItem.orderItemId != id)
            {
                return BadRequest();
            }
            string connectionString = _configuration.GetConnectionString("FoodDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("UPDATE OrderItem SET orderItemQuantity = @orderItemQuantity," +
                    " orderItemPrice = @orderItemPrice, orderId = @orderId, itemInformationId = @itemInformationId " +
                    "WHERE orderItemId = @orderItemId", connection))
                {
                    command.Parameters.AddWithValue("@orderItemQuantity", orderItem.orderItemQuantity);
                    command.Parameters.AddWithValue("@orderItemPrice", orderItem.orderItemPrice);
                    command.Parameters.AddWithValue("@orderId", orderItem.orderId);
                    command.Parameters.AddWithValue("@itemInformationId", orderItem.itemInformationId);
                    command.Parameters.AddWithValue("@orderItemId", id);
                    command.ExecuteNonQuery();
                }
            }
            return Ok("OrderItem Updated Successfully");
        }

        [HttpDelete("{id}", Name = "DeleteOrderItem")]
        public IActionResult Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("FoodDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("DELETE FROM OrderItem WHERE orderItemId = @orderItemId", connection))
                {
                    command.Parameters.AddWithValue("@orderItemId", id);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        return NotFound();
                    }
                    else
                    {
                        return Ok("OrderItem Deleted Successfully");
                    }
                }
            }
        }

        private void checkout(List<OrderItem> orderItems)
        {
            foreach (OrderItem orderItem in orderItems)
            {
                Post(orderItem);
            }
        }
    }
}