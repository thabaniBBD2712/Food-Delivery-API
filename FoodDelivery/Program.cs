﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FoodDelivery
{
  public class Program
  {
    public static async Task Main()
    {
      HttpClient httpClient = new HttpClient();

      HttpResponseMessage response = await httpClient.GetAsync("http://localhost:5263/api/FoodDeliver.com/v1/Restaurant");
      if (response.IsSuccessStatusCode)
      {
        // Read the response content as MyModel
        List<Restaurant> myModel = await response.Content.ReadAsAsync<List<Restaurant>>();
        foreach(var m in myModel)
        {
          Console.WriteLine($"MyModel: Id={m.restaurantId}, Name={m.restaurantName}");
        }
        
        Console.ReadLine();
      }
      else
      {
        Console.WriteLine($"Failed to fetch MyModel. StatusCode: {response.StatusCode}");
      }
    }
    
  }
}
