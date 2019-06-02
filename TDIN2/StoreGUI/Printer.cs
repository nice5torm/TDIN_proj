using Common.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StoreGUI
{
    public partial class Printer : Form
    {
        public Printer(string title, string price, string quantity, string name, string email, string address, string total)
        {
          
            InitializeComponent();
         
            this.title.Text = title;
            this.price.Text = price;
            this.name.Text = name;
            this.email.Text = email;
            this.address.Text = address;
            this.quantity.Text = quantity; 
            this.total.Text = total;

        }
    }
}
