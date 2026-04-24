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
    public partial class frmDiagnostic : Form
    {
        private clsAPI dbAPI;
        private clsINI dbSystem;
        private clsInternet dbInternet;
        private clsFunction dbFunction;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;   // WS_EX_COMPOSITED
                //cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT
                return cp;
            }
        }

        public frmDiagnostic()
        {
            InitializeComponent();

            dbFunction = new clsFunction();            
            dbFunction.setDoubleBuffer(lvwList, true);
            dbFunction.setDoubleBuffer(lvwDetail, true);

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmDiagnostic_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        private void frmDiagnostic_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            dbInternet = new clsInternet();
            dbSystem = new clsINI();
            
            dbSystem.InitAPISetting();

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ClearListViewItems(lvwList);
            dbFunction.ClearListViewItems(lvwDetail);

            btnSave.Text = "SAVE";
            btnReset.Text = "RESET";

            txtSearchFSRDesc.BackColor = txtServiceJobTypeDescription.BackColor = lblMainStatus.BackColor = Color.Navy;
            txtSearchFSRDesc.Text = txtServiceJobTypeDescription.Text = lblMainStatus.Text = clsFunction.sDash;

            Cursor.Current = Cursors.Default;
        }

        private void loadData(ListView obj)
        {
            int i = 0;
            int iLineNo = 0;

            Cursor.Current = Cursors.WaitCursor;

            dbFunction.ClearListViewItems(obj);
            dbAPI.ExecuteAPI("GET", "View", "Diagnostic Master List", "", "Advance Detail", "", "ViewAdvanceDetail");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (dbAPI.isNoRecordFound() == false)
            {   
                while (clsArray.ID.Length > i)
                {
                    // Add to List
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TypeID));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_FormatType));
                    item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_Description));
                   
                    obj.Items.Add(item);

                    i++;
                }

                dbFunction.ListViewAlternateBackColor(obj);
            }

            Cursor.Current = Cursors.Default;
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            int i = 0;
            int iLineNo = 0;
            bool isNumeric = false;

            if (lvwList.Items.Count > 0)
            {
                dbFunction.ClearListViewItems(lvwDetail);

                string pSelectedRow = clsSearch.ClassRowSelected = dbFunction.GetListViewSelectedRow(lvwList, 0);
                Debug.WriteLine("pSelectedRow=\n" + pSelectedRow);

                if (int.Parse(dbFunction.GetSearchValue("TYPEID")) > 0)
                {
                    Cursor.Current = Cursors.WaitCursor;

                    txtTypeID.Text = dbFunction.GetSearchValue("TYPEID");
                    txtFormatType.Text = dbFunction.GetSearchValue("FORMATTYPE");
                    txtCategory.Text = dbFunction.GetSearchValue("DESCRIPTION");

                    dbAPI.ExecuteAPI("GET", "View", "Diagnostic Detail List", dbFunction.GetSearchValue("TYPEID"), "Advance Detail", "", "ViewAdvanceDetail");

                    if (!clsGlobalVariables.isAPIResponseOK) return;

                    int formattype = int.Parse(dbFunction.CheckAndSetNumericValue(txtFormatType.Text));
                    if (formattype > 0)
                    {
                        switch (formattype)
                        {
                            case 3: // Numeric
                                isNumeric = true;
                                break;
                            default:
                                isNumeric = false;
                                break;
                        }
                    }

                    if (dbAPI.isNoRecordFound() == false)
                    {
                        lvwDetail.Items.Clear();
                        while (clsArray.ID.Length > i)
                        {
                            // Add to List
                            iLineNo++;
                            ListViewItem item = new ListViewItem(iLineNo.ToString());
                            item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_SelectionID));
                            item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TypeID));
                            item.SubItems.Add(dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_Description));

                            string pSearchValue = txtServiceNo.Text + clsDefines.gPipe +
                                                  txtFSRNo.Text + clsDefines.gPipe +
                                                  dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_TypeID) + clsDefines.gPipe +
                                                  dbAPI.GetValueFromJSONString(clsArray.detail_info[i], clsDefines.TAG_SelectionID);

                            string pJSONString = dbAPI.getInfoDetailJSON("Search", "Diagnostic Detail Info", pSearchValue);

                            string pSelectedValue = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SelectedValue);

                            pSelectedValue = (pSelectedValue.Equals(clsFunction.sDash) ? clsFunction.sZero : pSelectedValue);
                            
                            string pFormatedValue = ((!pSelectedValue.Equals(clsFunction.sDefaultSelect) || !pSelectedValue.Equals(clsFunction.sDash)) ? (!isNumeric ? dbFunction.setIntegerToYesNoString(int.Parse(pSelectedValue)) : pSelectedValue) : clsFunction.sDefaultSelect);
                            
                            pFormatedValue = (!isNumeric ? pFormatedValue : Convert.ToInt32(double.Parse(pFormatedValue)).ToString());

                            item.SubItems.Add(pFormatedValue.ToUpper());

                            lvwDetail.Items.Add(item);

                            i++;
                        }

                        dbFunction.ListViewAlternateBackColor(lvwDetail);
                    }

                    initComboBoxValue();

                    btnSave.Text = "SAVE " + txtCategory.Text + " CATEGORY";
                    btnReset.Text = "RESET " + txtCategory.Text + " CATEGORY";

                    txtDescription.Text = clsFunction.sNull;
                    txtSelectionID.Text = clsFunction.sNull;
                    cboValue.Text = clsFunction.sDefaultSelect;

                    Cursor.Current = Cursors.Default;

                }
            }
        }

        private void lvwDetail_DoubleClick(object sender, EventArgs e)
        {
            if (lvwDetail.Items.Count > 0)
            {
                string pSelectedRow = clsSearch.ClassRowSelected = dbFunction.GetListViewSelectedRow(lvwDetail, 0);
                Debug.WriteLine("pSelectedRow=\n" + pSelectedRow);

                txtLineNo.Text = dbFunction.GetSearchValue("LINE#");
                txtSelectionID.Text = dbFunction.GetSearchValue("SELECTIONID");
                txtDescription.Text = dbFunction.GetSearchValue("DESCRIPTION");
                cboValue.Text = dbFunction.GetSearchValue("VALUE");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dbFunction.ClearComboBox(this);
            dbFunction.ClearTextBox(this);
            dbFunction.ClearListViewItems(lvwList);
            dbFunction.ClearListViewItems(lvwDetail);

            btnSave.Text = "SAVE";
            btnReset.Text = "RESET";

            txtSearchFSRDesc.BackColor = txtServiceJobTypeDescription.BackColor = lblMainStatus.BackColor = Color.Navy;
            txtSearchFSRDesc.Text = txtServiceJobTypeDescription.Text = lblMainStatus.Text = clsFunction.sDash;
        }

        private void initComboBoxValue()
        {
            int formattype = int.Parse(dbFunction.CheckAndSetNumericValue(txtFormatType.Text));
            if (formattype > 0)
            {
                switch (formattype)
                {
                    case 1:
                        dbAPI.FillComboBoxYesNo(cboValue);
                        break;
                    case 2:
                        dbAPI.FillComboBoxYesNo(cboValue);
                        break;
                    case 3:
                        dbAPI.FillComboBoxSurvey(cboValue);
                        break;
                    default:
                        dbAPI.FillComboBoxYesNo(cboValue);
                        break;
                }
            }
        }

        private void btnSingleChanges_Click(object sender, EventArgs e)
        {
            if (!cboValue.Text.Equals(clsFunction.sDefaultSelect))
            {
                if (int.Parse(dbFunction.CheckAndSetNumericValue(txtSelectionID.Text)) > 0)
                    dbFunction.updateListViewByColRow(lvwDetail, 4, int.Parse(txtLineNo.Text), cboValue.Text);
                else
                    dbFunction.SetMessageBox("Description must be valid", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }
            else
            {
                dbFunction.SetMessageBox("Value must be valid", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }
        }

        private void btnMultipleChanges_Click(object sender, EventArgs e)
        {
            int iLineNo = 0;
            if (!cboValue.Text.Equals(clsFunction.sDefaultSelect))
            {
                for (int i = 0; i < lvwDetail.Items.Count; i++)
                {
                    iLineNo++;
                    dbFunction.updateListViewByColRow(lvwDetail, 4, iLineNo, cboValue.Text);
                }
            }
            else
            {
                dbFunction.SetMessageBox("Value must be valid", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }
        }

        private void btnSearchFSR_Click(object sender, EventArgs e)
        {
            frmSearchField.iSearchType = frmSearchField.SearchType.iFSR;
            frmSearchField.sHeader = "SEARCH SERVICE";
            frmSearchField.isCheckBoxes = false;
            frmSearchField frm = new frmSearchField();
            frm.ShowDialog();

            if (frmSearchField.fSelected)
            {
                Cursor.Current = Cursors.WaitCursor;

                dbFunction.ClearTextBox(this);
                dbFunction.TextBoxUnLock(false, this);
                dbFunction.ClearListViewItems(lvwList);
                dbFunction.ClearListViewItems(lvwDetail);

                txtServiceNo.Text = clsSearch.ClassServiceNo.ToString();
                txtFSRNo.Text = txtFSRNo.Text = clsSearch.ClassFSRNo.ToString();                     
                txtMerchantID.Text = clsSearch.ClassMerchantID.ToString();                             
                txtIRIDNo.Text = clsSearch.ClassIRIDNo.ToString();

                string pJSONString = dbAPI.getInfoDetailJSON("Search", "Merchant Servicing Info", txtServiceNo.Text + clsDefines.gPipe + txtIRIDNo.Text);

                dbFunction.parseDelimitedString(pJSONString, clsDefines.gComma, 0);

                txtServiceJobTypeDescription.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ServiceJobTypeDescription);
                txtMerchantName.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MERCHANTNAME);
                txtMerchantAddress.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Address);
                txtMerchantCity.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Province);
                txtMerchantRegion.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_REGION);
                txtAppVersion.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_AppVersion);
                txtAppCRC.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_AppCRC);
                txtIRTID.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TID);
                txtIRMID.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MID);

                txtCurTerminalSN.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalSN);
                txtCurTerminalType.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalType);
                txtCurTerminalModel.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TerminalModel);
                txtCurSIMSN.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SIMSN);
                txtCurSIMCarrier.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_SIMCarrier);

                txtRepTerminalSN.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReplaceTerminalSN);
                txtRepTerminalType.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReplaceTerminalType);
                txtRepTerminalModel.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReplaceTerminalModel);
                txtRepSIMSN.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReplaceSIMSN);
                txtRepSIMCarrier.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ReplaceSIMCarrier);

                txtReqInstallationDate.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_RequestDate);
                txtServiceReqDate.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ScheduleDate);
                txtCreatedDate.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ServiceCreatedDate);
                txtCreatedTime.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ServiceCreatedTime);

                txtIRNo.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_IRNO);
                txtRequestNo.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_RequestNo);
                txtReferenceNo.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_REFERENCENO);
                txtServiceStatus.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_JobTypeStatusDescription);

                txtMFSRDate.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FSRDate);               
                txtMReceiptTime.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FSRTime);
                txtMTimeArrived.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TimeArrived);
                txtMTimeStart.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TimeStart);
                txtMTimeEnd.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_TimeEnd);
                txtFSRDateTime.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FSRDate) + " " + dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FSRTime);
                txtServiceResult.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ActionMade);
                txtReason.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Reason);

                txtVendorRepName.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_FEName);
                txtVendorRepPosition.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Position);
                txtVendorRepEmail.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Email);

                txtMerchRepName.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MerchantRepresentative);
                txtMerchRepPosition.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MerchantRepresentativePosition);
                txtMerchRepEmail.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MerchantRepresentativeEmail);

                txtMobileID.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_MobileID);

                txtClientName.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_ClientName);
                txtDispatcher.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_Dispatcher);
                txtDispatcherEmail.Text = dbAPI.GetValueFromJSONString(pJSONString, clsDefines.TAG_DispatcherEmail);

                if (!isValidEFRMode())
                {
                    dbFunction.ClearListViewItems(lvwList);
                    dbFunction.ClearListViewItems(lvwDetail);
                    return;
                }

                loadData(lvwList);

                setModeStatus();

                txtSearchFSRDesc.Text = (dbFunction.isValidID(txtMobileID.Text) ? clsDefines.DIGITAL_FSR : clsDefines.MANUAL_FSR);

                txtSearchFSRDesc.BackColor = txtServiceJobTypeDescription.BackColor = lblMainStatus.BackColor = Color.Navy;
                
                btnClear.Focus();

                Cursor.Current = Cursors.Default;

            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            int iLineNo = 0;
            for (int i = 0; i < lvwDetail.Items.Count; i++)
            {
                iLineNo++;
                dbFunction.updateListViewByColRow(lvwDetail, 4, iLineNo, clsFunction.sDefaultSelect);
            }
        }

        private void saveDiagnosticDetail()
        {
            string sRowSQL = "";
            string sSQL = "";
            double pValue = 0;
            bool isNumeric = false;

            if (lvwDetail.Items.Count > 0)
            {
                foreach (ListViewItem i in lvwDetail.Items)
                {
                    int SelectionID = int.Parse(i.SubItems[1].Text);
                    int TypeID = int.Parse(i.SubItems[2].Text);
                    string sDescription = i.SubItems[3].Text;
                    string sValue = i.SubItems[4].Text;

                    Debug.WriteLine("SelectionID="+ SelectionID+ ",TypeID="+ TypeID + ",sDescription="+ sDescription + ",sValue="+ sValue);

                    isNumeric = false;
                    if (sValue.Equals(clsDefines.gYes))
                    {
                        isNumeric = true;
                        pValue = 1;
                    }
                    else if (sValue.Equals(clsDefines.gNo))
                    {
                        isNumeric = true;
                        pValue = 0;
                    }
                    else
                    {
                        pValue = double.Parse(sValue);
                    }

                    // Insert
                    sRowSQL = "";
                    sRowSQL = " (" + dbFunction.CheckAndSetNumericValue(txtFSRNo.Text) + "," +
                    sRowSQL + sRowSQL + "" + dbFunction.CheckAndSetNumericValue(txtServiceNo.Text) + "," +
                    sRowSQL + sRowSQL + "" + dbFunction.CheckAndSetNumericValue(TypeID.ToString()) + "," +
                    sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(txtCategory.Text) + "'," +
                    sRowSQL + sRowSQL + "" + dbFunction.CheckAndSetNumericValue(SelectionID.ToString()) + "," +
                    sRowSQL + sRowSQL + "'" + dbFunction.CheckAndSetStringValue(sDescription) + "'," +
                    sRowSQL + sRowSQL + "'" + (isNumeric ? (pValue > 0 ? 1 : 0) : pValue) + "')";

                    Debug.WriteLine("sRowSQL=" + sRowSQL);

                    if (sSQL.Length > 0)
                        sSQL = sSQL + ", " + sRowSQL;
                    else
                        sSQL = sSQL + sRowSQL;

                }

                Debug.WriteLine("sSQL=" + sSQL);

                dbAPI.ExecuteAPI("POST", "Insert", "", txtFSRNo.Text + clsDefines.gPipe + txtServiceNo.Text + clsDefines.gPipe + txtTypeID.Text, "Manual Diagnostic Detail", sSQL, "InsertCollectionDetail");
            }
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            // Check Application Version
            if (!dbAPI.isValidSystemVersion()) return;

            if (int.Parse(dbFunction.CheckAndSetNumericValue(txtFSRNo.Text)) > 0 && int.Parse(dbFunction.CheckAndSetNumericValue(txtServiceNo.Text)) > 0 && int.Parse(dbFunction.CheckAndSetNumericValue(txtTypeID.Text)) > 0)
            {
                if (!isValidEFRMode()) return;

                if (!validateListViewValue()) return;

                if (!dbFunction.fPromptConfirmation("Are you sure to save diagnostic for category " + "\n" + dbFunction.AddBracketStartEnd(txtCategory.Text))) return;
                
                saveDiagnosticDetail();

                dbFunction.SetMessageBox("Diagnostic for category " + "\n" + dbFunction.AddBracketStartEnd(txtCategory.Text) + "\n\n" + "has been successfully saved.", "Diagnostic", clsFunction.IconType.iInformation);
            }        
        }

        private bool validateListViewValue()
        {
            bool isValid = true;
            foreach (ListViewItem i in lvwDetail.Items)
            {
                string sValue = i.SubItems[4].Text;
                if (sValue.Equals(clsFunction.sDefaultSelect))
                {
                    isValid = false;
                    break;
                }
            }

            if (!isValid)
            {
                dbFunction.SetMessageBox("Invalid value found.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }

            return isValid;

        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (!isValidDiagnotic()) return;

            if (int.Parse(dbFunction.CheckAndSetNumericValue(txtFSRNo.Text)) > 0 && int.Parse(dbFunction.CheckAndSetNumericValue(txtServiceNo.Text)) > 0)
            {
                clsSearch.ClassIsExportToPDF = false;
                clsSearch.ClassFSRNo = int.Parse(txtFSRNo.Text);
                clsSearch.ClassServiceNo = int.Parse(txtServiceNo.Text);
                dbFunction.eDiagnosticReport(42);
            }
            else
            {
                dbFunction.SetMessageBox("Unable to preview diagnostic, select service to proceed", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }
            
        }

        private void btnResetEmail_Click(object sender, EventArgs e)
        {
            if (!isValidDiagnotic()) return;

            Cursor.Current = Cursors.WaitCursor;

            if (dbFunction.isValidID(txtFSRNo.Text) && dbFunction.isValidID(txtServiceNo.Text))
            {
                if (!dbFunction.fPromptConfirmation("Are you sure to reset eFSR email?" + "\n\n" +
                    " > ServiceNo " + txtServiceNo.Text + "\n" +
                    " > FSRNo " + txtFSRNo.Text + "\n" +
                    " > Service Type " + txtServiceJobTypeDescription.Text + "\n" +
                    " > Merchant " + txtMerchantName.Text)) return;

                dbAPI.ExecuteAPI("PUT", "Update", "Manual Diagnostic Detail", txtFSRNo.Text + clsFunction.sPipe + txtServiceNo.Text, "", "", "UpdateCollectionDetail");

                dbAPI.ExecuteAPI("PUT", "Update", "Update isPDF", txtFSRNo.Text + clsFunction.sPipe + clsFunction.sZero, "", "", "UpdateCollectionDetail");

                dbAPI.ExecuteAPI("PUT", "Update", "Update isDiagnostic", txtFSRNo.Text + clsFunction.sPipe + clsFunction.sZero, "", "", "UpdateCollectionDetail");

                dbFunction.SetMessageBox("Reset email complete." + "\n" + "You may re-send email" + "\n" + "or advise vendor representative to send email on their end.", "Reset email notification", clsFunction.IconType.iInformation);
            }
            else
            {
                dbFunction.SetMessageBox("Unable to process. No selected service.", "Reset email notification", clsFunction.IconType.iError);
            }

            Cursor.Current = Cursors.Default;
        }
        
        private void setModeStatus()
        {
            lblMainStatus.Text = clsDefines.UPDATE_RECORD;
            if (!dbAPI.isRecordExist("Search", "Diagnostic Detail", txtServiceNo.Text + clsDefines.gPipe + txtFSRNo.Text))
            {
                lblMainStatus.Text = clsDefines.NEW_RECORD;
            }
        }

        private bool isValidDiagnotic()
        {
            bool isValid = true;

            if (!dbAPI.isRecordExist("Search", "Diagnostic Detail", txtServiceNo.Text + clsDefines.gPipe + txtFSRNo.Text))
            {
                isValid = false;
            }

            if (!isValid)
            {
                dbFunction.SetMessageBox("Unable to process, diagnostic not found", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }

            return isValid;

        }
        
        private bool isValidEFRMode()
        {
            bool isValid = false;

            if (dbFunction.isValidID(txtMobileID.Text))
                isValid = true;

            if (!isValid)
            {
                dbFunction.SetMessageBox("Unable to process diagnostic." + "\n" + "Its not a DIGITAL FSR.", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }

            return isValid;
        }

        private void btnPreviewFSR_Click(object sender, EventArgs e)
        {
            if (int.Parse(dbFunction.CheckAndSetNumericValue(txtFSRNo.Text)) > 0 && int.Parse(dbFunction.CheckAndSetNumericValue(txtServiceNo.Text)) > 0)
            {
                clsSearch.ClassIsExportToPDF = false;
                dbFunction.eFSRReport(5);
            }
            else
            {
                dbFunction.SetMessageBox("Unable to preview FSR report, select service to proceed", clsDefines.FIELD_CHECK_MSG, clsFunction.IconType.iError);
            }                
        }

        private void btnMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
