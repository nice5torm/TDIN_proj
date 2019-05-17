﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Data.Repositories
{
    public class ClientRepository : Repository<Client>
    {
        public ClientRepository(ApplicationContext context) : base(context)
        {

        }

        public Client GetWithRelated(int id)
        {
            return Context.Clients
                .Include(c => c.OrdersClient)
                .FirstOrDefault(c => c.ID == id);
        }

        public List<Client> GetListWithRelated()
        {
            return Context.Clients
                .ToList();
        }

        public Client GetByName(string name)
        {
            return Context.Clients
                .FirstOrDefault(c => c.Name == name);
        }
    }
}
