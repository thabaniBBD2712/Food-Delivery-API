﻿using FoodDeliveryAPI.DatabaseAccess;
using FoodDeliveryAPI.Events;
using FoodDeliveryAPI.Models;
using System.Data;
using System.Data.SqlClient;

namespace FoodDeliveryAPI.Services
{
  public class OrderServices
  {
    private readonly SqlConnection _connection;
    public delegate void OnOrderTotalCalculateHandler(object sender, OrderEventArgs orderEventArgs);
    public event OnOrderTotalCalculateHandler OnOrderTotalCalculate;
    public OrderServices() 
    {
      _connection = DBConnection.Instance.Connection;
    }
    
    public OrderSummary GetOrderSummary(int id)
    { 
      OrderSummary summary = OrderSummary(id);
      OrderEventArgs args = new OrderEventArgs(summary);
      OnOrderTotalCalculateHandler e = OnOrderTotalCalculate;
      if (e != null)
      {
        e(this, args);
      }
      return summary;
    }

    private decimal CalculateTotal(int id)
    {
      
      string sqlStatement = @"SELECT COALESCE(SUM(oi.orderItemPrice * oi.orderItemQuantity), 0)
                              FROM [Order] o INNER JOIN OrderItem oi
                              ON o.orderID=oi.orderID WHERE oi.orderID=@id";
      using (SqlCommand command = new SqlCommand(sqlStatement, _connection))
      {
        command.Parameters.AddWithValue("id", id);
        using (SqlDataReader reader = command.ExecuteReader())
        {
          if (reader.Read())
          {
            return reader.GetDecimal(0);
          }
        }
      }
      return 0;
    }

        private List<OrderItemSummary> GetOrderItemSummaries(int id)
        {

            List<OrderItemSummary> orderItemSummaries = new List<OrderItemSummary>();
            string sqlStatement = @"
                SELECT 
                OI.orderItemQuantity, OI.orderItemPrice, II.itemName, II.itemDescription
                FROM [OrderItem] OI
                JOIN [ItemInformation] II	ON OI.itemInformationId = II.itemInformationId
                WHERE orderId = @order_id";
            using (SqlCommand command = new SqlCommand(sqlStatement, _connection))
            {
                command.Parameters.AddWithValue("order_id", id);
                SqlDataReader reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    OrderItemSummary itemSummary = new OrderItemSummary();
                    itemSummary.OrderItemName = reader.GetString("ItemName");
                    itemSummary.OrderItemDescription = reader.GetString("ItemDescription");
                    itemSummary.OrderItemPrice = reader.GetDecimal("orderItemPrice");
                    itemSummary.OrderItemQty = reader.GetInt32("orderItemQuantity");
                    orderItemSummaries.Add(itemSummary);
                }
            }

            return orderItemSummaries;
        } 
    
    private OrderSummary OrderSummary(int id)
    {
      decimal total = CalculateTotal(id);
      string sqlStatement = @"SELECT o.orderId, o.orderDate, r.restaurantName,r.restaurantAddress,r.restaurantDescription,r.restaurantContactNumber, 
                u.username,u.userContactNumber, 
                p.personeelName,p.personeelContactNumber,p.vehicleRegistrationNumber,
                CONCAT(a.streetName, ', ',a.city, ', ',a.province,', ',a.postalCode) AS [Address],
                os.orderStatusName
                FROM [Order] o
                JOIN Restaurant r ON o.restaurantId = r.restaurantId
                JOIN [AppUser] u ON o.userId = u.userId
                JOIN DeliveryPersoneel p ON o.personeelId = p.personeelId
                JOIN Address a ON o.addressId = a.addressId
                JOIN OrderStatus os ON o.orderStatusId = os.orderStatusId WHERE o.OrderId=@id;";
      using (SqlCommand command = new SqlCommand(sqlStatement, _connection))
      {
        command.Parameters.AddWithValue("id", id);
        using (SqlDataReader reader = command.ExecuteReader())
        {
          if (reader.Read())
          {
            OrderSummary order = new OrderSummary();

            order.Id = reader.GetInt32("orderId");
            order.RestaurantName = reader.GetString("restaurantName");
            order.RestaurantName = reader.GetString("restaurantName");
            order.RestaurantDescription = reader.GetString("restaurantDescription");
            order.RestaurantContactNumber = reader.GetString("restaurantContactNumber");
            order.Username = reader.GetString("username");
            order.ContactNumber = reader.GetString("userContactNumber");
            order.PersoneelName = reader.GetString("personeelName");
            order.PersoneelContactNumber = reader.GetString("personeelContactNumber");
            order.VehicleRegistrationNumber = reader.GetString("vehicleRegistrationNumber");
            order.RestaurantAddress = reader.GetString("Address");
            order.OrderStatusName = reader.GetString("orderStatusName");
            order.OrderItemSummaries = GetOrderItemSummaries(order.Id);
            order.TotalValue = total;
            return order;
          }
        }
      }
      return null;
    }

  }
}
