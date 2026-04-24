using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsMapping
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int _MapID;
        public static string _MapFrom;
        public static string _MapTo;
        public static string _Delimeter;
        public static string _ColumnIndex;

        public static int ClassRecordCount
        {
            get { return _RecordCount; }
            set { _RecordCount = value; }
        }
        public static int ClassMapID
        {
            get { return _MapID; }
            set { _MapID = value; }
        }
        public static string ClassMapFrom
        {
            get { return _MapFrom; }
            set { _MapFrom = value; }
        }
        public static string ClassMapTo
        {
            get { return _MapTo; }
            set { _MapTo = value; }
        }
        public static string ClassDelimeter
        {
            get { return _Delimeter; }
            set { _Delimeter = value; }
        }
        public static string ClassColumnIndex
        {
            get { return _ColumnIndex; }
            set { _ColumnIndex = value; }
        }
    }
}
