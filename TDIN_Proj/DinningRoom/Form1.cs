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
using static Models.Class1; 

namespace DinningRoom
{
    public partial class Form1 : Form
    {
        List<Item> items;
        List<Table> tables;
        IManagement listServer;
        

        public Form1()
        {
            RemotingConfiguration.Configure("DinningRoom.exe.config", false);
            listServer = (IManagement)RemoteNew.New(typeof(IManagement));
            items = listServer.GetItems();
            tables = listServer.GetTables();

            List<Table> payableTables = listServer.GetPayableTables();
            List<Order> ordersReady = listServer.GetOrdersReady();

            foreach(Item i in items)
            {
                checkedListBox1.Items.Add(i.Name);
            }

            foreach(Table t in tables)
            {
                comboBox1.Items.Add(t.Id.ToString());
            }

            foreach(Order or in ordersReady)
            {
                checkedListBox2.Items.Add(or.Id.ToString());
            }

            foreach(Table pt in payableTables)
            {
                comboBox2.Items.Add(pt.Id.ToString());
            }

            checkedListBox1.CheckOnClick = true;
            checkedListBox2.CheckOnClick = true;

            InitializeComponent();
        }
        
     
        private void button1_Click(object sender, EventArgs e)
        {
            Table selectedTable = tables.Where(t => t.Id.ToString() == comboBox1.SelectedItem.ToString()).First();
            List<Item> selectedItems = new List<Item>();

            foreach ( ListBox.SelectedObjectCollection si in checkedListBox1.SelectedItems)
            {
                foreach(Item it in items.Where(i => i.Name == si.ToString()))
                {
                    selectedItems.Add(it);
                }
            }

            listServer.InsertOrder(selectedTable, selectedItems);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach(CheckedListBox.SelectedObjectCollection so in checkedListBox2.SelectedItems)
            {
                listServer.UpdateOrderToDone(listServer.GetOrdersReady().Where(or => or.Id.ToString() == so.ToString()).First());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listServer.PayTable(tables.Where(t => t.Id.ToString() == comboBox2.SelectedItem.ToString()).First());
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<Order> ordersDonebyTable = listServer.GetOrdersDone(tables.Where(t => t.Id.ToString() == comboBox2.SelectedItem.ToString()).First());

            foreach (Order odt in ordersDonebyTable)
            {
                listBox2.Items.Add(odt.Id.ToString());
            }
        }
    }

    class RemoteNew
    {
        private static Hashtable types = null;

        private static void InitTypeTable()
        {
            types = new Hashtable();
            foreach (WellKnownClientTypeEntry entry in RemotingConfiguration.GetRegisteredWellKnownClientTypes())
                types.Add(entry.ObjectType, entry);
        }

        public static object New(Type type)
        {
            if (types == null)
                InitTypeTable();
            WellKnownClientTypeEntry entry = (WellKnownClientTypeEntry)types[type];
            if (entry == null)
                throw new RemotingException("Type not found!");
            return RemotingServices.Connect(type, entry.ObjectUrl);
        }
    }
}
