
namespace FoodDeliveryAPI.Services
{
    public class OrderService
    {
        public CompleteNotification completeNotification = new CompleteNotification();
        public IncompleteNotification incompleteNotification = new IncompleteNotification();

        public OrderService(){
            completeNotification.OnNotify += (sender,e) => Console.WriteLine("Order: orderStatus is complete");
            incompleteNotification.OnNotify += (sender,e) => Console.WriteLine("Order: orderStatus is NOT complete");
        }

        public void invokeComplete(){
            completeNotification.Raise();
        }

        public void invokeIncomplete(){
            incompleteNotification.Raise();
        }
    }

}