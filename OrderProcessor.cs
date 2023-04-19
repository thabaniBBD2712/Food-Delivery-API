using FoodDeliveryAPI.Models;
using System.Threading.Channels;

namespace FoodDeliveryAPI;

//takes a message and processes it {works as a background thread}
public class OrderProcessor : IHostedService
{
    private readonly Channel<OrderItem> channel;

    public OrderProcessor(Channel<OrderItem> channel)
    {
        this.channel = channel;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        // functions that does all the processing
        return Task.Factory.StartNew(async () =>
         {
             while (!channel.Reader.Completion.IsCompleted)
             {
                 var response = await channel.Reader.ReadAsync();
                 //push it to any table
                 Console.WriteLine(response);
             }
         });
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}