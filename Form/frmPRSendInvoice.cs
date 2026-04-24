using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace MIS
{
    public partial class frmPRSendInvoice : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        private clsReportFunc dbReportFunc;
        private bool fEdit = false;

        public frmPRSendInvoice()
        {
            InitializeComponent();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtFromInvoiceID.Text = txtFromInvoiceNo.Text = txtToInvoiceID.Text = txtToInvoiceNo.Text = txtTCount.Text = clsFunction.sZero;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSearchInvoiceFrom_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iInvoice;
            frmSearchField.sHeader = "INVOICE";
            frmSearchField.isCheckBoxes = false;
            frmSearchField.isPreview = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                Cursor.Current = Cursors.WaitCursor;

                txtFromInvoiceID.Text = clsSearch.ClassInvoiceID.ToString();
                txtFromInvoiceNo.Text = clsSearch.ClassInvoiceNo;
                
                Cursor.Current = Cursors.Default;


            }
        }

        private void btnSearchInvoiceTo_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iInvoice;
            frmSearchField.sHeader = "INVOICE";
            frmSearchField.isCheckBoxes = false;
            frmSearchField.isPreview = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                Cursor.Current = Cursors.WaitCursor;

                txtToInvoiceID.Text = clsSearch.ClassInvoiceID.ToString();
                txtToInvoiceNo.Text = clsSearch.ClassInvoiceNo;

                Cursor.Current = Cursors.Default;


            }
        }

        private void InitItemListView()
        {
            string outField = "";
            int outWidth = 0;
            string outTitle = "";
            HorizontalAlignment outAlign = 0;
            bool outVisible = false;
            bool outAutoWidth = false;
            string outFormat = "";

            lvwList.Clear();
            lvwList.View = View.Details;

            dbFunction.GetListViewHeaderColumnFromFile("", "LINE#", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ParticularID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "MerchantID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "InvoiceNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "AccountNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "CustomerNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Name", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "InvoiceDate", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ReferenceNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "DateCovered", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "DateDue", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "TAmtDue", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Processed By", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Processed Date", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

        }

        private void frmPRSendInvoice_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            dbReportFunc = new clsReportFunc();

            dbFunction.ClearTextBox(this);
            
            // Item
            dbFunction.ClearListView(lvwList);
            InitItemListView();
            InitSummaryTextBox();

            fEdit = false;
            
            Cursor.Current = Cursors.Default;
        }

        private void btnClearListView_Click(object sender, EventArgs e)
        {
            // Item
            dbFunction.ClearListView(lvwList);
            InitItemListView();
        }

        private void frmPRSendInvoice_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1: // New
                    
                    break;
                case Keys.F2: // Sesrch               
                    
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void pnlHeader_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnView_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iFromInvoice, txtFromInvoiceID.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iToInvoice, txtToInvoiceID.Text)) return;
            if (!CheckInvoiceRange(int.Parse(dbFunction.CheckAndSetNumericValue(txtFromInvoiceID.Text)),   int.Parse( dbFunction.CheckAndSetNumericValue(txtToInvoiceID.Text)))) return;

            if (dbFunction.isValidID(txtFromInvoiceID.Text) && dbFunction.isValidID(txtToInvoiceID.Text))
            {
                Cursor.Current = Cursors.WaitCursor;

                ViewDetail();
                ComputeSummary();

                Cursor.Current = Cursors.Default;
            }
             
        }

        private void ViewDetail()
        {
            int i = 0;
            int iLineNo = 0;
            
            lvwList.Items.Clear();

            dbAPI.ExecuteAPI("GET", "View", "Invoice Master Range", txtFromInvoiceID.Text + clsFunction.sPipe + txtToInvoiceID.Text, "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                
                while (clsArray.InvoiceID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.InvoiceID[i].ToString());
                    item.SubItems.Add(clsArray.ParticularID[i].ToString());
                    item.SubItems.Add(clsArray.MerchantID[i].ToString());
                    item.SubItems.Add(clsArray.InvoiceNo[i].ToString());
                    item.SubItems.Add(clsArray.AccountNo[i].ToString());
                    item.SubItems.Add(clsArray.CustomerNo[i].ToString());
                    item.SubItems.Add(clsArray.ParticularName[i].ToString());
                    item.SubItems.Add(clsArray.InvoiceDate[i].ToString());
                    item.SubItems.Add(clsArray.ReferenceNo[i].ToString());
                    item.SubItems.Add(clsArray.DateCoveredFrom[i].ToString() + " to " + clsArray.DateCoveredTo[i].ToString());
                    item.SubItems.Add(clsArray.DateDue[i].ToString());
                    item.SubItems.Add(Convert.ToDouble(clsArray.TAmtDue[i]).ToString("N"));
                    item.SubItems.Add(clsArray.ProcessedBy[i]);
                    item.SubItems.Add(clsArray.ProcessedDateTime[i]);

                    lvwList.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(lvwList);

                dbFunction.FocusFirstItemInListView(lvwList, 0);

                txtTCount.Text = lvwList.Items.Count.ToString();
            }
        }

        private void lvwList_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    lvwList_DoubleClick(this, e);
                    break;
                
            }
        }

        private void lvwList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwList.SelectedItems.Count > 0)
            {
                string LineNo = lvwList.SelectedItems[0].Text;
                txtLineNo.Text = LineNo;

                if (LineNo.Length > 0)
                {
                   
                }
            }
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.SelectedItems[0].SubItems[1].Text.Length > 0)
            {
                Cursor.Current = Cursors.WaitCursor;

                clsSearch.ClassInvoiceID = int.Parse(lvwList.SelectedItems[0].SubItems[1].Text);
                clsSearch.ClassParticularID = int.Parse(lvwList.SelectedItems[0].SubItems[2].Text);

                txtInvoiceID.Text = clsSearch.ClassInvoiceID.ToString();
                txtParticularID.Text = txtMerchantID.Text = clsSearch.ClassParticularID.ToString();

                ViewInfo();
                FillMerchantTextBox(); // Get Merchant Info

                dbFunction.TextBoxUnLock(true, this);

                Cursor.Current = Cursors.Default;

            }
        }

        private void ViewInfo()
        {
            if (dbFunction.isValidID(clsSearch.ClassInvoiceID.ToString()))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Invoice Master Info", clsSearch.ClassInvoiceID.ToString(), "Get Info Detail", "", "GetInfoDetail");

                if (dbAPI.isNoRecordFound() == false)
                {
                    txtID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtParticularID.Text = txtMerchantID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                    txtInvoiceNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 3);
                    txtAcntNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                    txtCustomerNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 5);
                    txtMerchantName.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                    txtInvoiceDate.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
                    txtReferenceNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 8);
                    txtDateCoveredFrom.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 9);
                    txtDateCoveredTo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 10);
                    txtDateDue.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 11);
                    txtAmtDue.Text =  double.Parse(dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 12)).ToString("N");
                    txtRentalType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 20);
                    txtRentalTerms.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 21);
                }
            }
        }

        private void InitSummaryTextBox()
        {
            txtAmtBalance.Text = txtAmtPaid.Text = txtAmtDue.Text = txtTAmtDue.Text = clsFunction.sDefaultAmount;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            fEdit = false;
            dbFunction.ClearTextBox(this);
            dbFunction.ComBoBoxUnLock(false, this);

            // Item
            dbFunction.ClearListView(lvwList);
            InitItemListView();
           
            InitSummaryTextBox();
        }

        private void ComputeSummary()
        {
            double dblTAmtDue = 0.00;

            if (lvwList.Items.Count > 0)
            {
                foreach (ListViewItem i in lvwList.Items)
                {
                    if (dbFunction.isValidID(i.SubItems[1].Text) && dbFunction.isValidID(i.SubItems[2].Text))
                    {

                        dblTAmtDue += Convert.ToDouble(i.SubItems[12].Text); // Amt

                    }
                }
            }

            txtTAmtDue.Text = (dblTAmtDue > 0 ? dblTAmtDue.ToString("N") : clsFunction.sDefaultAmount); // Gross Amt           
        }

        private void FillMerchantTextBox()
        {
            Debug.WriteLine("--FillMerchantTextBox--");
            Debug.WriteLine("fEdit=" + fEdit);
            Debug.WriteLine("txtMerchantID.Text=" + txtMerchantID.Text);
            Debug.WriteLine("txtParticularID.Text=" + txtParticularID.Text);


            txtMerchantName.Text =
            txtMerchantAddress.Text =
            txtMerchantRegion.Text =
            txtMerchantCity.Text =
            txtMerchantContactPerson.Text =
            
            txtMerchantMobileNo.Text =
            txtMerchantEmail.Text = clsFunction.sNull;

            if (dbFunction.isValidID(clsSearch.ClassParticularID.ToString()))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Particular Info", clsSearch.ClassParticularID.ToString(), "Get Info Detail", "", "GetInfoDetail");

                if (dbAPI.isNoRecordFound() == false)
                {
                    txtMerchantID.Text = txtParticularID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                    txtMerchantName.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtMerchantAddress.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);
                    txtMerchantRegion.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                    txtMerchantCity.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                    txtMerchantContactPerson.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 11);
                    
                    txtMerchantMobileNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);
                    txtMerchantEmail.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 14);
                    
                }
            }
            
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iInvoceNo, txtInvoiceID.Text)) return;

            if (dbFunction.isValidID(txtInvoiceID.Text))
            {
                Cursor.Current = Cursors.WaitCursor;

                clsSearch.ClassAdvanceSearchValue = txtInvoiceID.Text + clsFunction.sPipe + txtParticularID.Text;
                dbReportFunc.ViewPOSRentalReport();

                Cursor.Current = Cursors.Default;
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {  
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iInvoceNo, txtInvoiceID.Text)) return;

            if (!fConfirmDetails(true)) return;

            SendInvoice(txtInvoiceID.Text, txtInvoiceNo.Text, txtMerchantName.Text, 0);

            dbFunction.SetMessageBox(txtInvoiceNo.Text + " Invoice Sent", "Send Invoice", clsFunction.IconType.iInformation);
        }

        private void btnSendAll_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iItemList, lvwList.Items.Count.ToString())) return;

            if (!fConfirmDetails(false)) return;

            SendAllInvoice();

            dbFunction.SetMessageBox("List of Invoice Sent", "Invoice Sent", clsFunction.IconType.iInformation);
            
        }

        private void EmailNotification(string pPrefix)
        {
            Debug.WriteLine("--EmailNotification--");

            dbFunction.GetProcessedByAndDateTime();

            // Get User Mobile/Email
            clsUser.ClassProcessedContactNo = clsUser.ClassProcessedEmail = clsFunction.sDash;
            dbAPI.ExecuteAPI("GET", "Search", "User Info", clsUser.ClassUserID.ToString(), "Get Info Detail", "", "GetInfoDetail");
            if (dbAPI.isNoRecordFound() == false)
            {
                clsUser.ClassProcessedContactNo = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                clsUser.ClassProcessedEmail = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 7);
            }

            clsSearch.ClassAdvanceSearchValue = txtInvoiceNo.Text + clsFunction.sCaret+
                                                txtMerchantName.Text + clsFunction.sCaret +
                                                txtMerchantContactPerson.Text + clsFunction.sCaret +
                                                txtMerchantEmail.Text + clsFunction.sCaret +
                                                pPrefix;

            Debug.WriteLine("clsSearch.ClassAdvanceSearchValue=" + clsSearch.ClassAdvanceSearchValue);
            dbAPI.ExecuteAPI("POST", "Notify", "POS Rental", clsSearch.ClassAdvanceSearchValue, "Email Notification", "", "EmailNotification");
        }

        private void SendAllInvoice()
        {
            int iLineNo = 0;
            int i = 0;

            Debug.WriteLine("--SendAllInvoice--");
            Debug.WriteLine("fEdit=" + fEdit);
            
            foreach (ListViewItem x in lvwList.Items)
            {
                int iInvoiceID = int.Parse(x.SubItems[1].Text);
                int iParticularID = int.Parse(x.SubItems[2].Text);
                string sInvoiceNo = x.SubItems[4].Text;
                string sAccountNo = x.SubItems[5].Text;
                string sCustomerNo = x.SubItems[6].Text;
                string sName = x.SubItems[7].Text;

                iLineNo++;

                Debug.WriteLine("iLineNo="+ iLineNo.ToString()+ ",iInvoiceID="+ iInvoiceID+ ",iParticularID="+ iParticularID+ ",sInvoiceNo="+ sInvoiceNo+ ",sAccountNo="+ sAccountNo+ ",sCustomerNo="+ sCustomerNo+ ",sName="+ sName);

                if (!dbFunction.isValidEntry(clsFunction.CheckType.iInvoceNo, iInvoiceID.ToString())) return;
                if (!dbFunction.isValidEntry(clsFunction.CheckType.iParticularID, iParticularID.ToString())) return;


                // Get Invoice Info
                clsSearch.ClassInvoiceID = iInvoiceID;
                ViewInfo();

                // Get Merchant Info
                clsSearch.ClassParticularID = iParticularID;
                FillMerchantTextBox(); // Get Merchant Info

                clsSearch.ClassInvoiceID = iInvoiceID;
                clsSearch.ClassInvoiceNo = sInvoiceNo;
                SendInvoice(iInvoiceID.ToString(), sInvoiceNo, sName, iLineNo); // Send Invoice

                i++;
                
            }
            
        }

        private void SendInvoice(string pInvoiceID, string pInvoiceNo, string pName, int iLineNo)
        {
            string sMessage = clsFunction.sNull;
            string sPage = clsFunction.sNull;

            Debug.WriteLine("--SendInvoice--");
            Debug.WriteLine("pInvoiceID="+ pInvoiceID);
            Debug.WriteLine("pInvoiceNo=" + pInvoiceNo);
            Debug.WriteLine("pName=" + pName);
            Debug.WriteLine("iLineNo=" + iLineNo);

            ucStatus.iState = 3;        
            ucStatus.iMin = 0;
            ucStatus.iMax = 0;

            pName = clsFunction.sNull;

            if (dbFunction.isValidID(pInvoiceID))
            {
                Cursor.Current = Cursors.WaitCursor;
                sPage = (iLineNo > 0 ? "[" + iLineNo + " / " + lvwList.Items.Count.ToString() + "]" : "");

                clsSearch.ClassInvoiceNo = pInvoiceNo;
                clsSearch.ClassIsExportToPDF = true;
                clsSearch.ClassAdvanceSearchValue = pInvoiceID + clsFunction.sPipe + txtParticularID.Text;

                sMessage = "PROCESSING INVOICE " + pInvoiceNo + pName + " " + sPage;
                ucStatus.sMessage = sMessage;
                ucStatus.AnimateStatus();
                dbReportFunc.ViewPOSRentalReport();

                // Upload File to FTP  
                sMessage = "UPLOADING INVOICE " + pInvoiceNo + pName + " " + sPage;              
                ucStatus.sMessage = sMessage;            
                ucStatus.AnimateStatus();
                string sImportFileName = pInvoiceNo + ".pdf";
                Debug.WriteLine("=>>UploadFile=" + sImportFileName);
                ftp ftpClient = new ftp(clsGlobalVariables.strFTPURL, clsGlobalVariables.strFTPUserName, clsGlobalVariables.strFTPPassword);
                ftpClient.delete(clsGlobalVariables.strFTPUploadPath + sImportFileName);
                ftpClient.upload(clsGlobalVariables.strFTPUploadPath + sImportFileName, @clsGlobalVariables.strFTPLocalExportPath + sImportFileName);
                ftpClient.disconnect(); // ftp disconnect

                // Email notify
                sMessage = "SENDING EMAIL " + pInvoiceNo + pName +  " " + sPage;
                ucStatus.sMessage = sMessage;
                ucStatus.AnimateStatus();
                EmailNotification("*");

                sMessage = "SENDING EMAIL " + pInvoiceNo + pName + " " + "COMPLETE.";
                ucStatus.sMessage = sMessage;
                ucStatus.AnimateStatus();

                Cursor.Current = Cursors.Default;
            }
        }

        private bool fConfirmDetails(bool isSingleSend)
        {
            bool fConfirm = true;
            string sTemp = "";

            if (!isSingleSend)
            {
                sTemp = "Are you sure process the following details below?\n\n" +
                          clsFunction.sLineSeparator + "\n" +
                          "[Sending invoice(s)]" + "\n" +
                          clsFunction.sLineSeparator + "\n" +
                          " >Start from: " + txtFromInvoiceNo.Text + "\n" +
                          " >End to: " + txtToInvoiceNo.Text + "\n" +
                          " >Total Count: " + txtTCount.Text + "\n" +
                          clsFunction.sLineSeparator + "\n";
            }
            else
            {
                sTemp = "Are you sure process the following details below?\n\n" +
                          clsFunction.sLineSeparator + "\n" +
                          "[Sending invoice]" + "\n" +
                          clsFunction.sLineSeparator + "\n" +
                          " >Invoice No: " + txtInvoiceNo.Text + "\n" +
                          " >Merchant: " + txtMerchantName.Text + "\n" +
                          " >Contact Person: " + txtMerchantContactPerson.Text + "\n" +
                          " >Email: " + txtMerchantEmail.Text + "\n" +
                          clsFunction.sLineSeparator + "\n";
            }
           

            if (MessageBox.Show(sTemp, "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fConfirm = false;
            }

            return fConfirm;
        }

        private bool CheckInvoiceRange(int objFrom, int objTo)
        {
            bool fValid = true;
            int iResult;

            Debug.WriteLine("--CheckDateFromTo--");
            Debug.WriteLine("objFrom=" + objFrom.ToString());
            Debug.WriteLine("objTo=" + objTo.ToString());

            iResult = (objFrom > objTo ? 1 : 0);

            Debug.WriteLine("iResult=" + iResult);

            if (iResult > 0)
                fValid = false;

            if (!fValid)
            {
                MessageBox.Show("[Schedule Date] should be greater or equal [Request Date]" +
                                        "\n\n" +
                                        "Invoice No. [From]:  " + txtFromInvoiceNo.Text +
                                        Environment.NewLine +
                                        "Invoice No. [To]:       " + txtToInvoiceNo.Text, "Invoice Range", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

            }

            return fValid;
        }
    }
}
