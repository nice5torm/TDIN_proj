using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading;


public class Management : MarshalByRefObject, IManagement
{
    public List<Table> tables;
    public List<Item> itemsList;
    public event AlterDelegate alterEvent;

    public Management()
    {
        tables = new List<Table>();
        itemsList = new List<Item>();

        Item item1 = new Item(8, "Vinho da casa", Item.ItemTypeEnum.Bar);
        Item item2 = new Item(1, "Agua", Item.ItemTypeEnum.Bar);
        Item item3 = new Item(5, "Prego em Prato", Item.ItemTypeEnum.Kitchen);
        Item item4 = new Item(10, "Francesinha", Item.ItemTypeEnum.Kitchen);
        Item item5 = new Item(9, "Cachorro", Item.ItemTypeEnum.Kitchen);
        Item item6 = new Item(2, "Refrigerante", Item.ItemTypeEnum.Bar);

        Table table1 = new Table();
        Table table2 = new Table();
        Table table3 = new Table();
        Table table4 = new Table();
        Table table5 = new Table();

        itemsList.Add(item1);
        itemsList.Add(item2);
        itemsList.Add(item3);
        itemsList.Add(item4);
        itemsList.Add(item5);
        itemsList.Add(item6);

        tables.Add(table1);
        tables.Add(table2);
        tables.Add(table3);
        tables.Add(table4);
        tables.Add(table5);
    }

    public override object InitializeLifetimeService()
    {
        return null;
    }

    void NotifyClients(Operation op, int tabId)
    {
        if (alterEvent != null)
        {
            Delegate[] invkList = alterEvent.GetInvocationList();

            foreach (AlterDelegate handler in invkList)
            {
                new Thread(() =>
                {
                    try
                    {
                        handler(op, tabId);
                        Console.WriteLine("Invoking event handler");
                        Console.WriteLine(op.ToString());
                    }
                    catch (Exception)
                    {
                        alterEvent -= handler;
                        Console.WriteLine("Exception: Removed an event handler");
                    }
                }).Start();
            }
        }
    }

    #region Table
    public List<Table> GetTables()
    {
        return tables;
    }


    public Table GetTable(int id)
    {
        return tables.Where(t => t.Id == id).FirstOrDefault();
    }

    public List<Table> GetPayableTables()
    {
        return tables.Where(t => t.TableStatus == TableStatusEnum.HasUnpaidOrder && t.Orders.Count() == t.Orders.Where(o => o.OrderStatus == OrderStatusEnum.Done).Count()).ToList();
    }

    public void PayTable(int tabId)
    {
        tables.Where(t => t.Id == tabId).First().Orders.Clear();
        tables.Where(t => t.Id == tabId).First().TableStatus = TableStatusEnum.NoOrder;
        NotifyClients(Operation.Pay, tabId);
    }

    #endregion

    #region Order

    public List<Order> GetOrdersPending()
    {
        List<Order> orders = new List<Order>();
        foreach (Table t in tables)
        {
            foreach (Order o in t.Orders)
            {
                if (o.OrderStatus == OrderStatusEnum.Pending)
                {
                    orders.Add(o);
                }
            }
        }

        return orders;
    }

    public List<Order> GetOrdersInPreparation()
    {
        List<Order> orders = new List<Order>();
        foreach (Table t in tables)
        {
            foreach (Order o in t.Orders)
            {
                if (o.OrderStatus == OrderStatusEnum.InPreparation)
                {
                    orders.Add(o);
                }
            }
        }

        return orders;
    }

    public List<Order> GetOrdersReady()
    {
        List<Order> orders = new List<Order>();
        foreach (Table t in tables)
        {
            foreach (Order o in t.Orders)
            {
                if (o.OrderStatus == OrderStatusEnum.Ready)
                {
                    orders.Add(o);
                }
            }
        }

        return orders;
    }

    public List<Order> GetOrdersDone(int tabId)
    {
        return tables.Where(t => t.Id == tabId).First().Orders.Where(o => o.OrderStatus == OrderStatusEnum.Done).ToList();
    }

    public void InsertOrder(int tabId, List<Item> items)
    {
        List<Item> itemsKitchen = new List<Item>();
        List<Item> itemsBar = new List<Item>();
        foreach (Item i in items)
        {
            if (i.ItemType == Item.ItemTypeEnum.Kitchen)
            {
                itemsKitchen.Add(i);
            }
            else if (i.ItemType == Item.ItemTypeEnum.Bar)
            {
                itemsBar.Add(i);
            }
        }
        if (itemsBar.Count() == 0)
        {
            if (itemsKitchen.Count() != 0)
            {
                Order orderKitchen = new Order(OrderTypeEnum.Kitchen, itemsKitchen);
                tables.Where(t => t.Id == tabId).First().AddOrderTable(orderKitchen);
                NotifyClients(Operation.MakeOrder, tabId);
                NotifyClients(Operation.UpdatePending, 1);

                Console.WriteLine("orderkitchen:" + tables.Where(t => t.Id == tabId).First().Orders.Count);
            }
        }
        else
        {
            Order orderBar = new Order(OrderTypeEnum.Bar, itemsBar);
            tables.Where(t => t.Id == tabId).First().AddOrderTable(orderBar);
            NotifyClients(Operation.MakeOrder, tabId);
            NotifyClients(Operation.UpdatePending, 1);

            Console.WriteLine("orderbar:" + tables.Where(t => t.Id == tabId).First().Orders.Count);


            if (itemsKitchen.Count() != 0)
            {
                Order orderKitchen = new Order(OrderTypeEnum.Kitchen, itemsKitchen);
                tables.Where(t => t.Id == tabId).First().AddOrderTable(orderKitchen);
                NotifyClients(Operation.MakeOrder, tabId);
                NotifyClients(Operation.UpdatePending, 1);

                Console.WriteLine("orderkitchen2:" + tables.Where(t => t.Id == tabId).First().Orders.Count);
            }
        }

    }

    public void UpdateOrderToInPreparation(Order order)
    {
        order.OrderStatus = OrderStatusEnum.InPreparation;
        NotifyClients(Operation.UpdatePending, 1);
        NotifyClients(Operation.UpdateInPrep, 1);
    }

    public void UpdateOrderToReady(Order order)
    {
        order.OrderStatus = OrderStatusEnum.Ready;
        NotifyClients(Operation.UpdateInPrep, 1);
        NotifyClients(Operation.UpdateReady, 1);
    }

    public void UpdateOrderToDone(Order order)
    {
        order.OrderStatus = OrderStatusEnum.Done;
        NotifyClients(Operation.UpdateReady, 1);
        NotifyClients(Operation.PayableTables, 1);
    }

    #endregion

    #region Item
    public List<Item> GetItems()
    {
        return itemsList;
    }

    #endregion
}

