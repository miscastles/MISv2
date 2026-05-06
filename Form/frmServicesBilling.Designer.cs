namespace MIS
{
    partial class frmServicesBilling
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmServicesBilling));
            this.dgv_BillingRecords = new System.Windows.Forms.DataGridView();
            this.dgv_BillingRemove = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtRefNo = new System.Windows.Forms.TextBox();
            this.btn_GenExcel = new System.Windows.Forms.Button();
            this.btn_GenInvoice = new System.Windows.Forms.Button();
            this.cmb_BillingType = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.btn_Clear = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtDiscount = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel12 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dtpDueDate = new System.Windows.Forms.DateTimePicker();
            this.ucRoundPanel5 = new MIS.ControlObject.ucRoundPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dtpRefDate = new System.Windows.Forms.DateTimePicker();
            this.ucRoundPanel4 = new MIS.ControlObject.ucRoundPanel();
            this.label12 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.txtTerms = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.ucRoundPanel1 = new MIS.ControlObject.ucRoundPanel();
            this.ucRoundPanel2 = new MIS.ControlObject.ucRoundPanel();
            this.ucRoundPanel3 = new MIS.ControlObject.ucRoundPanel();
            this.ucRoundPanel7 = new MIS.ControlObject.ucRoundPanel();
            this.ucRoundPanel6 = new MIS.ControlObject.ucRoundPanel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.lb_Remove = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lb_Count = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.btn_Search = new System.Windows.Forms.Button();
            this.txt_SearchRecord = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.panel10 = new System.Windows.Forms.Panel();
            this.dgv_BillingHistory = new System.Windows.Forms.DataGridView();
            this.panel11 = new System.Windows.Forms.Panel();
            this.lblBillingHeader = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_BillingRecords)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_BillingRemove)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_BillingHistory)).BeginInit();
            this.panel11.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgv_BillingRecords
            // 
            this.dgv_BillingRecords.AllowUserToAddRows = false;
            this.dgv_BillingRecords.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgv_BillingRecords.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgv_BillingRecords.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgv_BillingRecords.BackgroundColor = System.Drawing.Color.White;
            this.dgv_BillingRecords.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv_BillingRecords.ColumnHeadersHeight = 30;
            this.dgv_BillingRecords.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_BillingRecords.GridColor = System.Drawing.Color.Silver;
            this.dgv_BillingRecords.Location = new System.Drawing.Point(3, 3);
            this.dgv_BillingRecords.Name = "dgv_BillingRecords";
            this.dgv_BillingRecords.ReadOnly = true;
            this.dgv_BillingRecords.RowTemplate.Height = 30;
            this.dgv_BillingRecords.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_BillingRecords.Size = new System.Drawing.Size(1415, 224);
            this.dgv_BillingRecords.TabIndex = 0;
            this.dgv_BillingRecords.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_BillingRecords_CellDoubleClick);
            this.dgv_BillingRecords.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgv_BillingRecords_RowsAdded);
            this.dgv_BillingRecords.DoubleClick += new System.EventHandler(this.dgv_BillingRecords_DoubleClick);
            // 
            // dgv_BillingRemove
            // 
            this.dgv_BillingRemove.AllowUserToAddRows = false;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgv_BillingRemove.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            this.dgv_BillingRemove.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgv_BillingRemove.BackgroundColor = System.Drawing.Color.White;
            this.dgv_BillingRemove.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv_BillingRemove.ColumnHeadersHeight = 30;
            this.dgv_BillingRemove.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_BillingRemove.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgv_BillingRemove.Location = new System.Drawing.Point(3, 3);
            this.dgv_BillingRemove.Name = "dgv_BillingRemove";
            this.dgv_BillingRemove.ReadOnly = true;
            this.dgv_BillingRemove.RowTemplate.Height = 30;
            this.dgv_BillingRemove.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_BillingRemove.Size = new System.Drawing.Size(1415, 224);
            this.dgv_BillingRemove.TabIndex = 1;
            this.dgv_BillingRemove.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_BillingRemove_CellDoubleClick);
            this.dgv_BillingRemove.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgv_BillingRemove_RowsAdded);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(241, 160);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Due";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(362, 60);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Ref. No.";
            // 
            // txtRefNo
            // 
            this.txtRefNo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtRefNo.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRefNo.Location = new System.Drawing.Point(376, 101);
            this.txtRefNo.Name = "txtRefNo";
            this.txtRefNo.Size = new System.Drawing.Size(236, 20);
            this.txtRefNo.TabIndex = 0;
            // 
            // btn_GenExcel
            // 
            this.btn_GenExcel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_GenExcel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btn_GenExcel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_GenExcel.Location = new System.Drawing.Point(130, 361);
            this.btn_GenExcel.Name = "btn_GenExcel";
            this.btn_GenExcel.Size = new System.Drawing.Size(105, 35);
            this.btn_GenExcel.TabIndex = 3;
            this.btn_GenExcel.Text = "Export";
            this.btn_GenExcel.UseVisualStyleBackColor = true;
            this.btn_GenExcel.Click += new System.EventHandler(this.btn_GenExcel_Click);
            // 
            // btn_GenInvoice
            // 
            this.btn_GenInvoice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_GenInvoice.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btn_GenInvoice.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_GenInvoice.Location = new System.Drawing.Point(19, 361);
            this.btn_GenInvoice.Name = "btn_GenInvoice";
            this.btn_GenInvoice.Size = new System.Drawing.Size(105, 35);
            this.btn_GenInvoice.TabIndex = 4;
            this.btn_GenInvoice.Text = "Invoice";
            this.btn_GenInvoice.UseVisualStyleBackColor = true;
            this.btn_GenInvoice.Click += new System.EventHandler(this.btn_GenInvoice_Click);
            // 
            // cmb_BillingType
            // 
            this.cmb_BillingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_BillingType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmb_BillingType.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmb_BillingType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cmb_BillingType.FormattingEnabled = true;
            this.cmb_BillingType.Items.AddRange(new object[] {
            "Net Matrix (Leasing)",
            "Servicing Fee (Services)",
            "Terminal Line Encryption (TLE)",
            "Warehouse"});
            this.cmb_BillingType.Location = new System.Drawing.Point(26, 98);
            this.cmb_BillingType.Name = "cmb_BillingType";
            this.cmb_BillingType.Size = new System.Drawing.Size(314, 25);
            this.cmb_BillingType.TabIndex = 5;
            this.cmb_BillingType.SelectedIndexChanged += new System.EventHandler(this.cmb_BillingType_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tabControl1.Location = new System.Drawing.Point(5, 52);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1429, 261);
            this.tabControl1.TabIndex = 11;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dgv_BillingRecords);
            this.tabPage1.Location = new System.Drawing.Point(4, 27);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1421, 230);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "   Included   ";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.dgv_BillingRemove);
            this.tabPage2.Location = new System.Drawing.Point(4, 27);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1421, 230);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "   Excluded   ";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // btn_Clear
            // 
            this.btn_Clear.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btn_Clear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_Clear.Location = new System.Drawing.Point(515, 361);
            this.btn_Clear.Name = "btn_Clear";
            this.btn_Clear.Size = new System.Drawing.Size(105, 35);
            this.btn_Clear.TabIndex = 12;
            this.btn_Clear.Text = "Clear";
            this.btn_Clear.UseVisualStyleBackColor = true;
            this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(16, 60);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 17);
            this.label5.TabIndex = 11;
            this.label5.Text = "Billing";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txtDiscount);
            this.panel1.Controls.Add(this.txtDescription);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.panel12);
            this.panel1.Controls.Add(this.panel9);
            this.panel1.Controls.Add(this.panel8);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.ucRoundPanel5);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.ucRoundPanel4);
            this.panel1.Controls.Add(this.btn_Clear);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.btn_GenExcel);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.btn_GenInvoice);
            this.panel1.Controls.Add(this.txtTerms);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.cmb_BillingType);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtRefNo);
            this.panel1.Controls.Add(this.ucRoundPanel1);
            this.panel1.Controls.Add(this.ucRoundPanel2);
            this.panel1.Controls.Add(this.ucRoundPanel3);
            this.panel1.Controls.Add(this.ucRoundPanel7);
            this.panel1.Controls.Add(this.ucRoundPanel6);
            this.panel1.Location = new System.Drawing.Point(12, 17);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(638, 415);
            this.panel1.TabIndex = 13;
            // 
            // txtDiscount
            // 
            this.txtDiscount.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtDiscount.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtDiscount.Location = new System.Drawing.Point(480, 300);
            this.txtDiscount.Name = "txtDiscount";
            this.txtDiscount.Size = new System.Drawing.Size(129, 20);
            this.txtDiscount.TabIndex = 33;
            this.txtDiscount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDiscount_KeyPress);
            // 
            // txtDescription
            // 
            this.txtDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtDescription.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtDescription.Location = new System.Drawing.Point(29, 300);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(410, 20);
            this.txtDescription.TabIndex = 32;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(465, 261);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 17);
            this.label7.TabIndex = 31;
            this.label7.Text = "Discount";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(17, 261);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 17);
            this.label4.TabIndex = 30;
            this.label4.Text = "Description";
            // 
            // panel12
            // 
            this.panel12.BackColor = System.Drawing.Color.Silver;
            this.panel12.Location = new System.Drawing.Point(16, 345);
            this.panel12.Name = "panel12";
            this.panel12.Size = new System.Drawing.Size(601, 1);
            this.panel12.TabIndex = 28;
            // 
            // panel9
            // 
            this.panel9.BackColor = System.Drawing.Color.Silver;
            this.panel9.Location = new System.Drawing.Point(19, 242);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(601, 1);
            this.panel9.TabIndex = 26;
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.Silver;
            this.panel8.Location = new System.Drawing.Point(19, 142);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(601, 1);
            this.panel8.TabIndex = 16;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnSave.Location = new System.Drawing.Point(241, 361);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(105, 35);
            this.btnSave.TabIndex = 25;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.Controls.Add(this.dtpDueDate);
            this.panel4.Location = new System.Drawing.Point(255, 198);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(184, 23);
            this.panel4.TabIndex = 21;
            // 
            // dtpDueDate
            // 
            this.dtpDueDate.CalendarFont = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpDueDate.CalendarForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dtpDueDate.CalendarTitleForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dtpDueDate.CustomFormat = "  MMM d, yyyy";
            this.dtpDueDate.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpDueDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpDueDate.Location = new System.Drawing.Point(-1, -1);
            this.dtpDueDate.Name = "dtpDueDate";
            this.dtpDueDate.Size = new System.Drawing.Size(186, 25);
            this.dtpDueDate.TabIndex = 15;
            // 
            // ucRoundPanel5
            // 
            this.ucRoundPanel5.BackColor = System.Drawing.Color.White;
            this.ucRoundPanel5.Location = new System.Drawing.Point(244, 191);
            this.ucRoundPanel5.Name = "ucRoundPanel5";
            this.ucRoundPanel5.Size = new System.Drawing.Size(205, 37);
            this.ucRoundPanel5.TabIndex = 24;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.dtpRefDate);
            this.panel3.Location = new System.Drawing.Point(29, 198);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(184, 23);
            this.panel3.TabIndex = 20;
            // 
            // dtpRefDate
            // 
            this.dtpRefDate.CalendarFont = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpRefDate.CalendarForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dtpRefDate.CalendarTitleForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dtpRefDate.CustomFormat = "  MMM d, yyyy";
            this.dtpRefDate.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpRefDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpRefDate.Location = new System.Drawing.Point(-1, -1);
            this.dtpRefDate.Name = "dtpRefDate";
            this.dtpRefDate.Size = new System.Drawing.Size(186, 25);
            this.dtpRefDate.TabIndex = 19;
            // 
            // ucRoundPanel4
            // 
            this.ucRoundPanel4.BackColor = System.Drawing.Color.White;
            this.ucRoundPanel4.Location = new System.Drawing.Point(19, 191);
            this.ucRoundPanel4.Name = "ucRoundPanel4";
            this.ucRoundPanel4.Size = new System.Drawing.Size(205, 37);
            this.ucRoundPanel4.TabIndex = 23;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.White;
            this.label12.Location = new System.Drawing.Point(16, 160);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(64, 17);
            this.label12.TabIndex = 18;
            this.label12.Text = "Ref. Date";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(37)))), ((int)(((byte)(59)))));
            this.panel2.Controls.Add(this.label6);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(636, 45);
            this.panel2.TabIndex = 14;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(15, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(142, 20);
            this.label6.TabIndex = 3;
            this.label6.Text = "Billing Information";
            // 
            // txtTerms
            // 
            this.txtTerms.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtTerms.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTerms.Location = new System.Drawing.Point(480, 201);
            this.txtTerms.Name = "txtTerms";
            this.txtTerms.Size = new System.Drawing.Size(129, 18);
            this.txtTerms.TabIndex = 17;
            this.txtTerms.Text = "30 Days";
            this.txtTerms.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.White;
            this.label11.Location = new System.Drawing.Point(465, 160);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(45, 17);
            this.label11.TabIndex = 16;
            this.label11.Text = "Terms";
            // 
            // ucRoundPanel1
            // 
            this.ucRoundPanel1.BackColor = System.Drawing.Color.White;
            this.ucRoundPanel1.Location = new System.Drawing.Point(19, 92);
            this.ucRoundPanel1.Name = "ucRoundPanel1";
            this.ucRoundPanel1.Size = new System.Drawing.Size(327, 37);
            this.ucRoundPanel1.TabIndex = 20;
            // 
            // ucRoundPanel2
            // 
            this.ucRoundPanel2.BackColor = System.Drawing.Color.White;
            this.ucRoundPanel2.Location = new System.Drawing.Point(365, 92);
            this.ucRoundPanel2.Name = "ucRoundPanel2";
            this.ucRoundPanel2.Size = new System.Drawing.Size(255, 37);
            this.ucRoundPanel2.TabIndex = 21;
            // 
            // ucRoundPanel3
            // 
            this.ucRoundPanel3.BackColor = System.Drawing.Color.White;
            this.ucRoundPanel3.Location = new System.Drawing.Point(468, 191);
            this.ucRoundPanel3.Name = "ucRoundPanel3";
            this.ucRoundPanel3.Size = new System.Drawing.Size(152, 37);
            this.ucRoundPanel3.TabIndex = 22;
            // 
            // ucRoundPanel7
            // 
            this.ucRoundPanel7.BackColor = System.Drawing.Color.White;
            this.ucRoundPanel7.Location = new System.Drawing.Point(468, 291);
            this.ucRoundPanel7.Name = "ucRoundPanel7";
            this.ucRoundPanel7.Size = new System.Drawing.Size(152, 37);
            this.ucRoundPanel7.TabIndex = 29;
            // 
            // ucRoundPanel6
            // 
            this.ucRoundPanel6.BackColor = System.Drawing.Color.White;
            this.ucRoundPanel6.Location = new System.Drawing.Point(20, 291);
            this.ucRoundPanel6.Name = "ucRoundPanel6";
            this.ucRoundPanel6.Size = new System.Drawing.Size(429, 37);
            this.ucRoundPanel6.TabIndex = 27;
            // 
            // panel5
            // 
            this.panel5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.panel7);
            this.panel5.Controls.Add(this.panel6);
            this.panel5.Controls.Add(this.tabControl1);
            this.panel5.Location = new System.Drawing.Point(12, 452);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1443, 365);
            this.panel5.TabIndex = 15;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(37)))), ((int)(((byte)(59)))));
            this.panel7.Controls.Add(this.label10);
            this.panel7.Controls.Add(this.lb_Remove);
            this.panel7.Controls.Add(this.label9);
            this.panel7.Controls.Add(this.lb_Count);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel7.Location = new System.Drawing.Point(0, 320);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(1441, 43);
            this.panel7.TabIndex = 17;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label10.ForeColor = System.Drawing.Color.White;
            this.label10.Location = new System.Drawing.Point(191, 15);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(93, 15);
            this.label10.TabIndex = 18;
            this.label10.Text = "Total Excluded :";
            // 
            // lb_Remove
            // 
            this.lb_Remove.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lb_Remove.ForeColor = System.Drawing.Color.White;
            this.lb_Remove.Location = new System.Drawing.Point(290, 15);
            this.lb_Remove.Name = "lb_Remove";
            this.lb_Remove.Size = new System.Drawing.Size(79, 15);
            this.lb_Remove.TabIndex = 17;
            this.lb_Remove.Text = "0";
            this.lb_Remove.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(17, 15);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(83, 15);
            this.label9.TabIndex = 16;
            this.label9.Text = "Total Record :";
            // 
            // lb_Count
            // 
            this.lb_Count.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lb_Count.ForeColor = System.Drawing.Color.White;
            this.lb_Count.Location = new System.Drawing.Point(106, 15);
            this.lb_Count.Name = "lb_Count";
            this.lb_Count.Size = new System.Drawing.Size(79, 15);
            this.lb_Count.TabIndex = 15;
            this.lb_Count.Text = "0";
            this.lb_Count.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(37)))), ((int)(((byte)(59)))));
            this.panel6.Controls.Add(this.btn_Search);
            this.panel6.Controls.Add(this.txt_SearchRecord);
            this.panel6.Controls.Add(this.label8);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(1441, 45);
            this.panel6.TabIndex = 14;
            // 
            // btn_Search
            // 
            this.btn_Search.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_Search.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btn_Search.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btn_Search.Location = new System.Drawing.Point(1356, 7);
            this.btn_Search.Name = "btn_Search";
            this.btn_Search.Size = new System.Drawing.Size(74, 30);
            this.btn_Search.TabIndex = 14;
            this.btn_Search.Text = "Search";
            this.btn_Search.UseVisualStyleBackColor = true;
            this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
            // 
            // txt_SearchRecord
            // 
            this.txt_SearchRecord.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_SearchRecord.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_SearchRecord.Location = new System.Drawing.Point(109, 10);
            this.txt_SearchRecord.Name = "txt_SearchRecord";
            this.txt_SearchRecord.Size = new System.Drawing.Size(1241, 25);
            this.txt_SearchRecord.TabIndex = 4;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(12, 11);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 20);
            this.label8.TabIndex = 3;
            this.label8.Text = "Records";
            // 
            // panel10
            // 
            this.panel10.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel10.Controls.Add(this.dgv_BillingHistory);
            this.panel10.Controls.Add(this.panel11);
            this.panel10.Location = new System.Drawing.Point(667, 18);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(787, 415);
            this.panel10.TabIndex = 16;
            // 
            // dgv_BillingHistory
            // 
            this.dgv_BillingHistory.AllowUserToAddRows = false;
            this.dgv_BillingHistory.AllowUserToDeleteRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            this.dgv_BillingHistory.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dgv_BillingHistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgv_BillingHistory.BackgroundColor = System.Drawing.Color.White;
            this.dgv_BillingHistory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgv_BillingHistory.ColumnHeadersHeight = 35;
            this.dgv_BillingHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgv_BillingHistory.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgv_BillingHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_BillingHistory.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dgv_BillingHistory.Location = new System.Drawing.Point(0, 45);
            this.dgv_BillingHistory.Name = "dgv_BillingHistory";
            this.dgv_BillingHistory.ReadOnly = true;
            this.dgv_BillingHistory.RowHeadersVisible = false;
            this.dgv_BillingHistory.RowTemplate.Height = 40;
            this.dgv_BillingHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_BillingHistory.Size = new System.Drawing.Size(785, 368);
            this.dgv_BillingHistory.TabIndex = 15;
            this.dgv_BillingHistory.DoubleClick += new System.EventHandler(this.dgv_BillingHistory_DoubleClick);
            this.dgv_BillingHistory.Leave += new System.EventHandler(this.dgv_BillingHistory_Leave);
            // 
            // panel11
            // 
            this.panel11.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(37)))), ((int)(((byte)(59)))));
            this.panel11.Controls.Add(this.lblBillingHeader);
            this.panel11.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel11.Location = new System.Drawing.Point(0, 0);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(785, 45);
            this.panel11.TabIndex = 14;
            // 
            // lblBillingHeader
            // 
            this.lblBillingHeader.AutoSize = true;
            this.lblBillingHeader.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBillingHeader.ForeColor = System.Drawing.Color.White;
            this.lblBillingHeader.Location = new System.Drawing.Point(12, 11);
            this.lblBillingHeader.Name = "lblBillingHeader";
            this.lblBillingHeader.Size = new System.Drawing.Size(109, 20);
            this.lblBillingHeader.TabIndex = 3;
            this.lblBillingHeader.Text = "Billing History";
            // 
            // frmServicesBilling
            // 
            this.AcceptButton = this.btn_Search;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(46)))), ((int)(((byte)(59)))));
            this.ClientSize = new System.Drawing.Size(1467, 833);
            this.Controls.Add(this.panel10);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "frmServicesBilling";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Services Billing";
            this.Load += new System.EventHandler(this.frmServicesBilling_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmServicesBilling_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_BillingRecords)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_BillingRemove)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgv_BillingHistory)).EndInit();
            this.panel11.ResumeLayout(false);
            this.panel11.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgv_BillingRecords;
        private System.Windows.Forms.DataGridView dgv_BillingRemove;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRefNo;
        private System.Windows.Forms.Button btn_GenExcel;
        private System.Windows.Forms.Button btn_GenInvoice;
        private System.Windows.Forms.ComboBox cmb_BillingType;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btn_Clear;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.TextBox txt_SearchRecord;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btn_Search;
        private System.Windows.Forms.Label lb_Count;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lb_Remove;
        private System.Windows.Forms.DateTimePicker dtpRefDate;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtTerms;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.DateTimePicker dtpDueDate;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel panel4;
        private ControlObject.ucRoundPanel ucRoundPanel5;
        private System.Windows.Forms.Panel panel3;
        private ControlObject.ucRoundPanel ucRoundPanel4;
        private ControlObject.ucRoundPanel ucRoundPanel1;
        private ControlObject.ucRoundPanel ucRoundPanel2;
        private ControlObject.ucRoundPanel ucRoundPanel3;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Label lblBillingHeader;
        private System.Windows.Forms.DataGridView dgv_BillingHistory;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label4;
        private ControlObject.ucRoundPanel ucRoundPanel7;
        private System.Windows.Forms.Panel panel12;
        private ControlObject.ucRoundPanel ucRoundPanel6;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.TextBox txtDiscount;
    }
}