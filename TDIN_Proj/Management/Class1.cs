using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using static Models.Class1;

namespace Management
{
    public class Management : MarshalByRefObject, IManagement
    {
        public List<Table> tables = new List<Table>();
        public List<Item> itemsList = new List<Item>();


        public Management()
        {
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


        #region Table
        public List<Table> GetTables()
        {
            return tables;
        }

        //a ver
        public List<Table> GetTablesUnpaid()
        {
            return tables.Where(t => t.TableStatus == TableStatusEnum.HasUnpaidOrder).ToList();   
        }

        public Table GetTable(int id)
        {
            return tables.Where(t => t.Id == id).FirstOrDefault();
        }

        public void AddOrderTable(Order order, Table table)
        {
            table.Orders.Add(order); 
        }

        public List<Table> GetPayableTables()
        {
            return tables.Where(t => t.TableStatus == TableStatusEnum.HasUnpaidOrder && t.Orders.Count() == t.Orders.Where(o => o.OrderStatus == OrderStatusEnum.Done).Count()).ToList();
        }

        public void PayTable(Table table)
        {
            table.Orders.Clear();
            table.TableStatus = TableStatusEnum.NoOrder;
        }

        #endregion

        #region Order

        public List<Order> GetOrdersPending(OrderTypeEnum orderType)
        {
            List<Order> orders = new List<Order> ();
            foreach(Table t in tables)
            {
                foreach(Order o in t.Orders)
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

        public List<Order> GetOrdersDone(Table table)
        {
            return table.Orders.Where(o => o.OrderStatus == OrderStatusEnum.Done).ToList();
        }

        public void InsertOrder(Table table, List<Item> items)
        {
            List<Item> itemsKitchen = items.Where(i => i.ItemType == Item.ItemTypeEnum.Kitchen).ToList();
            List<Item> itemsBar = items.Where(i => i.ItemType == Item.ItemTypeEnum.Bar).ToList();

            Order orderKitchen = new Order(OrderTypeEnum.Kitchen, itemsKitchen);
            Order orderBar = new Order(OrderTypeEnum.Bar, itemsBar);

            table.Orders.Add(orderKitchen);
            table.Orders.Add(orderBar);
        }

        public void UpdateOrderToInPreparation(Order order)
        {
            order.OrderStatus = OrderStatusEnum.InPreparation;
        }

        public void UpdateOrderToReady(Order order)
        {
            order.OrderStatus = OrderStatusEnum.Ready; 
        }

        public void UpdateOrderToDone(Order order)
        {
            order.OrderStatus = OrderStatusEnum.Done;
        }

        #endregion

        #region Item
        public List<Item> GetItems()
        {
            return itemsList;
        }

        #endregion
    }
}
