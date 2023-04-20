namespace FoodDeliveryAPI.Models
{
  public class OrderSummary
  {
    public string Username { get; set; }
    public string ContactNumber { get; set; }

    public DateTime orderDate { get; set; }

    public string RestaurantName { get; set; }
    public string RestaurantAddress { get; set; }
    public string RestaurantDescription { get; set; }
    public string RestaurantContactNumber { get; set; }
    public string PersoneelName { get; set; }
    public string PersoneelContactNumber { get; set; }
    public string VehicleRegistrationNumber { get; set; }
    public string OrderStatusName { get; set; }
    public List<OrderItemSummary> OrderItemSummaries { get; set; }
    public decimal TotalValue { get; set; }
  }

  public class OrderItemSummary
  {

  }
}
