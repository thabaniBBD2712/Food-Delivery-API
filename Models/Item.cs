using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;

namespace FoodDeliveryAPI.Models
{
    public class Item
    {

        //Column Names
        public static readonly string ITEMID = "itemId";
        public static readonly string NAME = "itemName";
        public static readonly string PRICE = "itemPrice";
        public static readonly string STATUS = "itemStatusName";
        public static readonly string CATEGORY = "itemCategoryName";
        public static readonly string DESCRIPTION = "itemDescription";

        public long ID { get; set; } = -1;
        public double Price { get; set; } = 0.0;

        // Item Information
        public int InformationID;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // Item Category
        public int CategoryID { get; set; }
        public string Category { get; set; } = string.Empty;

        // Item Status
        public int StatusID { get; set; }
        public string Status { get; set; } = string.Empty;

        // Restaurant Details
        public int RestaurantID { get; set; }
        public Item() {

        }

        public Item(SqlDataReader record)
        {
            ID = record.GetInt64(ITEMID);
            Price = record.GetDouble(PRICE);
            Name = record.GetString(NAME);
            Description = record.GetString(DESCRIPTION);
            Category = record.GetString(CATEGORY);
            Status = record.GetString(STATUS);
        }

    }
}
