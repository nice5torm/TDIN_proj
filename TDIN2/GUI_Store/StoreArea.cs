using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Models;
using Common;
using Common.Services;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using static Common.Services.MessageQueue;

namespace GUI_Store
{
    public partial class StoreArea : Form
    {
        INotifier notifier;

        Repeater evRepeater;

        delegate void UpdateDelegate();


        public StoreArea()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");

            //RemotingConfiguration.Configure("GUI_Store.exe.config", false);

            InitializeComponent();

            notifier = (INotifier)RemoteNew.New(typeof(INotifier));

            evRepeater = new Repeater();
            evRepeater.alterEvent += new AlterDelegate(DoAlterations);
            notifier.alterEvent += new AlterDelegate(evRepeater.RepeaterAll);

            

            HttpResponseMessage responsebook = client.GetAsync("api/Book/GetBooks").Result;
            //HttpResponseMessage responsebook = notifier.GetBookResponse();
            var book = responsebook.Content.ReadAsAsync<IEnumerable<Book>>().Result;

            this.dataGridView2.DataSource = book;

            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "store",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                channel.BasicConsume("store", false, "", false, false, null, consumer);
                consumer.Model.MessageCount("store");
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    int quantity = JsonConvert.DeserializeObject<WarehouseMessage>(message).quantity;
                    string title = JsonConvert.DeserializeObject<WarehouseMessage>(message).title;
                    int orderid = JsonConvert.DeserializeObject<WarehouseMessage>(message).orderid;

                    //if (client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.OrderType == OrderTypeEnum.Store)
                    //if (GetOrderType(orderid) == OrderTypeEnum.Store)
                    //{
                    if (listView1.InvokeRequired)
                        {

                            listView1.Invoke((MethodInvoker)delegate ()
                            {
                                listView1.Items.Add(new ListViewItem(new string[] { title, quantity.ToString(), orderid.ToString() }));
                            });
                        }
                        else
                        {
                            listView1.Items.Add(new ListViewItem(new string[] { title, quantity.ToString(), orderid.ToString() }));
                        }
                   // }
                    
                };
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            //HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("http://localhost:2222/");
            //HttpResponseMessage response = client.GetAsync("api/Book/GetBookByTitle?title=" + textBox1.Text).Result;
            //HttpResponseMessage response =  notifier.GetBookResponseByTitle(textBox1.Text).Result;

            if (textBox1.Text == "")
            {
                MessageBox.Show("Title need values!", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                if (notifier.GetBookResponseByTitle(textBox1.Text))
                {
                    OrderCreation orderCreation = new OrderCreation(notifier.GetIdBookByTitle(textBox1.Text));
                    orderCreation.ShowDialog();
                }
                else
                {
                    MessageBox.Show("that book doesn't exist", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");

            int rowindex = dataGridView2.CurrentCell.RowIndex;
            HttpResponseMessage response = client.GetAsync("api/Book/GetBook?id=" + dataGridView2.Rows[rowindex].Cells[0].Value).Result;

            if (dataGridView2.SelectedCells.Count == 0)
            {
                MessageBox.Show("Didn't select anything!", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                //if (notifier.GetBookResponseById(Convert.ToInt32(dataGridView2.Rows[rowindex].Cells[0].Value)))
                if(response.IsSuccessStatusCode)
                {
                    OrderCreation orderCreation = new OrderCreation(Convert.ToInt32(dataGridView2.Rows[dataGridView2.CurrentCell.RowIndex].Cells[0].Value.ToString()));
                    orderCreation.ShowDialog();
                }
                else
                {
                    MessageBox.Show("that book doesn't exist", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("select something", "not found order", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            else if (listView1.SelectedItems.Count > 0)
            {
                string title = Convert.ToString(listView1.SelectedItems[0].SubItems[0].Text);
                int quantity = Convert.ToInt32(listView1.SelectedItems[0].SubItems[1].Text);
                int orderid = Convert.ToInt32(listView1.SelectedItems[0].SubItems[2].Text);

                //if (client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.OrderType == OrderTypeEnum.Store)
                //{
                    Order order = new Order()
                    {
                        //BookId = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.BookId,
                        BookId = notifier.GetOrderBookId(orderid),
                        //Book = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.Book,
                        Book = notifier.GetOrderBook(orderid),
                        //ClientId = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.ClientId,
                        ClientId = notifier.GetOrderClientId(orderid),
                        //Client = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.Client,
                        Client = notifier.GetOrderClient(orderid),
                        //Quantity = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.Quantity,
                        Quantity = notifier.GetOrderQuantity(orderid),
                        GUID = orderid,
                        OrderType = OrderTypeEnum.Store,
                        OrderStatus = OrderStatusEnum.Dispatched,
                        //DispatchOccurence = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.DispatchOccurence,
                        DispatchOccurence = notifier.GetOrderDispatchOccurence(orderid),
                        DispatchedDate = DateTime.Now.AddDays(1)
                    };

                    Book book = new Book()
                    {
                        //Id = client.GetAsync("api/Book/GetBookByTitle?title=" + title).Result.Content.ReadAsAsync<Book>().Result.Id,
                        Id = notifier.GetIdBookByTitle(title),
                        //Amount = 10 + client.GetAsync("api/Book/GetBookByTitle?title=" + title).Result.Content.ReadAsAsync<Book>().Result.Amount,
                        Amount = 10 + notifier.GetAmountBookByTitle(title),
                        //Price = client.GetAsync("api/Book/GetBookByTitle?title=" + title).Result.Content.ReadAsAsync<Book>().Result.Price,
                        Price = notifier.GetPriceBookByTitle(title),
                        Title = title
                    };

                //HttpResponseMessage response = client.PutAsJsonAsync("api/Order/EditOrder", order).Result;
                //HttpResponseMessage response = notifier.EditOrderResponse(order).Result;
                if (notifier.EditOrderResponse(order))
                {
                   MessageBox.Show("Order sent with sucess!", "Sucess order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                //client.PutAsJsonAsync("api/Book/EditBook", book);

                //int clientId = Convert.ToInt32(client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.ClientId);
                int clientId = Convert.ToInt32(notifier.GetOrderClientId(orderid));
                //string emailClient = client.GetAsync("api/Client/GetClient?id=" + clientId).Result.Content.ReadAsAsync<Client>().Result.Email;
                string emailClient = notifier.GetClientEmail(clientId);
                //int quantityEmail = client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.Quantity;
                int quantityEmail = notifier.GetOrderQuantity(orderid);
                //double priceEmail = client.GetAsync("api/Book/GetBookByTitle?title=" + title).Result.Content.ReadAsAsync<Book>().Result.Price;
                double priceEmail = notifier.GetPriceBookByTitle(title);


                EmailSender.SendEmail(emailClient, "Order Status",
                            "The book you ordered " + title + " which cost is " + priceEmail + ", and you ordered " + quantityEmail + ". The total price is " + priceEmail * quantityEmail + " . The Order status is Dispached on the date: " + DateTime.Now.AddDays(1));

                //}
            }
        }

        public void DoAlterations(Operation op)
        {
            UpdateDelegate UpdateBooks;
            UpdateDelegate UpdateMessagesStore;

            switch (op)
            {
                case Operation.UpdateBooks:
                    UpdateBooks = new UpdateDelegate(UpdateBooksData);
                    BeginInvoke(UpdateBooks);
                    break;
                case Operation.UpdateMessagesStore:
                    UpdateMessagesStore = new UpdateDelegate(UpdateMessagesStoreList);
                    BeginInvoke(UpdateMessagesStore);
                    break;
                    

            }
        }


        private void UpdateBooksData()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

            HttpResponseMessage responsebook = client.GetAsync("api/Book/GetBooks").Result;
            //HttpResponseMessage responsebook = GetBookResponse();
            var book = responsebook.Content.ReadAsAsync<IEnumerable<Book>>().Result;

            this.dataGridView2.DataSource = book;
        }

        private void UpdateMessagesStoreList()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "store",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                channel.BasicConsume("store", false, "", false, false, null, consumer);
                consumer.Model.MessageCount("store");
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    int quantity = JsonConvert.DeserializeObject<WarehouseMessage>(message).quantity;
                    string title = JsonConvert.DeserializeObject<WarehouseMessage>(message).title;
                    int orderid = JsonConvert.DeserializeObject<WarehouseMessage>(message).orderid;

                    //if (client.GetAsync("api/Order/GetOrder?id=" + orderid).Result.Content.ReadAsAsync<Order>().Result.OrderType == OrderTypeEnum.Store)
                    //if (GetOrderType(orderid) == OrderTypeEnum.Store)
                    //{
                    if (listView1.InvokeRequired)
                    {

                        listView1.Invoke((MethodInvoker)delegate ()
                        {
                            listView1.Items.Add(new ListViewItem(new string[] { title, quantity.ToString(), orderid.ToString() }));
                        });
                    }
                    else
                    {
                        listView1.Items.Add(new ListViewItem(new string[] { title, quantity.ToString(), orderid.ToString() }));
                    }
                    // }

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
