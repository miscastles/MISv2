using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsVendor
    {
        public static string _ParticularName;
        public static string ClassParticularName
        {
            get { return _ParticularName; }
            set { _ParticularName = value; }
        }

        public static string _Position;
        public static string ClassPosition
        {
            get { return _Position; }
            set { _Position = value; }
        }

        public static string _ContactNumber;
        public static string ClassContactNumber
        {
            get { return _ContactNumber; }
            set { _ContactNumber = value; }
        }

        public static string _Email;
        public static string ClassEmail
        {
            get { return _Email; }
            set { _Email = value; }
        }
    }
}
