using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    [Serializable]
    public class Item
    {
        private static int IdCounter = 1;

        public Item(int price, string name, ItemTypeEnum itemType)
        {
            Id = IdCounter++;
            Price = price;
            Name = name;
            ItemType = itemType;
        }

        public int Id { get; set; }
        public ItemTypeEnum ItemType { get; set; }

        public double Price { get; set; }

        public string Name { get; set; }

        public enum ItemTypeEnum
        {
            Kitchen,
            Bar
        }
    }
}