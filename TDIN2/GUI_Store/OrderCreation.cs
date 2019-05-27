using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Models;
using Common.Services;
using Common;
using System.Collections;
using System.Runtime.Remoting;

namespace GUI_Store
{
    public partial class OrderCreation : Form
    {
        public int publicid;
        public HttpClient client = new HttpClient();

        INotifier notifier;

        Repeater evRepeater;

        delegate void UpdateDelegate();

        public OrderCreation(int id)
        {
            publicid = id;

            InitializeComponent();

            notifier = (INotifier)RemoteNew.New(typeof(INotifier));

            evRepeater = new Repeater();
            evRepeater.alterEvent += new AlterDelegate(DoAlterations);
            notifier.alterEvent += new AlterDelegate(evRepeater.RepeaterAll);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
        
            client.BaseAddress = new Uri("http://localhost:2222/");

            //if(client.GetAsync("api/Client/GetClientByEmail?email=" + textBox2.Text).Result.IsSuccessStatusCode)
            if(notifier.GetClientExists(textBox2.Text))
            {
                if (numericUpDown1.Value <= Convert.ToInt32(stock.Text))
                {
                    
                    Sale sale = new Sale()
                    {
                        Quantity = Convert.ToInt32(numericUpDown1.Value),
                        //ClientId = client.GetAsync("api/Client/GetClientByEmail?email=" + textBox2.Text).Result.Content.ReadAsAsync<Client>().Result.ID,
                        ClientId = notifier.GetClientId(textBox2.Text),
                        BookId = publicid
                        
                    };

                    Book book = new Book()
                    {
                        Id = publicid,
                        //Amount = client.GetAsync("api/Book/GetBook?id=" + publicid).Result.Content.ReadAsAsync<Book>().Result.Amount - Convert.ToInt32(numericUpDown1.Value),
                        Amount= notifier.GetAmountBook(publicid)-Convert.ToInt32(numericUpDown1.Value),
                        Price = Convert.ToInt32(price.Text),
                        Title = booktitle.Text
                    };

                    //if (client.PostAsJsonAsync("api/Sale/CreateSale", sale).Result.StatusCode == System.Net.HttpStatusCode.OK)
                    if(notifier.CreatedSale(sale))
                    {
                        MessageBox.Show("Sale made with sucess!", "Sucess sale", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        //client.PutAsJsonAsync("api/Book/EditBook", book);
                        notifier.EditBook(book);

                    }

                    

                }
                else
                {
                    Order order = new Order()
                    {
                        OrderStatus = OrderStatusEnum.Wainting_expedition,
                        OrderType = OrderTypeEnum.Store, 
                        Quantity = Convert.ToInt32(numericUpDown1.Value),
                        //ClientId = client.GetAsync("api/Client/GetClientByEmail?email=" + textBox2.Text).Result.Content.ReadAsAsync<Client>().Result.ID,
                        ClientId = notifier.GetClientId(textBox2.Text),
                        BookId = publicid
                    };

                    //HttpResponseMessage result = client.PostAsJsonAsync("api/Order/CreateOrder", order).Result;
                    //HttpResponseMessage result = CreatedOrderResponse(order);

                    if (notifier.CreatedOrderResponse(order))
                    {
                        MessageBox.Show("Order made with sucess!", "Sucess order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        //Message Queue
                        MessageQueue.SendMessageToWarehouse(booktitle.Text, Convert.ToInt32(numericUpDown1.Value)+10, notifier.CreatedOrderId());

                        //Email Sender
                        EmailSender.SendEmail(textBox2.Text, "Order Creation Information", 
                            "You just ordered the book: " + booktitle.Text+ " the cost is "+ price.Text + ". You ordered " + numericUpDown1.Value + ". The total price is " + Convert.ToInt32(numericUpDown1.Value) * Convert.ToInt32(price.Text)+ " . The Order status is  Waiting Expedition");
                        
                    }


                }
            }
            else
            {
                if (numericUpDown1.Value <= Convert.ToInt32(stock.Text))
                {
                    Sale sale = new Sale()
                    {
                        Quantity = Convert.ToInt32(numericUpDown1.Value),
                        Client = new Client()
                        {
                            Name = textBox1.Text,
                            Email = textBox2.Text,
                            Address = textBox3.Text
                        },
                        BookId = publicid
                    };

                    Book book = new Book()
                    {
                        Id = publicid,
                        Amount = Convert.ToInt32(stock.Text) - Convert.ToInt32(numericUpDown1.Value),
                        Price = Convert.ToInt32(price.Text),
                        Title = booktitle.Text
                    };
                    
                    //if(client.PostAsJsonAsync("api/Sale/CreateSale",sale).Result.StatusCode == System.Net.HttpStatusCode.OK)
                    if(notifier.CreatedSale(sale))
                    {
                        MessageBox.Show("Sale made with sucess!", "Sucess Sale", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        //client.PutAsJsonAsync("api/Book/EditBook", book);
                        notifier.EditBook(book);

                    }                       
                }
                else
                {
                    Order order = new Order()
                    {
                        OrderStatus = OrderStatusEnum.Wainting_expedition,
                        OrderType = OrderTypeEnum.Store,
                        Quantity = Convert.ToInt32(numericUpDown1.Value),
                        Client = new  Client()
                        {
                            Name = textBox1.Text,
                            Email = textBox2.Text,
                            Address = textBox3.Text
                        },
                        BookId = publicid
                    };
                    //HttpResponseMessage result = client.PostAsJsonAsync("api/Order/CreateOrder", order).Result;
                    //HttpResponseMessage result = CreatedOrderResponse(order);

                    if (notifier.CreatedOrderResponse(order))
                    {
                        MessageBox.Show("Order made with sucess!", "Sucess order", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        MessageQueue.SendMessageToWarehouse(booktitle.Text,Convert.ToInt32(numericUpDown1.Value)+10, notifier.CreatedOrderId());

                        EmailSender.SendEmail(textBox2.Text, "Order Creation Information",
                           "You just ordered the book: " + booktitle.Text + "the cost is " + price.Text + ". You ordered " + numericUpDown1.Value + ". The total price is " + Convert.ToInt32(numericUpDown1.Value) * Convert.ToInt32(price.Text) + " . The Order status is  Waiting Expedition");
                    }


                } 
            }           
        }

        private void OrderCreation_Load(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");


            //this.stock.Text = client.GetAsync("api/Book/GetBook?id=" + publicid).Result.Content.ReadAsAsync<Book>().Result.Amount.ToString();
            this.stock.Text = notifier.GetAmountBook(publicid);
            //this.booktitle.Text = client.GetAsync("api/Book/GetBook?id=" + publicid).Result.Content.ReadAsAsync<Book>().Result.Title;
            this.booktitle.Text = notifier.GetTitleBook(publicid);
            //this.price.Text = client.GetAsync("api/Book/GetBook?id=" + publicid).Result.Content.ReadAsAsync<Book>().Result.Price.ToString();
            this.price.Text = Convert.ToString(notifier.GetPriceBook(publicid));
        }

        public void DoAlterations(Operation op)
        {
            UpdateDelegate UpdateStock;
            
            switch (op)
            {
                case Operation.UpdateStock:
                    UpdateStock = new UpdateDelegate(UpdateStockText);
                    BeginInvoke(UpdateStock);
                    break;
                
            }
        }


        private void UpdateStockText()
        {
            //this.stock.Text = client.GetAsync("api/Book/GetBook?id=" + publicid).Result.Content.ReadAsAsync<Book>().Result.Amount.ToString();
            this.stock.Text = notifier.GetAmountBook(publicid).ToString();

        }


    }


}
