using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client; 


namespace Common.Services
{
    public class MessageQueue
    {

        public static void SendMessageToWarehouse(string title, int quantity, int orderid)     
        {
            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "warehouse",              
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = JsonConvert.SerializeObject(new StoreMessage( title, quantity, orderid));      
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                                     routingKey: "warehouse",                    
                                     basicProperties: properties,
                                     body: body);
            }
        }


        public static void SendMessageToStore(string title, int quantity, int orderid)     
        {
            ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "store",              
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = JsonConvert.SerializeObject(new WarehouseMessage(title, quantity, orderid));
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                                     routingKey: "store",                     
                                     basicProperties: properties,
                                     body: body);
            }
        }

        public class StoreMessage
        {
            public string title { get; set; }
            public int quantity { get; set; }
            public int orderid { get; set; }

            public StoreMessage( string t, int q, int o)
            {
                title = t;
                quantity = q;
                orderid = o;
            }
        }

        public class WarehouseMessage
        {
            public string title { get; set; }
            public int quantity { get; set; }
            public int orderid { get; set; }

            public WarehouseMessage(string t, int q, int o)
            {
                title = t;
                quantity = q;
                orderid = o; 
            }
        }
    }
}
