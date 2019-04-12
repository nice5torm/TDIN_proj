using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDIN_Project.Business;

namespace TDIN_Project.Models
{
    public class Order
    {
        private static int IdCounter = 1;

        public Order()
        {
            Id = IdCounter++;
        }

        public int Id { get; set; }

        public Enumerations.OrderTypeEnum OrderType { get; set; }

        public Enumerations.OrderStatusEnum OrderStatus { get; set; }

        public List<Item> Items { get; set; } = new List<Item>();
    }
}