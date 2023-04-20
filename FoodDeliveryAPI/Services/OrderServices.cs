using FoodDeliveryAPI.DatabaseAccess;
using FoodDeliveryAPI.Events;
using FoodDeliveryAPI.Models;
using System.Data.SqlClient;

namespace FoodDeliveryAPI.Services
{
  public class OrderServices
  {
    private readonly SqlConnection _connection;
    public delegate decimal OnOrderTotalCalculateHandler(object sender, OrderEventArgs orderEventArgs);
    public event EventHandler<OrderEventArgs> OnOrderTotalCalculate;
    public OrderServices() 
    {
      _connection = DBConnection.Instance.Connection;
    }
    
    public decimal GetTotal(int id)
    { 
      decimal total = CalculateTotal(id);
      OrderEventArgs args = new OrderEventArgs(total);
      if (OnOrderTotalCalculate != null)
      {
        OnOrderTotalCalculate(this, args);
      }
      return total;
    }

    private decimal CalculateTotal(int id)
    {
      
      
      string sqlStatement = @"SELECT SUM(oi.orderItemPrice * oi.orderItemQuantity)
                              FROM [Order] o INNER JOIN OrderItem oi
                              ON o.orderID=oi.orderID WHERE oi.orderID= @id";
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

  }
}
