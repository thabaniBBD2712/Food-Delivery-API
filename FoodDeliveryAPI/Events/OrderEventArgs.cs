using FoodDeliveryAPI.Models;

namespace FoodDeliveryAPI.Events
{
  public class OrderEventArgs: EventArgs
  {
    public decimal TotalValue { get; set; }
    public OrderEventArgs(decimal totalValue) 
    {
      TotalValue = totalValue;
    }

  }
}
