using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class Stock
    {
        public int? BookId { get; set; }

        [ForeignKey("BookId")]
        public Book Book { get; set; }

        public int Quantity { get; set; }

        public int? LocationId { get; set; }

        [ForeignKey("LocationId")]
        public Location Location { get; set; }

    }
}
