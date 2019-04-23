using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public interface IManagement
    {
        List<Table> GetTables();  //
        List<Table> GetPayableTables();
        void PayTable(Table table);
        List<Order> GetOrdersPending();
        List<Order> GetOrdersInPreparation();
        List<Order> GetOrdersReady();
        List<Order> GetOrdersDone(Table table);
        void InsertOrder(Table table, List<Item> items); //
        void UpdateOrderToInPreparation(Order order);
        void UpdateOrderToReady(Order order);
        void UpdateOrderToDone(Order order);
        List<Item> GetItems();  //
    }
}
