namespace FoodDeliveryAPI.Models
{

    public class Order
{
    public int orderId { get; set; }
    public DateTime? orderDate {get; set;}
    public int restaurantId { get; set; }
    public int userId { get; set; }
    public int personeelId { get; set; }
    public int addressId { get; set; }
    public int orderStatusId { get; set; }
    public string ?restaurantName { get; set; }
    public string ?restaurantAddress { get; set; }
    public string ?restaurantDescription { get; set; }
    public string ?restaurantContactNumber { get; set; }
    public string ?username { get; set; }
    public string ?userContactNumber { get; set; }
    public string ?personeelName { get; set; }
    public string ?personeelContactNumber { get; set; }
    public string ?vehicleRegistrationNumber { get; set; }
    public string ?orderStatusName { get; set; }
}


}