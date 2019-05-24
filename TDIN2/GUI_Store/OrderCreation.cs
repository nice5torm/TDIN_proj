using Newtonsoft.Json;
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
                Order order = new Order()
                {
                    Quantity = Convert.ToInt32(numericUpDown1.Value),
                    ClientId = client.GetAsync("api/Client/GetClientByEmail/" + textBox2.Text).Result.Content.ReadAsAsync<Client>().Id,
                    BookId = client.GetAsync("api/Book/GetBookByName/" + booktitle).Result.Content.ReadAsAsync<Book>().Result.Id
                };

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
                    BookId = client.GetAsync("api/Book/GetBookByName/"+ booktitle).Result.Content.ReadAsAsync<Book>().Result.Id
                };
            }           
            
            var myContent = JsonConvert.SerializeObject(order);
            var buffer = Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            if (numericUpDown1.Value <= Convert.ToInt32(stock.Text))
            {
                client.PostAsync("api/Sale/CreateSale", byteContent);
                //client.PutAsync("api/Book/UpdateSale",);
            }
        }
    }
}
