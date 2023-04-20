using FoodDeliveryAPI.Events;
using FoodDeliveryAPI.Models;

namespace FoodDeliveryAPI.Services
{
  public class AuditService
  {
    public void Subscribe(OrderServices orderService)
    {
      orderService.OnOrderTotalCalculate += new OrderServices.OnOrderTotalCalculateHandler(WriteAuditLog);
    }

    private void WriteAuditLog(object sender, OrderEventArgs e)
    {
      Console.WriteLine($"LOG: Order summary has been requested for user {e.OrderSumm.Username} for order number {e.OrderSumm.Id}");
    }
  }
}
