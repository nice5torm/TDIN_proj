using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public double Price { get; set; }

        public int Amount { get; set; }
        
    }
}
