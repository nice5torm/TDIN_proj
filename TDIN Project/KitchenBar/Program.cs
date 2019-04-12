using System;
using System.Runtime.Remoting;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KitchenBar
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            RemotingConfiguration.Configure("KitchenBar.exe.config", false);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


            Application.Run(new Form1(0));      //kitchen
            Application.Run(new Form1(1));      //bar

        }
    }
}
