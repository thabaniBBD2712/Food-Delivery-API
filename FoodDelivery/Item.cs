using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery
{
  public class Item
    {
        public int itemId { get; set; }
        public decimal? itemPrice { get; set; }
        public int? restaurantId { get; set; }
        public int? itemStatusId { get; set; }
        public int? itemInformationId { get; set; }
    }
}