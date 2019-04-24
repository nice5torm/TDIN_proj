using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Models;

namespace DinningRoom
{
    public partial class Form1 : Form
    {
        List<Item> items;
        List<Table> tables;
        

        public Form1()
        {
            //RemotingConfiguration.Configure("DinningRoom.exe.config", false);
            items = DinningRoom.listServer.GetItems();
            tables = DinningRoom.listServer.GetTables();

            List<Table> payableTables = DinningRoom.listServer.GetPayableTables();
            List<Order> ordersReady = DinningRoom.listServer.GetOrdersReady();

            InitializeComponent();

            foreach (Item i in items)
            {
                this.checkedListBox1.Items.Add(i.Name, false);
            }

            foreach(Table t in tables)
            {
                this.comboBox1.Items.Add(t.Id.ToString());
            }

            foreach(Order or in ordersReady)
            {
               this. checkedListBox2.Items.Add(or.Id.ToString(), false);
            }

            foreach(Table pt in payableTables)
            {
                this.comboBox2.Items.Add(pt.Id.ToString());
            }

        }
        
     
        private void button1_Click(object sender, EventArgs e)
        {
            Table selectedTable = tables.Where(t => t.Id.ToString() == this.comboBox1.SelectedItem.ToString()).First();
            List<Item> selectedItems = new List<Item>();

            
            foreach ( string si in this.checkedListBox1.CheckedItems)
            {
                foreach(Item it in items.Where(i => i.Name == si))
                {
                    selectedItems.Add(it);
                }
            }

            DinningRoom.listServer.InsertOrder(tables.Where(t => t.Id.ToString() == this.comboBox1.SelectedItem.ToString()).First(), selectedItems);
            Console.WriteLine("table "+tables.Where(t => t.Id.ToString() == this.comboBox1.SelectedItem.ToString()).First().Orders.Count);

            //foreach (int si in this.checkedListBox1.CheckedIndices)
            //{
            //    this.checkedListBox1.SetItemCheckState(si, CheckState.Unchecked);
            //}
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach(string so in this.checkedListBox2.CheckedItems)
            {
                DinningRoom.listServer.UpdateOrderToDone(DinningRoom.listServer.GetOrdersReady().Where(or => or.Id.ToString() == so).First());
            }

            foreach (int si in this.checkedListBox1.CheckedIndices)
            {
                this.checkedListBox1.SetItemCheckState(si, CheckState.Unchecked);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DinningRoom.listServer.PayTable(tables.Where(t => t.Id.ToString() == comboBox2.SelectedItem.ToString()).First());
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<Order> ordersDonebyTable = DinningRoom.listServer.GetOrdersDone(tables.Where(t => t.Id.ToString() == this.comboBox2.SelectedItem.ToString()).First());

            foreach (Order odt in ordersDonebyTable)
            {
                this.listBox2.Items.Add(odt.Id.ToString());
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach(int si in this.checkedListBox1.CheckedIndices)
            {
                this.checkedListBox1.SetItemCheckState(si, CheckState.Unchecked);
            }
        }
    }

}
