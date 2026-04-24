using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MIS.Enums
{
    public enum GroupType
    {
        SIMType = 1,
        RequestType = 2,
        BillingType = 3,
        AppType = 4,
        AssetType = 5,
        POSSetup = 6,
        PlanType = 7,
        TermType = 8,
        BillType = 9,
        SourceType = 10,
        CategoryType = 11,
        SubCategoryType = 12,
        BillingTypeID = 26,
    }

    public enum FileType
    {
        MSP = 1,
        IR = 2,
        Particular = 3,
        FSR = 4
    }

    public enum OtherType
    {
        CategoryType = 13,
        NoBType = 14,
        BusType = 15,
        ReferralType = 16,
        SchemeType = 17,
        StatusType = 18,
        AcqType = 19,
        MDRCreditType = 20,
        MDRDebitType = 21,
        MDRInstType = 22,
        ResultStatusType = 23,
        Depedency = 24,
        StatusReason = 25
    }

    public enum OptionType
    {
        YesNo = 1,
        YesNoOthers = 2,
        Others = 3
    }
    public enum StatusType
    {
        Init,
        Processing,
        Success,
        Error,
        Warning,
        Upload,
        Create,
        Export
    }

    public enum ReportProcessType
    {
        Idle = 1,
        Processing = 2,
        Failed = 3,
        Completed = 4
    }

    public enum JobType
    {
        Installation = 1,
        Servicing = 6,
        Pullout = 4,
        Replacement = 7,
        Reprogramming = 3,
        Hold = 9,
        Cancelled = 10
    }

    public enum BillableType
    {
        Billable = 1,
        NotBillable = 0
    }

    public enum DispatchType
    {
        Dispatch = 1,
        NotDispatch = 0
    }
}
