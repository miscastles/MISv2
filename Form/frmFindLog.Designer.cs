namespace MIS
{
    partial class frmFindLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFindLog));
            this.pnlList = new Bunifu.Framework.UI.BunifuCards();
            this.lvwSearch = new System.Windows.Forms.ListView();
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader20 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader21 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader22 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader24 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader25 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader26 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader27 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader28 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.bunifuElipse2 = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnPreview = new System.Windows.Forms.Button();
            this.bunifuElipse1 = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.btnSearchClear = new System.Windows.Forms.Button();
            this.pnlSearch = new Bunifu.Framework.UI.BunifuCards();
            this.lblSearchOptionDetails = new System.Windows.Forms.Label();
            this.btnSearchOption = new System.Windows.Forms.Button();
            this.txtLineNo = new System.Windows.Forms.TextBox();
            this.lblSearchStatus = new System.Windows.Forms.Label();
            this.bunifuDragControl2 = new Bunifu.Framework.UI.BunifuDragControl(this.components);
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.bunifuImageButton1 = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnExit = new Bunifu.Framework.UI.BunifuImageButton();
            this.lblHeader = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.pnlNavigator = new System.Windows.Forms.Panel();
            this.txtPage = new System.Windows.Forms.TextBox();
            this.btnNextPage = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnPreviousPage = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnLastPage = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnFirstPage = new Bunifu.Framework.UI.BunifuImageButton();
            this.lblTotalCount = new System.Windows.Forms.Label();
            this.pnlList.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).BeginInit();
            this.pnlNavigator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNextPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPreviousPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLastPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFirstPage)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlList
            // 
            this.pnlList.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlList.BorderRadius = 5;
            this.pnlList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlList.BottomSahddow = false;
            this.pnlList.color = System.Drawing.Color.Gray;
            this.pnlList.Controls.Add(this.lvwSearch);
            this.pnlList.LeftSahddow = false;
            this.pnlList.Location = new System.Drawing.Point(3, 68);
            this.pnlList.Name = "pnlList";
            this.pnlList.RightSahddow = false;
            this.pnlList.ShadowDepth = 20;
            this.pnlList.Size = new System.Drawing.Size(1361, 519);
            this.pnlList.TabIndex = 329;
            // 
            // lvwSearch
            // 
            this.lvwSearch.BackColor = System.Drawing.Color.GhostWhite;
            this.lvwSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvwSearch.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader14,
            this.columnHeader3,
            this.columnHeader15,
            this.columnHeader20,
            this.columnHeader21,
            this.columnHeader2,
            this.columnHeader22,
            this.columnHeader24,
            this.columnHeader25,
            this.columnHeader26,
            this.columnHeader27,
            this.columnHeader28,
            this.columnHeader4,
            this.columnHeader1,
            this.columnHeader5});
            this.lvwSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwSearch.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvwSearch.ForeColor = System.Drawing.Color.Black;
            this.lvwSearch.FullRowSelect = true;
            this.lvwSearch.HideSelection = false;
            this.lvwSearch.Location = new System.Drawing.Point(0, 0);
            this.lvwSearch.MultiSelect = false;
            this.lvwSearch.Name = "lvwSearch";
            this.lvwSearch.Size = new System.Drawing.Size(1359, 517);
            this.lvwSearch.TabIndex = 105;
            this.lvwSearch.UseCompatibleStateImageBehavior = false;
            this.lvwSearch.View = System.Windows.Forms.View.Details;
            this.lvwSearch.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvwSearch_ColumnClick);
            // 
            // columnHeader14
            // 
            this.columnHeader14.Text = "LINE#";
            this.columnHeader14.Width = 70;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "LOGID";
            this.columnHeader3.Width = 0;
            // 
            // columnHeader15
            // 
            this.columnHeader15.Text = "USERID";
            this.columnHeader15.Width = 0;
            // 
            // columnHeader20
            // 
            this.columnHeader20.Text = "USERNAME";
            this.columnHeader20.Width = 100;
            // 
            // columnHeader21
            // 
            this.columnHeader21.Text = "NAME";
            this.columnHeader21.Width = 180;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "USER LEVEL";
            this.columnHeader2.Width = 120;
            // 
            // columnHeader22
            // 
            this.columnHeader22.Text = "REMOTE IP";
            this.columnHeader22.Width = 110;
            // 
            // columnHeader24
            // 
            this.columnHeader24.Text = "COMPUTER NAME";
            this.columnHeader24.Width = 110;
            // 
            // columnHeader25
            // 
            this.columnHeader25.Text = "LOGIN DATE";
            this.columnHeader25.Width = 100;
            // 
            // columnHeader26
            // 
            this.columnHeader26.Text = "LOGIN TIME";
            this.columnHeader26.Width = 100;
            // 
            // columnHeader27
            // 
            this.columnHeader27.Text = "LOGOUT DATE";
            this.columnHeader27.Width = 100;
            // 
            // columnHeader28
            // 
            this.columnHeader28.Text = "LOGOUT TIME";
            this.columnHeader28.Width = 100;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "SESSIONSTATATUS";
            this.columnHeader4.Width = 0;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "SESSION STATUS";
            this.columnHeader1.Width = 110;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "PUBLISH VERSION";
            this.columnHeader5.Width = 150;
            // 
            // bunifuElipse2
            // 
            this.bunifuElipse2.ElipseRadius = 5;
            this.bunifuElipse2.TargetControl = this;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(1368, 29);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(2, 592);
            this.panel4.TabIndex = 327;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(1, 621);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1369, 2);
            this.panel3.TabIndex = 326;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 29);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1, 594);
            this.panel2.TabIndex = 325;
            // 
            // btnPreview
            // 
            this.btnPreview.AutoSize = true;
            this.btnPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnPreview.Enabled = false;
            this.btnPreview.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnPreview.FlatAppearance.BorderSize = 0;
            this.btnPreview.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnPreview.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPreview.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnPreview.Location = new System.Drawing.Point(1260, 592);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(105, 26);
            this.btnPreview.TabIndex = 332;
            this.btnPreview.Text = "PREVIEW REPORT";
            this.btnPreview.UseVisualStyleBackColor = false;
            // 
            // bunifuElipse1
            // 
            this.bunifuElipse1.ElipseRadius = 5;
            this.bunifuElipse1.TargetControl = this;
            // 
            // btnSearchClear
            // 
            this.btnSearchClear.AutoSize = true;
            this.btnSearchClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnSearchClear.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnSearchClear.FlatAppearance.BorderSize = 0;
            this.btnSearchClear.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSearchClear.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearchClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnSearchClear.Location = new System.Drawing.Point(1265, 7);
            this.btnSearchClear.Name = "btnSearchClear";
            this.btnSearchClear.Size = new System.Drawing.Size(95, 26);
            this.btnSearchClear.TabIndex = 312;
            this.btnSearchClear.Text = "CLEAR";
            this.btnSearchClear.UseVisualStyleBackColor = false;
            this.btnSearchClear.Click += new System.EventHandler(this.btnSearchClear_Click);
            // 
            // pnlSearch
            // 
            this.pnlSearch.BackColor = System.Drawing.Color.Lavender;
            this.pnlSearch.BorderRadius = 5;
            this.pnlSearch.BottomSahddow = false;
            this.pnlSearch.color = System.Drawing.Color.Gray;
            this.pnlSearch.Controls.Add(this.lblSearchOptionDetails);
            this.pnlSearch.Controls.Add(this.btnSearchOption);
            this.pnlSearch.Controls.Add(this.btnSearchClear);
            this.pnlSearch.LeftSahddow = false;
            this.pnlSearch.Location = new System.Drawing.Point(3, 30);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.RightSahddow = false;
            this.pnlSearch.ShadowDepth = 20;
            this.pnlSearch.Size = new System.Drawing.Size(1362, 37);
            this.pnlSearch.TabIndex = 328;
            // 
            // lblSearchOptionDetails
            // 
            this.lblSearchOptionDetails.AutoSize = true;
            this.lblSearchOptionDetails.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblSearchOptionDetails.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearchOptionDetails.ForeColor = System.Drawing.Color.Blue;
            this.lblSearchOptionDetails.Location = new System.Drawing.Point(22, 32);
            this.lblSearchOptionDetails.Name = "lblSearchOptionDetails";
            this.lblSearchOptionDetails.Size = new System.Drawing.Size(70, 14);
            this.lblSearchOptionDetails.TabIndex = 321;
            this.lblSearchOptionDetails.Text = "-Details-";
            // 
            // btnSearchOption
            // 
            this.btnSearchOption.AutoSize = true;
            this.btnSearchOption.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnSearchOption.Enabled = false;
            this.btnSearchOption.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnSearchOption.FlatAppearance.BorderSize = 0;
            this.btnSearchOption.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSearchOption.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearchOption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnSearchOption.Location = new System.Drawing.Point(1166, 7);
            this.btnSearchOption.Name = "btnSearchOption";
            this.btnSearchOption.Size = new System.Drawing.Size(99, 26);
            this.btnSearchOption.TabIndex = 0;
            this.btnSearchOption.Text = "SEARCH";
            this.btnSearchOption.UseVisualStyleBackColor = false;
            this.btnSearchOption.Click += new System.EventHandler(this.btnSearchOption_Click);
            // 
            // txtLineNo
            // 
            this.txtLineNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtLineNo.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLineNo.Location = new System.Drawing.Point(168, 4);
            this.txtLineNo.Name = "txtLineNo";
            this.txtLineNo.Size = new System.Drawing.Size(60, 20);
            this.txtLineNo.TabIndex = 305;
            this.txtLineNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtLineNo.Visible = false;
            // 
            // lblSearchStatus
            // 
            this.lblSearchStatus.AutoSize = true;
            this.lblSearchStatus.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearchStatus.ForeColor = System.Drawing.Color.White;
            this.lblSearchStatus.Location = new System.Drawing.Point(540, 4);
            this.lblSearchStatus.Name = "lblSearchStatus";
            this.lblSearchStatus.Size = new System.Drawing.Size(56, 14);
            this.lblSearchStatus.TabIndex = 331;
            this.lblSearchStatus.Text = "STATUS:";
            this.lblSearchStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblSearchStatus.Visible = false;
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
            this.pnlHeader.Controls.Add(this.bunifuImageButton1);
            this.pnlHeader.Controls.Add(this.btnExit);
            this.pnlHeader.Controls.Add(this.lblHeader);
            this.pnlHeader.Controls.Add(this.txtLineNo);
            this.pnlHeader.Controls.Add(this.lblSearchStatus);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1370, 29);
            this.pnlHeader.TabIndex = 324;
            // 
            // bunifuImageButton1
            // 
            this.bunifuImageButton1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.bunifuImageButton1.Image = ((System.Drawing.Image)(resources.GetObject("bunifuImageButton1.Image")));
            this.bunifuImageButton1.ImageActive = null;
            this.bunifuImageButton1.Location = new System.Drawing.Point(3, 2);
            this.bunifuImageButton1.Name = "bunifuImageButton1";
            this.bunifuImageButton1.Size = new System.Drawing.Size(26, 25);
            this.bunifuImageButton1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.bunifuImageButton1.TabIndex = 307;
            this.bunifuImageButton1.TabStop = false;
            this.bunifuImageButton1.Zoom = 10;
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageActive = null;
            this.btnExit.Location = new System.Drawing.Point(1342, 2);
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
            this.lblHeader.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeader.ForeColor = System.Drawing.Color.White;
            this.lblHeader.Location = new System.Drawing.Point(35, 4);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(66, 20);
            this.lblHeader.TabIndex = 7;
            this.lblHeader.Text = "SEARCH";
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
            this.btnRefresh.Location = new System.Drawing.Point(1155, 592);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(105, 26);
            this.btnRefresh.TabIndex = 333;
            this.btnRefresh.Text = "REFRESH";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // pnlNavigator
            // 
            this.pnlNavigator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlNavigator.Controls.Add(this.txtPage);
            this.pnlNavigator.Controls.Add(this.btnNextPage);
            this.pnlNavigator.Controls.Add(this.btnPreviousPage);
            this.pnlNavigator.Controls.Add(this.btnLastPage);
            this.pnlNavigator.Controls.Add(this.btnFirstPage);
            this.pnlNavigator.Location = new System.Drawing.Point(3, 592);
            this.pnlNavigator.Name = "pnlNavigator";
            this.pnlNavigator.Size = new System.Drawing.Size(198, 27);
            this.pnlNavigator.TabIndex = 417;
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
            // lblTotalCount
            // 
            this.lblTotalCount.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.lblTotalCount.Location = new System.Drawing.Point(934, 599);
            this.lblTotalCount.Name = "lblTotalCount";
            this.lblTotalCount.Size = new System.Drawing.Size(215, 14);
            this.lblTotalCount.TabIndex = 421;
            this.lblTotalCount.Text = "STATUS";
            this.lblTotalCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // frmFindLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1370, 623);
            this.Controls.Add(this.lblTotalCount);
            this.Controls.Add(this.pnlNavigator);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.pnlList);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.pnlSearch);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "frmFindLog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmFindLog_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmFindLog_KeyDown);
            this.pnlList.ResumeLayout(false);
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).EndInit();
            this.pnlNavigator.ResumeLayout(false);
            this.pnlNavigator.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNextPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPreviousPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLastPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFirstPage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Bunifu.Framework.UI.BunifuCards pnlList;
        private System.Windows.Forms.ListView lvwSearch;
        private System.Windows.Forms.ColumnHeader columnHeader14;
        private System.Windows.Forms.ColumnHeader columnHeader15;
        private Bunifu.Framework.UI.BunifuElipse bunifuElipse2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnPreview;
        private Bunifu.Framework.UI.BunifuCards pnlSearch;
        private System.Windows.Forms.TextBox txtLineNo;
        private System.Windows.Forms.Label lblSearchOptionDetails;
        private System.Windows.Forms.Button btnSearchOption;
        private System.Windows.Forms.Button btnSearchClear;
        private System.Windows.Forms.Label lblSearchStatus;
        private System.Windows.Forms.Panel pnlHeader;
        private Bunifu.Framework.UI.BunifuImageButton bunifuImageButton1;
        private Bunifu.Framework.UI.BunifuImageButton btnExit;
        private Bunifu.Framework.UI.BunifuCustomLabel lblHeader;
        private Bunifu.Framework.UI.BunifuElipse bunifuElipse1;
        private Bunifu.Framework.UI.BunifuDragControl bunifuDragControl2;
        private System.Windows.Forms.ColumnHeader columnHeader20;
        private System.Windows.Forms.ColumnHeader columnHeader21;
        private System.Windows.Forms.ColumnHeader columnHeader22;
        private System.Windows.Forms.ColumnHeader columnHeader24;
        private System.Windows.Forms.ColumnHeader columnHeader25;
        private System.Windows.Forms.ColumnHeader columnHeader26;
        private System.Windows.Forms.ColumnHeader columnHeader27;
        private System.Windows.Forms.ColumnHeader columnHeader28;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Panel pnlNavigator;
        private System.Windows.Forms.TextBox txtPage;
        private Bunifu.Framework.UI.BunifuImageButton btnNextPage;
        private Bunifu.Framework.UI.BunifuImageButton btnPreviousPage;
        private Bunifu.Framework.UI.BunifuImageButton btnLastPage;
        private Bunifu.Framework.UI.BunifuImageButton btnFirstPage;
        private System.Windows.Forms.Label lblTotalCount;
    }
}