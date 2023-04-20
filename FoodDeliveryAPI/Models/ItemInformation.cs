namespace FoodDeliveryAPI.Models
{
    public class ItemInformation
    {
        public int itemInformationId { get; set; }
        public string? itemName { get; set; }
        public string? itemDescription { get; set; }
        public int itemCategoryId { get; set; }
    }
}
