using Common;
using Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

public class Notifier : MarshalByRefObject, INotifier
    {
        public event AlterDelegate alterEvent;

    private int createdorderid;
        public override object InitializeLifetimeService()
        {
            return null;
        }

        #region Client
        public bool GetClientExists(string email)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

            return client.GetAsync("api/Client/GetClientByEmail?email=" + email).Result.IsSuccessStatusCode;
        }

        public int GetClientId(string email)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

            return client.GetAsync("api/Client/GetClientByEmail?email=" + email).Result.Content.ReadAsAsync<Client>().Result.ID;
        }

        public string GetClientEmail(int id)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

            return client.GetAsync("api/Client/GetClient?id=" + id).Result.Content.ReadAsAsync<Client>().Result.Email; 
        }
        #endregion

        #region Sale
        public bool CreatedSale(Sale sale)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

            if (client.PostAsJsonAsync("api/Sale/CreateSale", sale).Result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                NotifyClient(Operation.UpdateStock);
                NotifyClient(Operation.UpdateBooks);
                return true; 
            }
            else
                return false;
        }

        #endregion

        #region Order
        public bool CreatedOrderResponse(Order order)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

        HttpResponseMessage response = client.PostAsJsonAsync("api/Order/CreateOrder", order).Result;

        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        {
            NotifyClient(Operation.UpdateMessagesWarehouse);
            createdorderid = response.Content.ReadAsAsync<Order>().Id;

            return true;
        }
        else return false;                        
        }
        public int CreatedOrderId()
    {
        return createdorderid;
    }

        public bool EditOrderResponse(Order order)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");
        if (client.PutAsJsonAsync("api/Order/EditOrder", order).Result.StatusCode == System.Net.HttpStatusCode.OK)
        {
            NotifyClient(Operation.UpdateMessagesStore);

            return true;
        }
        else return false;

        }

        public int? GetOrderBookId(int id)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

            return client.GetAsync("api/Order/GetOrder?id=" + id).Result.Content.ReadAsAsync<Order>().Result.BookId;
        }

        public Book GetOrderBook(int id)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

            return client.GetAsync("api/Order/GetOrder?id=" + id).Result.Content.ReadAsAsync<Order>().Result.Book;
        }

        public int? GetOrderClientId(int id)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

            return client.GetAsync("api/Order/GetOrder?id=" + id).Result.Content.ReadAsAsync<Order>().Result.ClientId;
        }

        public Client GetOrderClient(int id)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

            return client.GetAsync("api/Order/GetOrder?id=" + id).Result.Content.ReadAsAsync<Order>().Result.Client;
        }

        public int GetOrderQuantity(int id)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

            return client.GetAsync("api/Order/GetOrder?id=" + id).Result.Content.ReadAsAsync<Order>().Result.Quantity;
        }

        public DateTime GetOrderDispatchOccurence(int id)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

            return client.GetAsync("api/Order/GetOrder?id=" + id).Result.Content.ReadAsAsync<Order>().Result.DispatchOccurence;
        }

        public OrderStatusEnum GetOrderStatus(int id)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

            return client.GetAsync("api/Order/GetOrder?id=" + id).Result.Content.ReadAsAsync<Order>().Result.OrderStatus;
        }

        public OrderTypeEnum GetOrderType(int id)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

            return client.GetAsync("api/Order/GetOrder?id=" + id).Result.Content.ReadAsAsync<Order>().Result.OrderType;
        }
             
        #endregion

        #region Book
        public bool GetBookResponse()
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");
        if (client.GetAsync("api/Book/GetBooks").Result.StatusCode == System.Net.HttpStatusCode.OK)
            return true;
        else return false;

        }

        public bool GetBookResponseByTitle(string title)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

        if (client.GetAsync("api/Book/GetBookByTitle?title=" + title).Result.StatusCode == System.Net.HttpStatusCode.OK)
            return true;
        else return false;
        }

        public bool GetBookResponseById(int id)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

        if (client.GetAsync("api/Book/GetBook?id=" + id).Result.StatusCode == System.Net.HttpStatusCode.OK)
            return true;
        else return false;
        }

        public int GetAmountBook(int id)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

            return client.GetAsync("api/Book/GetBook?id=" + id).Result.Content.ReadAsAsync<Book>().Result.Amount;
        }

        public double GetPriceBook(int id)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

            return client.GetAsync("api/Book/GetBook?id=" + id).Result.Content.ReadAsAsync<Book>().Result.Price;
        }

        public string GetTitleBook(int id)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

            return client.GetAsync("api/Book/GetBook?id=" + id).Result.Content.ReadAsAsync<Book>().Result.Title;
        }

        public int GetIdBookByTitle(string title)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

            return client.GetAsync("api/Book/GetBookByTitle?title=" + title).Result.Content.ReadAsAsync<Book>().Result.Id;
        }

        public int GetAmountBookByTitle(string title)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

            return client.GetAsync("api/Book/GetBookByTitle?title=" + title).Result.Content.ReadAsAsync<Book>().Result.Amount;
        }

        public double GetPriceBookByTitle(string title)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

            return client.GetAsync("api/Book/GetBookByTitle?title=" + title).Result.Content.ReadAsAsync<Book>().Result.Price;
        }

        public void EditBook(Book book)
        {
            HttpClient client = new HttpClient();

            client.BaseAddress = new Uri("http://localhost:2222/");

            NotifyClient(Operation.UpdateBooks);

            NotifyClient(Operation.UpdateStock);

            client.PutAsJsonAsync("api/Book/EditBook", book);
        }
        #endregion


        void NotifyClient(Operation op)
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
                            handler(op);
                            Console.WriteLine("Invoking event handler");
                            //Console.WriteLine(op.ToString());
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
    }

