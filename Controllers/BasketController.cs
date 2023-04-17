using Microsoft.AspNetCore.Mvc;
using FoodDeliveryAPI.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FoodDeliveryAPI.Controllers
{
    [Route("api/FoodDeliver.com/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private List<OrderItem> orderItems;

        public event EventHandler<List<OrderItem>
        > OnCheckout  = delegate{};

        public BasketController(IConfiguration configuration)
        {
            _configuration = configuration;
            orderItems = new List<OrderItem>();
        }

        [HttpPost("{itemId}")]
        public IActionResult Post([FromBody] OrderItem orderItem)
        {
            if (orderItem == null)
            {
                return BadRequest();
            }
            this.orderItems.Add(orderItem);
            return Ok();
        } 


        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            OrderItem itemToDelete = orderItems.ElementAt(id);
            if (itemToDelete == null)
            {
                return NotFound(); // Return a response indicating that the ID was not found
            }
            else
            {
                //reduce the quantity of the item if there is more than one in the basket
                if (itemToDelete.orderItemQuantity > 1)
                { 
                    itemToDelete.orderItemQuantity -= 1;
                    this.orderItems[id] = itemToDelete;
                }
                else //remove the item entirely if there is only one item in the basket
                {
                    this.orderItems.RemoveAt(id);
                }
                return Ok("Item Removed Successfully");
            }       
        }

        [HttpPost( Name = "CheckoutBasket")] 
        public IActionResult CheckoutBasket()
        {
            if (this.orderItems.Count == 0)
            {
                return BadRequest();
            }
            this.checkoutBasket();
            return Ok(); 
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(this.orderItems);
        }

        void checkoutBasket(){
            List<Exception> exceptions = new List<Exception>();

            foreach (Delegate handler in OnCheckout.GetInvocationList())
            {
                try
                {
                    //pass sender object and eventArgs while
                    handler.DynamicInvoke(this, orderItems);
                }
                catch (Exception e)
                {
                    //Add exception in exception list if occured any
                    exceptions.Add(e);
                }
            }

            if(exceptions.Any())
                {
                //Throw aggregate exception of all exceptions 
                //occured while invoking subscribers event handlers
                throw new AggregateException(exceptions);
            }
        }
       
    }
}