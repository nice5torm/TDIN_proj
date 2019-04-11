using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDIN_Project.Models
{
    public class Item
    {
        private static int IdCounter = 1;

        public Item()
        {
            Id = IdCounter++;
        }

        public int Id { get; set; }
    }
}