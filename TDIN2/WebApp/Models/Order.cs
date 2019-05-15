using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class Order
    {
        [Key]
        public int GUID { get; set; }

        public DateTime WaitingDate { get; set; }

        public DateTime DispatchedDate { get; set; }

        public OrderStatusEnum OrderStatus { get; set; }

        public OrderTypeEnum OrderType { get; set; }

        public int? BookId;

        [ForeignKey("BookId")]
        public Book Book; 
    }
    public enum OrderStatusEnum
    {
        Wainting_expedition,       
        Dispatched,                 
        Dispatch
    }
    public enum OrderTypeEnum
    {
        Web,
        Sell
    }
}
