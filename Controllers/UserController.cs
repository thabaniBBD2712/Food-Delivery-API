using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FoodDeliveryAPI.Controllers
{
    [Route("api/FoodDeliver.com/v1/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserController> _logger;

        public UserService userService = new UserService();
        public OrderService orderService = new OrderService();

        public UserController(ILogger<UserController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // GET: method to retrieve all the users from the db
        [HttpGet]
        public IActionResult Get()
        {
            userService.invokeComplete();
            orderService.invokeComplete();
            userService.invokeIncomplete();
            orderService.invokeIncomplete();
            List<User> lstUsers = new List<User>();
            // Retrieve the connection string from IConfiguration
            string connectionString = _configuration.GetConnectionString("FoodDB");
            
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM AppUser", connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Create a new User object and populate its properties
                            User tempUser = new User
                            {
                                userId = reader.GetInt32("userId"),
                                userName = reader.GetString("username"),
                                userContactNumber = reader.GetString("userContactNumber"),
                            };

                            lstUsers.Add(tempUser);
                        }
                    }
                }
            }
            return Ok(lstUsers);
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            // Retrieve the connection string from IConfiguration
            string connectionString = _configuration.GetConnectionString("FoodDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT * FROM AppUser WHERE userId = @userId", connection))
                {
                    command.Parameters.AddWithValue("@userId", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            User tempUser = new User
                            {
                                userId = reader.GetInt32("userId"),
                                userName = reader.GetString("username"),
                                userContactNumber = reader.GetString("userContactNumber"),
                            };
                            return Ok(tempUser);
                        }
                        else
                        {
                            return NotFound();
                        }
                    }
                }
            }
        }

        // POST api/<UserController>
        [HttpPost]
        public IActionResult Post([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            string connectionString = _configuration.GetConnectionString("FoodDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("INSERT INTO AppUser (username, userContactNumber) VALUES (@userName, @userContactNumber)", connection))
                {
                    command.Parameters.AddWithValue("@userName", user.userName);
                    command.Parameters.AddWithValue("@userContactNumber", user.userContactNumber);
                    command.ExecuteNonQuery();
                }
            }
            return Ok();
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}", Name = "UpdateUser")]
        public IActionResult Put(int id, [FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            } 
            string connectionString = _configuration.GetConnectionString("FoodDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("UPDATE AppUser SET username = @userName, userContactNumber = @userContactNumber" +
                    " WHERE userId = @userId", connection))
                {
                    command.Parameters.AddWithValue("@userName", user.userName);
                    command.Parameters.AddWithValue("@userContactNumber", user.userContactNumber);
                    command.Parameters.AddWithValue("@userId", id);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        return NotFound(); // Return a response indicating that the ID was not found
                    }
                    else
                    {
                        return Ok("User Updated Successfully");
                    }
                }
            }
        }

        // DELETE api/<UserController>/3
        [HttpDelete("{id}", Name = "DeleteUser")]
        public IActionResult Delete(int id)
        {
            string connectionString = _configuration.GetConnectionString("FoodDB");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("DELETE FROM AppUser WHERE userId = @userId", connection))
                {
                    command.Parameters.AddWithValue("@userId", id);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        return NotFound(); // Return a response indicating that the ID was not found
                    }
                    else
                    {
                        return Ok("User Deleted Successfully");
                    }
                }
            }
        }
    }
}