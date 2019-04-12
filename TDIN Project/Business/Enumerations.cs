using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDIN_Project.Business
{
    public class Enumerations
    {
        public enum OrderTypeEnum
        {
            Kitchen,
            Bar
        }

        public enum OrderStatusEnum
        {
            Pending,
            InPreparation,
            Ready,
            Done
        }

        public enum TableStatusEnum
        {
            //???
            Unpaid,
            Paid
        }
    }
}