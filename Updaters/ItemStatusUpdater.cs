using FoodDeliveryAPI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryAPI.Updaters
{
    public partial class Updater
    {
        public class UpdateItemStatusArgs : EventArgs
        {
            public int ItemStatusId { get; }
            public int ParentId { get; }

            public UpdateItemStatusArgs(int itemStatusId, int parentId)
            {
                ItemStatusId = itemStatusId;
                ParentId = parentId;
            }
        }
        public static event EventHandler<UpdateItemStatusArgs>? UpdateItemStatusEvent;

        // [HttpPost("itemStatus/{parentId}")]
        // public bool UpdateItemStatus([FromBody] string status, int parentId)
        public bool UpdateItemStatus(string status, int parentId)
        {
            int itemStatusId = ItemStatusController.GetIdByName(status);
            if (itemStatusId != 0)
            {
                RaiseUpdateItemStatusEvent(itemStatusId, parentId);
                return true;
            }
            else return false;
        }

        private void RaiseUpdateItemStatusEvent(int itemStatusId, int parentId)
        {
            UpdateItemStatusEvent?.Invoke(this, new UpdateItemStatusArgs(itemStatusId, parentId));
        }
    }
}