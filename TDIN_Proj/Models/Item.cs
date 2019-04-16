using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Item
    {
        private static int IdCounter = 1;

        public Item(int price, string name)
        {
            Id = IdCounter++;
            Price = price;
            Name = name;
        }

        public int Id { get; set; }

        public double Price { get; set; }

        public string Name { get; set; }
    }
}