namespace MIS
{
    partial class frmDiagSIM
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDiagSIM));
            this.bunifuElipse1 = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.txtLineNo = new System.Windows.Forms.TextBox();
            this.bunifuImageButton1 = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnExit = new Bunifu.Framework.UI.BunifuImageButton();
            this.lblHeader = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.bunifuDragControl2 = new Bunifu.Framework.UI.BunifuDragControl(this.components);
            this.pnlPreviewImage = new System.Windows.Forms.Panel();
            this.cboClient = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cboLocation = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cboType = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cboItemCategory = new System.Windows.Forms.ComboBox();
            this.picPreview = new System.Windows.Forms.PictureBox();
            this.panel36 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel37 = new System.Windows.Forms.Panel();
            this.ucKpiCardTotalStatusLocationMismatch = new MIS.ControlObject.ucKpiCard();
            this.ucKpiCardTotalLocationMismatch = new MIS.ControlObject.ucKpiCard();
            this.ucKpiCardTotalStatusMismatch = new MIS.ControlObject.ucKpiCard();
            this.ucKpiCardTotalInventory = new MIS.ControlObject.ucKpiCard();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tabControl3 = new System.Windows.Forms.TabControl();
            this.tabPage19 = new System.Windows.Forms.TabPage();
            this.panel25 = new System.Windows.Forms.Panel();
            this.lvwList = new System.Windows.Forms.ListView();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lvwListLocationIssues = new System.Windows.Forms.ListView();
            this.listView2 = new System.Windows.Forms.ListView();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.lvwListStatusIssues = new System.Windows.Forms.ListView();
            this.listView3 = new System.Windows.Forms.ListView();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtCurTerminalType = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblHeaderPreviewImage = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnClear = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).BeginInit();
            this.pnlPreviewImage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPreview)).BeginInit();
            this.panel36.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabControl3.SuspendLayout();
            this.tabPage19.SuspendLayout();
            this.panel25.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
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
            this.pnlHeader.Size = new System.Drawing.Size(1348, 29);
            this.pnlHeader.TabIndex = 403;
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
            this.btnExit.Location = new System.Drawing.Point(1318, 2);
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
            this.lblHeader.Size = new System.Drawing.Size(131, 18);
            this.lblHeader.TabIndex = 7;
            this.lblHeader.Text = "SIM DIAGNOSTIC";
            // 
            // bunifuDragControl2
            // 
            this.bunifuDragControl2.Fixed = true;
            this.bunifuDragControl2.Horizontal = true;
            this.bunifuDragControl2.TargetControl = this.pnlHeader;
            this.bunifuDragControl2.Vertical = true;
            // 
            // pnlPreviewImage
            // 
            this.pnlPreviewImage.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlPreviewImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlPreviewImage.Controls.Add(this.cboClient);
            this.pnlPreviewImage.Controls.Add(this.label7);
            this.pnlPreviewImage.Controls.Add(this.label5);
            this.pnlPreviewImage.Controls.Add(this.cboLocation);
            this.pnlPreviewImage.Controls.Add(this.label3);
            this.pnlPreviewImage.Controls.Add(this.cboType);
            this.pnlPreviewImage.Controls.Add(this.btnRefresh);
            this.pnlPreviewImage.Controls.Add(this.label2);
            this.pnlPreviewImage.Controls.Add(this.cboItemCategory);
            this.pnlPreviewImage.Controls.Add(this.picPreview);
            this.pnlPreviewImage.Controls.Add(this.panel36);
            this.pnlPreviewImage.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlPreviewImage.Location = new System.Drawing.Point(3, 33);
            this.pnlPreviewImage.Name = "pnlPreviewImage";
            this.pnlPreviewImage.Size = new System.Drawing.Size(1343, 62);
            this.pnlPreviewImage.TabIndex = 471;
            // 
            // cboClient
            // 
            this.cboClient.BackColor = System.Drawing.Color.White;
            this.cboClient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboClient.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboClient.ForeColor = System.Drawing.Color.Black;
            this.cboClient.FormattingEnabled = true;
            this.cboClient.Location = new System.Drawing.Point(188, 30);
            this.cboClient.Name = "cboClient";
            this.cboClient.Size = new System.Drawing.Size(142, 24);
            this.cboClient.TabIndex = 360;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(126, 35);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 14);
            this.label7.TabIndex = 359;
            this.label7.Text = "CLIENT:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(607, 35);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(70, 14);
            this.label5.TabIndex = 358;
            this.label5.Text = "LOCATION:";
            // 
            // cboLocation
            // 
            this.cboLocation.BackColor = System.Drawing.Color.White;
            this.cboLocation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboLocation.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboLocation.ForeColor = System.Drawing.Color.Black;
            this.cboLocation.FormattingEnabled = true;
            this.cboLocation.Location = new System.Drawing.Point(683, 30);
            this.cboLocation.Name = "cboLocation";
            this.cboLocation.Size = new System.Drawing.Size(163, 24);
            this.cboLocation.TabIndex = 357;
            this.cboLocation.SelectedIndexChanged += new System.EventHandler(this.cboLocation_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(336, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 14);
            this.label3.TabIndex = 354;
            this.label3.Text = "CARRIER:";
            // 
            // cboType
            // 
            this.cboType.BackColor = System.Drawing.Color.White;
            this.cboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboType.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboType.ForeColor = System.Drawing.Color.Black;
            this.cboType.FormattingEnabled = true;
            this.cboType.Location = new System.Drawing.Point(405, 30);
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(196, 24);
            this.cboType.TabIndex = 353;
            this.cboType.SelectedIndexChanged += new System.EventHandler(this.cboType_SelectedIndexChanged);
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
            this.btnRefresh.Location = new System.Drawing.Point(1195, 27);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(143, 29);
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
            this.label2.Location = new System.Drawing.Point(852, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 14);
            this.label2.TabIndex = 351;
            this.label2.Text = "ITEM CATEGORY:";
            // 
            // cboItemCategory
            // 
            this.cboItemCategory.BackColor = System.Drawing.Color.White;
            this.cboItemCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboItemCategory.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboItemCategory.ForeColor = System.Drawing.Color.Black;
            this.cboItemCategory.FormattingEnabled = true;
            this.cboItemCategory.Items.AddRange(new object[] {
            "[NOT SPECIFIED]",
            "STATUS MISMATCH",
            "LOCATION MISMATCH",
            "STATUS AND LOCATION MISMATCH"});
            this.cboItemCategory.Location = new System.Drawing.Point(963, 30);
            this.cboItemCategory.Name = "cboItemCategory";
            this.cboItemCategory.Size = new System.Drawing.Size(226, 24);
            this.cboItemCategory.TabIndex = 350;
            // 
            // picPreview
            // 
            this.picPreview.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.picPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picPreview.Cursor = System.Windows.Forms.Cursors.Hand;
            this.picPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picPreview.Location = new System.Drawing.Point(0, 24);
            this.picPreview.Name = "picPreview";
            this.picPreview.Size = new System.Drawing.Size(1341, 36);
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
            this.panel36.Size = new System.Drawing.Size(1341, 24);
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
            this.panel37.Size = new System.Drawing.Size(1341, 2);
            this.panel37.TabIndex = 0;
            // 
            // ucKpiCardTotalStatusLocationMismatch
            // 
            this.ucKpiCardTotalStatusLocationMismatch.Description = "Description";
            this.ucKpiCardTotalStatusLocationMismatch.DescriptionColor = System.Drawing.SystemColors.ControlText;
            this.ucKpiCardTotalStatusLocationMismatch.Location = new System.Drawing.Point(585, 102);
            this.ucKpiCardTotalStatusLocationMismatch.Name = "ucKpiCardTotalStatusLocationMismatch";
            this.ucKpiCardTotalStatusLocationMismatch.Size = new System.Drawing.Size(192, 102);
            this.ucKpiCardTotalStatusLocationMismatch.TabIndex = 506;
            this.ucKpiCardTotalStatusLocationMismatch.Title = "TITLE";
            this.ucKpiCardTotalStatusLocationMismatch.TitleColor = System.Drawing.Color.Black;
            this.ucKpiCardTotalStatusLocationMismatch.Value = "0";
            this.ucKpiCardTotalStatusLocationMismatch.ValueColor = System.Drawing.SystemColors.ControlText;
            // 
            // ucKpiCardTotalLocationMismatch
            // 
            this.ucKpiCardTotalLocationMismatch.Description = "Description";
            this.ucKpiCardTotalLocationMismatch.DescriptionColor = System.Drawing.SystemColors.ControlText;
            this.ucKpiCardTotalLocationMismatch.Location = new System.Drawing.Point(391, 101);
            this.ucKpiCardTotalLocationMismatch.Name = "ucKpiCardTotalLocationMismatch";
            this.ucKpiCardTotalLocationMismatch.Size = new System.Drawing.Size(192, 102);
            this.ucKpiCardTotalLocationMismatch.TabIndex = 505;
            this.ucKpiCardTotalLocationMismatch.Title = "TITLE";
            this.ucKpiCardTotalLocationMismatch.TitleColor = System.Drawing.Color.Black;
            this.ucKpiCardTotalLocationMismatch.Value = "0";
            this.ucKpiCardTotalLocationMismatch.ValueColor = System.Drawing.SystemColors.ControlText;
            // 
            // ucKpiCardTotalStatusMismatch
            // 
            this.ucKpiCardTotalStatusMismatch.Description = "Description";
            this.ucKpiCardTotalStatusMismatch.DescriptionColor = System.Drawing.SystemColors.ControlText;
            this.ucKpiCardTotalStatusMismatch.Location = new System.Drawing.Point(197, 101);
            this.ucKpiCardTotalStatusMismatch.Name = "ucKpiCardTotalStatusMismatch";
            this.ucKpiCardTotalStatusMismatch.Size = new System.Drawing.Size(192, 102);
            this.ucKpiCardTotalStatusMismatch.TabIndex = 504;
            this.ucKpiCardTotalStatusMismatch.Title = "TITLE";
            this.ucKpiCardTotalStatusMismatch.TitleColor = System.Drawing.Color.Black;
            this.ucKpiCardTotalStatusMismatch.Value = "0";
            this.ucKpiCardTotalStatusMismatch.ValueColor = System.Drawing.SystemColors.ControlText;
            // 
            // ucKpiCardTotalInventory
            // 
            this.ucKpiCardTotalInventory.Description = "Description";
            this.ucKpiCardTotalInventory.DescriptionColor = System.Drawing.SystemColors.ControlText;
            this.ucKpiCardTotalInventory.Location = new System.Drawing.Point(3, 101);
            this.ucKpiCardTotalInventory.Name = "ucKpiCardTotalInventory";
            this.ucKpiCardTotalInventory.Size = new System.Drawing.Size(192, 102);
            this.ucKpiCardTotalInventory.TabIndex = 503;
            this.ucKpiCardTotalInventory.Title = "TITLE";
            this.ucKpiCardTotalInventory.TitleColor = System.Drawing.Color.Black;
            this.ucKpiCardTotalInventory.Value = "0";
            this.ucKpiCardTotalInventory.ValueColor = System.Drawing.SystemColors.ControlText;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.tabControl3);
            this.panel1.Controls.Add(this.btnExport);
            this.panel1.Controls.Add(this.btnSearch);
            this.panel1.Controls.Add(this.txtCurTerminalType);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(4, 210);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1342, 485);
            this.panel1.TabIndex = 507;
            // 
            // tabControl3
            // 
            this.tabControl3.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl3.Controls.Add(this.tabPage19);
            this.tabControl3.Controls.Add(this.tabPage1);
            this.tabControl3.Controls.Add(this.tabPage2);
            this.tabControl3.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl3.Location = new System.Drawing.Point(5, 64);
            this.tabControl3.Name = "tabControl3";
            this.tabControl3.SelectedIndex = 0;
            this.tabControl3.Size = new System.Drawing.Size(1337, 421);
            this.tabControl3.TabIndex = 496;
            // 
            // tabPage19
            // 
            this.tabPage19.Controls.Add(this.panel25);
            this.tabPage19.ForeColor = System.Drawing.Color.Black;
            this.tabPage19.Location = new System.Drawing.Point(4, 27);
            this.tabPage19.Name = "tabPage19";
            this.tabPage19.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage19.Size = new System.Drawing.Size(1329, 390);
            this.tabPage19.TabIndex = 3;
            this.tabPage19.Text = "ALL ISSUES";
            this.tabPage19.UseVisualStyleBackColor = true;
            // 
            // panel25
            // 
            this.panel25.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel25.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel25.Controls.Add(this.lvwList);
            this.panel25.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel25.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel25.Location = new System.Drawing.Point(3, 3);
            this.panel25.Name = "panel25";
            this.panel25.Size = new System.Drawing.Size(1323, 384);
            this.panel25.TabIndex = 474;
            // 
            // lvwList
            // 
            this.lvwList.BackColor = System.Drawing.Color.GhostWhite;
            this.lvwList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvwList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwList.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvwList.ForeColor = System.Drawing.Color.Black;
            this.lvwList.FullRowSelect = true;
            this.lvwList.HideSelection = false;
            this.lvwList.Location = new System.Drawing.Point(0, 0);
            this.lvwList.Name = "lvwList";
            this.lvwList.Size = new System.Drawing.Size(1321, 382);
            this.lvwList.TabIndex = 315;
            this.lvwList.UseCompatibleStateImageBehavior = false;
            this.lvwList.View = System.Windows.Forms.View.Details;
            this.lvwList.DoubleClick += new System.EventHandler(this.lvwList_DoubleClick);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.lvwListLocationIssues);
            this.tabPage1.Controls.Add(this.listView2);
            this.tabPage1.Location = new System.Drawing.Point(4, 27);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(1329, 390);
            this.tabPage1.TabIndex = 4;
            this.tabPage1.Text = "LOCATION ISSUES";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lvwListLocationIssues
            // 
            this.lvwListLocationIssues.BackColor = System.Drawing.Color.GhostWhite;
            this.lvwListLocationIssues.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvwListLocationIssues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwListLocationIssues.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvwListLocationIssues.ForeColor = System.Drawing.Color.Black;
            this.lvwListLocationIssues.FullRowSelect = true;
            this.lvwListLocationIssues.HideSelection = false;
            this.lvwListLocationIssues.Location = new System.Drawing.Point(0, 0);
            this.lvwListLocationIssues.Name = "lvwListLocationIssues";
            this.lvwListLocationIssues.Size = new System.Drawing.Size(1329, 390);
            this.lvwListLocationIssues.TabIndex = 317;
            this.lvwListLocationIssues.UseCompatibleStateImageBehavior = false;
            this.lvwListLocationIssues.View = System.Windows.Forms.View.Details;
            this.lvwListLocationIssues.DoubleClick += new System.EventHandler(this.lvwListLocationIssues_DoubleClick);
            // 
            // listView2
            // 
            this.listView2.BackColor = System.Drawing.Color.GhostWhite;
            this.listView2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView2.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView2.ForeColor = System.Drawing.Color.Black;
            this.listView2.FullRowSelect = true;
            this.listView2.HideSelection = false;
            this.listView2.Location = new System.Drawing.Point(0, 0);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(1329, 390);
            this.listView2.TabIndex = 316;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.lvwListStatusIssues);
            this.tabPage2.Controls.Add(this.listView3);
            this.tabPage2.Location = new System.Drawing.Point(4, 27);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(1329, 390);
            this.tabPage2.TabIndex = 5;
            this.tabPage2.Text = "STATUS ISSUES";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // lvwListStatusIssues
            // 
            this.lvwListStatusIssues.BackColor = System.Drawing.Color.GhostWhite;
            this.lvwListStatusIssues.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvwListStatusIssues.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwListStatusIssues.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvwListStatusIssues.ForeColor = System.Drawing.Color.Black;
            this.lvwListStatusIssues.FullRowSelect = true;
            this.lvwListStatusIssues.HideSelection = false;
            this.lvwListStatusIssues.Location = new System.Drawing.Point(0, 0);
            this.lvwListStatusIssues.Name = "lvwListStatusIssues";
            this.lvwListStatusIssues.Size = new System.Drawing.Size(1329, 390);
            this.lvwListStatusIssues.TabIndex = 317;
            this.lvwListStatusIssues.UseCompatibleStateImageBehavior = false;
            this.lvwListStatusIssues.View = System.Windows.Forms.View.Details;
            this.lvwListStatusIssues.DoubleClick += new System.EventHandler(this.lvwListStatusIssues_DoubleClick);
            // 
            // listView3
            // 
            this.listView3.BackColor = System.Drawing.Color.GhostWhite;
            this.listView3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView3.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView3.ForeColor = System.Drawing.Color.Black;
            this.listView3.FullRowSelect = true;
            this.listView3.HideSelection = false;
            this.listView3.Location = new System.Drawing.Point(0, 0);
            this.listView3.Name = "listView3";
            this.listView3.Size = new System.Drawing.Size(1329, 390);
            this.listView3.TabIndex = 316;
            this.listView3.UseCompatibleStateImageBehavior = false;
            this.listView3.View = System.Windows.Forms.View.Details;
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
            this.btnExport.Location = new System.Drawing.Point(1194, 27);
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
            this.btnSearch.Location = new System.Drawing.Point(1045, 27);
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
            this.txtCurTerminalType.Size = new System.Drawing.Size(956, 26);
            this.txtCurTerminalType.TabIndex = 353;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(3, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(78, 18);
            this.label6.TabIndex = 352;
            this.label6.Text = "SEARCH:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 24);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1340, 459);
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
            this.panel2.Size = new System.Drawing.Size(1340, 24);
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
            this.panel3.Size = new System.Drawing.Size(1340, 2);
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
            this.btnClear.Location = new System.Drawing.Point(5, 702);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(143, 29);
            this.btnClear.TabIndex = 508;
            this.btnClear.Text = "CLEAR";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // frmDiagSIM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1348, 752);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.ucKpiCardTotalStatusLocationMismatch);
            this.Controls.Add(this.ucKpiCardTotalLocationMismatch);
            this.Controls.Add(this.ucKpiCardTotalStatusMismatch);
            this.Controls.Add(this.ucKpiCardTotalInventory);
            this.Controls.Add(this.pnlPreviewImage);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "frmDiagSIM";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmDiagSIM_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmDiagSIM_KeyDown);
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
            this.tabControl3.ResumeLayout(false);
            this.tabPage19.ResumeLayout(false);
            this.panel25.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
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
        private Bunifu.Framework.UI.BunifuDragControl bunifuDragControl2;
        private System.Windows.Forms.Panel pnlPreviewImage;
        private System.Windows.Forms.ComboBox cboClient;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cboLocation;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboType;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cboItemCategory;
        private System.Windows.Forms.PictureBox picPreview;
        private System.Windows.Forms.Panel panel36;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel37;
        private ControlObject.ucKpiCard ucKpiCardTotalStatusLocationMismatch;
        private ControlObject.ucKpiCard ucKpiCardTotalLocationMismatch;
        private ControlObject.ucKpiCard ucKpiCardTotalStatusMismatch;
        private ControlObject.ucKpiCard ucKpiCardTotalInventory;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabControl3;
        private System.Windows.Forms.TabPage tabPage19;
        private System.Windows.Forms.Panel panel25;
        private System.Windows.Forms.ListView lvwList;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListView lvwListLocationIssues;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView lvwListStatusIssues;
        private System.Windows.Forms.ListView listView3;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtCurTerminalType;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblHeaderPreviewImage;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnClear;
    }
}