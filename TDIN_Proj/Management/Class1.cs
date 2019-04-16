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
        public ArrayList itemsList = new ArrayList();

        Item item = new Item(8, "Vinho");
        Item item2 = new Item(1, "Agua");
        Item item3 = new Item(5, "Prego em Prato");
        Item item4 = new Item(10, "Francesinha");
        Item item5 = new Item(9, "Cachorro");
        Item item6 = new Item(2, "Refrigerante");

        public Management()
        {
            itemsList.Add(item);
            itemsList.Add(item2);
            itemsList.Add(item3);
            itemsList.Add(item4);
            itemsList.Add(item5);
            itemsList.Add(item6);
        }


        #region Table
        public List<Table> GetTables()
        {
            return tables;
        }

        //a ver
        public List<Table> GetTablesUnpaid()
        {
            return tables.Where(t => t.TableStatus == ); //unpaid
        }

        public Table GetTable(int id)
        {
            return tables.Where(t => t.Id == id).FirstOrDefault();
        }

        public void InsertTable(Table table)
        {
            tables.Add(table);
        }

        public void UpdateTable(Table table)
        {
            DeleteTable(table.Id);
            InsertTable(table);
        }

        public void DeleteTable(int id)
        {
            Table table = GetTable(id);
            tables.Remove(table);
        }
        #endregion

        #region Order

        public List<Order> GetOrders()
        {
            return Orders;
        }

        public List<Order> GetOrdersPending()
        {
            return orders.Where(o => o.OrderStatus == );
        }

        public List<Order> GetOrdersInPreparation()
        {
            return orders.Where(o => o.OrderStatus == );
        }

        public List<Order> GetOrdersReady()
        {
            return orders.Where(o => o.OrderStatus == );
        }

        public List<Order> GetOrdersDone(Table table)
        {
            return table.Orders.Where(o => o.OrderStatus ==);
        }

        public Order GetOrder(int id)
        {
            return orders.Where(o => o.Id == id).FirstOrDefault();
        }

        public void InsertOrder(Table table, Order order)
        {
            table.Orders.Add(order.Where(o => o.OrderStatus == Pending));
        }

        public void UpdateOrderToInPreparation(Order order)
        {

        }

        public void UpdateOrderToReady(Order order)
        {

        }

        public void UpdateOrderToDone(Order order)
        {

        }

        public void DeleteOrder(int id)
        {
            Order order = GetOrder(id);
            orders.Remove(order);
        }
        #endregion

        #region Item
        public List<Item> GetItems()
        {
            return items;
        }

        public Item GetItem(int id)
        {
            return items.Where(i => i.Id == id).FirstOrDefault();
        }

        public void AddItem(Item item)
        {
            items.Add(item);
        }

        public void Updateitem(Item item)
        {
            DeleteItem(item.Id);
            AddItem(item);
        }
        #endregion
    }
}
