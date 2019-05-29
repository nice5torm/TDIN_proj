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
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using Newtonsoft.Json;
using static Common.Services.MessageQueue;
using System.Net.Http;

namespace GUI_Warehouse
{
    public partial class WarehouseArea : Form
    {
        public HttpClient client = new HttpClient();

        public WarehouseArea()
        {
            InitializeComponent();

            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "warehouse",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);
                var consumer = new EventingBasicConsumer(channel);

                channel.BasicConsume("warehouse", false, "", false, false, null, consumer);
                consumer.Model.MessageCount("warehouse");
                consumer.Received+=(model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    //listView1.Items.Add(message);
                    int id = JsonConvert.DeserializeObject<StoreMessage>(message).id;
                    int quantity = JsonConvert.DeserializeObject<StoreMessage>(message).quantity;
                    string title = JsonConvert.DeserializeObject<StoreMessage>(message).title;
                    int orderid = JsonConvert.DeserializeObject<StoreMessage>(message).orderid;
                    if (listView1.InvokeRequired)
                    {
                        listView1.Invoke((MethodInvoker)delegate ()
                        {
                            this.listView1.Items.Add(new ListViewItem(new string[] { id.ToString(), title, quantity.ToString(), orderid.ToString() }));
                        });
                    }
                };
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            client.BaseAddress = new Uri("http://localhost:2222/");


            if (listView1.SelectedItems.Count>0)
            {
                int id = Convert.ToInt32(listView1.SelectedItems[0].SubItems[0].Text);
                string title = Convert.ToString(listView1.SelectedItems[0].SubItems[1].Text);
                int quantity = Convert.ToInt32(listView1.SelectedItems[0].SubItems[2].Text);
                int orderid = Convert.ToInt32(listView1.SelectedItems[0].SubItems[3].Text);
                if(client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.OrderType== OrderTypeEnum.Store)
                {
                    Order order = new Order()
                    {
                        BookId = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.BookId,
                        Book = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.Book,
                        ClientId = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.ClientId,
                        Client = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.Client,
                        Quantity = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.Quantity,
                        GUID = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.GUID,
                        OrderType = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.OrderType,
                        OrderStatus = OrderStatusEnum.DispatchOccurence,
                        DispatchOccurence = DateTime.Now.AddDays(2)
                    };

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
                        GUID = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.GUID,
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
            }
        }
    }
}
