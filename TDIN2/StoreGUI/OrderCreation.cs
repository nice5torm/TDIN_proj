using Common.Models;
using Common.Services;
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

namespace StoreGUI
{
    public partial class OrderCreation : Form
    {
        public int publicid;

        public OrderCreation(int id)
        {
            publicid = id;

            InitializeComponent();
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");

            if (client.GetAsync("api/Client/GetClientByEmail?email=" + textBox2.Text).Result.IsSuccessStatusCode)
            {
                if (numericUpDown1.Value <= Convert.ToInt32(stock.Text))
                {

                    Sale sale = CreateSale();


                    Book book = CreateBook(); 

                    if (client.PostAsJsonAsync("api/Sale/CreateSale", sale).Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("Sale made with sucess!", "Sucess sale", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        await client.PutAsJsonAsync("api/Book/EditBook", book);
                    }

                    Printer printer = new Printer(booktitle.Text, price.Text, numericUpDown1.Value.ToString(),  textBox1.Text, textBox2.Text, textBox3.Text, Convert.ToString(sale.Quantity * Convert.ToInt32(price.Text)));
                    printer.ShowDialog();


                    BookInfoLoad(publicid);
                }
                else
                {
                    Order order = CreateOrder(); 
                   
                    HttpResponseMessage result = client.PostAsJsonAsync("api/Order/CreateOrder", order).Result;

                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("Order made with sucess!", "Sucess order", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        //MessageQueue
                        MessageQueue.SendMessageToWarehouse(booktitle.Text, Convert.ToInt32(numericUpDown1.Value) + 10, result.Content.ReadAsAsync<Order>().Result.Id);

                        //Email Sender
                        EmailSender.SendEmail(textBox2.Text, "Order Creation Information",
                            "You just ordered the book: " + booktitle.Text + " the cost is " + price.Text + ". You ordered " + numericUpDown1.Value + ". The total price is " + Convert.ToInt32(numericUpDown1.Value) * Convert.ToInt32(price.Text) + " . The Order status is  Waiting Expedition");
                    }
                }
            }
            else
            {
                if (numericUpDown1.Value <= Convert.ToInt32(stock.Text))
                {
                    Sale sale = CreateSaleNewClient();
                   
                    Book book = CreateBook();

                    if (client.PostAsJsonAsync("api/Sale/CreateSale", sale).Result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("Sale made with sucess!", "Sucess Sale", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        
                        await client.PutAsJsonAsync("api/Book/EditBook", book);
                    }

                    Printer printer = new Printer(booktitle.Text, price.Text, numericUpDown1.Value.ToString(), textBox1.Text, textBox2.Text, textBox3.Text, Convert.ToString(sale.Quantity * Convert.ToInt32(price.Text)));
                    printer.ShowDialog();


                    BookInfoLoad(publicid);
                }
                else
                {
                    Order order = CreateOrderNewClient(); 

                    HttpResponseMessage result = client.PostAsJsonAsync("api/Order/CreateOrder", order).Result;
                    if (result.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        MessageBox.Show("Order made with sucess!", "Sucess order", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        //MessageQueue
                        MessageQueue.SendMessageToWarehouse(booktitle.Text, Convert.ToInt32(numericUpDown1.Value) + 10, result.Content.ReadAsAsync<Order>().Result.Id);

                        //Send Email 
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

            this.stock.Text = client.GetAsync("api/Book/GetBook?id=" + publicid).Result.Content.ReadAsAsync<Book>().Result.Amount.ToString();
            this.booktitle.Text = client.GetAsync("api/Book/GetBook?id=" + publicid).Result.Content.ReadAsAsync<Book>().Result.Title;
            this.price.Text = client.GetAsync("api/Book/GetBook?id=" + publicid).Result.Content.ReadAsAsync<Book>().Result.Price.ToString();
        }

        private async void BookInfoLoad(int id)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");

            var response = await client.GetAsync("api/Book/GetBook?id=" + id);

            this.stock.Text = response.Content.ReadAsAsync<Book>().Result.Amount.ToString();

        }

        public Sale CreateSaleNewClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");

            Sale sale = new Sale()
            {
                Quantity = Convert.ToInt32(numericUpDown1.Value),
                Client = new Client()
                {
                    Name = textBox1.Text,
                    Email = textBox2.Text,
                    Address = textBox3.Text
                },
                BookId = client.GetAsync("api/Book/GetBookByTitle?title=" + booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Id
            };

            return sale; 
        }
        public Sale CreateSale()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");

            Sale sale = new Sale()
            {
                Quantity = Convert.ToInt32(numericUpDown1.Value),
                ClientId = client.GetAsync("api/Client/GetClientByEmail?email=" + textBox2.Text).Result.Content.ReadAsAsync<Client>().Result.ID,
                BookId = client.GetAsync("api/Book/GetBookByTitle?title=" + booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Id
            };

            
            return sale; 
        }
        private Order CreateOrderNewClient()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");

            Order order = new Order()
            {
                OrderStatus = OrderStatusEnum.Wainting_expedition,
                OrderType = OrderTypeEnum.Store,
                Quantity = Convert.ToInt32(numericUpDown1.Value),
                Client = new Client()
                {
                    Name = textBox1.Text,
                    Email = textBox2.Text,
                    Address = textBox3.Text
                },
                BookId = client.GetAsync("api/Book/GetBookByTitle?title=" + booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Id
            };
            return order;
        }
        private Order CreateOrder()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");

            Order order = new Order()
            {
                OrderStatus = OrderStatusEnum.Wainting_expedition,
                OrderType = OrderTypeEnum.Store,
                Quantity = Convert.ToInt32(numericUpDown1.Value),
                ClientId = client.GetAsync("api/Client/GetClientByEmail?email=" + textBox2.Text).Result.Content.ReadAsAsync<Client>().Result.ID,
                BookId = client.GetAsync("api/Book/GetBookByTitle?title=" + booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Id
            };
            return order; 
        }
        private Book CreateBook()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");

            Book book = new Book()
            {
                Id = client.GetAsync("api/Book/GetBookByTitle?title=" + booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Id,
                Amount = client.GetAsync("api/Book/GetBookByTitle?title=" + booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Amount - Convert.ToInt32(numericUpDown1.Value),
                Price = Convert.ToInt32(price.Text),
                Title = booktitle.Text
            };
            return book; 
        }
       
    }
}
