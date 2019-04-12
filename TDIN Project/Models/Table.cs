using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDIN_Project.Business;


namespace TDIN_Project.Models
{
    public class Table
    {
        private static int IdCounter = 1;

        public Table()
        {
            Id = IdCounter++;
        }

        public int Id { get; set; }

        public Enumerations.TableStatusEnum TableStatus { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();
    }
}