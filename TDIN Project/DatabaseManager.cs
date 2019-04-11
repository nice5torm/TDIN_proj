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

        #endregion

        #region Item

        #endregion
    }
}