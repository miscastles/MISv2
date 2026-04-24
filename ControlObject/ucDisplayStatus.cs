using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace MIS.ControlObject
{
    public partial class ucDisplayStatus : UserControl
    {
        public ucDisplayStatus()
        {
            InitializeComponent();
        }

        public void SetStatus(string message, Enums.StatusType status)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => SetStatus(message, status)));
                return;
            }

            statusLabel.Text = message;

            statusLabel.BackColor = iconBox.BackColor = this.BackColor = Color.Transparent;

            switch (status)
            {
                case Enums.StatusType.Processing:
                    statusLabel.ForeColor = Color.DarkBlue;
                    iconBox.Image = Properties.Resources.ic_process;
                    break;

                case Enums.StatusType.Warning:
                    statusLabel.ForeColor = Color.Orange;
                    iconBox.Image = Properties.Resources.ic_warning;
                    break;

                case Enums.StatusType.Success:
                    statusLabel.ForeColor = Color.Green;
                    iconBox.Image = Properties.Resources.ic_success;
                    break;

                case Enums.StatusType.Error:
                    statusLabel.ForeColor = Color.Red;
                    iconBox.Image = Properties.Resources.ic_error;
                    break;

                case Enums.StatusType.Upload:
                    statusLabel.ForeColor = Color.Violet;
                    iconBox.Image = Properties.Resources.ic_upload;
                    break;

                case Enums.StatusType.Create:
                    statusLabel.ForeColor = Color.Indigo;
                    iconBox.Image = Properties.Resources.ic_create;
                    break;

                case Enums.StatusType.Export:
                    statusLabel.ForeColor = Color.Cyan;
                    iconBox.Image = Properties.Resources.ic_export;
                    break;

                default:
                    iconBox.Image = null;
                    break;
            }

            iconBox.Refresh();
        }
    }
}
