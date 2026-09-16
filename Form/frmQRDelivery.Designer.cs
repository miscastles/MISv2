namespace MIS
{
    partial class frmQRDelivery
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.FlowLayoutPanel pnlToolbar;
        private System.Windows.Forms.GroupBox grpService;
        private System.Windows.Forms.Label lblServiceNo;
        private System.Windows.Forms.TextBox txtServiceNo;
        private System.Windows.Forms.Label lblServiceDetails;
        private System.Windows.Forms.GroupBox grpScan;
        private System.Windows.Forms.Label lblScanHelp;
        private System.Windows.Forms.RichTextBox rtbQRContent;
        private System.Windows.Forms.Button btnValidate;
        private System.Windows.Forms.GroupBox grpResult;
        private System.Windows.Forms.DataGridView dgvValidation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colField;
        private System.Windows.Forms.DataGridViewTextBoxColumn colScanned;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMIS;
        private System.Windows.Forms.DataGridViewTextBoxColumn colResult;
        private System.Windows.Forms.Panel pnlStatus;
        private System.Windows.Forms.Label lblStatusTitle;
        private System.Windows.Forms.Label lblQRStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmQRDelivery));
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.bunifuImageButton1 = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnMinimize = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnExit = new Bunifu.Framework.UI.BunifuImageButton();
            this.lblHeader = new System.Windows.Forms.Label();
            this.pnlToolbar = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnPrintQR = new System.Windows.Forms.Button();
            this.btnHistory = new System.Windows.Forms.Button();
            this.grpService = new System.Windows.Forms.GroupBox();
            this.btnSearchService = new Bunifu.Framework.UI.BunifuImageButton();
            this.lblServiceNo = new System.Windows.Forms.Label();
            this.txtServiceNo = new System.Windows.Forms.TextBox();
            this.lblServiceDetails = new System.Windows.Forms.Label();
            this.grpScan = new System.Windows.Forms.GroupBox();
            this.lblScanHelp = new System.Windows.Forms.Label();
            this.rtbQRContent = new System.Windows.Forms.RichTextBox();
            this.btnValidate = new System.Windows.Forms.Button();
            this.grpResult = new System.Windows.Forms.GroupBox();
            this.dgvValidation = new System.Windows.Forms.DataGridView();
            this.colField = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colScanned = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMIS = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlStatus = new System.Windows.Forms.Panel();
            this.lblStatusTitle = new System.Windows.Forms.Label();
            this.lblQRStatus = new System.Windows.Forms.Label();
            this.bunifuElipse1 = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.bunifuDragControl2 = new Bunifu.Framework.UI.BunifuDragControl(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblAction = new System.Windows.Forms.Label();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabPage19 = new System.Windows.Forms.TabPage();
            this.panel25 = new System.Windows.Forms.Panel();
            this.tabPage20 = new System.Windows.Forms.TabPage();
            this.panel26 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.picQRCode = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtScanTID = new System.Windows.Forms.TextBox();
            this.txtScanMID = new System.Windows.Forms.TextBox();
            this.txtScanAddress = new System.Windows.Forms.TextBox();
            this.txtScanTerminalSN = new System.Windows.Forms.TextBox();
            this.txtScanSIMSN = new System.Windows.Forms.TextBox();
            this.txtScanMerchant = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btnScanClear = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).BeginInit();
            this.pnlToolbar.SuspendLayout();
            this.grpService.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearchService)).BeginInit();
            this.grpScan.SuspendLayout();
            this.grpResult.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvValidation)).BeginInit();
            this.pnlStatus.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl3.SuspendLayout();
            this.tabPage19.SuspendLayout();
            this.panel25.SuspendLayout();
            this.tabPage20.SuspendLayout();
            this.panel26.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picQRCode)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.DodgerBlue;
            this.pnlHeader.Controls.Add(this.bunifuImageButton1);
            this.pnlHeader.Controls.Add(this.btnMinimize);
            this.pnlHeader.Controls.Add(this.btnExit);
            this.pnlHeader.Controls.Add(this.lblHeader);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1121, 29);
            this.pnlHeader.TabIndex = 5;
            this.pnlHeader.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlHeader_Paint);
            // 
            // bunifuImageButton1
            // 
            this.bunifuImageButton1.BackColor = System.Drawing.Color.Transparent;
            this.bunifuImageButton1.Image = ((System.Drawing.Image)(resources.GetObject("bunifuImageButton1.Image")));
            this.bunifuImageButton1.ImageActive = null;
            this.bunifuImageButton1.Location = new System.Drawing.Point(3, 0);
            this.bunifuImageButton1.Name = "bunifuImageButton1";
            this.bunifuImageButton1.Size = new System.Drawing.Size(28, 30);
            this.bunifuImageButton1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.bunifuImageButton1.TabIndex = 412;
            this.bunifuImageButton1.TabStop = false;
            this.bunifuImageButton1.Zoom = 10;
            // 
            // btnMinimize
            // 
            this.btnMinimize.BackColor = System.Drawing.Color.Transparent;
            this.btnMinimize.Image = ((System.Drawing.Image)(resources.GetObject("btnMinimize.Image")));
            this.btnMinimize.ImageActive = null;
            this.btnMinimize.Location = new System.Drawing.Point(1068, 3);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(21, 21);
            this.btnMinimize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnMinimize.TabIndex = 411;
            this.btnMinimize.TabStop = false;
            this.btnMinimize.Zoom = 10;
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageActive = null;
            this.btnExit.Location = new System.Drawing.Point(1095, 3);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(21, 21);
            this.btnExit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnExit.TabIndex = 339;
            this.btnExit.TabStop = false;
            this.btnExit.Zoom = 10;
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.White;
            this.lblHeader.Location = new System.Drawing.Point(37, 5);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(103, 19);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "QR DELIVERY";
            this.lblHeader.Click += new System.EventHandler(this.lblHeader_Click);
            // 
            // pnlToolbar
            // 
            this.pnlToolbar.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlToolbar.Controls.Add(this.btnAdd);
            this.pnlToolbar.Controls.Add(this.btnSave);
            this.pnlToolbar.Controls.Add(this.btnClear);
            this.pnlToolbar.Controls.Add(this.btnPrintQR);
            this.pnlToolbar.Controls.Add(this.btnHistory);
            this.pnlToolbar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlToolbar.Location = new System.Drawing.Point(0, 710);
            this.pnlToolbar.Name = "pnlToolbar";
            this.pnlToolbar.Padding = new System.Windows.Forms.Padding(10, 8, 10, 6);
            this.pnlToolbar.Size = new System.Drawing.Size(1121, 48);
            this.pnlToolbar.TabIndex = 4;
            // 
            // btnAdd
            // 
            this.btnAdd.AutoSize = true;
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnAdd.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnAdd.Font = new System.Drawing.Font("Arial Narrow", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnAdd.Location = new System.Drawing.Point(13, 11);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(81, 27);
            this.btnAdd.TabIndex = 394;
            this.btnAdd.Text = "NEW";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnSave
            // 
            this.btnSave.AutoSize = true;
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSave.Font = new System.Drawing.Font("Arial Narrow", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnSave.Location = new System.Drawing.Point(100, 11);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(81, 27);
            this.btnSave.TabIndex = 396;
            this.btnSave.Text = "SAVE";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClear
            // 
            this.btnClear.AutoSize = true;
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnClear.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnClear.Font = new System.Drawing.Font("Arial Narrow", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnClear.Location = new System.Drawing.Point(187, 11);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(81, 27);
            this.btnClear.TabIndex = 397;
            this.btnClear.Text = "CLEAR";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnPrintQR
            // 
            this.btnPrintQR.AutoSize = true;
            this.btnPrintQR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnPrintQR.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnPrintQR.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnPrintQR.Font = new System.Drawing.Font("Arial Narrow", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrintQR.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnPrintQR.Location = new System.Drawing.Point(274, 11);
            this.btnPrintQR.Name = "btnPrintQR";
            this.btnPrintQR.Size = new System.Drawing.Size(181, 27);
            this.btnPrintQR.TabIndex = 398;
            this.btnPrintQR.Text = "PRINT QR";
            this.btnPrintQR.UseVisualStyleBackColor = false;
            // 
            // btnHistory
            // 
            this.btnHistory.AutoSize = true;
            this.btnHistory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnHistory.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnHistory.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnHistory.Font = new System.Drawing.Font("Arial Narrow", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHistory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnHistory.Location = new System.Drawing.Point(461, 11);
            this.btnHistory.Name = "btnHistory";
            this.btnHistory.Size = new System.Drawing.Size(81, 27);
            this.btnHistory.TabIndex = 399;
            this.btnHistory.Text = "HISTORY";
            this.btnHistory.UseVisualStyleBackColor = false;
            this.btnHistory.Click += new System.EventHandler(this.btnHistory_Click);
            // 
            // grpService
            // 
            this.grpService.Controls.Add(this.btnSearchService);
            this.grpService.Controls.Add(this.lblServiceNo);
            this.grpService.Controls.Add(this.txtServiceNo);
            this.grpService.Controls.Add(this.lblServiceDetails);
            this.grpService.ForeColor = System.Drawing.Color.Navy;
            this.grpService.Location = new System.Drawing.Point(14, 43);
            this.grpService.Name = "grpService";
            this.grpService.Size = new System.Drawing.Size(1090, 82);
            this.grpService.TabIndex = 3;
            this.grpService.TabStop = false;
            this.grpService.Tag = "SERVICE INFORMATION";
            this.grpService.Paint += new System.Windows.Forms.PaintEventHandler(this.sectionGroup_Paint);
            // 
            // btnSearchService
            // 
            this.btnSearchService.BackColor = System.Drawing.Color.Transparent;
            this.btnSearchService.Enabled = false;
            this.btnSearchService.Image = ((System.Drawing.Image)(resources.GetObject("btnSearchService.Image")));
            this.btnSearchService.ImageActive = null;
            this.btnSearchService.Location = new System.Drawing.Point(354, 40);
            this.btnSearchService.Name = "btnSearchService";
            this.btnSearchService.Size = new System.Drawing.Size(24, 25);
            this.btnSearchService.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnSearchService.TabIndex = 390;
            this.btnSearchService.TabStop = false;
            this.btnSearchService.Zoom = 10;
            // 
            // lblServiceNo
            // 
            this.lblServiceNo.AutoSize = true;
            this.lblServiceNo.ForeColor = System.Drawing.Color.Black;
            this.lblServiceNo.Location = new System.Drawing.Point(15, 25);
            this.lblServiceNo.Name = "lblServiceNo";
            this.lblServiceNo.Size = new System.Drawing.Size(55, 16);
            this.lblServiceNo.TabIndex = 0;
            this.lblServiceNo.Text = "QR ID:";
            // 
            // txtServiceNo
            // 
            this.txtServiceNo.BackColor = System.Drawing.Color.White;
            this.txtServiceNo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtServiceNo.Font = new System.Drawing.Font("Courier New", 10F);
            this.txtServiceNo.ForeColor = System.Drawing.Color.Black;
            this.txtServiceNo.Location = new System.Drawing.Point(18, 42);
            this.txtServiceNo.Name = "txtServiceNo";
            this.txtServiceNo.Size = new System.Drawing.Size(330, 23);
            this.txtServiceNo.TabIndex = 1;
            // 
            // lblServiceDetails
            // 
            this.lblServiceDetails.Font = new System.Drawing.Font("Courier New", 9F);
            this.lblServiceDetails.ForeColor = System.Drawing.Color.Black;
            this.lblServiceDetails.Location = new System.Drawing.Point(480, 20);
            this.lblServiceDetails.Name = "lblServiceDetails";
            this.lblServiceDetails.Size = new System.Drawing.Size(595, 48);
            this.lblServiceDetails.TabIndex = 3;
            this.lblServiceDetails.Text = "IR ID NO.: 77125       MERCHANT ID: 20914       PROCESSED BY: CURRENT USER";
            this.lblServiceDetails.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // grpScan
            // 
            this.grpScan.Controls.Add(this.btnScanClear);
            this.grpScan.Controls.Add(this.lblScanHelp);
            this.grpScan.Controls.Add(this.rtbQRContent);
            this.grpScan.Controls.Add(this.btnValidate);
            this.grpScan.ForeColor = System.Drawing.Color.Navy;
            this.grpScan.Location = new System.Drawing.Point(14, 135);
            this.grpScan.Name = "grpScan";
            this.grpScan.Size = new System.Drawing.Size(669, 226);
            this.grpScan.TabIndex = 2;
            this.grpScan.TabStop = false;
            this.grpScan.Tag = "TERMINAL QR SCAN";
            this.grpScan.Paint += new System.Windows.Forms.PaintEventHandler(this.sectionGroup_Paint);
            // 
            // lblScanHelp
            // 
            this.lblScanHelp.AutoSize = true;
            this.lblScanHelp.ForeColor = System.Drawing.Color.Black;
            this.lblScanHelp.Location = new System.Drawing.Point(15, 17);
            this.lblScanHelp.Name = "lblScanHelp";
            this.lblScanHelp.Size = new System.Drawing.Size(103, 16);
            this.lblScanHelp.TabIndex = 0;
            this.lblScanHelp.Text = "SCAN QR CODE";
            // 
            // rtbQRContent
            // 
            this.rtbQRContent.BackColor = System.Drawing.Color.White;
            this.rtbQRContent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbQRContent.Font = new System.Drawing.Font("Courier New", 10F);
            this.rtbQRContent.ForeColor = System.Drawing.Color.Black;
            this.rtbQRContent.Location = new System.Drawing.Point(15, 36);
            this.rtbQRContent.Name = "rtbQRContent";
            this.rtbQRContent.Size = new System.Drawing.Size(513, 181);
            this.rtbQRContent.TabIndex = 1;
            this.rtbQRContent.Text = "";
            this.rtbQRContent.TextChanged += new System.EventHandler(this.rtbQRContent_TextChanged);
            // 
            // btnValidate
            // 
            this.btnValidate.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnValidate.ForeColor = System.Drawing.Color.White;
            this.btnValidate.Location = new System.Drawing.Point(533, 36);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(134, 44);
            this.btnValidate.TabIndex = 2;
            this.btnValidate.Text = "VALIDATE";
            this.btnValidate.UseVisualStyleBackColor = false;
            this.btnValidate.Click += new System.EventHandler(this.btnValidate_Click);
            // 
            // grpResult
            // 
            this.grpResult.Controls.Add(this.dgvValidation);
            this.grpResult.ForeColor = System.Drawing.Color.Navy;
            this.grpResult.Location = new System.Drawing.Point(14, 371);
            this.grpResult.Name = "grpResult";
            this.grpResult.Size = new System.Drawing.Size(820, 332);
            this.grpResult.TabIndex = 1;
            this.grpResult.TabStop = false;
            this.grpResult.Tag = "VALIDATION RESULT";
            this.grpResult.Paint += new System.Windows.Forms.PaintEventHandler(this.sectionGroup_Paint);
            // 
            // dgvValidation
            // 
            this.dgvValidation.AllowUserToAddRows = false;
            this.dgvValidation.AllowUserToDeleteRows = false;
            this.dgvValidation.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvValidation.BackgroundColor = System.Drawing.Color.White;
            this.dgvValidation.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colField,
            this.colScanned,
            this.colMIS,
            this.colResult});
            this.dgvValidation.Location = new System.Drawing.Point(15, 27);
            this.dgvValidation.Name = "dgvValidation";
            this.dgvValidation.ReadOnly = true;
            this.dgvValidation.RowHeadersVisible = false;
            this.dgvValidation.Size = new System.Drawing.Size(790, 285);
            this.dgvValidation.TabIndex = 0;
            this.dgvValidation.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvValidation_CellContentClick);
            // 
            // colField
            // 
            this.colField.HeaderText = "FIELD";
            this.colField.Name = "colField";
            this.colField.ReadOnly = true;
            // 
            // colScanned
            // 
            this.colScanned.HeaderText = "SCANNED QR";
            this.colScanned.Name = "colScanned";
            this.colScanned.ReadOnly = true;
            // 
            // colMIS
            // 
            this.colMIS.HeaderText = "MIS RECORD";
            this.colMIS.Name = "colMIS";
            this.colMIS.ReadOnly = true;
            // 
            // colResult
            // 
            this.colResult.HeaderText = "RESULT";
            this.colResult.Name = "colResult";
            this.colResult.ReadOnly = true;
            // 
            // pnlStatus
            // 
            this.pnlStatus.BackColor = System.Drawing.Color.White;
            this.pnlStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlStatus.Controls.Add(this.lblStatusTitle);
            this.pnlStatus.Controls.Add(this.lblQRStatus);
            this.pnlStatus.Location = new System.Drawing.Point(3, 2);
            this.pnlStatus.Name = "pnlStatus";
            this.pnlStatus.Size = new System.Drawing.Size(256, 207);
            this.pnlStatus.TabIndex = 0;
            // 
            // lblStatusTitle
            // 
            this.lblStatusTitle.BackColor = System.Drawing.Color.DodgerBlue;
            this.lblStatusTitle.ForeColor = System.Drawing.Color.White;
            this.lblStatusTitle.Location = new System.Drawing.Point(0, 0);
            this.lblStatusTitle.Name = "lblStatusTitle";
            this.lblStatusTitle.Size = new System.Drawing.Size(254, 25);
            this.lblStatusTitle.TabIndex = 0;
            this.lblStatusTitle.Text = "OVERALL QR STATUS";
            this.lblStatusTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblQRStatus
            // 
            this.lblQRStatus.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQRStatus.ForeColor = System.Drawing.Color.Silver;
            this.lblQRStatus.Location = new System.Drawing.Point(6, 26);
            this.lblQRStatus.Name = "lblQRStatus";
            this.lblQRStatus.Size = new System.Drawing.Size(242, 172);
            this.lblQRStatus.TabIndex = 1;
            this.lblQRStatus.Text = "NOT VALIDATED";
            this.lblQRStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bunifuElipse1
            // 
            this.bunifuElipse1.ElipseRadius = 5;
            this.bunifuElipse1.TargetControl = this;
            // 
            // bunifuDragControl2
            // 
            this.bunifuDragControl2.Fixed = true;
            this.bunifuDragControl2.Horizontal = true;
            this.bunifuDragControl2.TargetControl = this.pnlHeader;
            this.bunifuDragControl2.Vertical = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lblAction);
            this.panel1.Location = new System.Drawing.Point(848, 622);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(261, 79);
            this.panel1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.DodgerBlue;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(259, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "ACTION";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAction
            // 
            this.lblAction.Font = new System.Drawing.Font("Century Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAction.ForeColor = System.Drawing.Color.Blue;
            this.lblAction.Location = new System.Drawing.Point(6, 30);
            this.lblAction.Name = "lblAction";
            this.lblAction.Size = new System.Drawing.Size(248, 42);
            this.lblAction.TabIndex = 1;
            this.lblAction.Text = "NEXT SCAN";
            this.lblAction.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabControl3
            // 
            this.tabControl3.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl3.Controls.Add(this.tabPage19);
            this.tabControl3.Controls.Add(this.tabPage20);
            this.tabControl3.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl3.Location = new System.Drawing.Point(840, 371);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.Size = new System.Drawing.Size(276, 252);
            this.tabControl3.TabIndex = 496;
            // 
            // tabPage19
            // 
            this.tabPage19.Controls.Add(this.panel25);
            this.tabPage19.ForeColor = System.Drawing.Color.Black;
            this.tabPage19.Location = new System.Drawing.Point(4, 27);
            this.tabPage19.Name = "tabPage19";
            this.tabPage19.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage19.Size = new System.Drawing.Size(268, 221);
            this.tabPage19.TabIndex = 3;
            this.tabPage19.Text = "OVERSALL STATUS";
            this.tabPage19.UseVisualStyleBackColor = true;
            // 
            // panel25
            // 
            this.panel25.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel25.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel25.Controls.Add(this.pnlStatus);
            this.panel25.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel25.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel25.Location = new System.Drawing.Point(3, 3);
            this.panel25.Name = "panel25";
            this.panel25.Size = new System.Drawing.Size(262, 215);
            this.panel25.TabIndex = 474;
            // 
            // tabPage20
            // 
            this.tabPage20.Controls.Add(this.panel26);
            this.tabPage20.ForeColor = System.Drawing.Color.Black;
            this.tabPage20.Location = new System.Drawing.Point(4, 27);
            this.tabPage20.Name = "tabPage20";
            this.tabPage20.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage20.Size = new System.Drawing.Size(268, 221);
            this.tabPage20.TabIndex = 2;
            this.tabPage20.Text = "INTERNAL";
            this.tabPage20.UseVisualStyleBackColor = true;
            // 
            // panel26
            // 
            this.panel26.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel26.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel26.Controls.Add(this.panel2);
            this.panel26.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel26.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel26.Location = new System.Drawing.Point(3, 3);
            this.panel26.Name = "panel26";
            this.panel26.Size = new System.Drawing.Size(262, 215);
            this.panel26.TabIndex = 474;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.picQRCode);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(256, 207);
            this.panel2.TabIndex = 1;
            // 
            // picQRCode
            // 
            this.picQRCode.Location = new System.Drawing.Point(3, 28);
            this.picQRCode.Name = "picQRCode";
            this.picQRCode.Size = new System.Drawing.Size(249, 174);
            this.picQRCode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picQRCode.TabIndex = 1;
            this.picQRCode.TabStop = false;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.DodgerBlue;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(254, 25);
            this.label2.TabIndex = 0;
            this.label2.Text = "QRCODE";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtScanMerchant);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.txtScanSIMSN);
            this.groupBox1.Controls.Add(this.txtScanTerminalSN);
            this.groupBox1.Controls.Add(this.txtScanAddress);
            this.groupBox1.Controls.Add(this.txtScanMID);
            this.groupBox1.Controls.Add(this.txtScanTID);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(689, 135);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(415, 227);
            this.groupBox1.TabIndex = 497;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "SCAN DETAILS";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(6, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 16);
            this.label3.TabIndex = 1;
            this.label3.Text = "TID";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(6, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(31, 16);
            this.label4.TabIndex = 2;
            this.label4.Text = "MID";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(6, 175);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 16);
            this.label5.TabIndex = 3;
            this.label5.Text = "TERMINAL SN";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(6, 201);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 16);
            this.label6.TabIndex = 4;
            this.label6.Text = "SIM SN";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(6, 107);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 16);
            this.label7.TabIndex = 5;
            this.label7.Text = "ADDRESS";
            // 
            // txtScanTID
            // 
            this.txtScanTID.BackColor = System.Drawing.Color.White;
            this.txtScanTID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtScanTID.Font = new System.Drawing.Font("Courier New", 10F);
            this.txtScanTID.ForeColor = System.Drawing.Color.Black;
            this.txtScanTID.Location = new System.Drawing.Point(108, 50);
            this.txtScanTID.Name = "txtScanTID";
            this.txtScanTID.Size = new System.Drawing.Size(303, 23);
            this.txtScanTID.TabIndex = 6;
            // 
            // txtScanMID
            // 
            this.txtScanMID.BackColor = System.Drawing.Color.White;
            this.txtScanMID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtScanMID.Font = new System.Drawing.Font("Courier New", 10F);
            this.txtScanMID.ForeColor = System.Drawing.Color.Black;
            this.txtScanMID.Location = new System.Drawing.Point(108, 75);
            this.txtScanMID.Name = "txtScanMID";
            this.txtScanMID.Size = new System.Drawing.Size(303, 23);
            this.txtScanMID.TabIndex = 7;
            // 
            // txtScanAddress
            // 
            this.txtScanAddress.BackColor = System.Drawing.Color.White;
            this.txtScanAddress.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtScanAddress.Font = new System.Drawing.Font("Courier New", 10F);
            this.txtScanAddress.ForeColor = System.Drawing.Color.Black;
            this.txtScanAddress.Location = new System.Drawing.Point(108, 100);
            this.txtScanAddress.Multiline = true;
            this.txtScanAddress.Name = "txtScanAddress";
            this.txtScanAddress.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtScanAddress.Size = new System.Drawing.Size(303, 66);
            this.txtScanAddress.TabIndex = 8;
            // 
            // txtScanTerminalSN
            // 
            this.txtScanTerminalSN.BackColor = System.Drawing.Color.White;
            this.txtScanTerminalSN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtScanTerminalSN.Font = new System.Drawing.Font("Courier New", 10F);
            this.txtScanTerminalSN.ForeColor = System.Drawing.Color.Black;
            this.txtScanTerminalSN.Location = new System.Drawing.Point(108, 169);
            this.txtScanTerminalSN.Name = "txtScanTerminalSN";
            this.txtScanTerminalSN.Size = new System.Drawing.Size(303, 23);
            this.txtScanTerminalSN.TabIndex = 9;
            // 
            // txtScanSIMSN
            // 
            this.txtScanSIMSN.BackColor = System.Drawing.Color.White;
            this.txtScanSIMSN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtScanSIMSN.Font = new System.Drawing.Font("Courier New", 10F);
            this.txtScanSIMSN.ForeColor = System.Drawing.Color.Black;
            this.txtScanSIMSN.Location = new System.Drawing.Point(108, 194);
            this.txtScanSIMSN.Name = "txtScanSIMSN";
            this.txtScanSIMSN.Size = new System.Drawing.Size(303, 23);
            this.txtScanSIMSN.TabIndex = 10;
            // 
            // txtScanMerchant
            // 
            this.txtScanMerchant.BackColor = System.Drawing.Color.White;
            this.txtScanMerchant.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtScanMerchant.Font = new System.Drawing.Font("Courier New", 10F);
            this.txtScanMerchant.ForeColor = System.Drawing.Color.Black;
            this.txtScanMerchant.Location = new System.Drawing.Point(108, 25);
            this.txtScanMerchant.Name = "txtScanMerchant";
            this.txtScanMerchant.Size = new System.Drawing.Size(303, 23);
            this.txtScanMerchant.TabIndex = 12;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(5, 27);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 16);
            this.label8.TabIndex = 11;
            this.label8.Text = "MERCHANT";
            // 
            // btnScanClear
            // 
            this.btnScanClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnScanClear.ForeColor = System.Drawing.Color.White;
            this.btnScanClear.Location = new System.Drawing.Point(533, 87);
            this.btnScanClear.Name = "btnScanClear";
            this.btnScanClear.Size = new System.Drawing.Size(134, 44);
            this.btnScanClear.TabIndex = 3;
            this.btnScanClear.Text = "CLEAR";
            this.btnScanClear.UseVisualStyleBackColor = false;
            this.btnScanClear.Click += new System.EventHandler(this.btnScanClear_Click);
            // 
            // frmQRDelivery
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(247)))), ((int)(((byte)(247)))));
            this.ClientSize = new System.Drawing.Size(1121, 758);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tabControl3);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grpResult);
            this.Controls.Add(this.grpScan);
            this.Controls.Add(this.grpService);
            this.Controls.Add(this.pnlToolbar);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Courier New", 9.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.Name = "frmQRDelivery";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QR DELIVERY";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).EndInit();
            this.pnlToolbar.ResumeLayout(false);
            this.pnlToolbar.PerformLayout();
            this.grpService.ResumeLayout(false);
            this.grpService.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearchService)).EndInit();
            this.grpScan.ResumeLayout(false);
            this.grpScan.PerformLayout();
            this.grpResult.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvValidation)).EndInit();
            this.pnlStatus.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabControl3.ResumeLayout(false);
            this.tabPage19.ResumeLayout(false);
            this.panel25.ResumeLayout(false);
            this.tabPage20.ResumeLayout(false);
            this.panel26.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picQRCode)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        private void ConfigureButton(System.Windows.Forms.Button button, string text, System.EventHandler handler)
        {
            button.BackColor = System.Drawing.Color.FromArgb(70, 70, 70); button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button.ForeColor = System.Drawing.Color.White; button.Margin = new System.Windows.Forms.Padding(3); button.Size = new System.Drawing.Size(125, 28);
            button.Text = text; button.UseVisualStyleBackColor = false; button.Click += handler;
        }

        private Bunifu.Framework.UI.BunifuImageButton btnExit;
        private Bunifu.Framework.UI.BunifuImageButton btnMinimize;
        private Bunifu.Framework.UI.BunifuImageButton bunifuImageButton1;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnPrintQR;
        private System.Windows.Forms.Button btnHistory;
        private Bunifu.Framework.UI.BunifuElipse bunifuElipse1;
        private Bunifu.Framework.UI.BunifuDragControl bunifuDragControl2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblAction;
        private Bunifu.Framework.UI.BunifuImageButton btnSearchService;
        private System.Windows.Forms.TabControl tabControl3;
        private System.Windows.Forms.TabPage tabPage19;
        private System.Windows.Forms.Panel panel25;
        private System.Windows.Forms.TabPage tabPage20;
        private System.Windows.Forms.Panel panel26;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox picQRCode;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.TextBox txtScanSIMSN;
        private System.Windows.Forms.TextBox txtScanTerminalSN;
        private System.Windows.Forms.TextBox txtScanAddress;
        private System.Windows.Forms.TextBox txtScanMID;
        private System.Windows.Forms.TextBox txtScanTID;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtScanMerchant;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnScanClear;
    }
}
