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
    public partial class frmPopUp : Form
    {
        public frmPopUp(string sMessage, AlertType Type)
        {
            InitializeComponent();

            switch (Type)
            {
                case AlertType.iSuccess:
                    //this.BackColor = Color.DarkSlateGray;
                    
                    break;
                case AlertType.iInfo:
                    //this.BackColor = Color.DarkSlateGray;
                    
                    break;
                case AlertType.iWarning:
                    //this.BackColor = Color.DarkSlateGray;
                    
                    break;
                case AlertType.iError:
                    //this.BackColor = Color.DarkSlateGray;
                    
                    break;
            }

            lblMessage.Text = sMessage;
        }
        public enum AlertType
        {
            iSuccess, iInfo, iWarning, iError
        }

        private void frmPopUp_Load(object sender, EventArgs e)
        {
            this.Top = 60;
            this.Left = Screen.PrimaryScreen.Bounds.Width - this.Width - 60;
        }

        private void tmrTimeOut_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        int iInterval = 0;
        private void tmrShow_Tick(object sender, EventArgs e)
        {
            if (this.Top < 60)
            {
                this.Top += iInterval;
                iInterval += 2;
            }
            else
            {
                tmrShow.Stop();
            }
        }

        private void tmrClose_Tick(object sender, EventArgs e)
        {
            if (this.Opacity > 0)
            {
                this.Opacity -= 0.1;
            }
            else
            {
                this.Close();
            }
        }
    }
}
