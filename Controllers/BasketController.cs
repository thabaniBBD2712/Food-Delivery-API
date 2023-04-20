using Microsoft.AspNetCore.Mvc;
using FoodDeliveryAPI.Models;
using System.Threading.Channels;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FoodDeliveryAPI.Controllers
{
    [Route("api/FoodDeliver.com/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly Channel<OrderItem> channel;

        private List<OrderItem> orderItems;

        public event EventHandler<List<OrderItem>
        > OnCheckout = delegate { };

        public BasketController(IConfiguration configuration, Channel<OrderItem> channel)
        {
            this.channel = channel;
            _configuration = configuration;
            orderItems = new List<OrderItem>();
        }

        [HttpPost]
        public async Task Post([FromBody] OrderItem value)
        {
            await channel.Writer.WriteAsync(value);
        }
    }
}