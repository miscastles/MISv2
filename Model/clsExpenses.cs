using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsExpenses
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _ExpensesID;
        public static string _Description;

        public static int ClassExpensesID
        {
            get { return _ExpensesID; }
            set { _ExpensesID = value; }
        }
        public static string ClassDescription
        {
            get { return _Description; }
            set { _Description = value; }
        }
    }
}
