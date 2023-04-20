using FoodDeliveryAPI.Events;

namespace FoodDeliveryAPI.Services
{
  public class AuditService
  {
    public void Subscribe(OrderServices orderService)
    {
      orderService.OnOrderTotalCalculate += WriteAuditLog;
    }

    private void WriteAuditLog(object sender, OrderEventArgs e)
    {
      Console.WriteLine($"Order total {e.OrderSumm}");
    }
  }
}
