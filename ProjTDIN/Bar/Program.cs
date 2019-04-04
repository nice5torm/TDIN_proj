using System;

namespace KitchenBar
{
    class Program
    {
        static void Main(string[] args)
        {
            RemotingConfiguration.Configure("KitchenBar.exe.config", false);
        }
    }
}
