using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.DatabaseAccess;

namespace FoodDeliveryAPI.Controllers
{
    [Route("api/FoodDelivery.com/v1/[controller]")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly SqlConnection _connection;

        public AddressController()
        {
            _connection = DBConnection.Instance.Connection;
        }

        // GET: retrieve all addresses from db
        [HttpGet]
        public IActionResult Get()
        {
            List<Address> addresses = new();
            using (SqlDataReader reader = new SqlCommand("SELECT * FROM Address", _connection)
                .ExecuteReader())
            {
                while (reader.Read())
                {
                    Address address = new Address
                    {
                        postalCode = reader.GetString("postalCode"),
                        province = reader.GetString("province"),
                        city = reader.GetString("city"),
                        streetName = reader.GetString("streetName"),
                        addressId = reader.GetInt32("addressId"),
                    };

                    addresses.Add(address);
                }
            }
            return Ok(addresses);
        }

        // GET: retrieve single address by id from db
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            using (SqlCommand command = new SqlCommand("SELECT * FROM Address WHERE addressId = @addressId", _connection))
            {
                command.Parameters.AddWithValue("@addressId", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Ok(new Address
                        {
                            postalCode = reader.GetString("postalCode"),
                            province = reader.GetString("province"),
                            city = reader.GetString("city"),
                            streetName = reader.GetString("streetName"),
                            addressId = reader.GetInt32("addressId"),
                        });
                    }
                    else
                    {
                        return NotFound($"Address with id {id} was not found");
                    }
                }
            }
        }

        // POST: add single address to db
        [HttpPost]
        public IActionResult Post([FromBody] Address address)
        {
            if (address == null)
            {
                return BadRequest();
            }
            using (SqlCommand command = new SqlCommand(
                "INSERT INTO Address (postalCode, province, city, streetName) " +
                "VALUES (@postalCode, @province, @city, @streetName)", _connection
            ))
            {

                command.Parameters.AddWithValue("@postalCode", address.postalCode);
                command.Parameters.AddWithValue("@province", address.province);
                command.Parameters.AddWithValue("@city", address.city);
                command.Parameters.AddWithValue("@streetName", address.streetName);
                command.ExecuteNonQuery();
            }
            return Ok("Address successfully posted");
        }

        // PUT: update single address in db by id 
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Address address)
        {
            if (address == null || address.addressId != id)
            {
                return BadRequest();
            }
            using (SqlCommand command = new SqlCommand(
                "UPDATE Address " +
                "SET streetName = @streetName, city = @city, province = @province, postalCode = @postalCode " +
                "WHERE addressId = @addressId", _connection
            ))
            {
                command.Parameters.AddWithValue("@streetName", address.streetName);
                command.Parameters.AddWithValue("@city", address.city);
                command.Parameters.AddWithValue("@province", address.province);
                command.Parameters.AddWithValue("@postalCode", address.postalCode);
                command.Parameters.AddWithValue("@addressId", id);
                if (command.ExecuteNonQuery() == 0)
                {
                    return NotFound($"Address with id {id} was not found");
                }
            }
            return Ok("Address successfully updated");
        }

        // DELETE: delete single address from db by id
        [HttpDelete("{id}", Name = "DeleteAddress")]
        public IActionResult Delete(int id)
        {
            using (SqlCommand command = new SqlCommand(
                "DELETE FROM Address WHERE addressId = @addressId", _connection
            ))
            {
                command.Parameters.AddWithValue("@addressId", id);
                if (command.ExecuteNonQuery() == 0)
                {
                    return NotFound($"Address with id {id} was not found");
                }
            }
            return Ok("Address successfully deleted");
        }
    }

}