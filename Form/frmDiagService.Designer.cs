namespace MIS
{
    partial class frmDiagService
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDiagService));
            this.bunifuElipse1 = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.txtLineNo = new System.Windows.Forms.TextBox();
            this.bunifuImageButton1 = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnExit = new Bunifu.Framework.UI.BunifuImageButton();
            this.lblHeader = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.pnlPreviewImage = new System.Windows.Forms.Panel();
            this.cboClient = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cboServiceStatus = new System.Windows.Forms.ComboBox();
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.panel36 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel37 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtCurTerminalType = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lvwList = new System.Windows.Forms.ListView();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblHeaderPreviewImage = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnClear = new System.Windows.Forms.Button();
            this.ucKpiCard10Days = new MIS.ControlObject.ucKpiCard();
            this.ucKpiCard180Days = new MIS.ControlObject.ucKpiCard();
            this.ucKpiCard90Days = new MIS.ControlObject.ucKpiCard();
            this.ucKpiCard30Days = new MIS.ControlObject.ucKpiCard();
            this.ucKpiCardWithinTAT = new MIS.ControlObject.ucKpiCard();
            this.ucKpiCardBeyondTAT = new MIS.ControlObject.ucKpiCard();
            this.ucKpiCardTotalJobs = new MIS.ControlObject.ucKpiCard();
            this.ucKpiCard5Days = new MIS.ControlObject.ucKpiCard();
            this.bunifuDragControl2 = new Bunifu.Framework.UI.BunifuDragControl(this.components);
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).BeginInit();
            this.pnlPreviewImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.panel36.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // bunifuElipse1
            // 
            this.bunifuElipse1.ElipseRadius = 5;
            this.bunifuElipse1.TargetControl = this;
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.Maroon;
            this.pnlHeader.Controls.Add(this.txtLineNo);
            this.pnlHeader.Controls.Add(this.bunifuImageButton1);
            this.pnlHeader.Controls.Add(this.btnExit);
            this.pnlHeader.Controls.Add(this.lblHeader);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1558, 29);
            this.pnlHeader.TabIndex = 401;
            // 
            // txtLineNo
            // 
            this.txtLineNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtLineNo.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLineNo.Location = new System.Drawing.Point(274, 4);
            this.txtLineNo.Name = "txtLineNo";
            this.txtLineNo.Size = new System.Drawing.Size(60, 20);
            this.txtLineNo.TabIndex = 306;
            this.txtLineNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtLineNo.Visible = false;
            // 
            // bunifuImageButton1
            // 
            this.bunifuImageButton1.BackColor = System.Drawing.Color.Maroon;
            this.bunifuImageButton1.Image = ((System.Drawing.Image)(resources.GetObject("bunifuImageButton1.Image")));
            this.bunifuImageButton1.ImageActive = null;
            this.bunifuImageButton1.Location = new System.Drawing.Point(3, 2);
            this.bunifuImageButton1.Name = "bunifuImageButton1";
            this.bunifuImageButton1.Size = new System.Drawing.Size(26, 25);
            this.bunifuImageButton1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.bunifuImageButton1.TabIndex = 108;
            this.bunifuImageButton1.TabStop = false;
            this.bunifuImageButton1.Zoom = 10;
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Maroon;
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageActive = null;
            this.btnExit.Location = new System.Drawing.Point(1522, 2);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(25, 25);
            this.btnExit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnExit.TabIndex = 8;
            this.btnExit.TabStop = false;
            this.btnExit.Zoom = 10;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.White;
            this.lblHeader.Location = new System.Drawing.Point(35, 4);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(165, 18);
            this.lblHeader.TabIndex = 7;
            this.lblHeader.Text = "SERVICE DIAGNOSTIC";
            // 
            // pnlPreviewImage
            // 
            this.pnlPreviewImage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlPreviewImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPreviewImage.Controls.Add(this.cboClient);
            this.pnlPreviewImage.Controls.Add(this.label7);
            this.pnlPreviewImage.Controls.Add(this.btnRefresh);
            this.pnlPreviewImage.Controls.Add(this.label2);
            this.pnlPreviewImage.Controls.Add(this.cboServiceStatus);
            this.pnlPreviewImage.Controls.Add(this.picPreview);
            this.pnlPreviewImage.Controls.Add(this.panel36);
            this.pnlPreviewImage.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlPreviewImage.Location = new System.Drawing.Point(3, 33);
            this.pnlPreviewImage.Name = "pnlPreviewImage";
            this.pnlPreviewImage.Size = new System.Drawing.Size(1550, 62);
            this.pnlPreviewImage.TabIndex = 469;
            // 
            // cboClient
            // 
            this.cboClient.BackColor = System.Drawing.Color.White;
            this.cboClient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboClient.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboClient.ForeColor = System.Drawing.Color.Black;
            this.cboClient.FormattingEnabled = true;
            this.cboClient.Location = new System.Drawing.Point(910, 31);
            this.cboClient.Name = "cboClient";
            this.cboClient.Size = new System.Drawing.Size(144, 24);
            this.cboClient.TabIndex = 362;
            this.cboClient.SelectedIndexChanged += new System.EventHandler(this.cboClient_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(848, 36);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 14);
            this.label7.TabIndex = 361;
            this.label7.Text = "CLIENT:";
            // 
            // btnRefresh
            // 
            this.btnRefresh.AutoSize = true;
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnRefresh.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnRefresh.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnRefresh.Location = new System.Drawing.Point(1397, 27);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(146, 29);
            this.btnRefresh.TabIndex = 352;
            this.btnRefresh.Text = "REFRESH";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(1057, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 14);
            this.label2.TabIndex = 351;
            this.label2.Text = "CONDITION:";
            // 
            // cboServiceStatus
            // 
            this.cboServiceStatus.BackColor = System.Drawing.Color.White;
            this.cboServiceStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboServiceStatus.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboServiceStatus.ForeColor = System.Drawing.Color.Black;
            this.cboServiceStatus.FormattingEnabled = true;
            this.cboServiceStatus.Items.AddRange(new object[] {
            "[NOT SPECIFIED]",
            "PROCESSING",
            "PENDING"});
            this.cboServiceStatus.Location = new System.Drawing.Point(1140, 30);
            this.cboServiceStatus.Name = "cboServiceStatus";
            this.cboServiceStatus.Size = new System.Drawing.Size(251, 24);
            this.cboServiceStatus.TabIndex = 350;
            // 
            // picPreview
            // 
            this.picPreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.picPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picPreview.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picPreview.Location = new System.Drawing.Point(0, 24);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(1548, 36);
            this.picPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picPreview.TabIndex = 313;
            this.picPreview.TabStop = false;
            // 
            // panel36
            // 
            this.panel36.BackColor = System.Drawing.Color.Gainsboro;
            this.panel36.Controls.Add(this.label1);
            this.panel36.Controls.Add(this.panel37);
            this.panel36.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel36.Location = new System.Drawing.Point(0, 0);
            this.panel36.Name = "panel36";
            this.panel36.Size = new System.Drawing.Size(1548, 24);
            this.panel36.TabIndex = 311;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(3, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 14);
            this.label1.TabIndex = 310;
            this.label1.Text = "FILTER OPTION";
            // 
            // panel37
            // 
            this.panel37.BackColor = System.Drawing.Color.Silver;
            this.panel37.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel37.Location = new System.Drawing.Point(0, 22);
            this.panel37.Name = "panel37";
            this.panel37.Size = new System.Drawing.Size(1548, 2);
            this.panel37.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnExport);
            this.panel1.Controls.Add(this.btnSearch);
            this.panel1.Controls.Add(this.txtCurTerminalType);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.lvwList);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(4, 204);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1549, 485);
            this.panel1.TabIndex = 470;
            // 
            // btnExport
            // 
            this.btnExport.AutoSize = true;
            this.btnExport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnExport.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnExport.FlatAppearance.BorderSize = 0;
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnExport.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExport.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnExport.Location = new System.Drawing.Point(1396, 27);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(143, 31);
            this.btnExport.TabIndex = 355;
            this.btnExport.Text = "EXPORT";
            this.btnExport.UseVisualStyleBackColor = false;
            // 
            // btnSearch
            // 
            this.btnSearch.AutoSize = true;
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnSearch.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSearch.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnSearch.Location = new System.Drawing.Point(1247, 27);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(143, 31);
            this.btnSearch.TabIndex = 354;
            this.btnSearch.Text = "SEARCH";
            this.btnSearch.UseVisualStyleBackColor = false;
            // 
            // txtCurTerminalType
            // 
            this.txtCurTerminalType.BackColor = System.Drawing.Color.White;
            this.txtCurTerminalType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtCurTerminalType.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtCurTerminalType.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCurTerminalType.Location = new System.Drawing.Point(87, 30);
            this.txtCurTerminalType.Name = "txtCurTerminalType";
            this.txtCurTerminalType.ReadOnly = true;
            this.txtCurTerminalType.Size = new System.Drawing.Size(1157, 26);
            this.txtCurTerminalType.TabIndex = 353;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(3, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 18);
            this.label3.TabIndex = 352;
            this.label3.Text = "SEARCH:";
            // 
            // lvwList
            // 
            this.lvwList.BackColor = System.Drawing.Color.GhostWhite;
            this.lvwList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvwList.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvwList.ForeColor = System.Drawing.Color.Black;
            this.lvwList.FullRowSelect = true;
            this.lvwList.HideSelection = false;
            this.lvwList.Location = new System.Drawing.Point(-1, 62);
            this.lvwList.Name = "lvwList";
            this.lvwList.Size = new System.Drawing.Size(1546, 419);
            this.lvwList.TabIndex = 314;
            this.lvwList.UseCompatibleStateImageBehavior = false;
            this.lvwList.View = System.Windows.Forms.View.Details;
            this.lvwList.DoubleClick += new System.EventHandler(this.lvwList_DoubleClick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 24);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1547, 459);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 313;
            this.pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Gainsboro;
            this.panel2.Controls.Add(this.lblHeaderPreviewImage);
            this.panel2.Controls.Add(this.panel3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1547, 24);
            this.panel2.TabIndex = 311;
            // 
            // lblHeaderPreviewImage
            // 
            this.lblHeaderPreviewImage.AutoSize = true;
            this.lblHeaderPreviewImage.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeaderPreviewImage.ForeColor = System.Drawing.Color.Black;
            this.lblHeaderPreviewImage.Location = new System.Drawing.Point(3, 4);
            this.lblHeaderPreviewImage.Name = "lblHeaderPreviewImage";
            this.lblHeaderPreviewImage.Size = new System.Drawing.Size(0, 14);
            this.lblHeaderPreviewImage.TabIndex = 310;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Silver;
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 22);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1547, 2);
            this.panel3.TabIndex = 0;
            // 
            // btnClear
            // 
            this.btnClear.AutoSize = true;
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnClear.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnClear.FlatAppearance.BorderSize = 0;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnClear.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnClear.Location = new System.Drawing.Point(3, 693);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(143, 29);
            this.btnClear.TabIndex = 471;
            this.btnClear.Text = "CLEAR";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // ucKpiCard10Days
            // 
            this.ucKpiCard10Days.Description = "Description";
            this.ucKpiCard10Days.DescriptionColor = System.Drawing.SystemColors.ControlText;
            this.ucKpiCard10Days.Location = new System.Drawing.Point(779, 97);
            this.ucKpiCard10Days.Name = "ucKpiCard10Days";
            this.ucKpiCard10Days.Size = new System.Drawing.Size(192, 102);
            this.ucKpiCard10Days.TabIndex = 472;
            this.ucKpiCard10Days.Title = "TITLE";
            this.ucKpiCard10Days.TitleColor = System.Drawing.Color.Black;
            this.ucKpiCard10Days.Value = "0";
            this.ucKpiCard10Days.ValueColor = System.Drawing.SystemColors.ControlText;
            // 
            // ucKpiCard180Days
            // 
            this.ucKpiCard180Days.Description = "Description";
            this.ucKpiCard180Days.DescriptionColor = System.Drawing.SystemColors.ControlText;
            this.ucKpiCard180Days.Location = new System.Drawing.Point(1360, 96);
            this.ucKpiCard180Days.Name = "ucKpiCard180Days";
            this.ucKpiCard180Days.Size = new System.Drawing.Size(192, 102);
            this.ucKpiCard180Days.TabIndex = 407;
            this.ucKpiCard180Days.Title = "TITLE";
            this.ucKpiCard180Days.TitleColor = System.Drawing.Color.Black;
            this.ucKpiCard180Days.Value = "0";
            this.ucKpiCard180Days.ValueColor = System.Drawing.SystemColors.ControlText;
            // 
            // ucKpiCard90Days
            // 
            this.ucKpiCard90Days.Description = "Description";
            this.ucKpiCard90Days.DescriptionColor = System.Drawing.SystemColors.ControlText;
            this.ucKpiCard90Days.Location = new System.Drawing.Point(1166, 96);
            this.ucKpiCard90Days.Name = "ucKpiCard90Days";
            this.ucKpiCard90Days.Size = new System.Drawing.Size(192, 102);
            this.ucKpiCard90Days.TabIndex = 406;
            this.ucKpiCard90Days.Title = "TITLE";
            this.ucKpiCard90Days.TitleColor = System.Drawing.Color.Black;
            this.ucKpiCard90Days.Value = "0";
            this.ucKpiCard90Days.ValueColor = System.Drawing.SystemColors.ControlText;
            // 
            // ucKpiCard30Days
            // 
            this.ucKpiCard30Days.Description = "Description";
            this.ucKpiCard30Days.DescriptionColor = System.Drawing.SystemColors.ControlText;
            this.ucKpiCard30Days.Location = new System.Drawing.Point(972, 96);
            this.ucKpiCard30Days.Name = "ucKpiCard30Days";
            this.ucKpiCard30Days.Size = new System.Drawing.Size(192, 102);
            this.ucKpiCard30Days.TabIndex = 405;
            this.ucKpiCard30Days.Title = "TITLE";
            this.ucKpiCard30Days.TitleColor = System.Drawing.Color.Black;
            this.ucKpiCard30Days.Value = "0";
            this.ucKpiCard30Days.ValueColor = System.Drawing.SystemColors.ControlText;
            // 
            // ucKpiCardWithinTAT
            // 
            this.ucKpiCardWithinTAT.Description = "Description";
            this.ucKpiCardWithinTAT.DescriptionColor = System.Drawing.SystemColors.ControlText;
            this.ucKpiCardWithinTAT.Location = new System.Drawing.Point(391, 96);
            this.ucKpiCardWithinTAT.Name = "ucKpiCardWithinTAT";
            this.ucKpiCardWithinTAT.Size = new System.Drawing.Size(192, 102);
            this.ucKpiCardWithinTAT.TabIndex = 404;
            this.ucKpiCardWithinTAT.Title = "TITLE";
            this.ucKpiCardWithinTAT.TitleColor = System.Drawing.Color.Black;
            this.ucKpiCardWithinTAT.Value = "0";
            this.ucKpiCardWithinTAT.ValueColor = System.Drawing.SystemColors.ControlText;
            // 
            // ucKpiCardBeyondTAT
            // 
            this.ucKpiCardBeyondTAT.Description = "Description";
            this.ucKpiCardBeyondTAT.DescriptionColor = System.Drawing.SystemColors.ControlText;
            this.ucKpiCardBeyondTAT.Location = new System.Drawing.Point(197, 96);
            this.ucKpiCardBeyondTAT.Name = "ucKpiCardBeyondTAT";
            this.ucKpiCardBeyondTAT.Size = new System.Drawing.Size(192, 102);
            this.ucKpiCardBeyondTAT.TabIndex = 403;
            this.ucKpiCardBeyondTAT.Title = "TITLE";
            this.ucKpiCardBeyondTAT.TitleColor = System.Drawing.Color.Black;
            this.ucKpiCardBeyondTAT.Value = "0";
            this.ucKpiCardBeyondTAT.ValueColor = System.Drawing.SystemColors.ControlText;
            // 
            // ucKpiCardTotalJobs
            // 
            this.ucKpiCardTotalJobs.Description = "Description";
            this.ucKpiCardTotalJobs.DescriptionColor = System.Drawing.SystemColors.ControlText;
            this.ucKpiCardTotalJobs.Location = new System.Drawing.Point(3, 96);
            this.ucKpiCardTotalJobs.Name = "ucKpiCardTotalJobs";
            this.ucKpiCardTotalJobs.Size = new System.Drawing.Size(192, 102);
            this.ucKpiCardTotalJobs.TabIndex = 402;
            this.ucKpiCardTotalJobs.Title = "TITLE";
            this.ucKpiCardTotalJobs.TitleColor = System.Drawing.Color.Black;
            this.ucKpiCardTotalJobs.Value = "0";
            this.ucKpiCardTotalJobs.ValueColor = System.Drawing.SystemColors.ControlText;
            // 
            // ucKpiCard5Days
            // 
            this.ucKpiCard5Days.Description = "Description";
            this.ucKpiCard5Days.DescriptionColor = System.Drawing.SystemColors.ControlText;
            this.ucKpiCard5Days.Location = new System.Drawing.Point(585, 96);
            this.ucKpiCard5Days.Name = "ucKpiCard5Days";
            this.ucKpiCard5Days.Size = new System.Drawing.Size(192, 102);
            this.ucKpiCard5Days.TabIndex = 473;
            this.ucKpiCard5Days.Title = "TITLE";
            this.ucKpiCard5Days.TitleColor = System.Drawing.Color.Black;
            this.ucKpiCard5Days.Value = "0";
            this.ucKpiCard5Days.ValueColor = System.Drawing.SystemColors.ControlText;
            // 
            // bunifuDragControl2
            // 
            this.bunifuDragControl2.Fixed = true;
            this.bunifuDragControl2.Horizontal = true;
            this.bunifuDragControl2.TargetControl = this.pnlHeader;
            this.bunifuDragControl2.Vertical = true;
            // 
            // frmDiagService
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1558, 727);
            this.Controls.Add(this.ucKpiCard5Days);
            this.Controls.Add(this.ucKpiCard10Days);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnlPreviewImage);
            this.Controls.Add(this.ucKpiCard180Days);
            this.Controls.Add(this.ucKpiCard90Days);
            this.Controls.Add(this.ucKpiCard30Days);
            this.Controls.Add(this.ucKpiCardWithinTAT);
            this.Controls.Add(this.ucKpiCardBeyondTAT);
            this.Controls.Add(this.ucKpiCardTotalJobs);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "frmDiagService";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmServiceDiagnostic_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmDiagService_KeyDown);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).EndInit();
            this.pnlPreviewImage.ResumeLayout(false);
            this.pnlPreviewImage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).EndInit();
            this.panel36.ResumeLayout(false);
            this.panel36.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Bunifu.Framework.UI.BunifuElipse bunifuElipse1;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.TextBox txtLineNo;
        private Bunifu.Framework.UI.BunifuImageButton bunifuImageButton1;
        private Bunifu.Framework.UI.BunifuImageButton btnExit;
        private Bunifu.Framework.UI.BunifuCustomLabel lblHeader;
        private ControlObject.ucKpiCard ucKpiCard180Days;
        private ControlObject.ucKpiCard ucKpiCard90Days;
        private ControlObject.ucKpiCard ucKpiCard30Days;
        private ControlObject.ucKpiCard ucKpiCardWithinTAT;
        private ControlObject.ucKpiCard ucKpiCardBeyondTAT;
        private ControlObject.ucKpiCard ucKpiCardTotalJobs;
        private System.Windows.Forms.Panel pnlPreviewImage;
        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.Panel panel36;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel37;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboServiceStatus;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblHeaderPreviewImage;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ListView lvwList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtCurTerminalType;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnClear;
        private ControlObject.ucKpiCard ucKpiCard10Days;
        private ControlObject.ucKpiCard ucKpiCard5Days;
        private System.Windows.Forms.ComboBox cboClient;
        private System.Windows.Forms.Label label7;
        private Bunifu.Framework.UI.BunifuDragControl bunifuDragControl2;
    }
}