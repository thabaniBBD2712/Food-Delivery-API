using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery
{
  public class OrderItem
    {
        public int orderItemId { get; set; }
        public int? orderItemQuantity { get; set; }
        public decimal? orderItemPrice { get; set; }
        public int? orderId { get; set; }
        public int? itemInformationId { get; set; }
    }
}
