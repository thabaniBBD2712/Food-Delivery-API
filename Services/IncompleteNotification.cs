namespace FoodDeliveryAPI.Services
{
    public class IncompleteNotification
    {
        public event EventHandler<int> OnNotify = delegate { };

        public void Raise()
        {
            OnNotify(this,1);
        }
    }

}