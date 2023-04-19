using FoodDeliveryAPI.Models;
using Microsoft.Extensions.Configuration;
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
                             Console.WriteLine("Pushed to the db yay");
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
}