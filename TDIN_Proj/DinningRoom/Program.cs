using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Models.Class1;

namespace DinningRoom
{
    static class Program
    {
        public static IManagement ListServer; //?

        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ListServer = (IManagement)Activator.GetObject(typeof(IManagement), "tcp://localhost:9000/Server/OrdersServer"); //?

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
