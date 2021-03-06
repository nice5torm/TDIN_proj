﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Order
    {
        [Key]
        public int GUID { get; set; }

        public DateTime DispatchedDate { get; set; }

        public DateTime DispatchOccurence { get; set; }

        public int Quantity { get; set; }

        public int? ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client Client { get; set; }

        public OrderStatusEnum OrderStatus { get; set; }

        public OrderTypeEnum OrderType { get; set; }

        public int? BookId { get; set; }

        [ForeignKey("BookId")]
        public Book Book { get; set; }
    }
    public enum OrderStatusEnum
    {
        Wainting_expedition,       
        Dispatched,                 
        DispatchOccurence
    }
    public enum OrderTypeEnum
    {
        Web,
        Store
    }
}
