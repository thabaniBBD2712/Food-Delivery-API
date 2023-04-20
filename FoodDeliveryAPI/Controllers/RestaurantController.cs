using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.DatabaseAccess;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FoodDeliveryAPI.Controllers
{
    [Route("api/FoodDeliver.com/v1/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase
    {
        private readonly SqlConnection _connection;

        public RestaurantController()
        {
            _connection = DBConnection.Instance.Connection;
        }

        // GET: method to retrieve all the restaurants from the db
        [HttpGet]
        public IActionResult Get()
        {
            List<Restaurant> lstRestaurants = new List<Restaurant>();

            using (SqlDataReader reader = new SqlCommand("SELECT * FROM Restaurant", _connection)
             .ExecuteReader())
            {
                while (reader.Read())
                {
                    // Create a new Restaurant object and populate its properties
                    Restaurant res = new Restaurant
                    {
                        restaurantId = reader.GetInt32("restaurantId"),
                        restaurantUserName = reader.GetString("restaurantUserName"),
                        restaurantName = reader.GetString("restaurantName"),
                        restaurantAddress = reader.GetInt32("restaurantAddress"),
                        restaurantDescription = reader.GetString("restaurantDescription"),
                        restaurantContactNumber = reader.GetString("restaurantContactNumber")
                    };

                    lstRestaurants.Add(res);
                }
            }
            return Ok(lstRestaurants);
        }

        // GET api/<RestaurantController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            using (SqlCommand command = new SqlCommand("SELECT * FROM Restaurant WHERE restaurantId = @restaurantId", _connection))
            {
                command.Parameters.AddWithValue("@restaurantId", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Restaurant res = new Restaurant
                        {
                            restaurantId = reader.GetInt32("restaurantId"),
                            restaurantUserName = reader.GetString("restaurantUserName"),
                            restaurantName = reader.GetString("restaurantName"),
                            restaurantAddress = reader.GetInt32("restaurantAddress"),
                            restaurantDescription = reader.GetString("restaurantDescription"),
                            restaurantContactNumber = reader.GetString("restaurantContactNumber")
                        };

                        return Ok(res);
                    }
                    else
                    {
                        return NotFound($"Restaurant with id {id} was not found");
                    }
                }
            }
        }

        // POST api/<RestaurantController>
        [HttpPost]
        public IActionResult Post([FromBody] Restaurant restaurant)
        {
            if (restaurant == null)
            {
                return BadRequest();
            }
            using (SqlCommand command = new SqlCommand("INSERT INTO Restaurant (restaurantUserName, restaurantName, restaurantAddress, restaurantDescription, restaurantContactNumber)" +
                " VALUES (@restaurantUserName, @restaurantName, @restaurantAddress, @restaurantDescription, @restaurantContactNumber)", _connection))
            {
                command.Parameters.AddWithValue("@restaurantUserName", restaurant.restaurantUserName);
                command.Parameters.AddWithValue("@restaurantName", restaurant.restaurantName);
                command.Parameters.AddWithValue("@restaurantAddress", restaurant.restaurantAddress);
                command.Parameters.AddWithValue("@restaurantDescription", restaurant.restaurantDescription);
                command.Parameters.AddWithValue("@restaurantContactNumber", restaurant.restaurantContactNumber);
                command.ExecuteNonQuery();
            }
            return Ok("Restaurant Added");
        }

        // PUT api/<RestaurantController>/5
        [HttpPut("{id}", Name = "UpdateRestaurant")]
        public IActionResult Put(int id, [FromBody] Restaurant restaurant)
        {
            if (restaurant == null || restaurant.restaurantId != id)
            {
                return BadRequest();
            }
            using (SqlCommand command = new SqlCommand("UPDATE Restaurant SET restaurantUserName = @restaurantUserName," +
                " restaurantName = @restaurantName, restaurantAddress = @restaurantAddress, restaurantDescription= @restaurantDescription, " +
                "restaurantContactNumber = @restaurantContactNumber WHERE restaurantId = @restaurantId", _connection))
            {
                command.Parameters.AddWithValue("@restaurantUserName", restaurant.restaurantUserName);
                command.Parameters.AddWithValue("@restaurantName", restaurant.restaurantName);
                command.Parameters.AddWithValue("@restaurantAddress", restaurant.restaurantAddress);
                command.Parameters.AddWithValue("@restaurantDescription", restaurant.restaurantDescription);
                command.Parameters.AddWithValue("@restaurantContactNumber", restaurant.restaurantContactNumber);
                command.Parameters.AddWithValue("@restaurantId", id);
                if (command.ExecuteNonQuery() == 0)
                {
                    return NotFound($"Restaurant with id {id} was not found");
                }
                command.ExecuteNonQuery();
            }
            return Ok("Restaurant Updated Successfully");
        }

        // DELETE api/<RestaurantController>/3
        [HttpDelete("{id}", Name = "DeleteRestaurant")]
        public IActionResult Delete(int id)
        {
            using (SqlCommand command = new SqlCommand("DELETE FROM Restaurant WHERE restaurantId = @restaurantId", _connection))
            {
                command.Parameters.AddWithValue("@restaurantId", id);
                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    return NotFound(); // Return a response indicating that the ID was not found
                }
                else
                {
                    return Ok("Restaurant Deleted Successfully");
                }
            }
        }
    }
}