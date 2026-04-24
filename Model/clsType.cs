using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsType
    {
        public static bool RecordFound;
        public static int TypeID;
        public static string Description;
        public static string Remarks;
        public static string QueryString;

        public static int ClassTypeID
        {
            get { return TypeID; }
            set { TypeID = value; }
        }
        public static string ClassDescription
        {
            get { return Description; }
            set { Description = value; }
        }
        public static string ClassRemarks
        {
            get { return Remarks; }
            set { Remarks = value; }
        }
        public static string ClassQueryString
        {
            get { return QueryString; }
            set { QueryString = value; }
        }

    }
}
