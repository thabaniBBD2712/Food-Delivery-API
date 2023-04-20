using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Services;
using FoodDeliveryAPI.DatabaseAccess;
using FoodDeliveryAPI.Events;
using FoodDeliveryAPI.Updaters;

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

        public OrderController(IConfiguration configuration, ILogger<Order> logger)
        {
            _orderService = new OrderServices();
            _auditService = new AuditService();
            _auditService.Subscribe(_orderService);
            _connection = DBConnection.Instance.Connection;            
            Updater.UpdateOrderStatusEvent += onUpdateOrderStatus;

        }
        //get order details

        

        // get all orders 
        [HttpGet]
        public IActionResult Get()
        {

            List<Order> orders = new List<Order>();
            string query= @"SELECT o.orderId, o.orderDate, r.restaurantId, r.restaurantName,r.restaurantAddress,r.restaurantDescription,r.restaurantContactNumber, 
                u.userId, u.username,u.userContactNumber, 
                p.personeelId, p.personeelName,p.personeelContactNumber,p.vehicleRegistrationNumber,
                a.addressId, CONCAT(a.streetName, ', ',a.city, ', ',a.province,', ',a.postalCode) AS [Address],
                os.orderStatusId, os.orderStatusName
                FROM [Order] o
                JOIN Restaurant r ON o.restaurantId = r.restaurantId
                JOIN [AppUser] u ON o.userId = u.userId
                JOIN DeliveryPersoneel p ON o.personeelId = p.personeelId
                JOIN Address a ON o.addressId = a.addressId
                JOIN OrderStatus os ON o.orderStatusId = os.orderStatusId";

                using (SqlCommand command = new SqlCommand(query, _connection))
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
                            order.restaurantName = reader.GetString("restaurantName");
                            order.restaurantName = reader.GetString("restaurantName");
                            order.restaurantDescription = reader.GetString("restaurantDescription");
                            order.restaurantContactNumber = reader.GetString("restaurantContactNumber");
                            order.username = reader.GetString("username");
                            order.userContactNumber = reader.GetString("userContactNumber");
                            order.personeelName = reader.GetString("personeelName");
                            order.personeelContactNumber = reader.GetString("personeelContactNumber");
                            order.vehicleRegistrationNumber = reader.GetString("vehicleRegistrationNumber");
                            order.restaurantAddress = reader.GetString("Address");
                            order.orderStatusName = reader.GetString("orderStatusName");
                            

                            orders.Add(order);
                        }
                    }
                }
            return Ok(orders);
        }
        //get specific Order 
        [HttpGet("{orderId}")]
        public ActionResult<Order> Get(int orderId)
        {
          OrderSummary total = _orderService.GetOrderSummary(orderId);
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

        private OrderSummary WriteAuditLog(object sender, OrderEventArgs e)
        {
          return e.OrderSumm;
        }
  }
}