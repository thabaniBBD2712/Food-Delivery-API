using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Services;

namespace FoodDeliveryAPI.Controllers
{
    [Route("api/FoodDeliver.com/v1/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<OrderController> _logger;
        private AuditService _auditService;
        private OrderServices _orderService;

        public OrderController(ILogger<OrderController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _orderService = new OrderServices();
            _auditService = new AuditService();
            _auditService.Subscribe(_orderService);
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
                using (SqlCommand command = new SqlCommand("SELECT * FROM [Order]", connection))
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
                SqlCommand command = new SqlCommand("SELECT * FROM [Order] WHERE orderId = @orderId", connection);
                command.Parameters.AddWithValue("@orderId", orderId);
                connection.Open();
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
            }
            return order;
        }

        [HttpGet("totalOrder/{orderId}")]
        public IActionResult GetTotal(int orderId) 
        {
          decimal total = _orderService.GetTotal(orderId);
          return Ok(total);
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
                using (SqlCommand command = new SqlCommand("INSERT INTO [Order] (orderDate, restaurantId, userId, personeelId, addressId, orderStatusId)" +
                "VALUES (@orderDate, @restaurantId, @userId, @personeelId, @addressId, @orderStatusId)", connection))
                {
                    command.Parameters.AddWithValue("@orderDate", currentDateTime);
                    command.Parameters.AddWithValue("@restaurantId", order.restaurantId);
                    command.Parameters.AddWithValue("@userId", order.userId);
                    command.Parameters.AddWithValue("@personeelId", order.personeelId);
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
                string query = @"UPDATE [Order]
                       SET restaurantId = @restaurantId,
                           orderDate = @orderDate,
                           userId = @userId,
                           personeelId = @personeelId,
                           addressId = @addressId,
                           orderStatusId = @orderStatusId
                       WHERE orderId = @orderId";

                using (SqlCommand command = new SqlCommand(query, connection))
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
            }
            return Ok("Order Updated Successfully");
        }
    }
}