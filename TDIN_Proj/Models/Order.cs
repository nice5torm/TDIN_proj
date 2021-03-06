﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


[Serializable]
public class Order
{
    private static int IdCounter = 1;

    public Order(OrderTypeEnum orderType, List<Item> items)
    {
        Id = IdCounter++;
        OrderStatus = OrderStatusEnum.Pending;
        OrderType = orderType;
        Items = items;
    }

    public void UpdatetoPreparation()
    {
        OrderStatus = OrderStatusEnum.InPreparation;
    }
    public void UpdatetoReady()
    {
        OrderStatus = OrderStatusEnum.Ready;
    }
    public void UpdatetoDone()
    {
        OrderStatus = OrderStatusEnum.Done;
    }

    public int Id { get; set; }

    public OrderStatusEnum OrderStatus { get; set; }

    public OrderTypeEnum OrderType { get; set; }

    public List<Item> Items { get; set; } = new List<Item>();
}

public enum OrderStatusEnum
{
    Pending,
    InPreparation,
    Ready,
    Done
}
public enum OrderTypeEnum
{
    Kitchen,
    Bar
}

