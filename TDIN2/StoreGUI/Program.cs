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
using System.Windows.Forms;
using static Common.Services.MessageQueue;

namespace StoreGUI
{
    class Program
    {
        public static void Main()
        {
            Thread t1 = new Thread(() => 
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new BookList());
            });

            Thread t2 = new Thread(() => 
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:2222/");

               
                var factory = new ConnectionFactory() { HostName = "localhost" };

                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: "store", durable: false, exclusive: false, autoDelete: false, arguments: null);

                        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);


                    var consumer = new EventingBasicConsumer(channel);
                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body;
                            var message = Encoding.UTF8.GetString(body);
                            Thread.Sleep(1000);


                            Console.WriteLine(" [x] Dispached {0}", message);

                            int quantity = JsonConvert.DeserializeObject<WarehouseMessage>(message).quantity;
                            string title = JsonConvert.DeserializeObject<WarehouseMessage>(message).title;
                            int orderid = JsonConvert.DeserializeObject<WarehouseMessage>(message).orderid;

                            Order order = CreateOrder(orderid);
                            Book book = CreateBook(title);

                            client.PutAsJsonAsync("api/Order/EditOrder", order);

                            client.PutAsJsonAsync("api/Book/EditBook", book);

                            int clientId = Convert.ToInt32(client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.ClientId);
                            string emailClient = client.GetAsync("api/Client/GetClient?id=" + clientId).Result.Content.ReadAsAsync<Client>().Result.Email;
                            int quantityEmail = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.Quantity;
                            double priceEmail = client.GetAsync("api/Book/GetBookByTitle?title=" + title).Result.Content.ReadAsAsync<Book>().Result.Price;

                            EmailSender.SendEmail(emailClient, "Order Status",
                                "The book you ordered " + title + " which cost is " + priceEmail + ", and you ordered " + quantityEmail + ". The total price is " + priceEmail * quantityEmail + " . The Order status is Dispached on the date: " + DateTime.Now.AddDays(1));

                            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                        };
                        channel.BasicConsume(queue: "store", autoAck: false, consumer: consumer);
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();

                }

            });

            t1.Start();
            t2.Start();
            t1.Join();
            t2.Join(); 
        }

        public static Order CreateOrder(int orderid)
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
                OrderType = OrderTypeEnum.Store,
                OrderStatus = OrderStatusEnum.Dispatched,
                DispatchOccurence = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.DispatchOccurence,
                DispatchedDate = DateTime.Now.AddDays(1)
            };
            return order; 
        }

        public static Book CreateBook(string title)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");

            Book book = new Book()
            {
                Id = client.GetAsync("api/Book/GetBookByTitle?title=" + title).Result.Content.ReadAsAsync<Book>().Result.Id,
                Amount = 10 + client.GetAsync("api/Book/GetBookByTitle?title=" + title).Result.Content.ReadAsAsync<Book>().Result.Amount,
                Price = client.GetAsync("api/Book/GetBookByTitle?title=" + title).Result.Content.ReadAsAsync<Book>().Result.Price,
                Title = title
            };
            return book; 
        }
    }
}
