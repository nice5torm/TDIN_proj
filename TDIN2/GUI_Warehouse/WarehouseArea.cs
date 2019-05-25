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

namespace GUI_Warehouse
{
    public partial class WarehouseArea : Form
    {
        

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
                    if (listView1.InvokeRequired)
                    {
                        listView1.Invoke((MethodInvoker)delegate ()
                        {
                            this.listView1.Items.Add(new ListViewItem(new string[] { id.ToString(), title,quantity.ToString() }));
                        });
                    }


                    //listView1.Items.Add(listit);
                };

                //channel.BasicConsume(queue: "warehouse",
                //                     autoAck: true,
                //                     consumer: consumer);

            }

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
