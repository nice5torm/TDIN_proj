using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Collections;

public partial class Form1 : Form
{
    IManagement listServer;
    AlterEventRepeater evRepeater;

    delegate void UpdateDelegate();

    List<Order> ordersPending;
    List<Order> ordersPreparation;

    List<Item> items;
    List<Table> tables;

    public Form1(int id)
    {

        if (id == 0)
        {
            //KITCHEN
            Text = "Kitchen";
            InitializeComponent();
            listServer = (IManagement)RemoteNew.New(typeof(IManagement));

            items = listServer.GetItems();
            tables = listServer.GetTables();
            ordersPending = listServer.GetOrdersPending(id);
            ordersPreparation = listServer.GetOrdersInPreparation(id);

            evRepeater = new AlterEventRepeater();
            evRepeater.alterEvent += new AlterDelegate(DoAlterations);
            listServer.alterEvent += new AlterDelegate(evRepeater.Repeater);

        }
        else if (id == 1)
        {
            //BAR
            Text = "Bar";
            InitializeComponent();
            listServer = (IManagement)RemoteNew.New(typeof(IManagement));

            items = listServer.GetItems();
            tables = listServer.GetTables();
            ordersPending = listServer.GetOrdersPending(id);
            ordersPreparation = listServer.GetOrdersInPreparation(id);

            evRepeater = new AlterEventRepeater();
            evRepeater.alterEvent += new AlterDelegate(DoAlterations);
            listServer.alterEvent += new AlterDelegate(evRepeater.Repeater);
        }


    }

    #region callbacks
    private void ChangePending()
    {
        listBox1.Items.Clear();

        if (this.Text == "Kitchen")
        {
            foreach (Order or in listServer.GetOrdersPending(0))
            {
                listBox1.Items.Add(or.Id.ToString());
            }
        }
        else if (this.Text == "Bar")
        {
            foreach (Order or in listServer.GetOrdersPending(1))
            {
                listBox1.Items.Add(or.Id.ToString());
            }
        }

    }
    private void ChangePreparation()
    {
        listBox2.Items.Clear();

        if (this.Text == "Kitchen")
        {
            foreach (Order or in listServer.GetOrdersInPreparation(0))
            {
                listBox2.Items.Add(or.Id.ToString());
            }
        }
        else if (this.Text == "Bar")
        {
            foreach (Order or in listServer.GetOrdersInPreparation(1))
            {
                listBox2.Items.Add(or.Id.ToString());
            }
        }

    }

    public void DoAlterations(Operation op, int tabId)
    {
        UpdateDelegate UpPending;
        UpdateDelegate UpPreparation;

        switch (op)
        {

            case Operation.UpdatePending:
                UpPending = new UpdateDelegate(ChangePending);
                BeginInvoke(UpPending);
                break;
            case Operation.UpdateInPrep:
                UpPreparation = new UpdateDelegate(ChangePreparation);
                BeginInvoke(UpPreparation);
                break;

        }
    }

    #endregion


    private void Form1_Load(object sender, EventArgs e)
    {
        foreach (Order op in ordersPending)
        {
            listBox1.Items.Add(op.Id.ToString());
        }

        foreach (Order p in ordersPreparation)
        {
            listBox2.Items.Add(p.Id.ToString());
        }
    }

    private void Form1_FormClosed(object sender, FormClosedEventArgs e)
    {
        listServer.alterEvent -= new AlterDelegate(evRepeater.Repeater);
        evRepeater.alterEvent -= new AlterDelegate(DoAlterations);
    }

    private void button2_Click(object sender, EventArgs e)
    {
        listServer.UpdateOrderToInPreparation(Convert.ToInt32(listBox1.SelectedItem));
    }

    private void button1_Click(object sender, EventArgs e)
    {
        listServer.UpdateOrderToReady(Convert.ToInt32(listBox2.SelectedItem));
    }


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
