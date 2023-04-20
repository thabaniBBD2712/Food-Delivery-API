using FoodDeliveryAPI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryAPI.Updaters
{
    public partial class Updater
    {
        public class UpdateOrderStatusArgs : EventArgs
        {
            public int OrderStatusId { get; }
            public int ParentId { get; }

            public UpdateOrderStatusArgs(int orderStatusId, int parentId)
            {
                OrderStatusId = orderStatusId;
                ParentId = parentId;
            }
        }
        public static event EventHandler<UpdateOrderStatusArgs>? UpdateOrderStatusEvent;

        [HttpPost("orderStatus/{parentId}")]
        public bool UpdateOrderStatus([FromBody] string status, int parentId)
        // public bool UpdateOrderStatus(string status, int parentId)
        {
            int orderStatusId = OrderStatusController.GetIdByName(status);
            if (orderStatusId != 0)
            {
                RaiseUpdateOrderStatusEvent(orderStatusId, parentId);
                return true;
            }
            else return false;
        }

        private void RaiseUpdateOrderStatusEvent(int orderStatusId, int parentId)
        {
            UpdateOrderStatusEvent?.Invoke(this, new UpdateOrderStatusArgs(orderStatusId, parentId));
        }
    }
}