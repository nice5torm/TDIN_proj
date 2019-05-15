using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    [Serializable]
    public class Book
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }
        
    }
}
