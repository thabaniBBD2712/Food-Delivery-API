namespace FoodDeliveryAPI.Models
{
    public class Item
    {
        public int itemId { get; set; }
        public decimal? itemPrice { get; set; }
        public int? restaurantId { get; set; }
        public int? itemStatusId { get; set; }
        public int? itemInformationId { get; set; }
    }
}