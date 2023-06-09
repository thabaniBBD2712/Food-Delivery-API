﻿using FoodDeliveryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;

namespace FoodDeliveryAPI.Controllers
{
  [Route("api/FoodDeliver.com/v1/[controller]")]
  [ApiController]
  public class DeliveryPersonnelController : ControllerBase
  {
    private readonly IConfiguration _configuration;
    private readonly ILogger<DeliveryPersonnel> _logger;

    public DeliveryPersonnelController(IConfiguration configuration, ILogger<DeliveryPersonnel> logger)
    {
      _configuration = configuration;
      _logger = logger;
    }
    [HttpGet]
    public IActionResult Get()
    {
      List<DeliveryPersonnel> personnelList = new List<DeliveryPersonnel>();
      string connectionString = _configuration.GetConnectionString("FoodDB");

      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        connection.Open();
        string sqlStatement = "SELECT * FROM DeliveryPersoneel";

        using (SqlCommand command = new SqlCommand(sqlStatement, connection))
        {
          using (SqlDataReader reader = command.ExecuteReader())
          {
            while (reader.Read())
            {
              DeliveryPersonnel personnel = new DeliveryPersonnel();
              personnel.PersonnelId = reader.GetInt32("personeelId");
              personnel.PersonnelName = reader.GetString("personeelName");
              personnel.PersonnelContactNumber = reader.GetString("personeelContactNumber");
              personnel.VehicleRegistrationNumber = reader.GetString("vehicleRegistrationNumber");

              personnelList.Add(personnel);
            }
          }
        }
      }

      return Ok(personnelList);
    }
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
      string connectionString = _configuration.GetConnectionString("FoodDB");

      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        connection.Open();
        string sqlStatement = "SELECT * FROM DeliveryPersoneel WHERE personeelId = @personeelId";

        using (SqlCommand command = new SqlCommand(sqlStatement, connection))
        {
          command.Parameters.AddWithValue("personeelId", id);
          using (SqlDataReader reader = command.ExecuteReader())
          {
            if (reader.Read())
            {
              DeliveryPersonnel personnel = new DeliveryPersonnel();
              personnel.PersonnelId = reader.GetInt32("personeelId");
              personnel.PersonnelName = reader.GetString("personeelName");
              personnel.PersonnelContactNumber = reader.GetString("personeelContactNumber");
              personnel.VehicleRegistrationNumber = reader.GetString("vehicleRegistrationNumber");

              return Ok(personnel);
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
    public IActionResult InsertPersonel([FromBody] DeliveryPersonnel personnel) 
    {
      if (personnel == null)
      {
        return BadRequest();
      }

      string connectionString = _configuration.GetConnectionString("FoodDB");

      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        connection.Open();
        string sqlStatement = @"INSERT INTO DeliveryPersoneel(personeelName, personeelContactNumber, vehicleRegistrationNumber) 
                                VALUES (@personnelName, @personnelContactNumber, @registrationNumber)";

        using (SqlCommand command = new SqlCommand(sqlStatement, connection))
        {
          command.Parameters.AddWithValue("@personnelName", personnel.PersonnelName);
          command.Parameters.AddWithValue("@personnelContactNumber", personnel.PersonnelContactNumber);
          command.Parameters.AddWithValue("@registrationNumber", personnel.VehicleRegistrationNumber);
          command.ExecuteNonQuery();
        }
      }
      
      return Ok("New delivery personnel added successfully");
    }

    [HttpPut("{id}", Name = "updateDeliveryPersonnel")]
    public IActionResult UpdatePersonnel(int id, [FromBody] DeliveryPersonnel personnel)
    {
      if (personnel == null)
      {
        return BadRequest();
      }
      string connectionString = _configuration.GetConnectionString("FoodDB");

      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        connection.Open();
        string sqlStatement = @"UPDATE DeliveryPersoneel 
                                SET personeelName = @personnelName, 
                                  personeelContactNumber = @personnelContactNumber, 
                                  vehicleRegistrationNumber = @registrationNumber
                                WHERE personeelId = @id";

        using (SqlCommand command = new SqlCommand(sqlStatement, connection))
        {
          command.Parameters.AddWithValue("@personnelName", personnel.PersonnelName);
          command.Parameters.AddWithValue("@personnelContactNumber", personnel.PersonnelContactNumber);
          command.Parameters.AddWithValue("@registrationNumber", personnel.VehicleRegistrationNumber);
          command.Parameters.AddWithValue("@id", id);
          command.ExecuteNonQuery();
        }
      }
      return Ok("Delivery personnel updated successfully");
    }
    [HttpDelete("{id}", Name = "deleteDeliveryPersonnel")]
    
    public IActionResult DeletePersonnel(int id)
    {
      string connectionString = _configuration.GetConnectionString("FoodDB");

      using (SqlConnection connection = new SqlConnection(connectionString))
      {
        connection.Open();
        string sqlStatement = @"DELETE  FROM DeliveryPersoneel
                                 WHERE personeelId = @id";

        using (SqlCommand command = new SqlCommand(sqlStatement, connection))
        {
          command.Parameters.AddWithValue("@id", id);
          command.ExecuteNonQuery();
        }
      }
      return Ok("Record of delivery personnel has been deleted successfully");
    }
  }
}
