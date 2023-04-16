namespace FoodDeliveryAPI.Models
{
    public class OrderItem
    {
        public int orderItemId { get; set; }
        public int? orderItemQuantity { get; set; }
        public decimal? orderItemPrice { get; set; }
        public int? orderId { get; set; }
        public int? itemInformationId { get; set; }
    }
}
