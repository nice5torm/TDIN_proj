﻿using System;
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
        private static ConnectionFactory factory = new ConnectionFactory() { HostName = "localhost" };
        private static string warehouse = "warehouse";          //TODO

        public static void SendMessageToWarehouse(string id, string title, int quantity, int orderId)     //TODO
        {

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: warehouse,              //TODO
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                string message = JsonConvert.SerializeObject(new StoreMessage(id, title, quantity, orderId));      
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: warehouse,                     //TODO
                                     basicProperties: null,
                                     body: body);
            }
        }

        private class StoreMessage
        {
            public string id { get; set; }
            public string title { get; set; }
            public int quantity { get; set; }
            public int orderId { get; set; }

            public StoreMessage(string i, string t, int q, int oi)
            {
                id = i;
                title = t;
                quantity = q;
                orderId = oi;
            }
        }
    }
}
