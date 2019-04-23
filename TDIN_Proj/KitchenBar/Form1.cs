using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Models;

namespace KitchenBar
{
    public partial class Form1 : Form
    {
        public Form1(int id)
        {
            if (id == 0)
            {
                //KITCHEN
                Text = "Kitchen";
                InitializeComponent();
            }
            else if (id == 1)
            {
                //BAR
                Text = "Bar";
                InitializeComponent();
            }

            List<Order> ordersPending = KitchenBar.listServer.GetOrdersPending();
            List<Order> ordersPreparation = KitchenBar.listServer.GetOrdersInPreparation();

            foreach (Order op in ordersPending)
            {
                this.listBox1.Items.Add(op.Id.ToString());
            }

            foreach (Order p in ordersPreparation)
            {
                this.listBox2.Items.Add(p.Id.ToString());
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            KitchenBar.listServer.UpdateOrderToInPreparation(KitchenBar.listServer.GetOrdersPending().Where(or => or.Id.ToString() == this.listBox1.SelectedItem.ToString()).First());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            KitchenBar.listServer.UpdateOrderToReady(KitchenBar.listServer.GetOrdersInPreparation().Where(or => or.Id.ToString() == this.listBox2.SelectedItem.ToString()).First());
        }
    }
}
