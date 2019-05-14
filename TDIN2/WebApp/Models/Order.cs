using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class Order
    {
        private static int GUID = 1;

        public Order(string title, int quantity, string cli_name, string address, string email)
        {
            GUID = GUID++;
        }

    }
    public enum OrderStatusEnum
    {
        Wainting_expedition,
        Dispatched, 
        Dispatch
    }
    public enum OrderTypeEnum
    {
        Web,
        Sell
    }
}
