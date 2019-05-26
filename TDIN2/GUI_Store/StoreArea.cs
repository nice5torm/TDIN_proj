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
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using static Common.Services.MessageQueue;

namespace GUI_Store
{
    public partial class StoreArea : Form
    {
       
        public StoreArea()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");

            InitializeComponent();

            HttpResponseMessage responsebook = client.GetAsync("api/Book/GetBooks").Result;
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
                //listView1.Items.Add(channel.BasicConsume("store", false, "", false, false, null, consumer).ToString());
                consumer.Model.MessageCount("store");
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    //listView1.Items.Add(message);
                    int quantity = JsonConvert.DeserializeObject<WarehouseMessage>(message).quantity;
                    string title = JsonConvert.DeserializeObject<WarehouseMessage>(message).title;
                    int orderid = JsonConvert.DeserializeObject<WarehouseMessage>(message).orderid;
                    //listView1.Items.Add( orderid.ToString());
                       
                    if (listView1.InvokeRequired)
                    {

                        listView1.Invoke((MethodInvoker)delegate ()
                        {
                            listView1.Items.Add(new ListViewItem(new string[] { title, quantity.ToString(), orderid.ToString()}));
                        });
                    }
                    else
                    {
                        listView1.Items.Add(new ListViewItem(new string[] { title, quantity.ToString(), orderid.ToString() }));
                    }
                };
            }

        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");
            HttpResponseMessage response = client.GetAsync("api/Book/GetBookByTitle?title=" + textBox1.Text).Result;

            if (textBox1.Text == "")
            {
                MessageBox.Show("Title need values!", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                if (response.IsSuccessStatusCode)
                {
                    OrderCreation orderCreation = new OrderCreation(response.Content.ReadAsAsync<Book>().Result.Id);
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
                if (response.IsSuccessStatusCode)
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

        }
    }
}
