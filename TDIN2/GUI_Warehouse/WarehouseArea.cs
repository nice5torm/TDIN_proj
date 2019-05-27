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
using System.Runtime.Remoting;
using System.Collections;
using Common;

namespace GUI_Warehouse
{
    public partial class WarehouseArea : Form
    {

        INotifier notifier;

        Repeater evRepeater;

        delegate void UpdateDelegate();

        public HttpClient client = new HttpClient();

        public WarehouseArea()
        {
            //RemotingConfiguration.Configure("GUI_Warehouse.exe.config", false);

            InitializeComponent();

            notifier = (INotifier)RemoteNew.New(typeof(INotifier));

            evRepeater = new Repeater();
            evRepeater.alterEvent += new AlterDelegate(DoAlterations);
            notifier.alterEvent += new AlterDelegate(evRepeater.RepeaterAll);


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
                    //if(GetOrderType(orderid) == OrderTypeEnum.Store)
                {
                    Order order = new Order()
                    {
                        BookId = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.BookId,
                        //BookId = GetOrderBookId(orderid),
                        Book = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.Book,
                        //Book = GetOrderBook(orderid),
                        ClientId = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.ClientId,
                        //ClientId = GetOrderClientId(orderid),
                        Client = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.Client,
                        //Client = GetOrderClient(orderid),
                        Quantity = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.Quantity,
                        //Quantity = GetOrderQuantity(orderid),
                        GUID = orderid,
                        OrderType = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.OrderType,
                        //OrderType = GetOrderType(orderid),
                        OrderStatus = OrderStatusEnum.DispatchOccurence,
                        DispatchOccurence = DateTime.Now.AddDays(2)
                    };

                    HttpResponseMessage response = client.PutAsJsonAsync("api/Order/EditOrder", order).Result;
                    //HttpResponseMessage response = EditOrderResponse(order);
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("Order sent with sucess!", "Sucess sale", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageQueue.SendMessageToStore(title, quantity, order.GUID);
                    }

                }
                else if (client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.OrderType == OrderTypeEnum.Web)
                //if(GetOrderType(orderid) == OrderTypeEnum.Web)
                {
                    Order order = new Order()
                    {
                        BookId = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.BookId,
                        //BookId = GetOrderBookId(orderid),
                        Book = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.Book,
                        //Book = GetOrderBook(orderid),
                        ClientId = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.ClientId,
                        //ClientId = GetOrderClientId(orderid),
                        Client = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.Client,
                        //Client = GetOrderClient(orderid),
                        Quantity = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.Quantity,
                        //Quantity = GetOrderQuantity(orderid),
                        GUID = orderid,
                        OrderType = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.OrderType,
                        //OrderType = GetOrderType(orderid),
                        OrderStatus = OrderStatusEnum.Dispatched,
                        DispatchedDate = DateTime.Now.AddDays(1)
                    };

                    //HttpResponseMessage response = EditOrderResponse(order);
                    HttpResponseMessage response = client.PutAsJsonAsync("api/Order/EditOrder", order).Result;
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                      {
                        MessageBox.Show("Order sent with sucess!", "Sucess sale", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        MessageQueue.SendMessageToStore(title, quantity, order.GUID);

                    }



                }                
            }
        }

        public void DoAlterations(Operation op)
        {
            UpdateDelegate UpdateMessagesWarehouse;

            switch (op)
            {
                case Operation.UpdateBooks:
                    UpdateMessagesWarehouse = new UpdateDelegate(UpdateMessagesWarehouseList);
                    BeginInvoke(UpdateMessagesWarehouse);
                    break;
                
            }
        }

        private void UpdateMessagesWarehouseList()
        {
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
                consumer.Received += (model, ea) =>
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

    }
    class RemoteNew
    {
        private static Hashtable types = null;

        private static void InitTypeTable()
        {
            types = new Hashtable();
            foreach (WellKnownClientTypeEntry entry in RemotingConfiguration.GetRegisteredWellKnownClientTypes())
                types.Add(entry.ObjectType, entry);
        }

        public static object New(Type type)
        {
            if (types == null)
                InitTypeTable();
            WellKnownClientTypeEntry entry = (WellKnownClientTypeEntry)types[type];
            if (entry == null)
                throw new RemotingException("Type not found!");
            return RemotingServices.Connect(type, entry.ObjectUrl);
        }
    }
}
