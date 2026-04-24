using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS
{
    public class clsIR
    {
        public static bool RecordFound;
        public static int _RecordCount;
        public static int IRIDNo;
        public static int IRID;
        public static int ParticularID;
        public static string IRNo;
        public static string IRDate;
        public static string InstallationDate;        
        public static string Address;
        public static string Address2;
        public static string Address3;
        public static string Address4;
        public static string ContactPerson;
        public static string TelNo;
        public static string Mobile;
        public static string Email;
        public static string City;
        public static string Province;
        public static string ParticularName;
        public static string TID;
        public static string MID;
        public static int IRStatus;
        public static string IRStatusDescription;
        public static string ServiceTypeDescription;
        
        public static int ClassIRIDNo
        {
            get { return IRIDNo; }
            set { IRIDNo = value; }
        }

        public static int ClassIRID
        {
            get { return IRID; }
            set { IRID = value; }
        }
        public static int ClassParticularID
        {
            get { return ParticularID; }
            set { ParticularID = value; }
        }
        public static string ClassIRNo
        {
            get { return IRNo; }
            set { IRNo = value; }
        }
        public static string ClassIRDate
        {
            get { return IRDate; }
            set { IRDate = value; }
        }
        public static string ClassInstallationDate
        {
            get { return InstallationDate; }
            set { InstallationDate = value; }
        }
        public static string ClassAddress
        {
            get { return Address; }
            set { Address = value; }
        }
        public static string ClassAddress2
        {
            get { return Address2; }
            set { Address2 = value; }
        }
        public static string ClassAddress3
        {
            get { return Address3; }
            set { Address3 = value; }
        }
        public static string ClassAddress4
        {
            get { return Address4; }
            set { Address4 = value; }
        }
        public static string ClassContactPerson
        {
            get { return ContactPerson; }
            set { ContactPerson = value; }
        }
        public static string ClassTelNo
        {
            get { return TelNo; }
            set { TelNo = value; }
        }
        public static string ClassMobile
        {
            get { return Mobile; }
            set { Mobile = value; }
        }
        public static string ClassEmail
        {
            get { return Email; }
            set { Email = value; }
        }
        public static string ClassCity
        {
            get { return City; }
            set { City = value; }
        }
        public static string ClassProvince
        {
            get { return Province; }
            set { Province = value; }
        }
        public static string ClassParticularName
        {
            get { return ParticularName; }
            set { ParticularName = value; }
        }
        public static string ClassTID
        {
            get { return TID; }
            set { TID = value; }
        }
        public static string ClassMID
        {
            get { return MID; }
            set { MID = value; }
        }
        public static int ClassIRStatus
        {
            get { return IRStatus; }
            set { IRStatus = value; }
        }
        public static string ClassIRStatusDescription
        {
            get { return IRStatusDescription; }
            set { IRStatusDescription = value; }
        }
        public static string ClassServiceTypeDescription
        {
            get { return ServiceTypeDescription; }
            set { ServiceTypeDescription = value; }
        }

        public static string PrimaryNum;
        public static string ClassPrimaryNum
        {
            get { return PrimaryNum; }
            set { PrimaryNum = value; }
        }

        public static string SecondaryNum;
        public static string ClassSecondaryNum
        {
            get { return SecondaryNum; }
            set { SecondaryNum = value; }
        }
    }
}
