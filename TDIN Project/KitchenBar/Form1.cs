using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
