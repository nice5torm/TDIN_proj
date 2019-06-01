using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Services;
using Common.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using Newtonsoft.Json;
using System.Net.Http;
using RabbitMQ.Client.Events;
using static Common.Services.MessageQueue;

namespace GUI_Warehouse
{
    public partial class WarehouseArea : Form
    {
        public WarehouseArea()
        {
            InitializeComponent();
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");


            if (listView1.Items.Count>0)
            {
                    string title = Convert.ToString(listView1.Items[0].SubItems[0].Text);
                    int quantity = Convert.ToInt32(listView1.Items[0].SubItems[1].Text);
                    int orderid = Convert.ToInt32(listView1.Items[0].SubItems[2].Text);
                    if (client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.OrderType == OrderTypeEnum.Store)
                    {
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

                        await client.PutAsJsonAsync("api/Order/EditOrder", order);
                        HttpResponseMessage response = client.PutAsJsonAsync("api/Order/EditOrder", order).Result;
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            MessageBox.Show("Order sent with sucess!", "Sucess sale", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageQueue.SendMessageToStore(title, quantity, order.GUID);
                        }
                    }
                    else if (client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.OrderType == OrderTypeEnum.Web)
                    {
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

                        if (client.PutAsJsonAsync("api/Order/EditOrder", order).Result.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            MessageBox.Show("Order sent with sucess!", "Sucess sale", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            MessageQueue.SendMessageToStore(title, quantity, order.GUID);
                        }
                    }
                ConsumeMessages(); // ver istoo
                UpdateList();
            }
        }
        private void UpdateList()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "warehouse",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                channel.BasicConsume("warehouse", false, "", false, false, null, consumer);
                consumer.Model.MessageCount("warehouse");
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    int quantity = JsonConvert.DeserializeObject<StoreMessage>(message).quantity;
                    string title = JsonConvert.DeserializeObject<StoreMessage>(message).title;
                    int orderid = JsonConvert.DeserializeObject<StoreMessage>(message).orderid;
                    if (listView1.InvokeRequired)
                    {
                        listView1.Invoke((MethodInvoker)delegate ()
                        {
                            this.listView1.Items.Add(new ListViewItem(new string[] { title, quantity.ToString(), orderid.ToString() }));
                        });
                    }
                    else
                    {
                        this.listView1.Items.Add(new ListViewItem(new string[] { title, quantity.ToString(), orderid.ToString() }));
                    }
                };
            }
        }
        private void ConsumeMessages()
        {
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

                var consumer = new EventingBasicConsumer(channel);
                //channel.BasicConsume("warehouse", true, "", false, false, null, consumer);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    int orderid = JsonConvert.DeserializeObject<StoreMessage>(message).orderid;
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    MessageBox.Show("Order" + orderid + " sent with sucess!", "Sucess sale", MessageBoxButtons.OK, MessageBoxIcon.Information);
                };
                channel.BasicConsume(queue: "warehouse", autoAck: false, consumer: consumer);
            }
        }

        private static IEnumerable<string> ReceiveMessages()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            List<string> bus = new List<string>();
            
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "warehouse", durable: true, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    
                    bus.Add(message);
                };
                channel.BasicConsume(queue: "warehouse", autoAck: false, consumer: consumer);
            }
            return bus.AsEnumerable();

        }

        private void WarehouseArea_Load(object sender, EventArgs e)
        {
            IEnumerable<string> bus = ReceiveMessages();
            dataGridView2.DataSource = bus;
            //for(int i = 0; i < bus.Count();i++)
            //{
            //    int quantity = JsonConvert.DeserializeObject<StoreMessage>(bus[i]).quantity;
            //    string title = JsonConvert.DeserializeObject<StoreMessage>(bus[i]).title;
            //    int orderid = JsonConvert.DeserializeObject<StoreMessage>(bus[i]).orderid;
            //    if (listView1.InvokeRequired)
            //    {
            //        listView1.Invoke((MethodInvoker)delegate ()
            //        {
            //            listView1.Items.Add(new ListViewItem(new string[] { title, quantity.ToString(), orderid.ToString() }));
            //        });
            //    }
            //    else
            //    {
            //        listView1.Items.Add(new ListViewItem(new string[] { title, quantity.ToString(), orderid.ToString() }));
            //    }
            //}

            

            //var factory = new ConnectionFactory() { HostName = "localhost" };

            //using (var connection = factory.CreateConnection())
            //using (var channel = connection.CreateModel())
            //{
            //    channel.QueueDeclare(queue: "warehouse",
            //                     durable: true,
            //                     exclusive: false,
            //                     autoDelete: false,
            //                     arguments: null);

            //    var consumer = new EventingBasicConsumer(channel);
            //    //channel.BasicConsume("warehouse", false, "", false, false, null, consumer);
            //    //consumer.Model.MessageCount("warehouse");
            //    consumer.Received += (model, ea) => 
            //    {
            //        var body = ea.Body;
            //        var message = Encoding.UTF8.GetString(body);
            //        int quantity = JsonConvert.DeserializeObject<StoreMessage>(message).quantity;
            //        string title = JsonConvert.DeserializeObject<StoreMessage>(message).title;
            //        int orderid = JsonConvert.DeserializeObject<StoreMessage>(message).orderid;
            //        if (listView1.InvokeRequired)
            //        {
            //            listView1.Invoke((MethodInvoker)delegate ()
            //            {
            //                this.listView1.Items.Add(new ListViewItem(new string[] { title, quantity.ToString(), orderid.ToString() }));
            //            });
            //        }
            //        else
            //        {
            //            this.listView1.Items.Add(new ListViewItem(new string[] { title, quantity.ToString(), orderid.ToString() }));
            //        }
            //    };
            //    channel.BasicConsume("warehouse", false, "", false, false, null, consumer: consumer);
            //}
        }
    }
}
