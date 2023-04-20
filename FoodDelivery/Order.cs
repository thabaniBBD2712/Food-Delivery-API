using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery
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
    }
}
