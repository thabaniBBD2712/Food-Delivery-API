namespace FoodDeliveryAPI.Models
{
  public class OrderSummary
  {
    public int Id { get; set; }
    public string Username { get; set; }
    public string ContactNumber { get; set; }

    public DateTime orderDate { get; set; }

    public string RestaurantName { get; set; }
    public string RestaurantAddress { get; set; }
    public string RestaurantDescription { get; set; }
    public string RestaurantContactNumber { get; set; }
    public string PersoneelName { get; set; }
    public string PersoneelContactNumber { get; set; }
    public string VehicleRegistrationNumber { get; set; }
    public string OrderStatusName { get; set; }
    public List<OrderItemSummary> OrderItemSummaries { get; set; }
    public decimal TotalValue { get; set; }
  }


    /**
     * 
SELECT 
OI.orderItemId, OI.orderItemQuantity, OI.orderItemPrice, II.itemName, II.itemDescription
FROM [OrderItem] OI
JOIN [ItemInformation] II	ON OI.itemInformationId = II.itemInformationId
WHERE orderId = 7
     */
    public class OrderItemSummary
  {
        public string OrderItemName { get; set; }
        public string OrderItemDescription { get; set; }   
        public Decimal OrderItemPrice { get; set; }
        public Int32 OrderItemQty { get; set; }
        

  }
}
