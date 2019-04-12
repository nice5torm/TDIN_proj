using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DinningRoom
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkedListBox1.Items.AddRange(DatabaseManager.getItems());
        }

        private void checkedListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkedListBox1.Items.AddRange(DatabaseManager.getOrdersReady());
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {            
            checkedListBox1.Items.AddRange(DatabaseManager.getOrdersDone());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkedListBox1.Items.AddRange(DatabaseManager.getTables());
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            checkedListBox1.Items.AddRange(DatabaseManager.getTablesUnPaid());
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
