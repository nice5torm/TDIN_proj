using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Models.Class1;

namespace KitchenBar
{
    static class KitchenBar
    {
        public static IManagement listServer;

        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            RemotingConfiguration.Configure("KitchenBar.exe.config", false);

            listServer = (IManagement)Activator.GetObject(typeof(IManagement), "tcp://localhost:9000/Server/ListServer");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Choice());
        }
    }
}
