using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



    public interface IManagement
    {
        event AlterDelegate alterEvent;

        List<Table> GetTables();  
        List<Table> GetPayableTables();
        void PayTable(Table table);
        List<Order> GetOrdersPending();
        List<Order> GetOrdersInPreparation();
        List<Order> GetOrdersReady();
        List<Order> GetOrdersDone(Table table);
        void InsertOrder(Table table, List<Item> items); 
        void UpdateOrderToInPreparation(Order order);
        void UpdateOrderToReady(Order order);
        void UpdateOrderToDone(Order order);
        List<Item> GetItems();  
    }

    public enum Operation {UpdatePending, UpdateInPrep, UpdateReady , Pay, Invoice }; //a ver isto


    public delegate void AlterDelegate(Operation op, Table t);

    public class AlterEventRepeater : MarshalByRefObject
    {
        public event AlterDelegate alterEvent;

        public override object InitializeLifetimeService()
        {
            return null;
        }


        public void Repeater(Operation op, Table t)
        {
            if (alterEvent != null)
                alterEvent(op, t);
        }

    }


