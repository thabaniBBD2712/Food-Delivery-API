using FoodDeliveryAPI.Models;

namespace FoodDeliveryAPI.Events
{
  public class OrderEventArgs: EventArgs
  {
    public OrderSummary OrderSumm { get; set; }
    public OrderEventArgs(OrderSummary orderSumm) 
    {
      OrderSumm = orderSumm;
    }

  }
}
