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
using System.IO;
using MIS.Controller;
using static MIS.Function.AppUtilities;
using MIS.Enums;
using System.Runtime.InteropServices;
using Xceed.Words.NET;

namespace MIS
{
    public partial class frmMerchantDocumentGenerator : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;
        private clsFile dbFile;
        public static string sHeader;
        bool fEdit = false;

        private string DocFilePath = @"C:\DocGenerator";
        private string DocFileName1 = "FileSampleWithSignature.docx";
        private string DocFileName2 = "FileSampleWithoutSignature.docx";
        private string Sole1 = "SoleProp1.docx";
        private string Sole2 = "SoleProp2.docx";
        private string Corporation1 = "Corporation1.docx";
        private string Corporation2 = "Corporation2.docx";
        private string Partnership1 = "Partnership1.docx";
        private string Partnership2 = "Partnership2.docx";
        private string templateFilePath;

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

        public frmMerchantDocumentGenerator()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void frmMerchantDocumentGenerator_Load(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();
            dbFile = new clsFile();

            fEdit = false;
            InitButton();

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.InitTextBoxCharacterCasing(CharacterCasing.Upper, this);
            dbFunction.ComBoBoxUnLock(false, this);
            dbFunction.ComBoBoxDropDownStyle(ComboBoxStyle.DropDownList, this);
            dbFunction.ComboBoxCaps(false, this);

            dbFunction.DatePickerUnlock(false, this);
            dbFunction.InitDateTimePicker(clsFunction.sDateDefaultFormat, this);

            // init document path
            string pTemplatePath = dbFile.sTemplatePath;
            DocFileName1 = $"{pTemplatePath}FileSampleWithSignature.docx";
            DocFileName2 = $"{pTemplatePath}FileSampleWithoutSignature.docx";
            Sole1 = $"{pTemplatePath}SoleProp1.docx";
            Sole2 = $"{pTemplatePath}SoleProp2.docx";
            Corporation1 = $"{pTemplatePath}Corporation1.docx";
            Corporation2 = $"{pTemplatePath}Corporation2.docx";
            Partnership1 = $"{pTemplatePath}Partnership1.docx";
            Partnership2 = $"{pTemplatePath}Partnership2.docx";
        
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            fEdit = false;
            InitButton();

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(true, this);
            dbFunction.ComBoBoxUnLock(true, this);
            dbFunction.DatePickerUnlock(true, this);

            btnAdd.Enabled = false;
            btnSave.Enabled = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //if (!dbFunction.fPromptConfirmation("Are you sure to save merchant registration?")) return;

            // Get the selected form of business
            string selectedFormOfBusiness = formofbusinessbx.SelectedItem?.ToString();

            string selectedTemplate = null;

            if (selectedFormOfBusiness == "SoleProprietorship" || selectedFormOfBusiness == "RegisteredFund")
            {
                // Ask whether to include signature
                var templateChoice = MessageBox.Show("Generate document with signature?",
                    "Template Selection",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                selectedTemplate = templateChoice == DialogResult.Yes
                    ? Sole1 // with signature
                    : Sole2; // without signature
            }

            else if (selectedFormOfBusiness == "StockCorporation" ||
                     selectedFormOfBusiness == "Publicly-listed" ||
                     selectedFormOfBusiness == "Private" ||
                     selectedFormOfBusiness == "Non-StockCorporation" ||
                     selectedFormOfBusiness == "Foundations" ||
                     selectedFormOfBusiness == "Associations" ||
                     selectedFormOfBusiness == "ReligousOrganization" ||
                     selectedFormOfBusiness == "Cooperative")
            {
                // Ask whether to include signature
                var templateChoice = MessageBox.Show("Generate document with signature?",
                    "Template Selection",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                selectedTemplate = templateChoice == DialogResult.Yes
                    ? Corporation1 // with signature
                    : Corporation2; // without signature
            }

            else if (selectedFormOfBusiness == "Partnership")
            {
                // Ask whether to include signature
                var templateChoice = MessageBox.Show("Generate document with signature?",
                    "Template Selection",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                selectedTemplate = templateChoice == DialogResult.Yes
                    ? Partnership1 // with signature
                    : Partnership2; // without signature
            }

            else if (selectedTemplate == null)
            {
                MessageBox.Show("No template selected. Please select a valid form of business.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            templateFilePath = Path.Combine(DocFilePath, selectedTemplate);

            // Check if selected template exists
            if (!File.Exists(templateFilePath))
            {
                MessageBox.Show($"Template file '{selectedTemplate}' not found in '{DocFilePath}'.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // CITAS Details
            // Get or generate docNo
            //int docNo = GetOrGenerateDocNo();
            int docNo = 1;
            if (docNo == -1)
            {
                return; // Exit if we couldn't get/generate a valid docNo
            }

            docNobx.Text = docNo.ToString();

            DateTime date;
            if (!DateTime.TryParse(casdatebx.Text, out date))
            {
                date = DateTime.Now;
            }

            string casemployee = casemployeebx.Text;
            string casdesig = casdesigbx.Text;
            string casmobile = casmobilebx.Text;
            string casemail = casemailbx.Text;
            string castitle = castitlebx.Text;
            string terminalmodel = terminalmodelbx.Text;
            string termqty = termqtybx.Text;
            string monthly = monthlybx.Text + " Months";
            string monthlyterminal = monthlyterminalbx.Text;

            int m = 0;
            int t = 0;

            int.TryParse(monthlybx.Text, out m);
            int.TryParse(monthlyterminalbx.Text, out t);

            int advrentalpayment = m * t;

            //BUSINESS Details
            string businessname = businessnamebx.Text;
            string dbaname = dbanamebx.Text;
            string businessaddress = businessaddressbx.Text.Trim();
            string businessbarangay = businessbarangaybx.Text.Trim();
            string businesscity = businesscitybx.Text.Trim();
            string businessprovince = businessprovincebx.Text;
            string mailaddress = mailaddressbx.Text;
            string businesszipcode = businesszipcodebx.Text;
            string fullbusinessaddress = $"{businessaddress}, {businessbarangay}, {businesscity}".Trim(',').Replace(",,", ",");

            DateTime regisdate;
            bool isValidregisdate = DateTime.TryParse(regisdatebx.Text, out regisdate);

            string businesscountry = businesscountrybx.Text;
            string businessNo = businessNobx.Text;
            string businessemail = businessemailbx.Text;
            string mobileNo = mobileNobx.Text;
            string businesstaxid = businesstaxidbx.Text;
            string mdr = mdrbx.Text + "%";
            //string qr = qrbx.Text + "%";


            // MERCHANT Details
            string merchfname = merchfnamebx.Text.Trim();
            string merchmname = merchmnamebx.Text.Trim();
            string merchlname = merchlnamebx.Text.Trim();
            string middleInitial = string.IsNullOrEmpty(merchmname) ? "" : merchmname[0] + ".";
            string merchname = $"{merchfname} {middleInitial} {merchlname}".Trim();

            DateTime merchdob;
            bool isValidmerchdob = DateTime.TryParse(merchdobbx.Text, out merchdob);

            string placeofbirth = placeofbirthbx.Text;
            string merchnationality = merchnationalitybx.Text;
            string merchaddress = merchaddressbx.Text.Trim();
            string merchbarangay = merchbarangaybx.Text.Trim();
            string merchcity = merchcitybx.Text.Trim();
            string fullMerchAddress = $"{merchaddress}, {merchbarangay}, {merchcity}".Trim(',').Replace(",,", ",");
            string merchNo = merchNobx.Text;
            string merchemail = merchemailbx.Text;
            string merchindustry = merchindustrybx.Text;
            //string merchbankname = merchbanknamebx.Text;

            string merchbankname = merchbanknamebx.SelectedItem != null
            ? merchbanknamebx.SelectedItem.ToString()
            : null;

            string merchaccNo = merchaccNobx.Text;
            string merchaccNo1 = merchaccNo1bx.Text;

            // Validate input: Must be exactly 13 numeric digits
            //if (merchaccNo.Length != 13 || !merchaccNo.All(char.IsDigit))
            //{
            //    MessageBox.Show("Please enter a valid 13-digit number.");
            //    return;
            //}

            // Split the input into individual characters
            char[] digits = merchaccNo.ToCharArray();

            // Define the custom spacing between each digit (12 gaps for 13 digits)
            int[] spaces = { 5, 5, 10, 6, 5, 6, 6, 5, 6, 5, 6, 5 };

            // Build the formatted string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < digits.Length; i++)
            {
                sb.Append(digits[i]);

                // Add spaces only if it's not the last digit
                if (i < spaces.Length)
                {
                    sb.Append(' ', spaces[i]); // Append the defined number of spaces
                }
            }

            string formattedNumber = sb.ToString();

            string merchsss = merchsssbx.Text;
            string merchmobile = merchmobilebx.Text;
            string merchdesig = merchdesigbx.Text;
            string merchcontactperson = merchcontactpersonbx.Text;
            string merchtitle = merchtitlebx.Text;
            string merchidtype = merchidtypebx.Text;
            string merchidNo = merchidNobx.Text;

            string merchemployedstat = merchemployedstatbx.SelectedItem != null
            ? merchemployedstatbx.SelectedItem.ToString()
            : null;

            string paymentmode = paymentmodebx.Text;
            string employername = employernamebx.Text;
            string outsideaffiliate = outsideaffiliatebx.Text;
            string sourceoffunds = sourceoffundsbx.Text;
            string fax = faxbx.Text;
            string natureofwork = natureofworkbx.Text;
            string postalcode = postalcodebx.Text;
            //string typeofbusiness = typeofbusinessbx.Text;
            string prodorservices = prodorservicesbx.Text;
            string contactpersonNo = contactpersonNobx.Text;
            string contactpersonemail = contactpersonemailbx.Text;

            string formofbusiness = formofbusinessbx.SelectedItem != null
            ? formofbusinessbx.SelectedItem.ToString()
            : null;

            string beneficial = beneficialbx.SelectedItem != null
            ? beneficialbx.SelectedItem.ToString()
            : null;

            string sourceofwealth = sourceofwealthbx.SelectedItem != null
            ? sourceofwealthbx.SelectedItem.ToString()
            : null;

            string sizeofbusiness = sizeofBbx.SelectedItem != null
            ? sizeofBbx.SelectedItem.ToString()
            : null;

            string typesofproduct = typesofproductbx.SelectedItem != null
            ? typesofproductbx.SelectedItem.ToString()
            : null;

            string USperson = USpersonbx.SelectedItem != null
            ? USpersonbx.SelectedItem.ToString()
            : null;

            string expectedfrequency = expectedfrequencybx.SelectedItem != null
            ? expectedfrequencybx.SelectedItem.ToString()
            : null;

            string averageamount = averageamountbx.SelectedItem != null
            ? averageamountbx.SelectedItem.ToString()
            : null;

            string purposeofacc = purposeofaccbx.SelectedItem != null
            ? purposeofaccbx.SelectedItem.ToString()
            : null;

            string modesofdeposit = modesofdepositbx.SelectedItem != null
            ? modesofdepositbx.SelectedItem.ToString()
            : null;

            string modeofdelivery = modeofdeliverybx.SelectedItem != null
            ? modeofdeliverybx.SelectedItem.ToString()
            : null;

            string secondarylicense = secondarylicensebx.SelectedItem != null
            ? secondarylicensebx.SelectedItem.ToString()
            : null;

            string publiclist = publiclistbx.SelectedItem != null
            ? publiclistbx.SelectedItem.ToString()
            : null;

            string familylist = familylistbx.SelectedItem != null
            ? familylistbx.SelectedItem.ToString()
            : null;

            string PEPlist = PEPlistbx.SelectedItem != null
            ? PEPlistbx.SelectedItem.ToString()
            : null;

            string branchref = branchrefbx.SelectedItem != null
            ? branchrefbx.SelectedItem.ToString()
            : null;

            string reqtype = reqtypebx.SelectedItem != null
            ? reqtypebx.SelectedItem.ToString()
            : null;

            string paymentgroup = paymentgroupbx.SelectedItem != null
            ? paymentgroupbx.SelectedItem.ToString()
            : null;

            string POS = POSbx.SelectedItem != null
            ? POSbx.SelectedItem.ToString()
            : null;

            string installmentrates = installmentratesbx.SelectedItem != null
            ? installmentratesbx.SelectedItem.ToString()
            : null;

            string chequeaddress = chequeaddressbx.Text;
            string chequezipcode = chequezipcodebx.Text;

            string sig1fullname = sig1fullnamebx.Text;
            string sig1dob = sig1dobbx.Text;
            string sig1placeofbirth = sig1placeofbirthbx.Text;
            string sig1nationality = sig1nationalitybx.Text;
            string sig1desig = sig1desigbx.Text;
            string sig1address = sig1addressbx.Text;
            string sig1sss = sig1sssbx.Text;
            string sig1No = sig1Nobx.Text;
            string sig1email = sig1emailbx.Text;
            string sig1industry = sig1industrybx.Text;

            string sig1employedstat = sig1employedstatbx.SelectedItem != null
            ? sig1employedstatbx.SelectedItem.ToString()
            : null;

            string sig1natureofwork = sig1natureofworkbx.Text;
            string sig1employername = sig1employernamebx.Text;
            string sig1outsideaffiliate = sig1outsideaffiliatebx.Text;
            string sig2fullname = sig2fullnamebx.Text;
            string sig2dob = sig2dobbx.Text;
            string sig2placeofbirth = sig2placeofbirthbx.Text;
            string sig2nationality = sig2nationalitybx.Text;
            string sig2desig = sig2desigbx.Text;
            string sig2address = sig2addressbx.Text;
            string sig2sss = sig2sssbx.Text;
            string sig2No = sig2Nobx.Text;
            string sig2email = sig2emailbx.Text;
            string sig2industry = sig2industrybx.Text;

            string sig2employedstat = sig2employedstatbx.SelectedItem != null
            ? sig2employedstatbx.SelectedItem.ToString()
            : null;

            string sig2natureofwork = sig2natureofworkbx.Text;
            string sig2employername = sig2employernamebx.Text;
            string sig2outsideaffiliate = sig2outsideaffiliatebx.Text;
            string man1fullname = man1fullnamebx.Text;
            string man1dob = man1dobbx.Text;
            string man1placeofbirth = man1placeofbirthbx.Text;
            string man1nationality = man1nationalitybx.Text;
            string man1desig = man1desigbx.Text;
            string man1address = man1addressbx.Text;
            string man1sss = man1sssbx.Text;
            string man1No = man1Nobx.Text;

            string man1employedstat = man1employedstatbx.SelectedItem != null
            ? man1employedstatbx.SelectedItem.ToString()
            : null;

            string man1natureofbusiness = man1natureofbusinessbx.Text;
            string man1natureofwork = man1natureofworkbx.Text;
            string man1employername = man1employernamebx.Text;
            string man1outsideaffiliate = man1outsideaffiliatebx.Text;
            string man1sourceoffunds = man1sourceoffundsbx.Text;
            string man2fullname = man2fullnamebx.Text;
            string man2dob = man2dobbx.Text;
            string man2placeofbirth = man2placeofbirthbx.Text;
            string man2nationality = man2nationalitybx.Text;
            string man2desig = man2desigbx.Text;
            string man2address = man2addressbx.Text;
            string man2sss = man2sssbx.Text;
            string man2No = man2Nobx.Text;

            string man2employedstat = man2employedstatbx.SelectedItem != null
            ? man2employedstatbx.SelectedItem.ToString()
            : null;

            string man2natureofbusiness = man2natureofbusinessbx.Text;
            string man2natureofwork = man2natureofworkbx.Text;
            string man2employername = man2employernamebx.Text;
            string man2outsideaffiliate = man2outsideaffiliatebx.Text;
            string man2sourceoffunds = man2sourceoffundsbx.Text;
            string secname = secnamebx.Text;
            string secage = secagebx.Text;
            string secnationality = secnationalitybx.Text;
            string secaddress = secaddressbx.Text;
            string spaname = spanamebx.Text;
            string spaID = spaIDbx.Text;
            string spaIDno = spaIDnobx.Text;

            DateTime datesub;
            bool isValiddatesub = DateTime.TryParse(datesubbx.Text, out datesub);


            SaveFileDialog Sfd = new SaveFileDialog();
            Sfd.Filter = "Word Files (*.docx)|*.docx";
            Sfd.FileName = $"{DateTime.Now:yyyy-MM-dd}-{businessname}-Client_Output"; // Default file name
            Sfd.InitialDirectory = dbFile.sExportPath; // Default save location

            // Prompt the user to choose a save location
            if (Sfd.ShowDialog() != DialogResult.OK) return;

            string outputFilePath = Sfd.FileName;

            // Save to database first
            bool isSavedToDB = SaveToDatabase(
                docNo, date, casemployee, casdesig, casmobile, casemail, castitle, terminalmodel, termqty, monthly, monthlyterminal, advrentalpayment,
                businessname, dbaname, businessaddress, businessbarangay, businesscity, businessprovince, mailaddress, businesszipcode, fullbusinessaddress, regisdate, businesscountry, businessNo,
                businessemail, mobileNo, businesstaxid, mdr,
                merchfname, merchmname, merchlname, merchname, merchdob, placeofbirth, merchnationality, merchaddress, merchbarangay, merchcity, fullMerchAddress, merchNo,
                merchemail, merchindustry, merchbankname, merchaccNo, merchaccNo1, formattedNumber, merchsss, merchmobile, merchdesig, merchcontactperson,
                merchtitle, merchidtype, merchidNo, merchemployedstat, paymentmode, employername, outsideaffiliate, sourceoffunds,
                fax, postalcode, natureofwork, prodorservices, contactpersonNo, contactpersonemail, formofbusiness, beneficial, sourceofwealth, sizeofbusiness, typesofproduct,
                USperson, expectedfrequency, averageamount, chequeaddress, chequezipcode, purposeofacc, modesofdeposit, modeofdelivery, secondarylicense, publiclist, familylist, PEPlist, branchref, reqtype, paymentgroup, POS, installmentrates,
                sig1fullname, sig1dob, sig1placeofbirth, sig1nationality, sig1desig, sig1address, sig1sss, sig1No, sig1email, sig1industry, sig1employedstat, sig1natureofwork, sig1employername,
                sig1outsideaffiliate, sig2fullname, sig2dob, sig2placeofbirth, sig2nationality, sig2desig, sig2address, sig2sss, sig2No, sig2email, sig2industry, sig2employedstat, sig2natureofwork,
                sig2employername, sig2outsideaffiliate, man1fullname, man1dob, man1placeofbirth, man1nationality, man1desig, man1address, man1sss, man1No, man1employedstat, man1natureofbusiness, man1natureofwork,
                man1employername, man1outsideaffiliate, man1sourceoffunds, man2fullname, man2dob, man2placeofbirth, man2nationality, man2desig, man2address, man2sss, man2No, man2employedstat, man2natureofbusiness,
                man2natureofwork, man2employername, man2outsideaffiliate, man2sourceoffunds, secname, secage, secnationality, secaddress, spaname, spaID, spaIDno,
                datesub);
            if (!isSavedToDB)
            {
                MessageBox.Show("Failed to save data. The document was not updated or created.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Generate and show the document
            PopulateWordDocument(
            docNo, date, casemployee, casdesig, casmobile, casemail, castitle, terminalmodel, termqty, monthly, monthlyterminal, advrentalpayment,
            businessname, dbaname, businessaddress, businessbarangay, businesscity, businessprovince, mailaddress, businesszipcode, fullbusinessaddress, regisdate, businesscountry, businessNo,
            businessemail, mobileNo, businesstaxid, mdr,
            merchfname, merchmname, merchlname, merchname, merchdob, placeofbirth, merchnationality, merchaddress, merchbarangay, merchcity, fullMerchAddress, merchNo,
            merchemail, merchindustry, merchbankname, merchaccNo, merchaccNo1, formattedNumber, merchsss, merchmobile, merchdesig, merchcontactperson,
            merchtitle, merchidtype, merchidNo, merchemployedstat, paymentmode, employername, outsideaffiliate, sourceoffunds,
            fax, postalcode, natureofwork, prodorservices, contactpersonNo, contactpersonemail, formofbusiness, beneficial, sourceofwealth, sizeofbusiness, typesofproduct,
            USperson, expectedfrequency, averageamount, chequeaddress, chequezipcode, purposeofacc, modesofdeposit, modeofdelivery, secondarylicense, publiclist, familylist, PEPlist, branchref, reqtype, paymentgroup, POS, installmentrates,
            sig1fullname, sig1dob, sig1placeofbirth, sig1nationality, sig1desig, sig1address, sig1sss, sig1No, sig1email, sig1industry, sig1employedstat, sig1natureofwork, sig1employername,
            sig1outsideaffiliate, sig2fullname, sig2dob, sig2placeofbirth, sig2nationality, sig2desig, sig2address, sig2sss, sig2No, sig2email, sig2industry, sig2employedstat, sig2natureofwork,
            sig2employername, sig2outsideaffiliate, man1fullname, man1dob, man1placeofbirth, man1nationality, man1desig, man1address, man1sss, man1No, man1employedstat, man1natureofbusiness, man1natureofwork,
            man1employername, man1outsideaffiliate, man1sourceoffunds, man2fullname, man2dob, man2placeofbirth, man2nationality, man2desig, man2address, man2sss, man2No, man2employedstat, man2natureofbusiness,
            man2natureofwork, man2employername, man2outsideaffiliate, man2sourceoffunds, secname, secage, secnationality, secaddress, spaname, spaID, spaIDno,
            datesub,
                templateFilePath, outputFilePath);

            MessageBox.Show($"The file has been saved to {outputFilePath}.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            OpenWordDocument(outputFilePath);
            ShowWordPrintPreview(outputFilePath);

            btnClear_Click(this, e);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            fEdit = false;
            InitButton();

            dbFunction.ClearTextBox(this);
            dbFunction.TextBoxUnLock(false, this);
            dbFunction.ComBoBoxUnLock(false, this);
            dbFunction.DatePickerUnlock(false, this);

            Cursor.Current = Cursors.Default;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {

        }

        private bool SaveToDatabase(
            //CITAS Details
            int docNo,
            DateTime date,
            string casemployee,
            string casdesig,
            string casmobile,
            string casemail,
            string castitle,
            string terminalmodel,
            string termqty,
            string monthly,
            string monthlyterminal,
            int advrentalpayment,

            //BUSINESS Details
            string businessname,
            string dbaname,
            string businessaddress,
            string businessbarangay,
            string businesscity,
            string businessprovince,
            string mailaddress,
            string businesszipcode,
            string fullbusinessaddress,
            DateTime regisdate,
            string businesscountry,
            string businessNo,
            string businessemail,
            string mobileNo,
            string businesstaxid,
            string mdr,

            //MERCHANT Details
            string merchfname,
            string merchmname,
            string merchlname,
            string merchname,
            DateTime merchdob,
            string placeofbirth,
            string merchnationality,
            string merchaddress,
            string merchbarangay,
            string merchcity,
            string fullMerchAddress,
            string merchNo,
            string merchemail,
            string merchindustry,
            string merchbankname,
            string merchaccNo,
            string merchaccNo1,
            string formattedNumber,
            string merchsss,
            string merchmobile,
            string merchdesig,
            string merchcontactperson,
            string merchtitle,
            string merchidtype,
            string merchidNo,
            string merchemployedstat,
            string paymentmode,
            string employername,
            string outsideaffiliate,
            string sourceoffunds,
            string fax,
            string postalcode,
            string natureofwork,
            string prodorservices,
            string contactpersonNo,
            string contactpersonemail,
            string formofbusiness,
            string beneficial,
            string sourceofwealth,
            string sizeofbusiness,
            string typesofproduct,
            string USperson,
            string expectedfrequency,
            string averageamount,
            string chequeaddress,
            string chequezipcode,
            string purposeofacc,
            string modesofdeposit,
            string modeofdelivery,
            string secondarylicense,
            string publiclist,
            string familylist,
            string PEPlist,
            string branchref,
            string reqtype,
            string paymentgroup,
            string POS,
            string installmentrates,

            string sig1fullname,
            string sig1dob,
            string sig1placeofbirth,
            string sig1nationality,
            string sig1desig,
            string sig1address,
            string sig1sss,
            string sig1No,
            string sig1email,
            string sig1industry,
            string sig1employedstat,
            string sig1natureofwork,
            string sig1employername,
            string sig1outsideaffiliate,
            string sig2fullname,
            string sig2dob,
            string sig2placeofbirth,
            string sig2nationality,
            string sig2desig,
            string sig2address,
            string sig2sss,
            string sig2No,
            string sig2email,
            string sig2industry,
            string sig2employedstat,
            string sig2natureofwork,
            string sig2employername,
            string sig2outsideaffiliate,
            string man1fullname,
            string man1dob,
            string man1placeofbirth,
            string man1nationality,
            string man1desig,
            string man1address,
            string man1sss,
            string man1No,
            string man1employedstat,
            string man1natureofbusiness,
            string man1natureofwork,
            string man1employername,
            string man1outsideaffiliate,
            string man1sourceoffunds,
            string man2fullname,
            string man2dob,
            string man2placeofbirth,
            string man2nationality,
            string man2desig,
            string man2address,
            string man2sss,
            string man2No,
            string man2employedstat,
            string man2natureofbusiness,
            string man2natureofwork,
            string man2employername,
            string man2outsideaffiliate,
            string man2sourceoffunds,
            string secname,
            string secage,
            string secnationality,
            string secaddress,
            string spaname,
            string spaID,
            string spaIDno,
            DateTime datesub
            )
        {
            string sSQL = "";

            try
            {
                var data = new
                {
                    docNo, date, casemployee, casdesig, casmobile, casemail, castitle, terminalmodel, termqty, monthly, monthlyterminal, advrentalpayment,
                    businessname, dbaname, businessaddress, businessbarangay, businesscity, businessprovince, mailaddress, businesszipcode, fullbusinessaddress, regisdate, businesscountry, businessNo,
                    businessemail, mobileNo, businesstaxid, mdr, merchfname, merchmname, merchlname, merchname, merchdob, placeofbirth, merchnationality, merchaddress,
                    merchbarangay, merchcity, fullMerchAddress, merchNo, merchemail, merchindustry, merchbankname, merchaccNo, merchaccNo1, formattedNumber, merchsss, merchmobile,
                    merchdesig, merchcontactperson, merchtitle, merchidtype, merchidNo, merchemployedstat, paymentmode, employername, outsideaffiliate, sourceoffunds, fax, postalcode,
                    natureofwork, prodorservices, contactpersonNo, contactpersonemail, formofbusiness, beneficial, sourceofwealth, sizeofbusiness, typesofproduct, USperson, expectedfrequency, averageamount,
                    chequeaddress, chequezipcode, purposeofacc, modesofdeposit, modeofdelivery, secondarylicense, publiclist, familylist, PEPlist, branchref, reqtype, paymentgroup,
                    POS, installmentrates, sig1fullname, sig1dob, sig1placeofbirth, sig1nationality, sig1desig, sig1address, sig1sss, sig1No, sig1email, sig1industry,
                    sig1employedstat, sig1natureofwork, sig1employername, sig1outsideaffiliate, sig2fullname, sig2dob, sig2placeofbirth, sig2nationality, sig2desig, sig2address, sig2sss, sig2No,
                    sig2email, sig2industry, sig2employedstat, sig2natureofwork, sig2employername, sig2outsideaffiliate, man1fullname, man1dob, man1placeofbirth, man1nationality, man1desig, man1address,
                    man1sss, man1No, man1employedstat, man1natureofbusiness, man1natureofwork, man1employername, man1outsideaffiliate, man1sourceoffunds, man2fullname, man2dob, man2placeofbirth, man2nationality,
                    man2desig, man2address, man2sss, man2No, man2employedstat, man2natureofbusiness, man2natureofwork, man2employername, man2outsideaffiliate, man2sourceoffunds, secname, secage,
                    secnationality, secaddress, spaname, spaID, spaIDno, datesub
                };
                
                sSQL = IFormat.Insert(data);

                dbFunction.parseDelimitedString(sSQL, clsDefines.gComma, 0);

                dbAPI.ExecuteAPI("POST", "Insert", "", "", "MSP-Merchant Registration", sSQL, "InsertCollectionMaster");

                clsSearch.ClassLastInsertedID = clsLastID.ClassLastInsertedID;
                
                // Success message after inserting into both tables
                MessageBox.Show("Data successfully saved to the database.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return true; // Return true if both inserts succeed
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving to database: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false; // Return false if error occurs
            }
        }

        private void PopulateWordDocument(
            // CITAS Details
            int docNo,
            DateTime date,
            string casemployee,
            string casdesig,
            string casmobile,
            string casemail,
            string castitle,
            string terminalmodel,
            string termqty,
            string monthly,
            string monthlyterminal,
            int advrentalpayment,

            //Business Details
            string businessname,
            string dbaname,
            string businessaddress,
            string businessbarangay,
            string businesscity,
            string businessprovince,
            string mailaddress,
            string businesszipcode,
            string fullbusinessaddress,
            DateTime regisdate,
            string businesscountry,
            string businessNo,
            string businessemail,
            string mobileNo,
            string businesstaxid,
            string mdr,


            // MERCHANT Details
            string merchfname,
            string merchmname,
            string merchlname,
            string merchname,
            DateTime merchdob,
            string placeofbirth,
            string merchnationality,
            string merchaddress,
            string merchbarangay,
            string merchcity,
            string fullMerchAddress,
            string merchNo,
            string merchemail,
            string merchindustry,
            string merchbankname,
            string merchaccNo,
            string merchaccNo1,
            string formattedNumber,
            string merchsss,
            string merchmobile,
            string merchdesig,
            string merchcontactperson,
            string merchtitle,
            string merchidtype,
            string merchidNo,
            string merchemployedstat,
            string paymentmode,
            string employername,
            string outsideaffiliate,
            string sourceoffunds,
            string fax,
            string postalcode,
            string natureofwork,
            string prodorservices,
            string contactpersonNo,
            string contactpersonemail,
            string formofbusiness,
            string beneficial,
            string sourceofwealth,
            string sizeofbusiness,
            string typesofproduct,
            string USperson,
            string expectedfrequency,
            string averageamount,
            string chequeaddress,
            string chequezipcode,
            string purposeofacc,
            string modesofdeposit,
            string modeofdelivery,
            string secondarylicense,
            string publiclist,
            string familylist,
            string PEPlist,
            string branchref,
            string reqtype,
            string paymentgroup,
            string POS,
            string installmentrates,


            string sig1fullname,
            string sig1dob,
            string sig1placeofbirth,
            string sig1nationality,
            string sig1desig,
            string sig1address,
            string sig1sss,
            string sig1No,
            string sig1email,
            string sig1industry,
            string sig1employedstat,
            string sig1natureofwork,
            string sig1employername,
            string sig1outsideaffiliate,
            string sig2fullname,
            string sig2dob,
            string sig2placeofbirth,
            string sig2nationality,
            string sig2desig,
            string sig2address,
            string sig2sss,
            string sig2No,
            string sig2email,
            string sig2industry,
            string sig2employedstat,
            string sig2natureofwork,
            string sig2employername,
            string sig2outsideaffiliate,
            string man1fullname,
            string man1dob,
            string man1placeofbirth,
            string man1nationality,
            string man1desig,
            string man1address,
            string man1sss,
            string man1No,
            string man1employedstat,
            string man1natureofbusiness,
            string man1natureofwork,
            string man1employername,
            string man1outsideaffiliate,
            string man1sourceoffunds,
            string man2fullname,
            string man2dob,
            string man2placeofbirth,
            string man2nationality,
            string man2desig,
            string man2address,
            string man2sss,
            string man2No,
            string man2employedstat,
            string man2natureofbusiness,
            string man2natureofwork,
            string man2employername,
            string man2outsideaffiliate,
            string man2sourceoffunds,
            string secname,
            string secage,
            string secnationality,
            string secaddress,
            string spaname,
            string spaID,
            string spaIDno,
            DateTime datesub,

            string inputFilePath,
            string outputFilePath)
        {
            // Use DocX to populate the Word document
            using (DocX document = DocX.Load(inputFilePath))
            {
                // CITAS Details 
                document.ReplaceText("[docNo]", docNo.ToString());
                document.ReplaceText("[date]", DateTime.Now.ToString("dd-MM-yyyy hh:mm tt"));
                document.ReplaceText("[casemployee]", casemployee);
                document.ReplaceText("[casdesig]", casdesig);
                document.ReplaceText("[casmobile]", casmobile);
                document.ReplaceText("[casemail]", casemail);
                document.ReplaceText("[castitle]", castitle);
                document.ReplaceText("[terminalmodel]", terminalmodel);
                document.ReplaceText("[termqty]", termqty);
                document.ReplaceText("[monthly]", monthly);
                document.ReplaceText("[monthlyterminal]", monthlyterminal);
                document.ReplaceText("[advrentalpayment]", advrentalpayment.ToString());

                //BUSINESS Details
                document.ReplaceText("[businessname]", businessname);
                document.ReplaceText("[dbaname]", dbaname);
                document.ReplaceText("[businessaddress]", businessaddress);
                document.ReplaceText("[businessbarangay]", businessbarangay);
                document.ReplaceText("[businesscity]", businesscity);
                document.ReplaceText("[businessprovince]", businessprovince);
                document.ReplaceText("[businesszipcode]", businesszipcode);
                document.ReplaceText("[mailaddress]", mailaddress);
                document.ReplaceText("[fullbusinessaddress]", fullbusinessaddress);
                document.ReplaceText("[regisdate]", regisdate.ToString("dd-MM-yyyy"));
                document.ReplaceText("[businesscountry]", businesscountry);
                document.ReplaceText("[businessNo]", businessNo);
                document.ReplaceText("[businessemail]", businessemail);
                document.ReplaceText("[mobileNo]", mobileNo);
                document.ReplaceText("[businesstaxid]", businesstaxid);
                document.ReplaceText("[mdr]", mdr);

                // MERCHANT Details 
                document.ReplaceText("[merchname]", merchname);
                document.ReplaceText("[merchfname]", merchfname);
                document.ReplaceText("[merchmname]", merchmname);
                document.ReplaceText("[merchlname]", merchlname);
                document.ReplaceText("[merchdob]", merchdob.ToString("MM-dd-yyyy"));
                document.ReplaceText("[placeofbirth]", placeofbirth);
                document.ReplaceText("[merchnationality]", merchnationality);
                document.ReplaceText("[merchaddress]", merchaddress);
                document.ReplaceText("[merchbarangay]", merchbarangay);
                document.ReplaceText("[merchcity]", merchcity);
                document.ReplaceText("[fullMerchAddress]", fullMerchAddress);
                document.ReplaceText("[merchNo]", merchNo);
                document.ReplaceText("[merchemail]", merchemail);
                document.ReplaceText("[merchindustry]", merchindustry);

                string metrobank = "Metrobank";
                string bdo = "Banco de Oro";
                string bdouni = "Banco de Oro Unibank";
                string bpi = "Bank of the Philippines Islands";
                string rcbc = "Rizal Commercial Banking Corporation";
                string pnb = "Philippine National Bank";
                string eastwest = "East West Bank";
                string security = "Security Bank";
                string land = "Land Bank";
                string union = "Union Bank";
                string psb = "Philippine Savings Bank";
                string robinson = "Robinsons Bank";
                string citystate = "Citystate Bank";
                string maybank = "Maybank";

                switch (merchbankname)
                {
                    case "Metro Bank":
                        document.ReplaceText("[metrobank]", metrobank);
                        break;
                    case "BDO":
                        merchaccNobx.Enabled = false;
                        document.ReplaceText("[otherbank]", bdo);
                        break;
                    case "BDO Unibank":
                        merchaccNobx.Enabled = false;
                        document.ReplaceText("[otherbank]", bdouni);
                        break;
                    case "BPI":
                        merchaccNobx.Enabled = false;
                        document.ReplaceText("[otherbank]", bpi);
                        break;
                    case "RCBC":
                        merchaccNobx.Enabled = false;
                        document.ReplaceText("[otherbank]", rcbc);
                        break;
                    case "PNB":
                        merchaccNobx.Enabled = false;
                        document.ReplaceText("[otherbank]", pnb);
                        break;
                    case "East West Bank":
                        merchaccNobx.Enabled = false;
                        document.ReplaceText("[otherbank]", eastwest);
                        break;
                    case "Security Bank":
                        merchaccNobx.Enabled = false;
                        document.ReplaceText("[otherbank]", security);
                        break;
                    case "Land Bank":
                        merchaccNobx.Enabled = false;
                        document.ReplaceText("[otherbank]", land);
                        break;
                    case "Union Bank":
                        merchaccNobx.Enabled = false;
                        document.ReplaceText("[otherbank]", union);
                        break;
                    case "Philippine Savings Bank":
                        merchaccNobx.Enabled = false;
                        document.ReplaceText("[otherbank]", psb);
                        break;
                    case "Robinsons Bank":
                        merchaccNobx.Enabled = false;
                        document.ReplaceText("[otherbank]", robinson);
                        break;
                    case "Citystate Bank":
                        merchaccNobx.Enabled = false;
                        document.ReplaceText("[otherbank]", citystate);
                        break;
                    case "Maybank":
                        merchaccNobx.Enabled = false;
                        document.ReplaceText("[otherbank]", maybank);
                        break;
                    default:
                        break;
                }

                document.ReplaceText("[metrobank]", "");
                document.ReplaceText("[otherbank]", "");

                document.ReplaceText("[merchaccNo]", merchaccNo);
                document.ReplaceText("[merchaccNo1]", merchaccNo1);
                document.ReplaceText("[formattedNumber]", formattedNumber);
                document.ReplaceText("[merchsss]", merchsss);
                document.ReplaceText("[merchmobile]", merchmobile);
                document.ReplaceText("[merchdesig]", merchdesig);
                document.ReplaceText("[merchcontactperson]", merchcontactperson);
                document.ReplaceText("[merchtitle]", merchtitle);
                document.ReplaceText("[merchidtype]", merchidtype);
                document.ReplaceText("[merchidNo]", merchidNo);
                document.ReplaceText("[paymentmode]", paymentmode);
                document.ReplaceText("[employername]", employername);
                document.ReplaceText("[outsideaffiliate]", outsideaffiliate);
                document.ReplaceText("[sourceoffunds]", sourceoffunds);
                document.ReplaceText("[fax]", fax);
                document.ReplaceText("[postalcode]", postalcode);
                document.ReplaceText("[natureofwork]", natureofwork);
                document.ReplaceText("[prodorservices]", prodorservices);
                document.ReplaceText("[contactpersonNo]", contactpersonNo);
                document.ReplaceText("[contactpersonemail]", contactpersonemail);
                document.ReplaceText("[merchid]", "");
                document.ReplaceText("[chequeaddress]", chequeaddress);
                document.ReplaceText("[chequezipcode]", chequezipcode);

                document.ReplaceText("[sig1fullname]", sig1fullname);
                document.ReplaceText("[sig1dob]", sig1dob);
                document.ReplaceText("[sig1placeofbirth]", sig1placeofbirth);
                document.ReplaceText("[sig1nationality]", sig1nationality);
                document.ReplaceText("[sig1desig]", sig1desig);
                document.ReplaceText("[sig1address]", sig1address);
                document.ReplaceText("[sig1sss]", sig1sss);
                document.ReplaceText("[sig1No]", sig1No);
                document.ReplaceText("[sig1email]", sig1email);
                document.ReplaceText("[sig1industry]", sig1industry);
                document.ReplaceText("[sig1natureofwork]", sig1natureofwork);
                document.ReplaceText("[sig1employername]", sig1employername);
                document.ReplaceText("[sig1outsideaffiliate]", sig1outsideaffiliate);
                document.ReplaceText("[sig2fullname]", sig2fullname);
                document.ReplaceText("[sig2dob]", sig2dob);
                document.ReplaceText("[sig2placeofbirth]", sig2placeofbirth);
                document.ReplaceText("[sig2nationality]", sig2nationality);
                document.ReplaceText("[sig2desig]", sig2desig);
                document.ReplaceText("[sig2address]", sig2address);
                document.ReplaceText("[sig2sss]", sig2sss);
                document.ReplaceText("[sig2No]", sig2No);
                document.ReplaceText("[sig2email]", sig2email);
                document.ReplaceText("[sig2industry]", sig2industry);
                document.ReplaceText("[sig2natureofwork]", sig2natureofwork);
                document.ReplaceText("[sig2employername]", sig2employername);
                document.ReplaceText("[sig2outsideaffiliate]", sig2outsideaffiliate);
                document.ReplaceText("[man1fullname]", man1fullname);
                document.ReplaceText("[man1dob]", man1dob);
                document.ReplaceText("[man1placeofbirth]", man1placeofbirth);
                document.ReplaceText("[man1nationality]", man1nationality);
                document.ReplaceText("[man1desig]", man1desig);
                document.ReplaceText("[man1address]", man1address);
                document.ReplaceText("[man1sss]", man1sss);
                document.ReplaceText("[man1No]", man1No);
                document.ReplaceText("[man1natureofbusiness]", man1natureofbusiness);
                document.ReplaceText("[man1natureofwork]", man1natureofwork);
                document.ReplaceText("[man1employername]", man1employername);
                document.ReplaceText("[man1outsideaffiliate]", man1outsideaffiliate);
                document.ReplaceText("[man1sourceoffunds]", man1sourceoffunds);
                document.ReplaceText("[man2fullname]", man2fullname);
                document.ReplaceText("[man2dob]", man2dob);
                document.ReplaceText("[man2placeofbirth]", man2placeofbirth);
                document.ReplaceText("[man2nationality]", man2nationality);
                document.ReplaceText("[man2desig]", man2desig);
                document.ReplaceText("[man2address]", man2address);
                document.ReplaceText("[man2sss]", man2sss);
                document.ReplaceText("[man2No]", man2No);
                document.ReplaceText("[man2natureofbusiness]", man2natureofbusiness);
                document.ReplaceText("[man2natureofwork]", man2natureofwork);
                document.ReplaceText("[man2employername]", man2employername);
                document.ReplaceText("[man2outsideaffiliate]", man2outsideaffiliate);
                document.ReplaceText("[man2sourceoffunds]", man2sourceoffunds);
                document.ReplaceText("[secname]", secname);
                document.ReplaceText("[secage]", secage);
                document.ReplaceText("[secnationality]", secnationality);
                document.ReplaceText("[secaddress]", secaddress);
                document.ReplaceText("[spaname]", spaname);
                document.ReplaceText("[spaID]", spaID);
                document.ReplaceText("[spaIDno]", spaIDno);


                string checkmark = "✓";

                // Set checkmark for the selected business type
                switch (formofbusiness)
                {
                    case "SoleProprietorship":
                        document.ReplaceText("[SoleProprietorship]", checkmark);
                        break;
                    case "StockCorporation":
                        document.ReplaceText("[StockCorporation]", checkmark);
                        break;
                    case "Publicly-listed":
                        document.ReplaceText("[Publicly-listed]", checkmark);
                        break;
                    case "Private":
                        document.ReplaceText("[Private]", checkmark);
                        break;
                    case "Non-StockCorporation":
                        document.ReplaceText("[Non-StockCorporation]", checkmark);
                        break;
                    case "Foundations":
                        document.ReplaceText("[Foundations]", checkmark);
                        break;
                    case "Associations":
                        document.ReplaceText("[Associations]", checkmark);
                        break;
                    case "ReligousOrganization":
                        document.ReplaceText("[ReligousOrganization]", checkmark);
                        break;
                    case "Partnership":
                        document.ReplaceText("[Partnership]", checkmark);
                        break;
                    case "Cooperative":
                        document.ReplaceText("[Cooperative]", checkmark);
                        break;
                    case "RegisteredFund":
                        document.ReplaceText("[RegisteredFund]", checkmark);
                        break;
                    default:

                        break;
                }

                // Clear all business type checkmarks first
                document.ReplaceText("[SoleProprietorship]", "");
                document.ReplaceText("[StockCorporation]", "");
                document.ReplaceText("[Publicly-listed]", "");
                document.ReplaceText("[Private]", "");
                document.ReplaceText("[Non-StockCorporation]", "");
                document.ReplaceText("[Foundations]", "");
                document.ReplaceText("[Associations]", "");
                document.ReplaceText("[ReligousOrganization]", "");
                document.ReplaceText("[Partnership]", "");
                document.ReplaceText("[Cooperative]", "");
                document.ReplaceText("[RegisteredFund]", "");
                document.ReplaceText("[formofbusiness]", formofbusiness);


                switch (beneficial)
                {
                    case "Yes":
                        document.ReplaceText("[Yes]", checkmark);
                        break;
                    case "None":
                        document.ReplaceText("[None]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[Yes]", "");
                document.ReplaceText("[None]", "");


                switch (sourceofwealth)
                {
                    case "Business":
                        document.ReplaceText("[Business]", checkmark);
                        break;
                    case "Donation":
                        document.ReplaceText("[Donation]", checkmark);
                        break;
                    case "Investment":
                        document.ReplaceText("[Investment]", checkmark);
                        break;
                    case "SalesofAssets":
                        document.ReplaceText("[SalesofAssets]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[Business]", "");
                document.ReplaceText("[Donation]", "");
                document.ReplaceText("[Investment]", "");
                document.ReplaceText("[SalesofAssets]", "");


                switch (sizeofbusiness)
                {
                    case "Micro":
                        document.ReplaceText("[Micro]", checkmark);
                        break;
                    case "Small":
                        document.ReplaceText("[Small]", checkmark);
                        break;
                    case "Medium":
                        document.ReplaceText("[Medium]", checkmark);
                        break;
                    case "Large":
                        document.ReplaceText("[Large]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[Micro]", "");
                document.ReplaceText("[Small]", "");
                document.ReplaceText("[Medium]", "");
                document.ReplaceText("[Large]", "");


                switch (typesofproduct)
                {
                    case "SavingsAccount":
                        document.ReplaceText("[SavingsAccount]", checkmark);
                        break;
                    case "PrepaidCard":
                        document.ReplaceText("[PrepaidCard]", checkmark);
                        break;
                    case "Payroll":
                        document.ReplaceText("[Payroll]", checkmark);
                        break;
                    case "PrivateBanking":
                        document.ReplaceText("[PrivateBanking]", checkmark);
                        break;
                    case "SafetyDeposit":
                        document.ReplaceText("[SafetyDeposit]", checkmark);
                        break;
                    case "Treasury":
                        document.ReplaceText("[Treasury]", checkmark);
                        break;
                    case "TimeDeposit":
                        document.ReplaceText("[TimeDeposit]", checkmark);
                        break;
                    case "CommercialLoans":
                        document.ReplaceText("[CommercialLoans]", checkmark);
                        break;
                    case "CreditCard":
                        document.ReplaceText("[CreditCard]", checkmark);
                        break;
                    case "CashManagement":
                        document.ReplaceText("[CashManagement]", checkmark);
                        break;
                    case "Trust":
                        document.ReplaceText("[Trust]", checkmark);
                        break;
                    case "PersonalManagement":
                        document.ReplaceText("[PersonalManagement]", checkmark);
                        break;
                    case "AssetManagement":
                        document.ReplaceText("[AssetManagement]", checkmark);
                        break;
                    case "FiduciaryServices":
                        document.ReplaceText("[FiduciaryServices]", checkmark);
                        break;
                    case "Remittance":
                        document.ReplaceText("[Remittance]", checkmark);
                        break;
                    default:

                        break;
                }

                // Clear all product type checkmarks first
                document.ReplaceText("[SavingsAccount]", "");
                document.ReplaceText("[PrepaidCard]", "");
                document.ReplaceText("[Payroll]", "");
                document.ReplaceText("[PrivateBanking]", "");
                document.ReplaceText("[SafetyDeposit]", "");
                document.ReplaceText("[Treasury]", "");
                document.ReplaceText("[TimeDeposit]", "");
                document.ReplaceText("[CommercialLoans]", "");
                document.ReplaceText("[CreditCard]", "");
                document.ReplaceText("[CashManagement]", "");
                document.ReplaceText("[Trust]", "");
                document.ReplaceText("[PersonalManagement]", "");
                document.ReplaceText("[AssetManagement]", "");
                document.ReplaceText("[FiduciaryServices]", "");
                document.ReplaceText("[Remittance]", "");


                switch (USperson)
                {
                    case "Yes":
                        document.ReplaceText("[Yess]", checkmark);
                        break;
                    case "No":
                        document.ReplaceText("[No]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[Yess]", "");
                document.ReplaceText("[No]", "");


                switch (expectedfrequency)
                {
                    case "Five":
                        document.ReplaceText("[Five]", checkmark);
                        break;
                    case "Six":
                        document.ReplaceText("[Six]", checkmark);
                        break;
                    case "Ten":
                        document.ReplaceText("[Ten]", checkmark);
                        break;
                    case "morethan15":
                        document.ReplaceText("[morethan15]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[Five]", "");
                document.ReplaceText("[Six]", "");
                document.ReplaceText("[Ten]", "");
                document.ReplaceText("[morethan15]", "");

                switch (averageamount)
                {
                    case "Below 500k":
                        document.ReplaceText("[Below 500k]", checkmark);
                        break;
                    case "501k-5M":
                        document.ReplaceText("[501k-5M]", checkmark);
                        break;
                    case "5.1M-20M":
                        document.ReplaceText("[5.1M-20M]", checkmark);
                        break;
                    case "20M+":
                        document.ReplaceText("[20M+]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[Below 500k]", "");
                document.ReplaceText("[501k-5M]", "");
                document.ReplaceText("[5.1M-20M]", "");
                document.ReplaceText("[20M+]", "");

                switch (purposeofacc)
                {
                    case "Payment for Vendor":
                        document.ReplaceText("[Payment for Vendor]", checkmark);
                        break;
                    case "Customer Transaction":
                        document.ReplaceText("[Customer Transaction]", checkmark);
                        break;
                    case "Parent Company/Subsidiary/Affiliate":
                        document.ReplaceText("[Parent Company/Subsidiary/Affiliate]", checkmark);
                        break;
                    case "Operations-related Transactions":
                        document.ReplaceText("[Operations-related Transactions]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[Payment for Vendor]", "");
                document.ReplaceText("[Customer Transaction]", "");
                document.ReplaceText("[Parent Company/Subsidiary/Affiliate]", "");
                document.ReplaceText("[Operations-related Transactions]", "");

                switch (modesofdeposit)
                {
                    case "Cash Deposit":
                        document.ReplaceText("[Cash Deposit]", checkmark);
                        break;
                    case "Check":
                        document.ReplaceText("[Check]", checkmark);
                        break;
                    case "Manager's Check":
                        document.ReplaceText("[Managers Check]", checkmark);
                        break;
                    case "Online Banking":
                        document.ReplaceText("[Online Banking]", checkmark);
                        break;
                    case "Remittance":
                        document.ReplaceText("[MRemittance]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[Cash Deposit]", "");
                document.ReplaceText("[Check]", "");
                document.ReplaceText("[Managers Check]", "");
                document.ReplaceText("[Online Banking]", "");
                document.ReplaceText("[MRemittance]", "");

                switch (modeofdelivery)
                {
                    case "Electronic":
                        document.ReplaceText("[Electronic]", checkmark);
                        break;
                    case "Mail":
                        document.ReplaceText("[Mail]", checkmark);
                        break;
                    case "N/A":
                        document.ReplaceText("[N/A]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[Electronic]", "");
                document.ReplaceText("[Mail]", "");
                document.ReplaceText("[N/A]", "");

                switch (secondarylicense)
                {
                    case "Yes":
                        document.ReplaceText("[SYes]", checkmark);
                        break;
                    case "No":
                        document.ReplaceText("[SNo]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[SYes]", "");
                document.ReplaceText("[SNo]", "");

                switch (publiclist)
                {
                    case "Activity Authorized Signatory":
                        document.ReplaceText("[ActivityAuthorizedSignatory1]", checkmark);
                        break;
                    case "Director/Trustee":
                        document.ReplaceText("[Director/Trustee1]", checkmark);
                        break;
                    case "Primary Officer":
                        document.ReplaceText("[PrimaryOfficer1]", checkmark);
                        break;
                    case "Stockholder":
                        document.ReplaceText("[Stockholder1]", checkmark);
                        break;
                    case "None":
                        document.ReplaceText("[None1]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[ActivityAuthorizedSignatory1", "");
                document.ReplaceText("[Director/Trustee1]", "");
                document.ReplaceText("[PrimaryOfficer1]", "");
                document.ReplaceText("[Stockholder1]", "");
                document.ReplaceText("[None1]", "");

                switch (familylist)
                {
                    case "Activity Authorized Signatory":
                        document.ReplaceText("[ActivityAuthorizedSignatory2]", checkmark);
                        break;
                    case "Director/Trustee":
                        document.ReplaceText("[Director/Trustee2]", checkmark);
                        break;
                    case "Primary Officer":
                        document.ReplaceText("[PrimaryOfficer2]", checkmark);
                        break;
                    case "Stockholder":
                        document.ReplaceText("[Stockholder2]", checkmark);
                        break;
                    case "None":
                        document.ReplaceText("[None2]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[ActivityAuthorizedSignatory2]", "");
                document.ReplaceText("[Director/Trustee2]", "");
                document.ReplaceText("[PrimaryOfficer2]", "");
                document.ReplaceText("[Stockholder2]", "");
                document.ReplaceText("[None2]", "");

                switch (PEPlist)
                {
                    case "Activity Authorized Signatory":
                        document.ReplaceText("[ActivityAuthorizedSignatory3]", checkmark);
                        break;
                    case "Director/Trustee":
                        document.ReplaceText("[Director/Trustee3]", checkmark);
                        break;
                    case "Primary Officer":
                        document.ReplaceText("[PrimaryOfficer3]", checkmark);
                        break;
                    case "Stockholder":
                        document.ReplaceText("[Stockholder3]", checkmark);
                        break;
                    case "None":
                        document.ReplaceText("[None3]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[ActivityAuthorizedSignatory3]", "");
                document.ReplaceText("[Director/Trustee3]", "");
                document.ReplaceText("[PrimaryOfficer3]", "");
                document.ReplaceText("[Stockholder3]", "");
                document.ReplaceText("[None3]", "");

                switch (branchref)
                {
                    case "Yes":
                        document.ReplaceText("[Y]", checkmark);
                        break;
                    case "No":
                        document.ReplaceText("[N]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[Y]", "");
                document.ReplaceText("[N]", "");

                switch (reqtype)
                {
                    case "New Merchant":
                        document.ReplaceText("[New]", checkmark);
                        break;
                    case "Additional Location":
                        document.ReplaceText("[Add]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[New]", "");
                document.ReplaceText("[Add]", "");

                switch (paymentgroup)
                {
                    case "Consolidate":
                        document.ReplaceText("[C]", checkmark);
                        break;
                    case "Individual Payment":
                        document.ReplaceText("[I]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[C]", "");
                document.ReplaceText("[I]", "");

                switch (merchemployedstat)
                {
                    case "Employed":
                        document.ReplaceText("[employed]", checkmark);
                        break;
                    case "Selfemployed":
                        document.ReplaceText("[selfemployed]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[employed]", "");
                document.ReplaceText("[selfemployed]", "");

                switch (sig1employedstat)
                {
                    case "Employed":
                        document.ReplaceText("[sig1employed]", checkmark);
                        break;
                    case "Selfemployed":
                        document.ReplaceText("[sig1selfemployed]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[sig1employed]", "");
                document.ReplaceText("[sig1selfemployed]", "");

                switch (sig2employedstat)
                {
                    case "Employed":
                        document.ReplaceText("[sig2employed]", checkmark);
                        break;
                    case "Selfemployed":
                        document.ReplaceText("[sig2selfemployed]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[sig2employed]", "");
                document.ReplaceText("[sig2selfemployed]", "");

                switch (man1employedstat)
                {
                    case "Employed":
                        document.ReplaceText("[man1employed]", checkmark);
                        break;
                    case "Selfemployed":
                        document.ReplaceText("[man1selfemployed]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[man1employed]", "");
                document.ReplaceText("[man1selfemployed]", "");

                switch (man2employedstat)
                {
                    case "Employed":
                        document.ReplaceText("[man2employed]", checkmark);
                        break;
                    case "Selfemployed":
                        document.ReplaceText("[man2selfemployed]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[man2employed]", "");
                document.ReplaceText("[man2selfemployed]", "");

                switch (POS)
                {
                    case "Standard":
                        document.ReplaceText("[standard]", checkmark);
                        break;
                    case "Prepaid":
                        document.ReplaceText("[prepaid]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[standard]", "");
                document.ReplaceText("[prepaid]", "");

                switch (installmentrates)
                {
                    case "POS":
                        document.ReplaceText("[P]", checkmark);
                        break;
                    case "Online":
                        document.ReplaceText("[O]", checkmark);
                        break;
                    default:

                        break;
                }

                document.ReplaceText("[P]", "");
                document.ReplaceText("[O]", "");

                document.ReplaceText("[datesub]", datesub.ToString("MM-dd-yyyy"));

                // Save the modified document
                document.SaveAs(outputFilePath);
            }
        }

        private void OpenWordDocument(string filePath)
        {
            try
            {
                // Check if file exists
                if (!File.Exists(filePath))
                {
                    MessageBox.Show("The document file was not found.", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Open with default associated program
                System.Diagnostics.Process.Start(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open document: {ex.Message}", "Error",
                               MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowWordPrintPreview(string filePath)
        {
            var wordApp = new Microsoft.Office.Interop.Word.Application();
            try
            {
                wordApp.Visible = true;
                var doc = wordApp.Documents.Open(filePath);
                doc.PrintPreview();

                // Optionally: you can wait for user input here before closing, e.g. message box
                MessageBox.Show("Close Word when done with print preview.");

                doc.Close(false);
                wordApp.Quit(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not show print preview: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Marshal.ReleaseComObject(wordApp);
            }
        }

        private void frmMerchantDocumentGenerator_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }
    }
}
