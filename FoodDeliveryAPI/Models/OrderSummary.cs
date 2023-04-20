namespace FoodDeliveryAPI.Models
{
  public class OrderSummary
  {
    public string Username { get; set; }
    public string RestaurantName { get; set; }
    public string Address { get; set; }
    public string Description { get; set; }
    public List<OrderItemSummary> orderItemSummaries { get; set; }
    public decimal TotalValue { get; set; }
  }

  public class OrderItemSummary
  {

  }
}
