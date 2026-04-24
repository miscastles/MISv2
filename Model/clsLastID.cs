using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsLastID
    {
        public static bool RecordFound;
        public static int _LastInsertedID;
        public static int _LastInsertedCityID;
        public static int _LastInsertedProvinceID;
        public static string _LastTableName;        

        public static int ClassLastInsertedID
        {
            get { return _LastInsertedID; }
            set { _LastInsertedID = value; }
        }
        public static int ClassLastInsertedCityID
        {
            get { return _LastInsertedCityID; }
            set { _LastInsertedCityID = value; }
        }
        public static int ClassLastInsertedProvinceID
        {
            get { return _LastInsertedProvinceID; }
            set { _LastInsertedProvinceID = value; }
        }
        public static string ClassLastTableName
        {
            get { return _LastTableName; }
            set { _LastTableName = value; }
        }
    }
}
