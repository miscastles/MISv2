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
    public partial class frmPRCreateInvoice : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        private bool fEdit = false;
        private int iModeOfPayment = 0;

        public frmPRCreateInvoice()
        {
            InitializeComponent();
        }

        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmPRCreateInvoice_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);

            // Item
            dbFunction.ClearListView(lvwList);
            InitItemListView();

            InitDate();
            fEdit = false;
            InitButton();
            InitCheckBox();
            InitTextBoxDecimalValue();

            Cursor.Current = Cursors.Default;
        }

        private void InitCheckBox()
        {
            chkOptPayment1.Checked = false;
            chkOptPayment2.Checked = false;
            chkOptPayment3.Checked = false;
            chkOptPayment4.Checked = false;
            chkOptPayment5.Checked = false;
            
        }

        private void frmPRCreateInvoice_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.F1: // New
                    btnAdd_Click(this, e);
                    break;
                case Keys.F2: // Sesrch               
                    btnSearchMerchant_Click(this, e);
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void InitDate()
        {
            dteDateCoveredFrom.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteDateCoveredFrom, clsFunction.sDateDefaultFormat);

            dteDateCoveredTo.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteDateCoveredTo, clsFunction.sDateDefaultFormat);

            dteDateDue.Value = DateTime.Now.Date;
            dbFunction.SetDateFormat(dteDateDue, clsFunction.sDateDefaultFormat);

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            fEdit = false;
            InitButton();

            dbFunction.TextBoxUnLock(true, this);
            dbFunction.ComBoBoxUnLock(true, this);

            btnAdd.Enabled = false;
            btnSave.Enabled = true;

            // Generate ControlID
            txtID.Text = dbAPI.GetControlID("Invoice Master").ToString();
            txtInvoiceNo.Text = dbFunction.GenerateControlNo(int.Parse(txtID.Text), "PR-100000", false);

            InitCreatedDateTime();

            SetMKTextBoxBackColor();
                
            Cursor.Current = Cursors.Default;
        }

        private void InitButton()
        {
            if (fEdit)
            {
                btnAdd.Enabled = false;
                btnSave.Enabled = true;
            }
            else
            {
                btnAdd.Enabled = true;
                btnSave.Enabled = false;
            }
        }

        private void SelecPaymentOption(CheckBox obj)
        {
            if (obj.Checked)
            {
                if (obj.Name.Equals("chkOptPayment1"))
                {
                    iModeOfPayment = 1;
                    chkOptPayment2.Checked = false;
                    chkOptPayment3.Checked = false;
                    chkOptPayment4.Checked = false;
                    chkOptPayment5.Checked = false;
                }

                if (obj.Name.Equals("chkOptPayment2"))
                {
                    iModeOfPayment = 2;
                    chkOptPayment1.Checked = false;
                    chkOptPayment3.Checked = false;
                    chkOptPayment4.Checked = false;
                    chkOptPayment5.Checked = false;
                }

                if (obj.Name.Equals("chkOptPayment3"))
                {
                    iModeOfPayment = 3;
                    chkOptPayment1.Checked = false;
                    chkOptPayment2.Checked = false;
                    chkOptPayment4.Checked = false;
                    chkOptPayment5.Checked = false;
                }

                if (obj.Name.Equals("chkOptPayment4"))
                {
                    iModeOfPayment = 4;
                    chkOptPayment1.Checked = false;
                    chkOptPayment2.Checked = false;
                    chkOptPayment3.Checked = false;
                    chkOptPayment5.Checked = false;
                }

                if (obj.Name.Equals("chkOptPayment5"))
                {
                    iModeOfPayment = 5;
                    chkOptPayment1.Checked = false;
                    chkOptPayment2.Checked = false;
                    chkOptPayment3.Checked = false;
                    chkOptPayment4.Checked = false;
                }
            }

        }

        private void chkOptPayment1_CheckedChanged(object sender, EventArgs e)
        {
            SelecPaymentOption(chkOptPayment1);
        }

        private void chkOptPayment2_CheckedChanged(object sender, EventArgs e)
        {
            SelecPaymentOption(chkOptPayment2);
        }

        private void chkOptPayment3_CheckedChanged(object sender, EventArgs e)
        {
            SelecPaymentOption(chkOptPayment3);
        }

        private void chkOptPayment4_CheckedChanged(object sender, EventArgs e)
        {
            SelecPaymentOption(chkOptPayment4);
        }

        private void chkOptPayment5_CheckedChanged(object sender, EventArgs e)
        {
            SelecPaymentOption(chkOptPayment5);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);

            // Item
            dbFunction.ClearListView(lvwList);
            InitItemListView();

            InitTextBoxDecimalValue();
            
            InitCheckBox();

            fEdit = false;
            InitButton();
            
            dbFunction.ClearListView(lvwList);
            
            Cursor.Current = Cursors.Default;
        }

        private void InitTextBoxDecimalValue()
        {
            txtTAmtDue.Text = clsFunction.sDefaultAmount;
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (!CheckDateFromTo(dteDateCoveredFrom, dteDateCoveredTo)) return;

            if (!dbFunction.isValidEntry(clsFunction.CheckType.iItemList, lvwList.Items.Count.ToString())) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iInvoceNo, txtInvoiceNo.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iAcntNo, txtAcntNo.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iCustomerNo, txtCustomerNo.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantName, txtMerchantName.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRentalType, txtRentalType.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iRentalTerms, txtRentalTerms.Text)) return;
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iReferenceNo, txtReferenceNo.Text)) return;

            if (!fConfirm(fEdit)) return;

            if (!fEdit)
            {
                SaveMaster();
                SaveDetail();
            }
            else
            {
                // Update here...
            }

            Cursor.Current = Cursors.Default;

            if (fEdit)
                dbFunction.SetMessageBox("Invoice has been successfully modified.", "Updated", clsFunction.IconType.iInformation);
            else
                dbFunction.SetMessageBox("Invoice has been successfully added.", "Saved", clsFunction.IconType.iInformation);


            btnClear_Click(this, e);
        }

        private void SaveMaster()
        {
            string sRowSQL = "";
            string sSQL = "";
            
            dbFunction.GetProcessedByAndDateTime();
            dbFunction.GetModifiedByAndDateTime();

            clsSearch.ClassRentalFee = 0.00;
            clsSearch.ClassRentalFee = Convert.ToDouble(txtTAmtDue.Text); // Amt Due

            sRowSQL = "";
            sRowSQL = " ('" + dbFunction.CheckAndSetStringValue(txtInvoiceNo.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtAcntNo.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtCustomerNo.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtParticularID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(txtMerchantID.Text) + "', " +
            sRowSQL + sRowSQL + " '" + txtDate.Text + "', " +
            sRowSQL + sRowSQL + " '" + txtTime.Text + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtReferenceNo.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetDatePickerValueToDate(dteDateCoveredFrom) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetDatePickerValueToDate(dteDateCoveredTo) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetDatePickerValueToDate(dteDateDue) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(clsUser.ProcessedBy) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(clsUser.ClassProcessedDateTime) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(clsUser.ModifiedBy) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(clsUser.ClassModifiedDateTime) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(iModeOfPayment.ToString()) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtNoteToCustomer.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtNoteToSelf.Text) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(clsSearch.ClassRentalFee.ToString()) + "', " +
            sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetNumericValue(clsSearch.ClassCurrentUserID.ToString()) + "') ";
            sSQL = sSQL + sRowSQL;

            Debug.WriteLine("sSQL=" + sSQL);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "Invoice Master", sSQL, "InsertCollectionMaster");

            clsSearch.ClassLastInsertedID = clsLastID.ClassLastInsertedID;
            txtID.Text = clsSearch.ClassLastInsertedID.ToString();

        }

        private void SaveDetail()
        {
            string sRowSQL = "";
            string sSQL = "";
            
            Debug.WriteLine("--SaveDetail--");
            Debug.WriteLine("fEdit=" + fEdit);

            sSQL = "";
            
            dbFunction.GetProcessedByAndDateTime();

            foreach (ListViewItem x in lvwList.Items)
            {
                clsSearch.ClassIRIDNo = int.Parse(x.SubItems[1].Text);
                clsSearch.ClassDescription = x.SubItems[5].Text;
                clsSearch.ClassMID = x.SubItems[6].Text;
                clsSearch.ClassTID = x.SubItems[7].Text;

                clsSearch.ClassRentalFee = 0.00;
                clsSearch.ClassRentalFee = Convert.ToDouble(x.SubItems[8].Text);
                
                if (clsSearch.ClassIRIDNo > 0 && lvwList.Items.Count > 0)
                {

                    // Insert
                    sRowSQL = "";
                    sRowSQL = " (" + dbFunction.CheckAndSetNumericValue(txtID.Text) + ", " +
                    sRowSQL + sRowSQL + " '" + dbFunction.CheckAndSetStringValue(txtInvoiceNo.Text) + "', " +
                    sRowSQL + sRowSQL + " '" + txtDate.Text + "', " +
                    sRowSQL + sRowSQL + " '" + txtTime.Text + "', " +
                    sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(clsSearch.ClassIRIDNo.ToString()) + ", " +
                    sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(txtParticularID.Text) + ", " +
                    sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(txtMerchantID.Text) + ", " +
                    sRowSQL + sRowSQL + " '" + clsSearch.ClassTID + "', " +
                    sRowSQL + sRowSQL + " '" + clsSearch.ClassMID + "', " +
                    sRowSQL + sRowSQL + " " + dbFunction.AddSingleQuote(clsSearch.ClassDescription) + ", " +                  
                    sRowSQL + sRowSQL + " " + dbFunction.CheckAndSetNumericValue(clsSearch.ClassRentalFee.ToString()) + ") ";

                    if (sSQL.Length > 0)
                        sSQL = sSQL + ", " + sRowSQL;
                    else
                        sSQL = sSQL + sRowSQL;
                }

            }

            Debug.WriteLine("sSQL=" + sSQL);

            dbAPI.ExecuteAPI("POST", "Insert", "", "", "Invoice Detail", sSQL, "InsertCollectionDetail");

        }

        private void InitCreatedDateTime()
        {
            DateTime ProcessDateTime = DateTime.Now;
            string sProcessDate = "";
            string sProcessTime = "";

            sProcessDate = ProcessDateTime.ToString("yyyy-MM-dd");
            sProcessTime = ProcessDateTime.ToString("HH:mm:ss");

            txtDate.Text = sProcessDate;
            txtTime.Text = sProcessTime;
        }

        private void btnSearchMerchant_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iMerchantList;
            frmSearchField.sHeader = "MERCHANT";
            frmSearchField.isCheckBoxes = false;
            frmSearchField.isPreview = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                Cursor.Current = Cursors.WaitCursor;

                dbFunction.TextBoxUnLock(false, this);

                btnAdd.Enabled = false;
                btnSave.Enabled = true;

                txtMerchantID.Text = txtParticularID.Text = clsSearch.ClassParticularID.ToString();
                txtMerchantName.Text = clsSearch.ClassParticularName;

                FillMerchantTextBox();

                if (dbFunction.isValidID(txtMerchantID.Text))
                    LoadDetail();

                PKTextBoxReadOnly(false);

                SetMKTextBoxBackColor();

                btnSearchInvoice.Enabled = true;

                txtReferenceNo.Focus();

                Cursor.Current = Cursors.Default;


            }
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
            txtMerchantTelNo.Text =
            txtMerchantMobileNo.Text =
            txtMerchantEmail.Text = clsFunction.sNull;

            if (dbFunction.isValidID(txtMerchantID.Text))
            {
                dbAPI.ExecuteAPI("GET", "Search", "Particular Info", txtMerchantID.Text, "Get Info Detail", "", "GetInfoDetail");

                if (dbAPI.isNoRecordFound() == false)
                {
                    txtMerchantID.Text = txtParticularID.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 0);
                    txtMerchantName.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 1);
                    txtMerchantAddress.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 2);                  
                    txtMerchantRegion.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 4);
                    txtMerchantCity.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 6);
                    txtMerchantContactPerson.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 11);
                    txtMerchantTelNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 12);
                    txtMerchantMobileNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 13);
                    txtMerchantEmail.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 14);

                    // POS Rental
                    txtRentalType.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 17);
                    txtRentalTerms.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 19);
                    txtAcntNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 20);
                    txtCustomerNo.Text = dbFunction.getDelimitedString(clsSearch.ClassOutParamValue, clsFunction.cPipe, 21);

                }
            }

            txtReferenceNo.Focus();

        }

        private void SetMKTextBoxBackColor()
        {
            txtMerchantName.BackColor = txtAcntNo.BackColor = txtCustomerNo.BackColor = clsFunction.MKBackColor;

            txtRentalType.BackColor = clsFunction.PKBackColor;
            txtRentalTerms.BackColor = clsFunction.PKBackColor;

        }

        private void PKTextBoxReadOnly(bool fReadOnly)
        {
            txtReferenceNo.ReadOnly = fReadOnly;
            txtNoteToCustomer.ReadOnly = fReadOnly;
            txtNoteToSelf.ReadOnly = fReadOnly;

            if (!fReadOnly)
            {
                txtReferenceNo.BackColor = clsFunction.EntryBackColor;
                txtNoteToCustomer.BackColor = clsFunction.EntryBackColor;
                txtNoteToSelf.BackColor = clsFunction.EntryBackColor;
                
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

            dbFunction.GetListViewHeaderColumnFromFile("", "IRNo", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "ParticularID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "MerchantID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Description", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "MID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "TID", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);

            dbFunction.GetListViewHeaderColumnFromFile("", "Amount", out outField, out outWidth, out outTitle, out outAlign, out outVisible, out outAutoWidth, out outFormat);
            lvwList.Columns.Add(outTitle, outWidth, outAlign);
            
        }

        private void btnClearListView_Click(object sender, EventArgs e)
        {
            // Item
            dbFunction.ClearListView(lvwList);
            InitItemListView();
        }

        private void LoadDetail()
        {
            int i = 0;
            int iLineNo = 0;

            dbFunction = new clsFunction();

            dbFunction.ClearListView(lvwList);
            InitItemListView();

            clsSearch.ClassSearchValue = txtMerchantID.Text;

            dbAPI.ExecuteAPI("GET", "View", "POS Rental IR List", clsSearch.ClassSearchValue, "IR", "", "ViewAdvanceIR");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {
                lvwList.Items.Clear();
                while (clsArray.IRIDNo.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.IRIDNo[i].ToString());
                    item.SubItems.Add(clsArray.IRNo[i].ToString());
                    item.SubItems.Add(clsArray.ParticularID[i].ToString());
                    item.SubItems.Add(clsArray.MerchantID[i].ToString());
                    item.SubItems.Add(clsArray.Description[i]);
                    item.SubItems.Add(clsArray.MID[i]);
                    item.SubItems.Add(clsArray.TID[i]);
                    item.SubItems.Add(Convert.ToDouble(clsArray.RentalFee[i].ToString()).ToString("N"));

                    lvwList.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(lvwList);

                dbFunction.FocusFirstItemInListView(lvwList, 0); // focus 1st row

                ComputeSummary();
            }
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
                        
                        dblTAmtDue += Convert.ToDouble(i.SubItems[8].Text); // Rental Fee

                    }
                }
            }        

            txtTAmtDue.Text = (dblTAmtDue > 0 ? dblTAmtDue.ToString("N") : clsFunction.sDefaultAmount); // Gross Amt           
        }

        private bool fConfirm(bool fUpdate)
        {
            bool fConfirm = true;
            string sMessage = clsFunction.sPipe;

            sMessage =
                        "Confirm details below:" + "\n" +
                        clsFunction.sLineSeparator + "\n" +
                        "Invoice information" + "\n" +
                        clsFunction.sLineSeparator + "\n" +
                        " >Invoice ID: " + txtID.Text + "\n" +
                        " >Invoice No: " + txtInvoiceNo.Text + "\n" +
                        " >Date: " + txtDate.Text + "\n" +
                        " >Reference No: " + txtReferenceNo.Text + "\n" +
                        " >Rental Type: " + txtRentalType.Text + "\n" +
                        " >Rental Terms: " + txtRentalTerms.Text + "\n" +
                        " >Date Covered: " + "From " + dteDateCoveredFrom.Value.ToString("MM-dd-yyyy") + " to " + dteDateCoveredTo.Value.ToString("MM-dd-yyyy") + "\n" +
                        " >Due Date: " + dteDateDue.Value.ToString("MM-dd-yyyy") + "\n" +
                         clsFunction.sLineSeparator + "\n" +
                        "Merchant information" + "\n" +
                        clsFunction.sLineSeparator + "\n" +
                        " >Merchant: " + txtMerchantName.Text + "\n" +                    
                        " >Account No.: " + txtAcntNo.Text + "\n" +
                        " >Customer No.: " + txtCustomerNo.Text + "\n" +
                        " >Contact Person: " + txtMerchantContactPerson.Text + "\n" +
                        " >Email: " + txtMerchantEmail.Text;

            if (MessageBox.Show("Are you sure you want to " + (fUpdate == true ? "update" : "save") + " " + "record?" +
                                    "\n\n" +
                                    sMessage,
                                    (fUpdate ? "Confirm update" : "Confirm save"), MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
            {
                fConfirm = false;
            }

            return fConfirm;
        }

        private bool CheckDateFromTo(DateTimePicker objFrom, DateTimePicker objTo)
        {
            bool fValid = true;
            int iResult;

            Debug.WriteLine("--CheckDateFromTo--");
            Debug.WriteLine("objFrom=" + objFrom.ToString());
            Debug.WriteLine("objTo=" + objTo.ToString());

            iResult = DateTime.Compare(DateTime.Parse(objFrom.Value.ToShortDateString()), DateTime.Parse(objTo.Value.ToShortDateString()));

            Debug.WriteLine("iResult=" + iResult);

            if (iResult > 0)
                fValid = false;

            if (!fValid)
            {
                MessageBox.Show("[Schedule Date] should be greater or equal [Request Date]" +
                                        "\n\n" +
                                        "Date Covered To:  " + objTo.Value.ToString("MM-dd-yyyy") +
                                        Environment.NewLine +
                                        "Date Covered From:   " + objFrom.Value.ToString("MM-dd-yyyy"), "Date Convered Range", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);

            }

            return fValid;
        }

        private void btnSearchInvoice_Click(object sender, EventArgs e)
        {
            if (!dbFunction.isValidEntry(clsFunction.CheckType.iMerchantName, txtMerchantName.Text)) return;

            clsSearch.ClassParticularID = int.Parse(txtParticularID.Text);
            frmSearchField.iSearchType = frmSearchField.SearchType.iSearchInvoice;
            frmSearchField.sHeader = "INVOICE";
            frmSearchField.isCheckBoxes = false;
            frmSearchField.isPreview = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();
        }
    }
}
