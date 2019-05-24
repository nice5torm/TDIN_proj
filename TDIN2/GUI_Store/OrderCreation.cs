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
        public OrderCreation(int id)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");

            booktitle.Text = client.GetAsync("api/Book/GetBook/" + id).Result.Content.ReadAsAsync<Book>().Result.Title;
            stock.Text = client.GetAsync("api/Book/GetBook/" + id).Result.Content.ReadAsAsync<Book>().Result.Amount.ToString();
            price.Text = client.GetAsync("api/Book/GetBook/" + id).Result.Content.ReadAsAsync<Book>().Result.Price.ToString();

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");

            if(client.GetAsync("api/Client/GetClientByEmail/" + textBox2.Text).Result.IsSuccessStatusCode)
            {
                if (numericUpDown1.Value <= Convert.ToInt32(stock.Text))
                {
                    Sale sale = new Sale
                    {
                        Quantity = Convert.ToInt32(numericUpDown1.Value),
                        ClientId = client.GetAsync("api/Client/GetClientByEmail/" + textBox2.Text).Result.Content.ReadAsAsync<Client>().Id,
                        BookId = client.GetAsync("api/Book/GetBookByName/" + booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Id
                    };

                    var salestringContent = new StringContent(sale.ToString());

                    Book book = new Book
                    {
                        Id = client.GetAsync("api/Book/GetBookByName/" + booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Id,
                        Amount = Convert.ToInt32(stock.Text) - Convert.ToInt32(numericUpDown1.Value),
                        Price = Convert.ToInt32(price.Text),
                        Title = booktitle.Text
                    };

                    var bookstringContent = new StringContent(book.ToString());

                    //var myContent = JsonConvert.SerializeObject(sale);
                    //var buffer = Encoding.UTF8.GetBytes(myContent);
                    //var byteContent = new ByteArrayContent(buffer);
                    //byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    client.PostAsync("api/Sale/CreateSale", salestringContent);
                    client.PutAsync("api/Book/UpdateBook", bookstringContent);

                }
                else
                {
                    Order order = new Order()
                    {
                        Quantity = Convert.ToInt32(numericUpDown1.Value),
                        ClientId = client.GetAsync("api/Client/GetClientByEmail/" + textBox2.Text).Result.Content.ReadAsAsync<Client>().Id,
                        BookId = client.GetAsync("api/Book/GetBookByName/" + booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Id
                    };

                    var orderstringContent = new StringContent(order.ToString());


                    //var myContent = JsonConvert.SerializeObject(order);
                    //var buffer = Encoding.UTF8.GetBytes(myContent);
                    //var byteContent = new ByteArrayContent(buffer);
                    //byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    client.PostAsync("api/Order/CreateOrder", orderstringContent);

                    MessageQueue.SendMessageToWarehouse(order.GUID, booktitle.Text, Convert.ToInt32(numericUpDown1.Value));

                }


            }
            else
            {
                if (numericUpDown1.Value <= Convert.ToInt32(stock.Text))
                {
                    Sale sale = new Sale
                    {
                        Quantity = Convert.ToInt32(numericUpDown1.Value),
                        Client = new Client
                        {
                            Name = textBox1.Text,
                            Email = textBox2.Text,
                            Address = textBox3.Text
                        },
                        BookId = client.GetAsync("api/Book/GetBookByName/" + booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Id
                    };

                    var salestringContent = new StringContent(sale.ToString());

                    Book book = new Book
                    {
                        Id = client.GetAsync("api/Book/GetBookByName/" + booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Id,
                        Amount = Convert.ToInt32(stock.Text) - Convert.ToInt32(numericUpDown1.Value),
                        Price = Convert.ToInt32(price.Text),
                        Title = booktitle.Text
                    };

                    var bookstringContent = new StringContent(book.ToString());

                    //var myContent = JsonConvert.SerializeObject(sale);
                    //var buffer = Encoding.UTF8.GetBytes(myContent);
                    //var byteContent = new ByteArrayContent(buffer);
                    //byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    client.PostAsync("api/Sale/CreateSale", salestringContent);
                    client.PutAsync("api/Book/UpdateBook", bookstringContent);

                }
                else
                {
                    Order order = new Order
                    {
                        Quantity = Convert.ToInt32(numericUpDown1.Value),
                        Client = new  Client{
                            Name = textBox1.Text,
                            Email = textBox2.Text,
                            Address = textBox3.Text
                        },
                        BookId = client.GetAsync("api/Book/GetBookByName/"+ booktitle.Text).Result.Content.ReadAsAsync<Book>().Result.Id
                    };

                    var orderstringContent = new StringContent(order.ToString());

                    //var myContent = JsonConvert.SerializeObject(order);
                    //var buffer = Encoding.UTF8.GetBytes(myContent);
                    //var byteContent = new ByteArrayContent(buffer);
                    //byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    client.PostAsync("api/Order/CreateOrder", orderstringContent);

                    MessageQueue.SendMessageToWarehouse(order.GUID,booktitle.Text,Convert.ToInt32(numericUpDown1.Value));

                } 
            }           
        }
    }
}
