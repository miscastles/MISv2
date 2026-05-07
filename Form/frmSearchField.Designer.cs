namespace MIS
{
    partial class frmSearchField
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSearchField));
            this.bunifuElipse1 = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.txtLineNo = new System.Windows.Forms.TextBox();
            this.bunifuDragControl2 = new Bunifu.Framework.UI.BunifuDragControl(this.components);
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.bunifuImageButton1 = new Bunifu.Framework.UI.BunifuImageButton();
            this.lblHeader = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.pnlHeader2 = new System.Windows.Forms.Panel();
            this.lblSearchMessage = new System.Windows.Forms.Label();
            this.btnExit = new Bunifu.Framework.UI.BunifuImageButton();
            this.lblSearchStatus = new System.Windows.Forms.Label();
            this.pnlNavigator = new System.Windows.Forms.Panel();
            this.txtPage = new System.Windows.Forms.TextBox();
            this.btnNextPage = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnPreviousPage = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnLastPage = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnFirstPage = new Bunifu.Framework.UI.BunifuImageButton();
            this.chkShowAll = new System.Windows.Forms.CheckBox();
            this.panel134 = new System.Windows.Forms.Panel();
            this.lvwSearch = new System.Windows.Forms.ListView();
            this.panel140 = new System.Windows.Forms.Panel();
            this.panel144 = new System.Windows.Forms.Panel();
            this.chkSelect = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnOK = new System.Windows.Forms.Button();
            this.panel135 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.lblSearchString = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.pnlHeader.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).BeginInit();
            this.pnlHeader2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).BeginInit();
            this.pnlNavigator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNextPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPreviousPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLastPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFirstPage)).BeginInit();
            this.panel134.SuspendLayout();
            this.panel140.SuspendLayout();
            this.panel144.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel135.SuspendLayout();
            this.panel6.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // bunifuElipse1
            // 
            this.bunifuElipse1.ElipseRadius = 5;
            this.bunifuElipse1.TargetControl = this;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1, 577);
            this.panel2.TabIndex = 304;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(1389, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(2, 577);
            this.panel4.TabIndex = 305;
            // 
            // txtLineNo
            // 
            this.txtLineNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtLineNo.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLineNo.Location = new System.Drawing.Point(873, 5);
            this.txtLineNo.Name = "txtLineNo";
            this.txtLineNo.Size = new System.Drawing.Size(32, 20);
            this.txtLineNo.TabIndex = 306;
            this.txtLineNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtLineNo.Visible = false;
            // 
            // bunifuDragControl2
            // 
            this.bunifuDragControl2.Fixed = true;
            this.bunifuDragControl2.Horizontal = true;
            this.bunifuDragControl2.TargetControl = this.pnlHeader;
            this.bunifuDragControl2.Vertical = true;
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.pnlHeader.Controls.Add(this.panel3);
            this.pnlHeader.Controls.Add(this.pnlHeader2);
            this.pnlHeader.Controls.Add(this.txtLineNo);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1386, 78);
            this.pnlHeader.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.bunifuImageButton1);
            this.panel3.Controls.Add(this.lblHeader);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(301, 78);
            this.panel3.TabIndex = 443;
            // 
            // bunifuImageButton1
            // 
            this.bunifuImageButton1.BackColor = System.Drawing.Color.Transparent;
            this.bunifuImageButton1.Image = ((System.Drawing.Image)(resources.GetObject("bunifuImageButton1.Image")));
            this.bunifuImageButton1.ImageActive = null;
            this.bunifuImageButton1.Location = new System.Drawing.Point(3, 2);
            this.bunifuImageButton1.Name = "bunifuImageButton1";
            this.bunifuImageButton1.Size = new System.Drawing.Size(24, 23);
            this.bunifuImageButton1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.bunifuImageButton1.TabIndex = 110;
            this.bunifuImageButton1.TabStop = false;
            this.bunifuImageButton1.Zoom = 10;
            // 
            // lblHeader
            // 
            this.lblHeader.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.White;
            this.lblHeader.Location = new System.Drawing.Point(29, 4);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(269, 20);
            this.lblHeader.TabIndex = 109;
            this.lblHeader.Text = "SEARCH";
            // 
            // pnlHeader2
            // 
            this.pnlHeader2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.pnlHeader2.Controls.Add(this.lblSearchMessage);
            this.pnlHeader2.Controls.Add(this.btnExit);
            this.pnlHeader2.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlHeader2.Location = new System.Drawing.Point(911, 0);
            this.pnlHeader2.Name = "pnlHeader2";
            this.pnlHeader2.Size = new System.Drawing.Size(475, 78);
            this.pnlHeader2.TabIndex = 442;
            // 
            // lblSearchMessage
            // 
            this.lblSearchMessage.AutoSize = true;
            this.lblSearchMessage.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearchMessage.ForeColor = System.Drawing.Color.Yellow;
            this.lblSearchMessage.Location = new System.Drawing.Point(386, 8);
            this.lblSearchMessage.Name = "lblSearchMessage";
            this.lblSearchMessage.Size = new System.Drawing.Size(56, 14);
            this.lblSearchMessage.TabIndex = 329;
            this.lblSearchMessage.Text = "MESSAGE";
            this.lblSearchMessage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblSearchMessage.Visible = false;
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageActive = null;
            this.btnExit.Location = new System.Drawing.Point(448, 4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(23, 21);
            this.btnExit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnExit.TabIndex = 8;
            this.btnExit.TabStop = false;
            this.btnExit.Zoom = 10;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblSearchStatus
            // 
            this.lblSearchStatus.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearchStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblSearchStatus.Location = new System.Drawing.Point(408, 7);
            this.lblSearchStatus.Name = "lblSearchStatus";
            this.lblSearchStatus.Size = new System.Drawing.Size(191, 14);
            this.lblSearchStatus.TabIndex = 328;
            this.lblSearchStatus.Text = "STATUS";
            this.lblSearchStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblSearchStatus.Visible = false;
            // 
            // pnlNavigator
            // 
            this.pnlNavigator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlNavigator.Controls.Add(this.txtPage);
            this.pnlNavigator.Controls.Add(this.btnNextPage);
            this.pnlNavigator.Controls.Add(this.btnPreviousPage);
            this.pnlNavigator.Controls.Add(this.btnLastPage);
            this.pnlNavigator.Controls.Add(this.btnFirstPage);
            this.pnlNavigator.Location = new System.Drawing.Point(204, 1);
            this.pnlNavigator.Name = "pnlNavigator";
            this.pnlNavigator.Size = new System.Drawing.Size(198, 27);
            this.pnlNavigator.TabIndex = 417;
            this.pnlNavigator.Visible = false;
            // 
            // txtPage
            // 
            this.txtPage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.txtPage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPage.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtPage.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPage.Location = new System.Drawing.Point(56, 3);
            this.txtPage.Name = "txtPage";
            this.txtPage.ReadOnly = true;
            this.txtPage.Size = new System.Drawing.Size(85, 20);
            this.txtPage.TabIndex = 416;
            this.txtPage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnNextPage
            // 
            this.btnNextPage.BackColor = System.Drawing.Color.Lavender;
            this.btnNextPage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnNextPage.ErrorImage = null;
            this.btnNextPage.Image = ((System.Drawing.Image)(resources.GetObject("btnNextPage.Image")));
            this.btnNextPage.ImageActive = null;
            this.btnNextPage.Location = new System.Drawing.Point(143, 3);
            this.btnNextPage.Name = "btnNextPage";
            this.btnNextPage.Size = new System.Drawing.Size(25, 21);
            this.btnNextPage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnNextPage.TabIndex = 296;
            this.btnNextPage.TabStop = false;
            this.btnNextPage.Zoom = 10;
            this.btnNextPage.Click += new System.EventHandler(this.btnNextPage_Click);
            // 
            // btnPreviousPage
            // 
            this.btnPreviousPage.BackColor = System.Drawing.Color.Lavender;
            this.btnPreviousPage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnPreviousPage.ErrorImage = null;
            this.btnPreviousPage.Image = ((System.Drawing.Image)(resources.GetObject("btnPreviousPage.Image")));
            this.btnPreviousPage.ImageActive = null;
            this.btnPreviousPage.Location = new System.Drawing.Point(29, 3);
            this.btnPreviousPage.Name = "btnPreviousPage";
            this.btnPreviousPage.Size = new System.Drawing.Size(25, 21);
            this.btnPreviousPage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnPreviousPage.TabIndex = 295;
            this.btnPreviousPage.TabStop = false;
            this.btnPreviousPage.Zoom = 10;
            this.btnPreviousPage.Click += new System.EventHandler(this.btnPreviousPage_Click);
            // 
            // btnLastPage
            // 
            this.btnLastPage.BackColor = System.Drawing.Color.Lavender;
            this.btnLastPage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLastPage.ErrorImage = null;
            this.btnLastPage.Image = ((System.Drawing.Image)(resources.GetObject("btnLastPage.Image")));
            this.btnLastPage.ImageActive = null;
            this.btnLastPage.Location = new System.Drawing.Point(169, 3);
            this.btnLastPage.Name = "btnLastPage";
            this.btnLastPage.Size = new System.Drawing.Size(25, 21);
            this.btnLastPage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnLastPage.TabIndex = 294;
            this.btnLastPage.TabStop = false;
            this.btnLastPage.Zoom = 10;
            this.btnLastPage.Click += new System.EventHandler(this.btnLastPage_Click);
            // 
            // btnFirstPage
            // 
            this.btnFirstPage.BackColor = System.Drawing.Color.Lavender;
            this.btnFirstPage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnFirstPage.ErrorImage = null;
            this.btnFirstPage.Image = ((System.Drawing.Image)(resources.GetObject("btnFirstPage.Image")));
            this.btnFirstPage.ImageActive = null;
            this.btnFirstPage.Location = new System.Drawing.Point(3, 3);
            this.btnFirstPage.Name = "btnFirstPage";
            this.btnFirstPage.Size = new System.Drawing.Size(25, 21);
            this.btnFirstPage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnFirstPage.TabIndex = 293;
            this.btnFirstPage.TabStop = false;
            this.btnFirstPage.Zoom = 10;
            this.btnFirstPage.Click += new System.EventHandler(this.btnFirstPage_Click);
            // 
            // chkShowAll
            // 
            this.chkShowAll.AutoSize = true;
            this.chkShowAll.Enabled = false;
            this.chkShowAll.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold);
            this.chkShowAll.ForeColor = System.Drawing.Color.Blue;
            this.chkShowAll.Location = new System.Drawing.Point(53, 7);
            this.chkShowAll.Name = "chkShowAll";
            this.chkShowAll.Size = new System.Drawing.Size(89, 18);
            this.chkShowAll.TabIndex = 437;
            this.chkShowAll.Text = "SHOW ALL?";
            this.chkShowAll.UseVisualStyleBackColor = true;
            this.chkShowAll.Visible = false;
            this.chkShowAll.CheckedChanged += new System.EventHandler(this.chkShowAll_CheckedChanged);
            // 
            // panel134
            // 
            this.panel134.Controls.Add(this.lvwSearch);
            this.panel134.Controls.Add(this.panel140);
            this.panel134.Controls.Add(this.panel135);
            this.panel134.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel134.Location = new System.Drawing.Point(1, 0);
            this.panel134.Name = "panel134";
            this.panel134.Size = new System.Drawing.Size(1388, 577);
            this.panel134.TabIndex = 463;
            // 
            // lvwSearch
            // 
            this.lvwSearch.BackColor = System.Drawing.Color.GhostWhite;
            this.lvwSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvwSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwSearch.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvwSearch.ForeColor = System.Drawing.Color.Black;
            this.lvwSearch.FullRowSelect = true;
            this.lvwSearch.HideSelection = false;
            this.lvwSearch.Location = new System.Drawing.Point(0, 80);
            this.lvwSearch.MultiSelect = false;
            this.lvwSearch.Name = "lvwSearch";
            this.lvwSearch.Size = new System.Drawing.Size(1388, 466);
            this.lvwSearch.TabIndex = 309;
            this.lvwSearch.UseCompatibleStateImageBehavior = false;
            this.lvwSearch.View = System.Windows.Forms.View.Details;
            this.lvwSearch.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvwSearch_ColumnClick);
            this.lvwSearch.SelectedIndexChanged += new System.EventHandler(this.lvwSearch_SelectedIndexChanged);
            this.lvwSearch.DoubleClick += new System.EventHandler(this.lvwSearch_DoubleClick);
            // 
            // panel140
            // 
            this.panel140.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel140.Controls.Add(this.panel144);
            this.panel140.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel140.Location = new System.Drawing.Point(0, 546);
            this.panel140.Name = "panel140";
            this.panel140.Size = new System.Drawing.Size(1388, 31);
            this.panel140.TabIndex = 314;
            // 
            // panel144
            // 
            this.panel144.Controls.Add(this.chkSelect);
            this.panel144.Controls.Add(this.panel1);
            this.panel144.Controls.Add(this.pnlNavigator);
            this.panel144.Controls.Add(this.lblSearchStatus);
            this.panel144.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel144.Location = new System.Drawing.Point(0, 0);
            this.panel144.Name = "panel144";
            this.panel144.Size = new System.Drawing.Size(1386, 29);
            this.panel144.TabIndex = 1;
            // 
            // chkSelect
            // 
            this.chkSelect.AutoSize = true;
            this.chkSelect.Enabled = false;
            this.chkSelect.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkSelect.ForeColor = System.Drawing.Color.Red;
            this.chkSelect.Location = new System.Drawing.Point(8, 5);
            this.chkSelect.Name = "chkSelect";
            this.chkSelect.Size = new System.Drawing.Size(96, 18);
            this.chkSelect.TabIndex = 442;
            this.chkSelect.Text = "CHECK ALL?";
            this.chkSelect.UseVisualStyleBackColor = true;
            this.chkSelect.CheckedChanged += new System.EventHandler(this.chkSelect_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel7);
            this.panel1.Controls.Add(this.panel5);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(1090, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(296, 29);
            this.panel1.TabIndex = 441;
            // 
            // panel7
            // 
            this.panel7.Controls.Add(this.chkShowAll);
            this.panel7.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel7.Location = new System.Drawing.Point(0, 0);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(147, 29);
            this.panel7.TabIndex = 1;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.btnOK);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel5.Location = new System.Drawing.Point(144, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(152, 29);
            this.panel5.TabIndex = 0;
            // 
            // btnOK
            // 
            this.btnOK.AutoSize = true;
            this.btnOK.BackColor = System.Drawing.Color.Blue;
            this.btnOK.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnOK.FlatAppearance.BorderSize = 0;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOK.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnOK.Location = new System.Drawing.Point(4, 1);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(146, 26);
            this.btnOK.TabIndex = 437;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // panel135
            // 
            this.panel135.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel135.Controls.Add(this.panel6);
            this.panel135.Controls.Add(this.pnlHeader);
            this.panel135.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel135.Location = new System.Drawing.Point(0, 0);
            this.panel135.Name = "panel135";
            this.panel135.Size = new System.Drawing.Size(1388, 80);
            this.panel135.TabIndex = 313;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.panel6.Controls.Add(this.pnlSearch);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel6.Location = new System.Drawing.Point(0, 30);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(1386, 48);
            this.panel6.TabIndex = 1;
            // 
            // pnlSearch
            // 
            this.pnlSearch.BackColor = System.Drawing.SystemColors.Control;
            this.pnlSearch.Controls.Add(this.lblSearchString);
            this.pnlSearch.Controls.Add(this.txtSearch);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlSearch.Location = new System.Drawing.Point(0, 0);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Padding = new System.Windows.Forms.Padding(5, 0, 5, 5);
            this.pnlSearch.Size = new System.Drawing.Size(1386, 48);
            this.pnlSearch.TabIndex = 397;
            // 
            // lblSearchString
            // 
            this.lblSearchString.AutoSize = true;
            this.lblSearchString.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearchString.ForeColor = System.Drawing.Color.Black;
            this.lblSearchString.Location = new System.Drawing.Point(4, 0);
            this.lblSearchString.Name = "lblSearchString";
            this.lblSearchString.Size = new System.Drawing.Size(14, 14);
            this.lblSearchString.TabIndex = 3;
            this.lblSearchString.Text = ">";
            this.lblSearchString.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtSearch.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtSearch.Font = new System.Drawing.Font("Courier New", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.txtSearch.Location = new System.Drawing.Point(5, 13);
            this.txtSearch.MaxLength = 45;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(1376, 30);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            // 
            // frmSearchField
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1391, 577);
            this.Controls.Add(this.panel134);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "frmSearchField";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Activated += new System.EventHandler(this.frmSearchField_Activated);
            this.Load += new System.EventHandler(this.frmSearchField_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmSearchField_KeyDown);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).EndInit();
            this.pnlHeader2.ResumeLayout(false);
            this.pnlHeader2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).EndInit();
            this.pnlNavigator.ResumeLayout(false);
            this.pnlNavigator.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNextPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPreviousPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLastPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFirstPage)).EndInit();
            this.panel134.ResumeLayout(false);
            this.panel140.ResumeLayout(false);
            this.panel144.ResumeLayout(false);
            this.panel144.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel135.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Bunifu.Framework.UI.BunifuElipse bunifuElipse1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox txtLineNo;
        private Bunifu.Framework.UI.BunifuDragControl bunifuDragControl2;
        private System.Windows.Forms.Label lblSearchStatus;
        private System.Windows.Forms.Panel pnlNavigator;
        private System.Windows.Forms.TextBox txtPage;
        private Bunifu.Framework.UI.BunifuImageButton btnNextPage;
        private Bunifu.Framework.UI.BunifuImageButton btnPreviousPage;
        private Bunifu.Framework.UI.BunifuImageButton btnLastPage;
        private Bunifu.Framework.UI.BunifuImageButton btnFirstPage;
        private System.Windows.Forms.CheckBox chkShowAll;
        private System.Windows.Forms.Panel panel134;
        private System.Windows.Forms.Panel panel140;
        private System.Windows.Forms.Panel panel144;
        private System.Windows.Forms.Panel panel135;
        private System.Windows.Forms.ListView lvwSearch;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel pnlSearch;
        private System.Windows.Forms.Label lblSearchString;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Panel panel3;
        private Bunifu.Framework.UI.BunifuImageButton bunifuImageButton1;
        private Bunifu.Framework.UI.BunifuCustomLabel lblHeader;
        private System.Windows.Forms.Panel pnlHeader2;
        private System.Windows.Forms.Label lblSearchMessage;
        private Bunifu.Framework.UI.BunifuImageButton btnExit;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.CheckBox chkSelect;
    }
}