using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FoodDelivery
{
    public class Program
    {
        public static List<ItemInformation> itemInformation { get; set; }
        public static int currentOrderId { get; set; }
        public static Item currentItem { get; set; }
        public static User currentUser { get; set; }

        public static Restaurant currentRestaurant { get; set; }

        public static List<OrderItem> orderItems { get; set; }

        public static DeliveryPersonnel deliveryPersonnel { get; set; }

        public static async Task Main()
        {
            await login();
            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.GetAsync("http://localhost:5263/api/FoodDeliver.com/v1/Restaurant");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Which restaurant would you like to order from?");
                List<Restaurant> restaurants = await response.Content.ReadAsAsync<List<Restaurant>>();
                foreach (var r in restaurants)
                {
                    Console.WriteLine($"{r.restaurantId}: {r.restaurantName}");
                }

                var restaurantId = Int32.Parse(Console.ReadLine());

                await getRestaurant(restaurantId);


                await createOrder(restaurantId);
                await getMostRecentOrder();
                await getItemInformation();
                await getItems(restaurantId);
            }
            else
            {
                Console.WriteLine($"Failed to fetch MyModel. StatusCode: {response.StatusCode}");
            }


        }


        public static async Task login()
        {
            HttpClient httpClient = new HttpClient();

            Console.WriteLine("Enter you userId:");
            var userId = Int32.Parse(Console.ReadLine());

            var requestString = "http://localhost:5263/api/FoodDeliver.com/v1/User/" + userId;

            HttpResponseMessage response = await httpClient.GetAsync(requestString);
            if (response.IsSuccessStatusCode)
            {
                User currUser = await response.Content.ReadAsAsync<User>();

                currentUser = currUser;

                Console.WriteLine("Welcome: " + currentUser.userName);
            }
            else
            {
                Console.WriteLine($"Failed to fetch MyModel. StatusCode: {response.StatusCode}");
            }
        }

        public static async Task getItems(int restaurantId)
        {
            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.GetAsync("http://localhost:5263/api/FoodDelivery.com/v1/Item");
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Which Item would you like?");
                List<Item> items = await response.Content.ReadAsAsync<List<Item>>();
                foreach (var i in items)
                {
                    foreach (var ii in itemInformation)
                    {
                        if (i.restaurantId == restaurantId && i.itemInformationId == ii.itemInformationId)
                        {
                            if (ii.itemDescription != "")
                            {
                                Console.WriteLine($"{i.itemId}: {ii.itemName} - {ii.itemDescription}. (R{i.itemPrice})");
                            }
                            else
                            {
                                Console.WriteLine($"{i.itemId}: {ii.itemName}. (R{i.itemPrice})");
                            }

                        }
                    }
                }

                var itemId = Int32.Parse(Console.ReadLine());

                await getCurrentItem(itemId);
                await addOrderItem(itemId, restaurantId);

            }
            else
            {
                Console.WriteLine($"Failed to fetch MyModel. StatusCode: {response.StatusCode}");
            }
        }

        public static async Task getItemInformation()
        {
            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.GetAsync("http://localhost:5263/api/FoodDelivery.com/v1/ItemInformation");
            if (response.IsSuccessStatusCode)
            {
                itemInformation = await response.Content.ReadAsAsync<List<ItemInformation>>();
            }
            else
            {
                Console.WriteLine($"Failed to fetch ItemInformation. StatusCode: {response.StatusCode}");
            }
        }

        public static async Task addOrderItem(int itemId, int restaurantId)
        {
            Console.WriteLine("How many of this item would you like to order?");
            var quantity = Int32.Parse(Console.ReadLine());

            Console.WriteLine("Would you like to order more items? (Y/N)");
            var res = Console.ReadLine();

            var orderItemForCreation = new OrderItem
            {
                orderItemQuantity = quantity,
                orderItemPrice = currentItem.itemPrice,
                orderId = currentOrderId,
                itemInformationId = currentItem.itemInformationId,
            };

            var json = JsonConvert.SerializeObject(orderItemForCreation);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "http://localhost:5263/api/FoodDeliver.com/v1/OrderItem";
            using var client = new HttpClient();

            var response = await client.PostAsync(url, data);

            var result = await response.Content.ReadAsStringAsync();

            if (res == "Y")
            {
                await getItems(restaurantId);
            }
            else
            {
                Console.WriteLine("Would you like to finalise your order? (Y/N) [N will cancel you order]");
                res = Console.ReadLine();

                if (res == "Y")
                {
                    await displayFinalOrder();
                }
                else
                {
                    Console.WriteLine("Thank you. Enjoy your day");
                    Environment.Exit(0);
                }
            }
        }

        public static async Task createOrder(int restaurantId)
        {
            var orderForCreation = new Order
            {
                orderDate = DateTime.Now,
                restaurantId = restaurantId,
                userId = currentUser.userId,
                personeelId = 1,
                addressId = 1,
                orderStatusId = 1,
            };

            var json = JsonConvert.SerializeObject(orderForCreation);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var url = "http://localhost:5263/api/FoodDeliver.com/v1/Order";
            using var client = new HttpClient();

            var response = await client.PostAsync(url, data);

            var result = await response.Content.ReadAsStringAsync();
        }

        public static async Task getMostRecentOrder()
        {
            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.GetAsync("http://localhost:5263/api/FoodDeliver.com/v1/Order");
            if (response.IsSuccessStatusCode)
            {
                List<Order> orders = await response.Content.ReadAsAsync<List<Order>>();

                if (orders.Count > 0)
                {
                    var lastItem = orders[^1];
                    currentOrderId = lastItem.orderId;
                    Console.WriteLine(currentOrderId);
                }

            }
            else
            {
                Console.WriteLine($"Failed to fetch Order. StatusCode: {response.StatusCode}");
            }
        }

        public static async Task getCurrentItem(int itemId)
        {
            HttpClient httpClient = new HttpClient();

            var requestString = "http://localhost:5263/api/FoodDelivery.com/v1/Item/" + itemId;

            HttpResponseMessage response = await httpClient.GetAsync(requestString);
            if (response.IsSuccessStatusCode)
            {
                Item item = await response.Content.ReadAsAsync<Item>();

                currentItem = item;

            }
            else
            {
                Console.WriteLine($"Failed to fetch Item. StatusCode: {response.StatusCode}");
            }
        }

        public static async Task getRestaurant(int restaurantId)
        {
            HttpClient httpClient = new HttpClient();

            var requestString = "http://localhost:5263/api/FoodDeliver.com/v1/Restaurant/" + restaurantId;

            HttpResponseMessage response = await httpClient.GetAsync(requestString);
            if (response.IsSuccessStatusCode)
            {
                Restaurant restaurant = await response.Content.ReadAsAsync<Restaurant>();

                currentRestaurant = restaurant;

            }
            else
            {
                Console.WriteLine($"Failed to fetch Restaurant. StatusCode: {response.StatusCode}");
            }
        }

        public static async Task displayFinalOrder()
        {
            await getOrderItems();

            HttpClient httpClient = new HttpClient();

            var requestString = "http://localhost:5263/api/FoodDeliver.com/v1/Order/" + currentOrderId;

            HttpResponseMessage response = await httpClient.GetAsync(requestString);
            if (response.IsSuccessStatusCode)
            {
                decimal totalPrice = 0;
                Order order = await response.Content.ReadAsAsync<Order>();

                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine("Current Order for user " + currentUser.userName + ":");
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine("From " + currentRestaurant.restaurantName + ":");
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine("Items:               Quantity:     Price:");
                Console.WriteLine("------------------------------------------------------------");

                foreach (var oi in orderItems)
                {
                    if (oi.orderId == currentOrderId)
                    {
                        foreach (var ii in itemInformation)
                        {
                            if (ii.itemInformationId == oi.itemInformationId)
                            {
                                totalPrice += (oi.orderItemQuantity * oi.orderItemPrice).GetValueOrDefault(); ;
                                Console.WriteLine(ii.itemName + " " + oi.orderItemQuantity + " " + oi.orderItemPrice);
                            }
                        }
                    }
                }

                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine("Total price: " + totalPrice);
                Console.WriteLine("------------------------------------------------------------");
                Console.WriteLine("Confirm order? (Y/N)");
                var res = Console.ReadLine();

                if(res == "Y")
                {
                    Console.WriteLine(order.personeelId);
                    await confirmOrder(order.personeelId);
                }
                else
                {
                  Environment.Exit(0);
                }

            }
            else
            {
                Console.WriteLine($"Failed to fetch Order. StatusCode: {response.StatusCode}");
            }
        }

        public static async Task getOrderItems()
        {
            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.GetAsync("http://localhost:5263/api/FoodDeliver.com/v1/OrderItem");
            if (response.IsSuccessStatusCode)
            {
                List<OrderItem> oi = await response.Content.ReadAsAsync<List<OrderItem>>();

                orderItems = oi;
            }
            else
            {
                Console.WriteLine($"Failed to fetch OrderItem. StatusCode: {response.StatusCode}");
            }


        }

        public static async Task confirmOrder(int deliveryId)
        {
            Console.WriteLine("You order has been placed.");
        }

        public static async Task getDeliveryPersonel(int deliveryId)
        {
            HttpClient httpClient = new HttpClient();

            var request = "http://localhost:5263/api/FoodDeliver.com/v1/DeliveryPersonnel/" + deliveryId;

            HttpResponseMessage response = await httpClient.GetAsync(request);
            if (response.IsSuccessStatusCode)
            {
                DeliveryPersonnel dp = await response.Content.ReadAsAsync<DeliveryPersonnel>();

                deliveryPersonnel = dp;
            }
            else
            {
                Console.WriteLine($"Failed to fetch DeliveryPersonnel. StatusCode: {response.StatusCode}");
            }


        }

    }


}
