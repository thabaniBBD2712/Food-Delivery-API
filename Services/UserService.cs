
namespace FoodDeliveryAPI.Services
{
    public class UserService
    {
        public CompleteNotification completeNotification = new CompleteNotification();
        public IncompleteNotification incompleteNotification = new IncompleteNotification();

        public UserService(){
            completeNotification.OnNotify += (sender,e) =>  Console.WriteLine("User: orderStatus is complete");
            incompleteNotification.OnNotify += (sender,e) =>  Console.WriteLine("User: orderStatus is NOT complete");
        }

        public void invokeComplete(){
            completeNotification.Raise();
        }

        public void invokeIncomplete(){
            incompleteNotification.Raise();
        }
    }

}