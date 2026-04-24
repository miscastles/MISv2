using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MIS.ControlObject
{
    public partial class ucAttachDocument : UserControl
    {   
        public static List<Tuple<string, string, string, string>> FileDetails = new List<Tuple<string, string, string, string>>();

        private clsFunction dbFunction;

        public ucAttachDocument()
        {
            InitializeComponent();

            dbFunction = new clsFunction();

            txtFilePath.Text = txtRemarks.Text = "";
            initDate();
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Files|*.png;*.jpg;*.bmp;*.pdf";
            openFileDialog.InitialDirectory = @"C:\CASTLESTECH_MIS\IMPORT\";
            openFileDialog.Title = "Select file to import...";

            txtFilePath.Text = txtRemarks.Text = "";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string FilePath = openFileDialog.FileName;
                string FileName = Path.GetFileName(FilePath);
                txtFilePath.Text = openFileDialog.FileName;
                txtFilePath.ReadOnly = true;
                txtRemarks.Text = FileName;
                txtFileName.Text = FileName;
                
                
            }
        }

        private void initDate()
        {
            dteReceivedDate.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteReceivedDate, clsFunction.sStandardDateDefault);            
        }

        public void clearEntry()
        {
            txtFilePath.Text = txtRemarks.Text = txtFileName.Text = "";
            initDate();
        }

        public void clearList()
        {
            FileDetails.Clear();
        }

        public List<Tuple<string, string, string, string>> getFileDetails()
        {
            return new List<Tuple<string, string, string, string>>(FileDetails);
        }

        public void addToList()
        {
            // Add to static list
            FileDetails.Add(new Tuple<string, string, string, string>(txtFilePath.Text, txtFileName.Text, txtRemarks.Text, dbFunction.CheckAndSetDatePickerValueToDate(dteReceivedDate)));
        }
    }
}
