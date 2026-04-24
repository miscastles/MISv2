using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace MIS
{
    public partial class ucLoading : UserControl
    {
        private clsFunction dbFunction;
        public int iDir = 1;
        public int iMin = 0;
        public int iMax = 100;
        private bool fLoop = true;
        
        public ucLoading()
        {
            InitializeComponent();        
        }

        private void tmrAnimate_Tick(object sender, EventArgs e)
        {
            iMin++;

            if (iMin >= iMax)
            {
                iMin = 0;
            }
            else
            {
                objProgress.Value = iMin;                
            }
        }

        public void InitTimerAnimate()
        {
            
        }

        public void InitTimerTimeOut()
        {
            
        }

        private void ucLoading_Load(object sender, EventArgs e)
        {
            
        }
        
        public void InitProgress()
        {
            objProgress.Value = iMin;
        }

        public void InitTitle()
        {
            dbFunction = new clsFunction();
            dbFunction.UpdateAnimateHeader(txtHeader1, "Wait while processing.", txtHeader2, "Please wait...");
        }

        public void StartAnimation()
        {
            //InitTimerTimeOut();

            InitImageGIF();

            while (fLoop)
            {                
                InitTitle();
                InitProgress();                
                Application.DoEvents();
            }            
        }

        private void InitImageGIF()
        {
            string FullPath = "";
            string FileName = "";

            FileName = "loading4.gif";
            FullPath = "C:\\CASTLESTECH_MIS\\IMAGE\\" + FileName;

            picGIF.Image = Image.FromFile(FullPath);
        }

    }
}
