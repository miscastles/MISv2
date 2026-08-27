using System;
using System.Windows.Forms;
using MIS.Function;

namespace MIS.ControlObject
{
    public partial class ucInfoDataGridView : UserControl
    {
        private clsFunction dbFunction;

        public ucInfoDataGridView()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
        }

        public ucInfoDataGridView(string jsonData) : this()
        {
            SetData(jsonData);
        }

        public void SetData(string jsonData)
        {
            dbFunction.populateListViewFromJsonString(grdData,jsonData,"",clsDefines.NESTED_OBJECT_VALUES);
        }
        public void ClearData()
        {
            grdData.DataSource = null;
            grdData.Rows.Clear();
        }
    }
}