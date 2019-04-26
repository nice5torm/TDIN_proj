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
    delegate void InvoiceDelegate(int tabId);
    delegate void MakeDelegate();


    public Form1()
    {
        RemotingConfiguration.Configure("DinningRoom.exe.config", false);
        InitializeComponent();
        listServer = (IManagement)RemoteNew.New(typeof(IManagement));


        items = listServer.GetItems();
        tables = listServer.GetTables();


        evRepeater = new AlterEventRepeater();
        evRepeater.alterEvent += new AlterDelegate(DoAlterations);
        listServer.alterEvent += new AlterDelegate(evRepeater.Repeater);
  

    }
    #region callbacks

    private void MakeOrderTable()
    {
        foreach (int si in this.checkedListBox1.CheckedIndices)                               
        {
            this.checkedListBox1.SetItemCheckState(si, CheckState.Unchecked);
        }
    }
    private void ChangeReady()
    {
        this.checkedListBox2.Items.Clear();

        foreach (Order or in listServer.GetOrdersReady())
        {
           
            this.checkedListBox2.Items.Add(or.Id.ToString(), false);
        }
    }

    private void ChangePayTables()
    {
        this.comboBox2.Items.Clear();

        foreach (Table pt in listServer.GetPayableTables())
        {
            this.comboBox2.Items.Add(pt.Id.ToString());
        }
    }

    private void ChangeInvoice(int tabId)
    {
        this.listBox2.Items.Clear();

        foreach (Order or in listServer.GetPayableTables().Where(tab => tab.Id == tabId).First().Orders)
        {
            this.listBox2.Items.Add(or.Id);
        }
    }

    public void DoAlterations(Operation op, int tabId)
    {
        MakeDelegate MakeOr;
        UpdateDelegate UpReady;
        UpdateDelegate UpTab;
        InvoiceDelegate Invoice; 

        switch (op)
        {
            case Operation.MakeOrder:
                MakeOr = new MakeDelegate(MakeOrderTable);
                BeginInvoke(MakeOr);
                break;
            case Operation.UpdateReady:
                UpReady = new UpdateDelegate(ChangeReady);
                BeginInvoke(UpReady);
                break;
            case Operation.PayableTables:
                UpTab = new UpdateDelegate(ChangePayTables);
                BeginInvoke(UpTab);
                break;
            case Operation.Invoice:
                Invoice = new InvoiceDelegate(ChangeInvoice);
                BeginInvoke(Invoice, new object[] { tabId });
                break;

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

        foreach(Order or in listServer.GetOrdersReady())
        {
            this.checkedListBox2.Items.Add(or.Id);
        }
        
        foreach(Table t in listServer.GetPayableTables())
        {
            this.comboBox2.Items.Add(t.Id.ToString());
        }

    }

    private void Form1_FormClosed(object sender, FormClosedEventArgs e)
    {
        listServer.alterEvent -= new AlterDelegate(evRepeater.Repeater);
        evRepeater.alterEvent -= new AlterDelegate(DoAlterations);
    }

    private void button1_Click(object sender, EventArgs e)
    {
        Table selectedTable = tables.Where(t => t.Id == Convert.ToInt32(this.comboBox1.SelectedItem)).First();
        List<Item> selectedItems = new List<Item>();


        foreach (string si in this.checkedListBox1.CheckedItems)
        {
            foreach (Item it in items.Where(i => i.Name == si))
            {
                selectedItems.Add(it);
            }
        }
        listServer.InsertOrder(Convert.ToInt32(this.comboBox1.SelectedItem), selectedItems);
    }

    private void button3_Click(object sender, EventArgs e)
    {
        foreach (string so in this.checkedListBox2.CheckedItems)
        {
            listServer.UpdateOrderToDone(Convert.ToInt32(so));
        }
        checkedListBox2.Items.Clear();

        foreach (Order or in listServer.GetOrdersReady())
        {
            this.checkedListBox2.Items.Add(or.Id);
        }

    }

    private void button2_Click(object sender, EventArgs e)
    {
        listServer.PayTable(Convert.ToInt32(comboBox2.SelectedItem));

        comboBox2.Items.Remove(comboBox2.SelectedItem);
        listBox2.Items.Clear();

    }

    private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
    {
        listBox2.Items.Clear();
        foreach (Order odt in listServer.GetOrdersDone(Convert.ToInt32(comboBox2.SelectedItem)))
        {
            listBox2.Items.Add(odt.Id.ToString());
        }
    }

    private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        foreach (int si in checkedListBox1.CheckedIndices)
        {
            checkedListBox1.SetItemCheckState(si, CheckState.Unchecked);
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
