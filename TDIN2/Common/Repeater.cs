using Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public interface INotifier
{
    event AlterDelegate alterEvent;

    bool GetClientExists(string email);
    int GetClientId(string email);
    string GetClientEmail(int id);
    bool CreatedSale(Sale sale);
    bool CreatedOrderResponse(Order order);
    bool EditOrderResponse(Order order);
    int CreatedOrderId();
    int? GetOrderBookId(int id);
    Book GetOrderBook(int id);
    int? GetOrderClientId(int id);
    Client GetOrderClient(int id);
    int GetOrderQuantity(int id);
    DateTime GetOrderDispatchOccurence(int id);
    OrderStatusEnum GetOrderStatus(int id);
    OrderTypeEnum GetOrderType(int id);
    bool GetBookResponse();
    bool GetBookResponseByTitle(string title);
    bool GetBookResponseById(int id);
    int GetAmountBook(int id);
    double GetPriceBook(int id);
    string GetTitleBook(int id);
    int GetIdBookByTitle(string title);
    int GetAmountBookByTitle(string title);
    double GetPriceBookByTitle(string title);
    void EditBook(Book book);

}

    public enum Operation { UpdateStock, UpdateBooks, UpdateMessagesWarehouse, UpdateMessagesStore }; 

    public delegate void AlterDelegate(Operation op);



    public class Repeater : MarshalByRefObject
    {
        public event AlterDelegate alterEvent;

        public void RepeaterAll(Operation op)
        {
            if (alterEvent != null)
                alterEvent(op);

        }

       
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
