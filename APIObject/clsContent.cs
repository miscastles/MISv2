using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;

namespace MIS
{
    public class clsContent
    {
        // User
        public string p_UserID { get; set; }
        public string p_UserName { get; set; }
        public string p_Password { get; set; }
        public string p_FullName { get; set; }
        public string p_UserType { get; set; }

        // Search
        public string p_StatementType { get; set; }
        public string p_SearchBy { get; set; }
        public string p_SearchValue { get; set; }

        // City
        public string p_CityID { get; set; }
        public string p_City { get; set; }

        // Province
        public string p_ProvinceID { get; set; }

        // Service Type
        public string p_ServiceTypeID { get; set; }

        // Terminal Type
        public string p_TypeID { get; set; }

        // Terminal Model
        public string p_ModelID { get; set; }

        // Terminal Brand
        public string p_BrandID { get; set; }        

        // Terminal Status
        public string p_TerminalStatusID { get; set; }

        // FE
        public string p_FEID { get; set; }
        public string p_Name { get; set; }
        public string p_Address { get; set; }
        public string p_Address2 { get; set; }
        public string p_Address3 { get; set; }
        public string p_Address4 { get; set; }

        public string p_ContactNo { get; set; }
        public string p_Description { get; set; }
        public string p_Code { get; set; }

        // Particular
        public string p_ParticularID { get; set; }        
        public string p_ParticularTypeID { get; set; }
        public string p_ParticularDescription { get; set; }
        public string p_ContactPerson { get; set; }
        public string p_TelNo { get; set; }
        public string p_Mobile { get; set; }
        public string p_Fax { get; set; }
        public string p_Email { get; set; }
        public string p_ContractTerms { get; set; }

        // Update Status
        public string p_StatusID { get; set; }
        public string p_StatusDescription { get; set; }

        // Insert Maintenance
        public string p_MaintenanceType { get; set; }
        public string p_SQL { get; set; }

        // Create Temp Table
        public string p_TableName { get; set; }

        public static string SetCityContentData(string StatementType, string CityID, string City)
        {
            clsContent p = new clsContent();
            p.p_CityID = CityID;
            p.p_City = City;
            p.p_StatementType = StatementType;

            string jsonData = JsonConvert.SerializeObject(p,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            return jsonData;
        }
        public static string SetProvinceContentData(string StatementType, string ProvinceID, string Name)
        {
            clsContent p = new clsContent();
            p.p_ProvinceID = ProvinceID;
            p.p_Name = Name;
            p.p_StatementType = StatementType;

            string jsonData = JsonConvert.SerializeObject(p,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            return jsonData;
        }
        public static string SetServiceTypeContentData(string StatementType, string ServiceTypeID, string Description, string Code)
        {
            clsContent p = new clsContent();
            p.p_ServiceTypeID = ServiceTypeID;
            p.p_Description = Description;
            p.p_Code = Code;
            p.p_StatementType = StatementType;

            string jsonData = JsonConvert.SerializeObject(p,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            return jsonData;
        }
        public static string SetTerminalStatusContentData(string StatementType, string TerminalStatusID, string Description)
        {
            clsContent p = new clsContent();
            p.p_TerminalStatusID = TerminalStatusID;
            p.p_Description = Description;
            p.p_StatementType = StatementType;

            string jsonData = JsonConvert.SerializeObject(p,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            return jsonData;
        }
        public static string SetTerminalTypeContentData(string StatementType, string TypeID, string Description)
        {
            clsContent p = new clsContent();
            p.p_TypeID = TypeID;
            p.p_Description = Description;
            p.p_StatementType = StatementType;

            string jsonData = JsonConvert.SerializeObject(p,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            return jsonData;
        }
        public static string SetTerminalModelContentData(string StatementType, string ModelID, string Description)
        {
            clsContent p = new clsContent();
            p.p_ModelID = ModelID;
            p.p_Description = Description;
            p.p_StatementType = StatementType;

            string jsonData = JsonConvert.SerializeObject(p,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            return jsonData;
        }
        public static string SetTerminalBrandContentData(string StatementType, string BrandID, string Description)
        {
            clsContent p = new clsContent();
            p.p_BrandID = BrandID;
            p.p_Description = Description;
            p.p_StatementType = StatementType;

            string jsonData = JsonConvert.SerializeObject(p,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            return jsonData;
        }
        public static string SetFEContentData(string StatementType, string FEID, string Name, string Address, string ContactNo)
        {
            clsContent p = new clsContent();
            p.p_FEID = FEID;
            p.p_Name = Name;
            p.p_Address = Address;
            p.p_ContactNo = ContactNo;
            p.p_StatementType = StatementType;

            string jsonData = JsonConvert.SerializeObject(p,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            return jsonData; 
        }
        public static string SetUserContentData(string StatementType, string UserID, string UserName, string Password, string FullName, string UserType)
        {
            clsContent p = new clsContent();
            p.p_UserID = UserID;
            p.p_UserName = UserName;
            p.p_Password = Password;
            p.p_FullName = FullName;
            p.p_UserType = UserType;
            p.p_StatementType = StatementType;

            string jsonData = JsonConvert.SerializeObject(p,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            return jsonData;
        }

        public static string SetParticularContentData(string StatementType, string ParticularID, string ProvinceID, string CityID, string ParticularTypeID, string ParticularDescription, string Name, string Address, string Address2, string Address3, string Address4, string ContactPerson, string TelNo, string Mobile, string Fax, string Email, string ContractTerms)
        {
            clsContent p = new clsContent();
            p.p_ParticularID = ParticularID;
            p.p_ProvinceID = ProvinceID;
            p.p_CityID = CityID;
            p.p_ParticularTypeID = ParticularTypeID;
            p.p_ParticularDescription = ParticularDescription;
            p.p_Name = Name;
            p.p_Address = Address;
            p.p_Address2 = Address2;
            p.p_Address3 = Address3;
            p.p_Address4 = Address4;
            p.p_ContactPerson = ContactPerson;
            p.p_TelNo = TelNo;
            p.p_Mobile = Mobile;
            p.p_Fax = Fax;
            p.p_Email = Email;
            p.p_ContractTerms = ContractTerms;
            p.p_StatementType = StatementType;

            string jsonData = JsonConvert.SerializeObject(p,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            return jsonData;
        }

        public static string SetUpdateStatusContentData(string StatementType, string MaintenanceType, string SearchBy, string SearchValue, string StatusID, string StatusDescription)
        {
            clsContent p = new clsContent();
            p.p_StatementType = StatementType;
            p.p_MaintenanceType = MaintenanceType;
            p.p_SearchBy = SearchBy;
            p.p_SearchValue = SearchValue;
            p.p_StatusID = StatusID;
            p.p_StatusDescription = StatusDescription;

            string jsonData = JsonConvert.SerializeObject(p,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            return jsonData;
        }
        public static string InsertMaintenanceMasterContentData(string StatementType, string MaintenanceType, string SQL)
        {
            clsContent p = new clsContent();
            
            switch (StatementType)
            {
                case "Create":
                    p.p_StatementType = clsSearch.ClassStatementType;
                    p.p_TableName = clsSearch.ClassMaintenanceType;                                        
                    break;
                case "Insert-Select":
                    p.p_StatementType = clsSearch.ClassStatementType;
                    p.p_MaintenanceType = clsSearch.ClassMaintenanceType;
                    p.p_SearchBy = clsSearch.ClassSearchBy;
                    p.p_SearchValue = clsSearch.ClassSearchValue;
                    break;
                case "Import":
                    p.p_StatementType = clsSearch.ClassStatementType;
                    p.p_MaintenanceType = clsSearch.ClassMaintenanceType;
                    p.p_SearchBy = clsSearch.ClassSearchBy;
                    p.p_SearchValue = clsSearch.ClassSearchValue;
                    break;
                case "Notify":
                    p.p_StatementType = clsSearch.ClassStatementType;
                    p.p_MaintenanceType = clsSearch.ClassMaintenanceType;
                    p.p_SearchBy = clsSearch.ClassSearchBy;
                    p.p_SearchValue = clsSearch.ClassSearchValue;
                    break;
                default:
                    p.p_StatementType = clsSearch.ClassStatementType;
                    p.p_SearchBy = clsSearch.ClassSearchBy;
                    p.p_SearchValue = clsSearch.ClassSearchValue;
                    p.p_MaintenanceType = clsSearch.ClassMaintenanceType;
                    p.p_SQL = clsSearch.ClassSQL;                    
                    break;
            }

            string jsonData = JsonConvert.SerializeObject(p,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            return jsonData;
        }

        public static string SetUpdateCollectionDetailContentData(string StatementType, string SearchBy, string SearchValue)
        {
            clsContent p = new clsContent();

            switch (StatementType)
            {                
                case "Update":
                    p.p_StatementType = clsSearch.ClassStatementType;                    
                    p.p_SearchBy = clsSearch.ClassSearchBy;
                    p.p_SearchValue = clsSearch.ClassSearchValue;
                break;                
            }

            string jsonData = JsonConvert.SerializeObject(p,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            return jsonData;
        }

        public static string SetUpdateBulkCollectionDetailContentData(string StatementType, string SearchBy, string SearchValue)
        {
            clsContent p = new clsContent();

            switch (StatementType)
            {
                case "Update":
                    p.p_StatementType = clsSearch.ClassStatementType;
                    p.p_SearchBy = clsSearch.ClassSearchBy;
                    p.p_SearchValue = clsSearch.ClassSearchValue;
                    break;
            }

            string jsonData = JsonConvert.SerializeObject(p,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            return jsonData;
        }

        public static string SetDeleteCollectionDetailContentData(string StatementType, string SearchBy, string SearchValue, string MaintenanceType)
        {
            clsContent p = new clsContent();

            switch (StatementType)
            {
                case "Delete":
                    p.p_StatementType = clsSearch.ClassStatementType;
                    p.p_SearchBy = clsSearch.ClassSearchBy;
                    p.p_SearchValue = clsSearch.ClassSearchValue;
                    p.p_MaintenanceType = clsSearch.ClassMaintenanceType;

                    break;
            }

            string jsonData = JsonConvert.SerializeObject(p,
                            Newtonsoft.Json.Formatting.None,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });

            return jsonData;
        }
    }
}
