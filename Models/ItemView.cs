using Microsoft.SqlServer.Server;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

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
        public static readonly string INFORMATION_ID = "itemInformationId";
        public static readonly string CATEGORY_ID = "itemCategoryId";
        public static readonly string STATUS_ID = "itemStatusId";
        public static readonly string RESTAURANT_ID = "restaurantId";

        public long ID { get; set; } = -1;
        public double Price { get; set; } = 0.0;

        // Item Information
        public int InformationID { get; set; }
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

        public Item(ref SqlDataReader record)
        {
            try
            {
                ID = record.GetInt32(ITEMID);
                InformationID = record.GetInt32(INFORMATION_ID);
                StatusID = record.GetInt32(STATUS_ID);
                RestaurantID = record.GetInt32(RESTAURANT_ID);
                CategoryID = record.GetInt32(CATEGORY_ID);

                Price = record.GetSqlMoney(1).ToDouble();
                Name = record.GetString(NAME);
                Description = record.GetString(DESCRIPTION);
                Category = record.GetString(CATEGORY);
                Status = record.GetString(STATUS);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            
        }

    }
}
