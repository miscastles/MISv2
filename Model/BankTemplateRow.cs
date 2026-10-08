namespace MIS.Model
{
    // Source-field model used by the Bank Template Generator pipeline.
    internal sealed class BankTemplateRow
    {
        public BankTemplateRow()
        {
            Section = string.Empty;
            SourceRowNumber = string.Empty;
            Tag = string.Empty;
            Value = string.Empty;
            MccColumn = string.Empty;
            MccValue = string.Empty;
        }

        public int ExcelRowNumber { get; set; }

        public string Section { get; set; }

        public string SourceRowNumber { get; set; }

        public string Tag { get; set; }

        public string Value { get; set; }

        public string MccColumn { get; set; }

        public bool IsMandatory { get; set; }

        public bool IsSelectable { get; set; }

        public bool IsSelected { get; set; }

        public string MccValue { get; set; }
    }
}
