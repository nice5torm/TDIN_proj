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
    void PayTable(int tabId);
    double GetOrderPrice(int orId);
    double GetTablePrice(int tabId);
    List<Order> GetOrdersPending(int kb);
    List<Order> GetOrdersInPreparation(int kb);
    List<Order> GetOrdersReady();
    List<Order> GetOrdersDone(int tabId);
    void InsertOrder(int tabId, List<Item> items);
    void UpdateOrderToInPreparation(int orderId);
    void UpdateOrderToReady(int orderId);
    void UpdateOrderToDone(int orderId);
    List<Item> GetItems();
}

public enum Operation { MakeOrder, UpdatePending, UpdateInPrep, UpdateReady, PayableTables, Invoice, Pay }; //a ver isto


public delegate void AlterDelegate(Operation op, int tabId);

public class AlterEventRepeater : MarshalByRefObject
{
    public event AlterDelegate alterEvent;
    public event AlterDelegate alterEvent1;

    public override object InitializeLifetimeService()
    {
        return null;
    }


    public void Repeater(Operation op, int tabId)
    {
        if (alterEvent != null)
            alterEvent(op,  tabId);

    }

}


