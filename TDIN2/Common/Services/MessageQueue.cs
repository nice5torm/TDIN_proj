using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client; 


namespace Common.Services
{
    public static class MessageQueue
    {
        static ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };

        public static void SendMessageToWarehouse(string title, int quantity)     //TODO
        {

            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "warehouse",              //TODO
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = JsonConvert.SerializeObject(new StoreMessage( title, quantity));      
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "warehouse",                     //TODO
                                     basicProperties: null,
                                     body: body);
            }
        }

        public class StoreMessage
        {
            private static int IdCounter = 1;

            public int id { get; set; }
            public string title { get; set; }
            public int quantity { get; set; }

            public StoreMessage( string t, int q)
            {
                id = IdCounter++;
                title = t;
                quantity = q;
            }
        }
    }
}
