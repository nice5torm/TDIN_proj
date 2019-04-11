using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TDIN_Project.Models;

namespace TDIN_Project
{
    public class DatabaseManager
    {
        public List<Table> tables = new List<Table>();

        #region Table
        public List<Table> GetTables()
        {
            return tables;
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
            return orders;
        }

        public Order GetOrder(int id)
        {
            return orders.Where(o => o.Id == id).FirstOrDefault();
        }

        public void InsertOrder(Order order)
        {
            orders.Add(order);
        }

        public void UpdateOrder(Order order)
        {
            DeleteOrder(order.Id);
            InsertOrder(order);
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

        public void InsertItem(Item item)
        {
            items.Add(item);
        }

        public void Updateitem(Item item)
        {
            DeleteItem(item.Id);
            InsertItem(item);
        }

        public void DeleteItem(int id)
        {
            Item item = GetItem(id);
            items.Remove(item);
        }
        #endregion
    }
}