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

namespace GUI_Store
{
    public partial class StoreArea : Form
    {
       
        public StoreArea()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");

            HttpResponseMessage responsebook = client.GetAsync("api/Book/GetBooks").Result;
            var book = responsebook.Content.ReadAsAsync<IEnumerable<Book>>().Result;

            HttpResponseMessage responseorder = client.GetAsync("api/Order/getOrders").Result;
            var order = responseorder.Content.ReadAsAsync<IEnumerable<Order>>().Result.Where(o => o.OrderStatus == OrderStatusEnum.Wainting_expedition);
            
            dataGridView1.DataSource = order;
            dataGridView2.DataSource = book; 


            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:2222/");
            HttpResponseMessage response = client.GetAsync("api/Book/GetBookByName/" + textBox1.Text).Result;

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
            HttpResponseMessage response = client.GetAsync("api/Book/GetBook/" + dataGridView2.Rows[rowindex].Cells[0].Value).Result;

            if (dataGridView2.SelectedCells.Count == 0)
            {
                MessageBox.Show("Title need values!", "Insufficient data", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                if (response.IsSuccessStatusCode)
                {
                    OrderCreation orderCreation = new OrderCreation(response.Content.ReadAsAsync<Book>().Result.Id);
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
