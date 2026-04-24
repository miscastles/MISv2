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
using MIS.Function;
using System.Text.RegularExpressions;

namespace MIS
{
    public partial class frmTools : Form
    {
        private clsFunction dbFunction;

        public frmTools()
        {
            InitializeComponent();

            dbFunction = new clsFunction();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            if (dbFunction.isValidDescription(txtInput.Text))
                txtParse.Text = cleanInput(dbFunction.parseDelimitedString(txtInput.Text, clsDefines.gComma, 0));

        }

        private void frmTools_Load(object sender, EventArgs e)
        {

        }

        private string cleanInput(string input)
        {
            // Regex to remove \" and \\ characters
            string pattern = @"[\\\""]";
            return Regex.Replace(input, pattern, "");
        }

        private void frmTools_KeyDown(object sender, KeyEventArgs e)
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
