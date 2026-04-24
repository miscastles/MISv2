using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace MIS
{
    public partial class frmProgress : Form
    {
        public static bool isExitForm;

        public frmProgress()
        {
            InitializeComponent();
        }

        private void InitImageGIF()
        {
            string FullPath = "";
            string FileName = "";

            FileName = "loading4.gif";
            FullPath = "C:\\CASTLESTECH_MIS\\IMAGE\\" + FileName;

            picGIF.Image = Image.FromFile(FullPath);
        }

        private void frmProgress_Activated(object sender, EventArgs e)
        {          
            InitImageGIF();
        }
    }
}
