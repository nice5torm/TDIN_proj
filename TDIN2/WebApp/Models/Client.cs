using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class Client
    {
        [Key]
        public int ID;

        public string Name;

        public string Email;

        public string Address;

        public List<Order> OrdersClient; 

    }
}
