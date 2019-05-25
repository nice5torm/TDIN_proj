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

namespace GUI_Store
{
    public partial class OrderCreation : Form
    {
        public int publicid;
        public HttpClient client = new HttpClient();

        public OrderCreation(int id)
        {
            client.BaseAddress = new Uri("http://localhost:2222/");
            publicid = id;

            InitializeComponent();

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");

            if(client.GetAsync("api/Client/GetClientByEmail?email=" + textBox2.Text).Result.IsSuccessStatusCode)
            {
                if (numericUpDown1.Value <= Convert.ToInt32(stock.Text))
                {
                    
                    Sale sale = new Sale()
                    {
                        Quantity = Convert.ToInt32(numericUpDown1.Value),
                        ClientId = client.GetAsync("api/Client/GetClientByEmail?email=" + textBox2.Text).Result.Content.ReadAsAsync<Client>().Result.ID,
                        BookId = client.GetAsync("api/Book/GetBookByName?title=" + booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Id
                    };

                    Book book = new Book()
                    {
                        Id = client.GetAsync("api/Book/GetBookByName?title=" + booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Id,
                        Amount = client.GetAsync("api/Book/GetBookByName?title=" + booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Amount - Convert.ToInt32(numericUpDown1.Value),
                        Price = Convert.ToInt32(price.Text),
                        Title = booktitle.Text
                    };

                    if (client.PostAsJsonAsync("api/Sale/CreateSale", sale).Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("Sale made with sucess!", "Sucess sale", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    client.PutAsJsonAsync("api/Book/EditBook", book);
                    //this.stock.Text = client.GetAsync("api/Book/GetBookByName?title=" + booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Amount.ToString();
                    this.stock.Text = client.GetAsync("api/Book/GetBook?id=" + publicid).Result.Content.ReadAsAsync<Book>().Result.Amount.ToString();

                }
                else
                {
                    Order order = new Order()
                    {
                        OrderStatus = OrderStatusEnum.Wainting_expedition,
                        OrderType = OrderTypeEnum.Store, 
                        Quantity = Convert.ToInt32(numericUpDown1.Value),
                        ClientId = client.GetAsync("api/Client/GetClientByEmail?email=" + textBox2.Text).Result.Content.ReadAsAsync<Client>().Result.ID,
                        BookId = client.GetAsync("api/Book/GetBookByName?title=" + booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Id
                    };

                    if (client.PostAsJsonAsync("api/Order/CreateOrder", order).Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("Order made with sucess!", "Sucess order", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }

                    MessageQueue.SendMessageToWarehouse(order.GUID, booktitle.Text, Convert.ToInt32(numericUpDown1.Value));

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
                        BookId = client.GetAsync("api/Book/GetBookByName?title=" + booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Id
                    };

                    Book book = new Book()
                    {
                        Id = client.GetAsync("api/Book/GetBookByName?title=" + booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Id,
                        Amount = Convert.ToInt32(stock.Text) - Convert.ToInt32(numericUpDown1.Value),
                        Price = Convert.ToInt32(price.Text),
                        Title = booktitle.Text
                    };
                    
                    if(client.PostAsJsonAsync("api/Sale/CreateSale",sale).Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("Sale made with sucess!", "Sucess Sale", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    client.PutAsJsonAsync("api/Book/EditBook", book);
                    //this.stock.Text = client.GetAsync("api/Book/GetBookByName?title=" + booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Amount.ToString();
                    this.stock.Text = client.GetAsync("api/Book/GetBook?id=" + publicid).Result.Content.ReadAsAsync<Book>().Result.Amount.ToString();

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
                        BookId = client.GetAsync("api/Book/GetBookByName?title="+ booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Id
                    };

                    if(client.PostAsJsonAsync("api/Order/CreateOrder", order).Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("Order made with sucess!", "Sucess order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    MessageQueue.SendMessageToWarehouse(order.GUID,booktitle.Text,Convert.ToInt32(numericUpDown1.Value));

                } 
            }           
        }

        private void OrderCreation_FormClosed(object sender, FormClosedEventArgs e)
        {

        }


        private void OrderCreation_Load(object sender, EventArgs e)
        {
            this.stock.Text = client.GetAsync("api/Book/GetBook?id=" + publicid).Result.Content.ReadAsAsync<Book>().Result.Amount.ToString();
            this.booktitle.Text = client.GetAsync("api/Book/GetBook?id=" + publicid).Result.Content.ReadAsAsync<Book>().Result.Title;
            this.price.Text = client.GetAsync("api/Book/GetBook?id=" + publicid).Result.Content.ReadAsAsync<Book>().Result.Price.ToString();
        }
    }
}
