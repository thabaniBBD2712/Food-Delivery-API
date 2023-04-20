using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery
{
  public class Address
    {
        public int addressId { get; set; }
        public string? streetName { get; set; }
        public string? city { get; set; }
        public string? province { get; set; }
        public string? postalCode { get; set; }
    }
}
