﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


[Serializable]
public class Table
{
    private static int IdCounter = 1;

    public Table()
    {
        Id = IdCounter++;
        TableStatus = TableStatusEnum.NoOrder;
        Orders = new List<Order>();
    }

    public void AddOrderTable(Order order)
    {
        TableStatus = TableStatusEnum.HasUnpaidOrder;
        Orders.Add(order);
    }

    public int Id { get; set; }

    public TableStatusEnum TableStatus { get; set; }

    public List<Order> Orders { get; set; } = new List<Order>();
}

public enum TableStatusEnum
{
    HasUnpaidOrder,
    NoOrder
}
