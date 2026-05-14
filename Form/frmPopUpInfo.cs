using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace MIS
{
    public partial class frmPopUpInfo : Form
    {
        private clsFunction dbFunction;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED
                //cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        public frmPopUpInfo(string jsonData)
        {
            InitializeComponent();
            
            dbFunction = new clsFunction();            
            dbFunction.populateListViewFromJsonString(grdData, jsonData, "", clsDefines.NESTED_OBJECT_VALUES);                   
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmPopUp_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;                
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
