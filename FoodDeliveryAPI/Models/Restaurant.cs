namespace FoodDeliveryAPI.Models
{
    public class Restaurant
    {
        public int restaurantId { get; set; }
        public string? restaurantUserName { get; set; }
        public string? restaurantName { get; set; }
        public int? restaurantAddress { get; set; }
        public string? restaurantDescription { get; set; }
        public string? restaurantContactNumber { get; set; }
    }
}