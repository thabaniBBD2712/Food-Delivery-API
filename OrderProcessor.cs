using FoodDeliveryAPI.Models;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Channels;

namespace FoodDeliveryAPI;

//takes a message and processes it {works as a background thread}
public class OrderProcessor : IHostedService
{
    private readonly Channel<OrderItem> channel;
    private readonly IConfiguration _configuration;

    public OrderProcessor(Channel<OrderItem> channel, IConfiguration configuration)
    {
        this.channel = channel;
        _configuration = configuration;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // functions that does all the processing
        return Task.Factory.StartNew(async () =>
         {
             while (!channel.Reader.Completion.IsCompleted)
             {
                 var response = await channel.Reader.ReadAsync();
                 if (response == null)
                 {
                     Console.WriteLine("Sorry");
                 }
                 else
                 {
                     string connectionString = _configuration.GetConnectionString("FoodDB");
                     using (SqlConnection connection = new SqlConnection(connectionString))
                     {
                         connection.Open();
                         using (SqlCommand command = new SqlCommand("INSERT INTO OrderItem (orderItemQuantity, orderItemPrice, orderId, itemInformationId)" +
                             " VALUES (@orderItemQuantity, @orderItemPrice, @orderId, @itemInformationId)", connection))
                         {
                             command.Parameters.AddWithValue("@orderItemQuantity", response.orderItemQuantity);
                             command.Parameters.AddWithValue("@orderItemPrice", response.orderItemPrice);
                             command.Parameters.AddWithValue("@orderId", response.orderId);
                             command.Parameters.AddWithValue("@itemInformationId", response.itemInformationId);
                             command.ExecuteNonQuery();
                             Console.WriteLine("Checkout successful");
                             if (response.orderId != null)
                             {
                                 string status = updateOrderStatus(connection, (int)response.orderId);
                                 Console.WriteLine(status);
                             }
                         }
                     }
                 }
                 Console.WriteLine(response);
             }
         });
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private string updateOrderStatus(SqlConnection connection, int orderId)
    {
        Order order = new Order();
        SqlCommand command = new SqlCommand("SELECT * FROM [Order] WHERE orderId = @orderId", connection);
        command.Parameters.AddWithValue("@orderId", orderId);
        SqlDataReader reader = command.ExecuteReader();
        //get the particular order
        if (reader.Read())
        {
            order.orderId = (int)reader["orderId"];
            order.orderDate = reader.GetDateTime("orderDate");
            order.restaurantId = (int)reader["restaurantId"];
            order.userId = (int)reader["userId"];
            order.personeelId = (int)reader["personeelId"];
            order.addressId = (int)reader["addressId"];
            order.orderStatusId = (int)reader["orderStatusId"];
        }
        else
        {
            return "Order Not Found";
        }

        // update that order's status to pending
        string query = @"UPDATE [Order]
                       SET restaurantId = @restaurantId,
                           orderDate = @orderDate,
                           userId = @userId,
                           personeelId = @personeelId,
                           addressId = @addressId,
                           orderStatusId = @orderStatusId
                       WHERE orderId = @orderId";

        using (SqlCommand updateCommand = new SqlCommand(query, connection))
        {
            updateCommand.Parameters.AddWithValue("@orderId", order.orderId);
            updateCommand.Parameters.AddWithValue("@orderDate", order.orderDate);
            updateCommand.Parameters.AddWithValue("@restaurantId", order.restaurantId);
            updateCommand.Parameters.AddWithValue("@userId", order.userId);
            updateCommand.Parameters.AddWithValue("@personeelId", order.personeelId);
            updateCommand.Parameters.AddWithValue("@addressId", order.addressId);
            updateCommand.Parameters.AddWithValue("@orderStatusId", 1);
            updateCommand.ExecuteNonQuery();
        }
        reader.Close();
        return "Your order is Pending and will be delivered shortly";
    }
}