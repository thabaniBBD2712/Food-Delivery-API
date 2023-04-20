namespace FoodDeliveryAPI.Models
{
    public class Address
    {
        public int addressId { get; set; }
        public string? streetName { get; set; }
        public string? city { get; set; }
        public string? province { get; set; }
        public string? postalCode { get; set; }
    }
}