using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Order
    {
        private static int IdCounter = 1;

        public Order()
        {
            Id = IdCounter++;
        }

        public int Id { get; set; }

        public OrderTypeEnum OrderType { get; set; }

        public OrderStatusEnum OrderStatus { get; set; }

        public List<Item> Items { get; set; } = new List<Item>();
    }

    public enum OrderTypeEnum
    {
        Kitchen,
        Bar
    }

    public enum OrderStatusEnum
    {
        Pending,
        InPreparation,
        Ready,
        Done
    }

   
}