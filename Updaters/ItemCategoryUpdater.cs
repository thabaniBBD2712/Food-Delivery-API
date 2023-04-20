using FoodDeliveryAPI.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace FoodDeliveryAPI.Updaters
{
    public partial class Updater
    {
        public class UpdateItemCategoryArgs : EventArgs
        {
            public int ItemCategoryId { get; }
            public int ParentId { get; }

            public UpdateItemCategoryArgs(int itemCategoryId, int parentId)
            {
                ItemCategoryId = itemCategoryId;
                ParentId = parentId;
            }
        }
        public static event EventHandler<UpdateItemCategoryArgs>? UpdateItemCategoryEvent;

        [HttpPost("itemCategory/{parentId}")]
        public bool UpdateItemCategory([FromBody] string category, int parentId)
        // public bool UpdateItemCategory(string category, int parentId)
        {
            int itemCategoryId = ItemCategoryController.GetIdByName(category);
            if (itemCategoryId != 0)
            {
                RaiseUpdateItemCategoryEvent(itemCategoryId, parentId);
                return true;
            }
            else return false;
        }

        private void RaiseUpdateItemCategoryEvent(int itemCategoryId, int parentId)
        {
            UpdateItemCategoryEvent?.Invoke(this, new UpdateItemCategoryArgs(itemCategoryId, parentId));
        }
    }
}