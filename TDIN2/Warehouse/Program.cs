using Common.Models;
using Common.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Common.Services.MessageQueue;

namespace Warehouse
{
    class Program
    {
        public static void Main()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            { 
                    channel.QueueDeclare(queue: "warehouse",
                                              durable: true,
                                              exclusive: false,
                                              autoDelete: false,
                                              arguments: null);

                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                        Console.WriteLine(" [*] Waiting for messages.");

                        var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            Console.WriteLine(" [x] Received {0}", message);

                            Thread.Sleep(1000);

                            Console.WriteLine(" [x] Done");

                            int quantity = JsonConvert.DeserializeObject<WarehouseMessage>(message).quantity;
                            string title = JsonConvert.DeserializeObject<WarehouseMessage>(message).title;
                            int orderid = JsonConvert.DeserializeObject<WarehouseMessage>(message).orderid;

                            if (client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.OrderType == OrderTypeEnum.Store)
                            {
                                Order order = CreateOrderStore(orderid);

                                client.PutAsJsonAsync("api/Order/EditOrder", order);

                                MessageQueue.SendMessageToStore(title, quantity, order.GUID);
                            }
                            else if (client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.OrderType == OrderTypeEnum.Web)
                            {
                                Order order = CreateOrderWeb(orderid);

                                client.PutAsJsonAsync("api/Order/EditOrder", order);
                            }

                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        };
                        channel.BasicConsume(queue: "warehouse",
                                             autoAck: false,
                                             consumer: consumer);

                        Console.WriteLine(" Press [enter] to exit.");
                        Console.ReadLine();

            }
           
                       
        }

        public static Order CreateOrderWeb(int orderid)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");

            Order order = new Order()
            {
                BookId = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.BookId,
                Book = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.Book,
                ClientId = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.ClientId,
                Client = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.Client,
                Quantity = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.Quantity,
                GUID = orderid,
                OrderType = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.OrderType,
                OrderStatus = OrderStatusEnum.Dispatched,
                DispatchedDate = DateTime.Now.AddDays(1)
            };
            return order; 
        }
        public static Order CreateOrderStore(int orderid)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");

            Order order = new Order()
            {
                BookId = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.BookId,
                Book = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.Book,
                ClientId = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.ClientId,
                Client = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.Client,
                Quantity = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.Quantity,
                GUID = orderid,
                OrderType = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.OrderType,
                OrderStatus = OrderStatusEnum.DispatchOccurence,
                DispatchOccurence = DateTime.Now.AddDays(2)
            };
            return order;
        }
    }
}
