using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KitchenBar
{
    public partial class Choice : Form
    {
        IManagement listServer;

        public Choice()
        {
            RemotingConfiguration.Configure("KitchenBar.exe.config", false);
            //listServer = (IManagement)RemoteNew.New(typeof(IManagement));

            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 formBar = new Form1(1);
            formBar.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 formKitchen = new Form1(0);
            formKitchen.ShowDialog();
        }
    }
}
