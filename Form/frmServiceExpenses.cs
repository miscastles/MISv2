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
    public partial class frmServiceExpenses : Form
    {
        private clsAPI dbAPI;
        private clsFunction dbFunction;

        public static List<string> ExpensesIDCol = new List<String>();
        public static List<string> ExpensesDescriptionCol = new List<String>();
        public static List<string> ExpensesAmountCol = new List<String>();

        public bool fSelected = false;
        public string sTExpenses = clsFunction.sDefaultAmount;

        // Set QTY
        string sInputAmount = "";        

        public frmServiceExpenses()
        {
            InitializeComponent();
        }
        private void LoadExpenses()
        {
            int i = 0;
            int iLineNo = 0;
            bool fEdit = false;

            try
            {
                if (clsArray.HoldExpensesID.Length > 0)
                    fEdit = true;
            }
            catch
            {
                fEdit = false;
            }
            
            lvwList.Items.Clear();
            dbAPI.ExecuteAPI("GET", "View", "", "", "Expenses", "", "ViewExpenses");

            if (!clsGlobalVariables.isAPIResponseOK) return;

            if (clsExpenses.RecordFound)
            {
                while (clsArray.ExpensesID.Length > i)
                {
                    iLineNo++;
                    ListViewItem item = new ListViewItem(iLineNo.ToString());
                    item.SubItems.Add(clsArray.ExpensesID[i]);
                    item.SubItems.Add(clsArray.ExpensesDescription[i]);

                    if (fEdit)                    
                        item.SubItems.Add(GetExpensesAmountFromList(clsArray.ExpensesID[i]));                    
                    else                    
                        item.SubItems.Add(clsFunction.sDefaultAmount);
                                        
                    lvwList.Items.Add(item);

                    i++;
                }

                ComputeTotals();
            }

            dbFunction.ListViewAlternateBackColor(lvwList);

            dbFunction.ListViewRowFocus(lvwList, 0);
        }   

        private void frmServiceExpenses_Load(object sender, EventArgs e)
        {
            dbAPI = new clsAPI();
            dbFunction = new clsFunction();

            fSelected = false;
            LoadExpenses();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            fSelected = false;
            this.Close();
        }

        private void frmServiceExpenses_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    fSelected = false;
                    this.Close();
                    break;
            }
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            fSelected = true;
            sTExpenses = txtTExpenses.Text;
            HoldExpenses();
            this.Close();
        }

        private bool ShowMenuInputBox()
        {
            string DefaultAmmount = clsFunction.sDefaultAmount;

            if (clsSearch.ClassExpensesID > 0)
            {
                InputBox.iInputType = clsFunction.Numeric_Input;
                InputBox.iInputLimitSize = 12;
                InputBoxResult MenuNum = InputBox.Show("Enter your expenses for " + clsSearch.ClassExpensesDescription + "\n", "Expenses Entry", DefaultAmmount, 100, 0, 255, (int)Enums.OptionType.Others);

                if (MenuNum.ReturnCode == DialogResult.OK)
                {
                    sInputAmount = MenuNum.Text;
                    return true;
                }

                if (MenuNum.ReturnCode == DialogResult.No || MenuNum.ReturnCode == DialogResult.Cancel)
                {
                    sInputAmount = clsFunction.sDefaultAmount;
                    return true;
                }
            }
            

            return true;
        }

        private void lvwList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwList.SelectedItems.Count > 0)
            {
                string LineNo = lvwList.SelectedItems[0].Text;
                txtLineNo.Text = LineNo;

                if (LineNo.Length > 0)
                {
                    clsSearch.ClassExpensesID = int.Parse(lvwList.SelectedItems[0].SubItems[1].Text);
                    clsSearch.ClassExpensesDescription =lvwList.SelectedItems[0].SubItems[2].Text;
                    clsSearch.ClassExpensesAmount = double.Parse(lvwList.SelectedItems[0].SubItems[3].Text);
                }
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

        private void AddItemToList(string sAmount)
        {
            bool fListed = false;            
            double dAmount = 0.00;

            if (lvwList.Items.Count > 0)
            {
                foreach (ListViewItem i in lvwList.Items)
                {
                    if (i.SubItems[1].Text.CompareTo(clsSearch.ClassExpensesID.ToString()) == 0)
                    {
                        fListed = true;
                    }

                    if (fListed)
                    {
                        // Gross Amt
                        if (dbFunction.isValidAmount(sAmount))
                        {
                            dAmount = double.Parse(sAmount);
                            i.SubItems[3].Text = dbFunction.FormatAmount(dAmount);                            
                        }

                        break;
                    }
                }

                ComputeTotals();
            }            
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (dbFunction.isValidAmount(txtTExpenses.Text))
            {
                if (MessageBox.Show("Expenses will be removed.\n" +
                                    "\n\n",
                                    "Continue?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                {
                    return;
                }                
            }

            txtLineNo.Text = clsFunction.sZero;
            txtTExpenses.Text = clsFunction.sDefaultAmount;

            ResetHoldExpenses();
            LoadExpenses();
        }

        public static void ResetHoldExpenses()
        {
            ExpensesIDCol.Clear();
            ExpensesDescriptionCol.Clear();
            ExpensesAmountCol.Clear();

            clsArray.HoldExpensesID = ExpensesIDCol.ToArray();
            clsArray.HoldExpensesDescription = ExpensesDescriptionCol.ToArray();
            clsArray.HoldExpensesAmount = ExpensesAmountCol.ToArray();
        }

        private void ComputeTotals()
        {
            double TExpenses = 0;
          
            if (lvwList.Items.Count > 0)
            {
                foreach (ListViewItem i in lvwList.Items)
                {
                    TExpenses += Convert.ToDouble(i.SubItems[3].Text);                    
                }
            }

            txtTExpenses.Text = dbFunction.FormatAmount(TExpenses);
        }

        private void HoldExpenses()
        {
            ExpensesIDCol.Clear();
            ExpensesDescriptionCol.Clear();
            ExpensesAmountCol.Clear();

            if (lvwList.Items.Count > 0)
            {
                foreach (ListViewItem i in lvwList.Items)
                {
                    string sExpensesID = i.SubItems[1].Text;
                    string sExpensesDescription = i.SubItems[2].Text;
                    string sExpensesAmount = i.SubItems[3].Text;

                    ExpensesIDCol.Add(sExpensesID);
                    ExpensesDescriptionCol.Add(sExpensesDescription);
                    ExpensesAmountCol.Add(sExpensesAmount);
                }

                clsArray.HoldExpensesID = ExpensesIDCol.ToArray();
                clsArray.HoldExpensesDescription = ExpensesDescriptionCol.ToArray();
                clsArray.HoldExpensesAmount = ExpensesAmountCol.ToArray();
            }
        }

        private string GetExpensesAmountFromList(string sExpensesID)
        {
            string sAmount = clsFunction.sDefaultAmount;
            int i = 0;

            while (clsArray.HoldExpensesID.Length > i)
            {
                if (sExpensesID.CompareTo(clsArray.HoldExpensesID[i]) == 0)
                {
                    sAmount = clsArray.HoldExpensesAmount[i];
                    break;
                }

                i++;
            }

            return sAmount;
        }

        private void lvwList_DoubleClick(object sender, EventArgs e)
        {
            if (lvwList.Items.Count > 0)
            {
                if (!ShowMenuInputBox()) return;

                AddItemToList(sInputAmount);

                if (dbFunction.isValidID(txtLineNo.Text))
                {
                    int iRowIndex = int.Parse(txtLineNo.Text);
                    dbFunction.ListViewRowFocus(lvwList, iRowIndex);
                }
            }
        }
    }
}
