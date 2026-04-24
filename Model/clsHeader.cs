using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsHeader
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int HeaderID;
        public static string Name;
        public static string Header1;
        public static string Header2;
        public static string Header3;
        public static string Header4;
        public static string Header5;

        public static bool ClassRecordFound
        {
            get { return RecordFound; }
            set { RecordFound = value; }
        }
        public static int ClassHeaderID
        {
            get { return HeaderID; }
            set { HeaderID = value; }
        }
        public static string ClassName
        {
            get { return Name; }
            set { Name = value; }
        }
        public static string ClassHeader1
        {
            get { return Header1; }
            set { Header1 = value; }
        }
        public static string ClassHeader2
        {
            get { return Header2; }
            set { Header2 = value; }
        }
        public static string ClassHeader3
        {
            get { return Header3; }
            set { Header3 = value; }
        }
        public static string ClassHeader4
        {
            get { return Header4; }
            set { Header4 = value; }
        }
        public static string ClassHeader5
        {
            get { return Header5; }
            set { Header5 = value; }
        }
    }
}
