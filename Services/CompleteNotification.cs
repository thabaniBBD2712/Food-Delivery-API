namespace FoodDeliveryAPI.Services
{
    public class CompleteNotification
    {
        public event EventHandler<int> OnNotify = delegate { };

        public void Raise()
        {
            OnNotify(this,1);
        }
    }

}