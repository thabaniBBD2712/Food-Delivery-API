using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery
{
  public class ItemInformation
    {
        public int itemInformationId { get; set; }
        public string? itemName { get; set; }
        public string? itemDescription { get; set; }
        public int itemCategoryId { get; set; }
    }
}
