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
using System.Drawing.Imaging;

namespace MIS
{
    public partial class frmLoading : Form
    {
       
        public frmLoading()
        {
            InitializeComponent();            
        }
        
        private void frmLoading_Load(object sender, EventArgs e)
        {
            
        }

        private void frmLoading_Activated(object sender, EventArgs e)
        {
            ucLoadingX.StartAnimation();

        }

        private void frmLoading_Shown(object sender, EventArgs e)
        {
            
        }        

    }
}
