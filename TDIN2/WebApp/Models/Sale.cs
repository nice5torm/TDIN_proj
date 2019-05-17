using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class Sale
    {
        [Key]
        public int GUID { get; set; }

        public int Quantity { get; set; }

        public int? ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        public int? BookId { get; set; }

        [ForeignKey("BookId")]
        public Book Book { get; set; }

    }
}
