using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MIS
{
    public partial class frmPromptMessage : Form
    {
        public static string sMenuHeader;
        public static string sMessage;

        public static string sHint;
        public static string sHintMessage;
        public static string sCount;

        int iSecondLimit = 5;
        int iCounter;

        public frmPromptMessage()
        {
            InitializeComponent();
        }
        public enum MessageType
        {
            iSuccess, iInfo, iWarning, iError
        }

        private void frmPromptMessage_Load(object sender, EventArgs e)
        {
            lblMenuHeader.Text = sMenuHeader;
            txtMessage.Text = sMessage;

            lblMenuHint.Text = "HINT: " + sHintMessage;
            txtHint.Text = sHint;

            lblCount.Text = "COUNT: " + sCount;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmPromptMessage_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void tmrAlert_Tick(object sender, EventArgs e)
        {
            iCounter++;            
            if (iCounter > iSecondLimit)
            {
                this.Close();
            }
        }
        private void InitAlertTimer()
        {
            tmrAlert.Enabled = true;
            tmrAlert.Interval = 1000;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmPromptMessage_Activated(object sender, EventArgs e)
        {
            btnOK.Focus();         
        }
    }
}
