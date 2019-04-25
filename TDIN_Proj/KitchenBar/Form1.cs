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

        public Form1(int id)
        {

            if (id == 0)
            {
                //KITCHEN
                Text = "Kitchen";
                InitializeComponent();
                listServer = (IManagement)RemoteNew.New(typeof(IManagement));

            }
            else if (id == 1)
            {
                //BAR
                Text = "Bar";
                InitializeComponent();
                listServer = (IManagement)RemoteNew.New(typeof(IManagement));

            }

            ordersPending = listServer.GetOrdersPending();
            ordersPreparation = listServer.GetOrdersInPreparation();

            evRepeater = new AlterEventRepeater();
            evRepeater.alterEvent += new AlterDelegate(DoAlterations);
            listServer.alterEvent += new AlterDelegate(evRepeater.Repeater);

            
        }

        #region callbacks
        private void ChangePending()
        {
            foreach (Order or in listServer.GetOrdersPending())
            {
                this.listBox1.Items.Add(or.Id.ToString());
            }
        }
        private void ChangePreparation()
        {
            foreach (Order or in listServer.GetOrdersInPreparation())
            {
                this.listBox2.Items.Add(or.Id.ToString());
            }
        }

    public void DoAlterations(Operation op, int tabId)
        {
            UpdateDelegate UpPending;
            UpdateDelegate UpPreparation;

            switch (op)
            {
               
                case Operation.UpdateReady:
                    UpPending = new UpdateDelegate(ChangePending);
                    BeginInvoke(UpPending);
                    break;
                case Operation.PayableTables:
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
                this.listBox1.Items.Add(op.Id.ToString());
            }

            foreach (Order p in ordersPreparation)
            {
                this.listBox2.Items.Add(p.Id.ToString());
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            listServer.alterEvent -= new AlterDelegate(evRepeater.Repeater);
            evRepeater.alterEvent -= new AlterDelegate(DoAlterations);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listServer.UpdateOrderToInPreparation(listServer.GetOrdersPending().Where(or => or.Id == Convert.ToInt32(this.listBox1.SelectedItem)).First());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listServer.UpdateOrderToReady(listServer.GetOrdersInPreparation().Where(or => or.Id == Convert.ToInt32(this.listBox2.SelectedItem)).First());
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
