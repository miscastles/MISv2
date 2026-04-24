using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MIS
{

	#region InputBox return result

	/// <summary>
	/// Class used to store the result of an InputBox.Show message.
	/// </summary>
	public class InputBoxResult 
	{
		public DialogResult ReturnCode;
		public string Text;
	}

	#endregion

	/// <summary>
	/// Summary description for InputBox.
	/// </summary>
	public class InputBox
	{

		#region Private Windows Contols and Constructor

		// Create a new instance of the form.
		private static Form frmInputDialog;
        private static Label lblDescription;
        private static Label lblValue;
		private static Button btnOK;
		private static Button btnCancel;
        private static Label lblInput;
        private static TextBox txtPrompt;
        private static TextBox txtInput;
        private static GroupBox separator;
        public static int iInputType;
        public static int iInputLimitSize;

        private static RadioButton rbYes;
        private static RadioButton rbNo;
        private static RadioButton rbOthers;

        public InputBox()
		{
		}

		#endregion

		#region Private Variables

		private static string _formCaption = string.Empty;
		private static string _formPrompt = string.Empty;
		private static InputBoxResult _outputResponse = new InputBoxResult();
		private static string _defaultValue = string.Empty;
		private static int _xPos = -1;
		private static int _yPos = -1;
        private static int _MaxLimit = 255;

        private static int _OptionType = 0;

        #endregion

        #region Windows Form code

        private static void InitializeComponent()
		{
			// Create a new instance of the form.
			frmInputDialog = new Form();
            lblDescription = new Label();
            lblValue = new Label();
            lblInput = new Label();
            btnOK = new Button();
			btnCancel = new Button();
            txtPrompt = new TextBox();
            txtInput = new TextBox();
            separator = new GroupBox();

            rbYes = new RadioButton();
            rbNo = new RadioButton();
            rbOthers = new RadioButton();

            frmInputDialog.SuspendLayout();

            // 
            // lblDescription
            // 
            lblDescription.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
            lblDescription.BackColor = SystemColors.Control;
            lblDescription.Font = new Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((Byte)(0)));
            lblDescription.Location = new Point(12, 0);
            lblDescription.Name = "lblDescription";                       
            lblDescription.AutoSize = true;
            lblDescription.Text = "Description";

            // 
            // lblValue
            // 
            lblValue.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
            lblValue.BackColor = SystemColors.Control;
            lblValue.Font = new Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((Byte)(0)));
            lblValue.Location = new Point(12, 14);
            lblValue.Name = "lblValue";                   
            lblValue.AutoSize = true;
            lblValue.Text = "Value";

            // 
            // txtPrompt
            // 
            txtPrompt.Location = new Point(8, 30);
            txtPrompt.Multiline = true;
            txtPrompt.MaxLength = iInputLimitSize;
            txtPrompt.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            txtPrompt.Name = "txtPrompt";
            txtPrompt.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            txtPrompt.Size = new Size(330, 50);
            txtPrompt.BackColor = Color.FromArgb(255, 255, 192);
            txtPrompt.ReadOnly = true;
            txtPrompt.TabIndex = 99;

            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
			btnOK.FlatStyle = FlatStyle.Popup;
			btnOK.Location = new Point(340, 8);
			btnOK.Name = "btnOK";
			btnOK.Size = new Size(64, 24);
			btnOK.TabIndex = 2;
			btnOK.Text = "&OK";
			btnOK.Click += new EventHandler(btnOK_Click);
			// 
			// btnCancel
			// 
			btnCancel.DialogResult = DialogResult.Cancel;
			btnCancel.FlatStyle = FlatStyle.Popup;
			btnCancel.Location = new Point(340, 40);
			btnCancel.Name = "btnCancel";
			btnCancel.Size = new Size(64, 24);
			btnCancel.TabIndex = 3;
			btnCancel.Text = "&Cancel";
			btnCancel.Click += new EventHandler(btnCancel_Click);


            // 
            // seprator
            // 
            separator.Location = new System.Drawing.Point(8, 84);
            separator.Name = "seprator";
            separator.Size = new System.Drawing.Size(330, 3);            
            separator.TabStop = false;

            // 
            // rbYes
            // 
            rbYes.AutoSize = true;
            rbYes.Font = new System.Drawing.Font("Courier New", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            rbYes.Location = new System.Drawing.Point(8, 86);
            rbYes.Name = "rbYes";
            rbYes.Size = new System.Drawing.Size(50, 20);
            rbYes.TabIndex = 4;
            rbYes.TabStop = true;
            rbYes.Text = "Yes";
            rbYes.ForeColor = System.Drawing.Color.Blue;
            rbYes.UseVisualStyleBackColor = true;
            rbYes.CheckedChanged += new System.EventHandler(rbYes_CheckedChanged);
            // 
            // rbNo
            // 
            rbNo.AutoSize = true;
            rbNo.Font = new System.Drawing.Font("Courier New", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            rbNo.Location = new System.Drawing.Point(8, 108);
            rbNo.Name = "rbNo";
            rbNo.Size = new System.Drawing.Size(42, 20);
            rbNo.TabIndex = 5;
            rbNo.TabStop = true;
            rbNo.Text = "No";
            rbNo.ForeColor = System.Drawing.Color.Blue;
            rbNo.UseVisualStyleBackColor = true;
            rbNo.CheckedChanged += new System.EventHandler(rbNo_CheckedChanged);
            // 
            // rbOthers
            // 
            rbOthers.AutoSize = true;
            rbOthers.Font = new System.Drawing.Font("Courier New", 8.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            rbOthers.Location = new System.Drawing.Point(8, 130);
            rbOthers.Name = "rbOthers";
            rbOthers.Size = new System.Drawing.Size(74, 96);
            rbOthers.TabIndex = 6;
            rbOthers.TabStop = true;
            rbOthers.Text = "Others";
            rbOthers.ForeColor = System.Drawing.Color.Blue;
            rbOthers.UseVisualStyleBackColor = true;
            rbOthers.CheckedChanged += new System.EventHandler(rbOthers_CheckedChanged);

            // 
            // lblInput
            // 
            lblInput.Anchor = ((AnchorStyles)((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right)));
            lblInput.BackColor = SystemColors.Control;
            lblInput.Font = new Font("Courier New", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((Byte)(0)));
            lblInput.Location = new Point(8, 170);
            lblInput.Name = "lblInput";
            lblInput.AutoSize = true;
            lblInput.Text = "Please enter NEW value";
            
            // 
            // txtInput
            // 
            txtInput.Location = new Point(8, 180);
            txtInput.Multiline = true;
            txtInput.MaxLength = iInputLimitSize;
            txtInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            txtInput.Name = "txtInput";
            txtInput.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            txtInput.Size = new Size(400, 40);
			txtInput.TabIndex = 1;
			txtInput.Text = "";
            txtInput.CharacterCasing = CharacterCasing.Normal;
            txtInput.KeyDown += new System.Windows.Forms.KeyEventHandler(txtInput_KeyDown);
            txtInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(txtInput_KeyPress);
            // 
            // InputBoxDialog
            // 
            frmInputDialog.AutoScaleBaseSize = new Size(5, 13);
			frmInputDialog.ClientSize = new Size(420, 240);

            frmInputDialog.Controls.Add(txtPrompt);
            frmInputDialog.Controls.Add(txtInput);

			frmInputDialog.Controls.Add(btnCancel);
			frmInputDialog.Controls.Add(btnOK);
            frmInputDialog.Controls.Add(separator);
            frmInputDialog.Controls.Add(lblDescription);
            frmInputDialog.Controls.Add(lblValue);
            frmInputDialog.Controls.Add(lblInput);

            frmInputDialog.Controls.Add(rbYes);
            frmInputDialog.Controls.Add(rbNo);
            frmInputDialog.Controls.Add(rbOthers);

            frmInputDialog.FormBorderStyle = FormBorderStyle.FixedDialog;
			frmInputDialog.MaximizeBox = false;
			frmInputDialog.MinimizeBox = false;
			frmInputDialog.Name = "InputBoxDialog";
			frmInputDialog.ResumeLayout(false);
		}

		#endregion

		#region Private function, InputBox Form move and change size

		static private void LoadForm()
		{
			OutputResponse.ReturnCode = DialogResult.Ignore;
			OutputResponse.Text = string.Empty;

            lblDescription.Text = (_defaultValue.Length > 0 ? "Description: " + _defaultValue : "");
            txtPrompt.Text = _formPrompt;
            txtInput.Text = "";			
			frmInputDialog.Text = _formCaption;

			// Retrieve the working rectangle from the Screen class
			// using the PrimaryScreen and the WorkingArea properties.
			System.Drawing.Rectangle workingRectangle = Screen.PrimaryScreen.WorkingArea;
            
			if((_xPos >= 0 && _xPos < workingRectangle.Width-100) && (_yPos >= 0 && _yPos < workingRectangle.Height-100))
			{
                //frmInputDialog.StartPosition = FormStartPosition.Manual;
                frmInputDialog.StartPosition = FormStartPosition.CenterScreen;
                frmInputDialog.Location = new System.Drawing.Point(_xPos, _yPos);
			}
			else
				frmInputDialog.StartPosition = FormStartPosition.CenterScreen;
            

            string PrompText = txtPrompt.Text;

			int n = 0;
			int Index = 0;
			while(PrompText.IndexOf("\n",Index) > -1)
			{
				Index = PrompText.IndexOf("\n",Index)+1;
				n++;
			}

			if( n == 0 )
				n = 1;

            // OptionType
            if (_OptionType == (int)Enums.OptionType.Others)
            {
                rbYes.Enabled = rbNo.Enabled = false;
                rbOthers.Enabled = true;
                rbOthers.Checked = true;
            }
            else if (_OptionType == (int)Enums.OptionType.YesNo)
            {
                rbOthers.Enabled = false;
                rbYes.Enabled = rbNo.Enabled = true;
            }
            else if (_OptionType == (int)Enums.OptionType.YesNoOthers)
            {
                rbYes.Enabled = rbNo.Enabled = rbOthers.Enabled = true;
            }
            else
            {
                rbYes.Enabled = rbNo.Enabled = false;
                rbOthers.Enabled = true;
                rbOthers.Checked = true;
            }

            System.Drawing.Point Txt = txtInput.Location; 
			Txt.Y = Txt.Y + (n*4);
			txtInput.Location = Txt; 
			System.Drawing.Size form = frmInputDialog.Size; 
			form.Height = form.Height + (n*4);
			frmInputDialog.Size = form; 

			txtInput.SelectionStart = 0;
			txtInput.SelectionLength = txtInput.Text.Length;
            txtInput.MaxLength = _MaxLimit;

            txtInput.Focus();
		}

        static private void ActivateForm()
        {
            txtInput.Focus();
        }

        #endregion

        #region Button control click event

        static private void btnOK_Click(object sender, System.EventArgs e)
		{
			OutputResponse.ReturnCode = DialogResult.OK;
			OutputResponse.Text = txtInput.Text;
			frmInputDialog.Dispose();
		}

		static private void btnCancel_Click(object sender, System.EventArgs e)
		{
			OutputResponse.ReturnCode = DialogResult.Cancel;
			OutputResponse.Text = string.Empty; //Clean output response
			frmInputDialog.Dispose();
		}

        static private void txtInput_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    btnOK_Click(frmInputDialog, e);
                    break;
                case Keys.Escape:
                    btnCancel_Click(frmInputDialog, e);
                    break;
            }
        }

        static private void txtInput_KeyPress(object sender, KeyPressEventArgs e)
        {            
            if (iInputType == clsFunction.Numeric_Input)
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
                {
                    e.Handled = true;
                }

                // only allow one decimal point
                if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
                {
                    e.Handled = true;
                }
            }

            if (iInputType == clsFunction.AlphaNumeric_Input)
            {
                // do nothing...
            }


            if (iInputType == clsFunction.Alpha_Input)
            {
                // do nothing...
            }

        }
        #endregion

        #region Public Static Show functions
        
        static public InputBoxResult Show(string Prompt,string Title,string Default,int XPos,int YPos, int pMaxLimit, int pOptionType)
		{
			InitializeComponent();
			FormCaption = Title;
			FormPrompt = Prompt;
			DefaultValue = Default;
			XPosition = XPos;
			YPosition = YPos;
            MaxLimit = pMaxLimit;
            OptionType = pOptionType;

            // Display the form as a modal dialog box.            
            LoadForm();            
			frmInputDialog.ShowDialog();
            ActivateForm();
            return OutputResponse;
		}

		#endregion

		#region Private Properties

		static private string FormCaption
		{
			set
			{
				_formCaption = value;
			}
		} // property FormCaption
		
		static private string FormPrompt
		{
			set
			{
				_formPrompt = value;
			}
		} // property FormPrompt
		
		static private InputBoxResult OutputResponse
		{
			get
			{
				return _outputResponse;
			}
			set
			{
				_outputResponse = value;
			}
		} // property InputResponse
		
		static private string DefaultValue
		{
			set
			{
				_defaultValue = value;
			}
		} // property DefaultValue

		static private int XPosition
		{
			set
			{
				if( value >= 0 )
					_xPos = value;
			}
		} // property XPos

		static private int YPosition
		{
			set
			{
				if( value >= 0 )
					_yPos = value;
			}
		} // property YPos

        static private int MaxLimit
        {
            set
            {
                if (value >= 0)
                    _MaxLimit = value;
            }
        } // property YPos

        static private int OptionType
        {
            set
            {
                if (value >= 0)
                    _OptionType = value;
            }
        } // property YPos

        static void rbYes_CheckedChanged(object sender, EventArgs e)
        {
            txtInput.Text = "Yes";
            txtInput.Focus();
        }

        static void rbNo_CheckedChanged(object sender, EventArgs e)
        {
            txtInput.Text = "No";
            txtInput.Focus();
        }

        static void rbOthers_CheckedChanged(object sender, EventArgs e)
        {
            txtInput.Text = "";
            txtInput.Focus();
        }

        #endregion
    }
}
