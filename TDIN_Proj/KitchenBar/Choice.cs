﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


    public partial class Choice : Form
    {

        public Choice()
        {
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
