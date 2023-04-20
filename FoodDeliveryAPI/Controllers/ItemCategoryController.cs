using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using FoodDeliveryAPI.Models;
using FoodDeliveryAPI.DatabaseAccess;

namespace FoodDeliveryAPI.Controllers
{
    [Route("api/FoodDelivery.com/v1/[controller]")]
    [ApiController]
    public class ItemCategoryController : ControllerBase
    {
        private readonly SqlConnection _connection;

        public ItemCategoryController()
        {
            _connection = DBConnection.Instance.Connection;
        }

        // GET: retrieve all itemCategories from db
        [HttpGet]
        public IActionResult Get()
        {
            List<ItemCategory> itemCategories = new();
            using (SqlDataReader reader = new SqlCommand("SELECT * FROM ItemCategory", _connection)
                .ExecuteReader())
            {
                while (reader.Read())
                {
                    ItemCategory itemCategory = new ItemCategory
                    {
                        itemCategoryId = reader.GetInt32("itemCategoryId"),
                        itemCategoryName = reader.GetString("itemCategoryName")
                    };

                    itemCategories.Add(itemCategory);
                }
            }
            return Ok(itemCategories);
        }

        // GET: retrieve single itemCategory by id from db
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            using (SqlCommand command = new SqlCommand("SELECT * FROM ItemCategory WHERE itemCategoryId = @itemCategoryId", _connection))
            {
                command.Parameters.AddWithValue("@itemCategoryId", id);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return Ok(new ItemCategory
                        {
                            itemCategoryId = reader.GetInt32("itemCategoryId"),
                            itemCategoryName = reader.GetString("itemCategoryName")
                        });
                    }
                    else
                    {
                        return NotFound($"ItemCategory with id {id} was not found");
                    }
                }
            }
        }

        public static int GetIdByName(string name)
        {
            using (SqlCommand command = new SqlCommand(
                @"SELECT TOP (1) [itemCategoryId]
                FROM [FoodDeliveryDB].[dbo].[ItemCategory] category
                WHERE category.itemCategoryName = @name", DBConnection.Instance.Connection
            ))
            {
                command.Parameters.AddWithValue("@name", name);
                object result = command.ExecuteScalar();
                return result == null ? 0 : (Int32)result;
            }
        }
    }
}