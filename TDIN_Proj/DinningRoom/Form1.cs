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


public partial class Form1 : Form
{
    IManagement listServer;
    AlterEventRepeater evRepeater;

    List<Item> items;
    List<Table> tables;

    delegate void UpdateDelegate();
    //delegate void InvoiceDelegate(Table t);
    delegate void MakeDelegate(Table t, Order o);


    public Form1()
    {
        RemotingConfiguration.Configure("DinningRoom.exe.config", false);
        InitializeComponent();
        listServer = (IManagement)RemoteNew.New(typeof(IManagement));

        //listServer = (IManagement)Activator.GetObject(typeof(IManagement), "tcp://localhost:9000/Server/ListServer"); //?

        items = listServer.GetItems();
        tables = listServer.GetTables();

        //List<Table> payableTables = listServer.GetPayableTables();
        //List<Order> ordersReady = listServer.GetOrdersReady();

        evRepeater = new AlterEventRepeater();
        evRepeater.alterEvent += new AlterDelegate(DoAlterations);
        listServer.alterEvent += new AlterDelegate(evRepeater.Repeater);
  

        //foreach (Item i in items)
        //{
        //    this.checkedListBox1.Items.Add(i.Name, false);
        //}

        //foreach (Table t in tables)
        //{
        //    this.comboBox1.Items.Add(t.Id.ToString());
        //}

        //foreach(Order or in ordersReady)
        //{
        //   this. checkedListBox2.Items.Add(or.Id.ToString(), false);
        //}

        //foreach(Table pt in payableTables)
        //{
        //    this.comboBox2.Items.Add(pt.Id.ToString());
        //}

    }
    #region functionsweird

    private void MakeOrderTable(Table tab, Order order)
    {
        //listServer.InsertOrder(tab, order.Items);

        tables.Where(t => t.Id.ToString() == this.comboBox1.SelectedItem.ToString()).First().AddOrderTable(order);

        foreach (int si in this.checkedListBox1.CheckedIndices)                               //outra change nas funções weird
        {
            this.checkedListBox1.SetItemCheckState(si, CheckState.Unchecked);
        }
    }
    private void ChangeReady()
    {
        foreach (Order or in listServer.GetOrdersReady())
        {
            this.checkedListBox2.Items.Add(or.Id.ToString(), false);
        }
    }

    private void ChangePayTables()
    {
        foreach (Table pt in listServer.GetPayableTables())
        {
            this.comboBox2.Items.Add(pt.Id.ToString());
        }
    }

    private void ChangeInvoice(Table t)
    {
        foreach (Order or in listServer.GetPayableTables().Where(tab => tab == t).First().Orders)
        {
            this.listBox2.Items.Add(or.Id);
        }
    }

    public void DoAlterations(Operation op, Table tab, Order ord)
    {
        MakeDelegate MakeOr;
        UpdateDelegate UpReady;
        UpdateDelegate UpTab;

        switch (op)
        {
            case Operation.MakeOrder:
                MakeOr = new MakeDelegate(MakeOrderTable);
                BeginInvoke(MakeOr, new object[] { tab, ord });
                break;
            case Operation.UpdateReady:
                UpReady = new UpdateDelegate(ChangeReady);
                BeginInvoke(UpReady);
                break;
            case Operation.PayableTables:
                UpTab = new UpdateDelegate(ChangePayTables);
                BeginInvoke(UpTab);
                break;
            //case Operation.Invoice:
            //    Invoice = new InvoiceDelegate(ChangeInvoice);
            //    BeginInvoke(Invoice, new object[] { tab });
            //    break;

        }
    }

    #endregion


    #region functions 
    private void Form1_Load(object sender, EventArgs e)
    {
        foreach (Item i in items)
        {
            this.checkedListBox1.Items.Add(i.Name, false);
        }

        foreach (Table t in tables)
        {
            this.comboBox1.Items.Add(t.Id.ToString());
        }
    }

    private void Form1_FormClosed(object sender, FormClosedEventArgs e)
    {
        listServer.alterEvent -= new AlterDelegate(evRepeater.Repeater);
        evRepeater.alterEvent -= new AlterDelegate(DoAlterations);
    }

    private void button1_Click(object sender, EventArgs e)
    {
        Table selectedTable = tables.Where(t => t.Id.ToString() == this.comboBox1.SelectedItem.ToString()).First();
        List<Item> selectedItems = new List<Item>();


        foreach (string si in this.checkedListBox1.CheckedItems)
        {
            foreach (Item it in items.Where(i => i.Name == si))
            {
                selectedItems.Add(it);
            }
        }

        this.listServer.InsertOrder(this.listServer.GetTables().Where(t => t.Id.ToString() == this.comboBox1.SelectedItem.ToString()).First(), selectedItems);
        //Console.WriteLine("table "+tables.Where(t => t.Id.ToString() == this.comboBox1.SelectedItem.ToString()).First().Orders.Count);

        //foreach (int si in this.checkedListBox1.CheckedIndices)                               //outra change nas funções weird
        //{
        //    this.checkedListBox1.SetItemCheckState(si, CheckState.Unchecked);
        //}
    }

    private void button3_Click(object sender, EventArgs e)
    {
        foreach (string so in this.checkedListBox2.CheckedItems)
        {
            listServer.UpdateOrderToDone(listServer.GetOrdersReady().Where(or => or.Id.ToString() == so).First());
        }

        foreach (int si in this.checkedListBox1.CheckedIndices)
        {
            this.checkedListBox1.SetItemCheckState(si, CheckState.Unchecked);
        }
    }

    private void button2_Click(object sender, EventArgs e)
    {
        listServer.PayTable(tables.Where(t => t.Id.ToString() == comboBox2.SelectedItem.ToString()).First());
    }

    private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
    {
        List<Order> ordersDonebyTable = listServer.GetOrdersDone(tables.Where(t => t.Id.ToString() == this.comboBox2.SelectedItem.ToString()).First());

        foreach (Order odt in ordersDonebyTable)
        {
            this.listBox2.Items.Add(odt.Id.ToString());
        }
    }

    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        foreach (int si in this.checkedListBox1.CheckedIndices)
        {
            this.checkedListBox1.SetItemCheckState(si, CheckState.Unchecked);
        }
    }
    #endregion


}
#region remote

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
#endregion
