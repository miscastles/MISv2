using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIS
{
    public partial class ucStatus : UserControl
    {
        private clsFunction dbFunction;
        public int iState = 4;        
        public string sMessage= "";
        public int iMin = 0;
        public int iMax = 0;
        
        public ucStatus()
        {
            InitializeComponent();
            rchStatus.Text = "";
        }
        
        private void ucStatus_Load(object sender, EventArgs e)
        {
            
        }

        public void AnimateStatus()
        {
            dbFunction = new clsFunction();
            dbFunction.UpdateProgressStatus(rchStatus, iState, sMessage, iMin, iMax);
        }

        public enum StateType
        {
            iWait, iProgessLine, 
        }
    }
}
