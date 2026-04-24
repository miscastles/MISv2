using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MIS
{
    public partial class frmCSV : Form
    {
        private clsFunction dbFunction;
        public static int iTab;        
        public frmCSV()
        {
            InitializeComponent();
        }

        private void InitListView(int iType)
        {
            lvwList.View = View.Details;

            switch (iType)
            {
                case 0: // FSR
                    lvwList.Columns.Add("Line#", 50, HorizontalAlignment.Left);
                    lvwList.Columns.Add("Merchant Name", 180, HorizontalAlignment.Left);
                    lvwList.Columns.Add("MID", 160, HorizontalAlignment.Left);
                    lvwList.Columns.Add("TID", 100, HorizontalAlignment.Left);
                    lvwList.Columns.Add("Invoice No", 100, HorizontalAlignment.Left);
                    lvwList.Columns.Add("Batch No", 100, HorizontalAlignment.Left);
                    lvwList.Columns.Add("PAN", 100, HorizontalAlignment.Left);
                    lvwList.Columns.Add("Date", 100, HorizontalAlignment.Left);
                    lvwList.Columns.Add("Time", 100, HorizontalAlignment.Left);
                    lvwList.Columns.Add("Txn Amt", 100, HorizontalAlignment.Left);
                    lvwList.Columns.Add("Auth Code", 100, HorizontalAlignment.Left);
                    lvwList.Columns.Add("Invoice No", 100, HorizontalAlignment.Left);
                    lvwList.Columns.Add("Ref No", 100, HorizontalAlignment.Left);
                    lvwList.Columns.Add("Phone No", 100, HorizontalAlignment.Left);
                    lvwList.Columns.Add("Email", 100, HorizontalAlignment.Left);
                    lvwList.Columns.Add("NRIC", 100, HorizontalAlignment.Left);
                    lvwList.Columns.Add("Additional Information", 100, HorizontalAlignment.Left);                    
                    break;
                case 1: // Terminal
                    break;
                case 2: // IR
                    break;                
            }
        }

        private void InitTab()
        {
            tabCSV.SelectedIndex = iTab;
        }

        private void frmCSV_Load(object sender, EventArgs e)
        {
            InitTab();
            InitListView(0);
            btnCancel_Click(this, e);
            dbFunction = new clsFunction();
        }

        private void LoadFile()
        {
            OpenFileDialog ExcelDialog = new OpenFileDialog();
            ExcelDialog.Filter = "CSV File|*.csv";
            ExcelDialog.InitialDirectory = @"C:\CASTLESTECH_MIS\IMPORT\";
            ExcelDialog.Title = "Select FSR CSV file.";

            if (ExcelDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ExcelFilePath = ExcelDialog.FileName;
                txtPathFileName.Text = ExcelDialog.FileName;
                txtPathFileName.ReadOnly = true;
                btnLoadFile.Enabled = false;
                btnImport.Enabled = true;
                string sFileName = System.IO.Path.GetFileName(txtPathFileName.Text);

                try
                {
                    // Waiting / Hour Glass
                    Cursor.Current = Cursors.WaitCursor;

                    ReadCSVFile(txtPathFileName.Text);
                    LoadToListView();

                    // Back to normal 
                    Cursor.Current = Cursors.Default;

                    btnLoadFile.Enabled = false;
                    btnImport.Enabled = true;
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    btnLoadFile.Enabled = true;
                    btnImport.Enabled = false;
                    return;
                }

            }
        }

        private void btnLoadFile_Click(object sender, EventArgs e)
        {
            LoadFile();            
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            btnLoadFile.Enabled = true;
            btnImport.Enabled = false;
            ClearTextBox();
        }

        private void ClearTextBox()
        {
            txtPathFileName.Text = "";

            pbCSV.Visible = false;
            pbCSV.Value = 0;
        }

        private void ReadCSVFile(string sFullFileNamePath)
        {           
            {
                StreamReader reader = new StreamReader(File.OpenRead(sFullFileNamePath));

                List<string> Header1Col0 = new List<String>();
                List<string> Header1Col1 = new List<String>();
                List<string> Header1Col2 = new List<String>();
                List<string> Header1Col3 = new List<String>();
                List<string> Header1Col4 = new List<String>();
                List<string> Header1Col5 = new List<String>();
                List<string> Header1Col6 = new List<String>();
                List<string> Header1Col7 = new List<String>();
                List<string> Header1Col8 = new List<String>();
                List<string> Header1Col9 = new List<String>();
                List<string> Header1Col10 = new List<String>();
                List<string> Header1Col11 = new List<String>();
                List<string> Header1Col12 = new List<String>();
                List<string> Header1Col13 = new List<String>();
                List<string> Header1Col14 = new List<String>();
                List<string> Header1Col15 = new List<String>();
                
                //string vara1, vara2, vara3, vara4;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (!String.IsNullOrWhiteSpace(line))
                    {
                        string[] values = line.Split(',');
                        if (values.Length >= 5 && (values[0].CompareTo("No") != 0) && (values[1].CompareTo("Merchant") != 0))
                        {
                            Header1Col0.Add(values[0]);
                            Header1Col1.Add(values[1]);
                            Header1Col2.Add(values[2]);
                            Header1Col3.Add(values[3]);
                            Header1Col4.Add(values[4]);
                            Header1Col5.Add(values[5]);
                            Header1Col6.Add(values[6]);
                            Header1Col7.Add(values[7]);
                            Header1Col8.Add(values[8]);
                            Header1Col9.Add(values[9]);
                            Header1Col10.Add(values[10]);
                            Header1Col11.Add(values[11]);
                            Header1Col12.Add(values[12]);
                            Header1Col13.Add(values[13]);
                            Header1Col14.Add(values[14]);
                            Header1Col15.Add(values[15]);
                        }
                    }
                }

                clsArray.FSRHeader0 = Header1Col0.ToArray();
                clsArray.FSRHeader1 = Header1Col1.ToArray();
                clsArray.FSRHeader2 = Header1Col2.ToArray();
                clsArray.FSRHeader3 = Header1Col3.ToArray();
                clsArray.FSRHeader4 = Header1Col4.ToArray();
                clsArray.FSRHeader5 = Header1Col5.ToArray();
                clsArray.FSRHeader6 = Header1Col6.ToArray();
                clsArray.FSRHeader7 = Header1Col7.ToArray();
                clsArray.FSRHeader8 = Header1Col8.ToArray();
                clsArray.FSRHeader9 = Header1Col9.ToArray();
                clsArray.FSRHeader10 = Header1Col10.ToArray();
                clsArray.FSRHeader11 = Header1Col11.ToArray();
                clsArray.FSRHeader12 = Header1Col12.ToArray();
                clsArray.FSRHeader13 = Header1Col13.ToArray();
                clsArray.FSRHeader14 = Header1Col14.ToArray();
                clsArray.FSRHeader15 = Header1Col15.ToArray();
            }
        }

        private void LoadToListView()
        {
            int i = 0;

            lvwList.Items.Clear();

            while (clsArray.FSRHeader0.Length > i)
            {
                clsFSR.ClassNo = clsArray.FSRHeader0[i];
                clsFSR.ClassMerchant = clsArray.FSRHeader1[i];
                clsFSR.ClassMID = clsArray.FSRHeader2[i];
                clsFSR.ClassTID = clsArray.FSRHeader3[i];
                clsFSR.ClassTimeArrived = clsArray.FSRHeader4[i];
                clsFSR.ClassTimeStart = clsArray.FSRHeader5[i];
                clsFSR.ClassFSR = clsArray.FSRHeader6[i];
                clsFSR.ClassFSRDate = clsArray.FSRHeader7[i];
                clsFSR.ClassFSRTime = clsArray.FSRHeader8[i];
                clsFSR.ClassTxnAmt = clsArray.FSRHeader9[i];
                clsFSR.ClassTimeEnd = clsArray.FSRHeader10[i];
                clsFSR.ClassSerialNo = clsArray.FSRHeader11[i];
                clsFSR.ClassMerchantContactNo = clsArray.FSRHeader12[i];
                clsFSR.ClassMerchantRepresentative = clsArray.FSRHeader13[i];
                clsFSR.ClassFEName = clsArray.FSRHeader14[i];
                clsFSR.ClassSerialNo = clsArray.FSRHeader15[i];

                i++;

                AddItem(i);
            }

            dbFunction.ListViewAlternateBackColor(lvwList);

        }
        private void AddItem(int inLineNo)
        {
            // Add to List            
            ListViewItem item = new ListViewItem(inLineNo.ToString());            
            item.SubItems.Add(clsFSR.ClassMerchant);
            item.SubItems.Add(clsFSR.ClassMID);
            item.SubItems.Add(clsFSR.ClassTID);
            item.SubItems.Add(clsFSR.ClassTimeArrived);
            item.SubItems.Add(clsFSR.ClassTimeStart);
            item.SubItems.Add(clsFSR.ClassFSR);
            item.SubItems.Add(clsFSR.ClassFSRDate);
            item.SubItems.Add(clsFSR.ClassFSRTime);
            item.SubItems.Add(clsFSR.ClassTxnAmt);
            item.SubItems.Add(clsFSR.ClassTimeEnd);
            item.SubItems.Add(clsFSR.ClassSerialNo);
            item.SubItems.Add(clsFSR.ClassMerchantContactNo);
            item.SubItems.Add(clsFSR.ClassMerchantRepresentative);
            item.SubItems.Add(clsFSR.ClassFEName);
            item.SubItems.Add(clsFSR.ClassSerialNo);
            
            lvwList.Items.Add(item);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
