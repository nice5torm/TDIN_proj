using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace TDIN_Project
{
    class Server
    {
        static void Main(string[] args)
        {
            RemotingConfiguration.Configure("Server.exe.config", false);

            Console.WriteLine("Welcome to the server \n Press return to exit");
            Console.ReadLine();
        }
    }
}