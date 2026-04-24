namespace MIS
{
    partial class frmFindME
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmFindME));
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.bunifuImageButton1 = new Bunifu.Framework.UI.BunifuImageButton();
            this.lblSearchStatus = new System.Windows.Forms.Label();
            this.btnExit = new Bunifu.Framework.UI.BunifuImageButton();
            this.lblHeader = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.pnlSearch = new Bunifu.Framework.UI.BunifuCards();
            this.txtLineNo = new System.Windows.Forms.TextBox();
            this.lblSearchOptionDetails = new System.Windows.Forms.Label();
            this.lblSearchOption = new System.Windows.Forms.Label();
            this.btnSearchOption = new System.Windows.Forms.Button();
            this.btnSearchClear = new System.Windows.Forms.Button();
            this.pnlList = new Bunifu.Framework.UI.BunifuCards();
            this.lvwSearch = new System.Windows.Forms.ListView();
            this.btnPreview = new System.Windows.Forms.Button();
            this.bunifuDragControl2 = new Bunifu.Framework.UI.BunifuDragControl(this.components);
            this.bunifuElipse1 = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.pnlNavigator = new System.Windows.Forms.Panel();
            this.txtPage = new System.Windows.Forms.TextBox();
            this.btnNextPage = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnPreviousPage = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnLastPage = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnFirstPage = new Bunifu.Framework.UI.BunifuImageButton();
            this.lblTotalCount = new System.Windows.Forms.Label();
            this.ucStatus = new MIS.ucStatus();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).BeginInit();
            this.pnlSearch.SuspendLayout();
            this.pnlList.SuspendLayout();
            this.pnlNavigator.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnNextPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnPreviousPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnLastPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFirstPage)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1, 623);
            this.panel2.TabIndex = 304;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(1, 621);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1351, 2);
            this.panel3.TabIndex = 306;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(1350, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(2, 621);
            this.panel4.TabIndex = 307;
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.pnlHeader.Controls.Add(this.bunifuImageButton1);
            this.pnlHeader.Controls.Add(this.lblSearchStatus);
            this.pnlHeader.Controls.Add(this.btnExit);
            this.pnlHeader.Controls.Add(this.lblHeader);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(1, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1349, 29);
            this.pnlHeader.TabIndex = 308;
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
            // lblSearchStatus
            // 
            this.lblSearchStatus.AutoSize = true;
            this.lblSearchStatus.BackColor = System.Drawing.Color.White;
            this.lblSearchStatus.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearchStatus.ForeColor = System.Drawing.Color.Red;
            this.lblSearchStatus.Location = new System.Drawing.Point(581, 9);
            this.lblSearchStatus.Name = "lblSearchStatus";
            this.lblSearchStatus.Size = new System.Drawing.Size(56, 14);
            this.lblSearchStatus.TabIndex = 311;
            this.lblSearchStatus.Text = "STATUS:";
            this.lblSearchStatus.Visible = false;
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageActive = null;
            this.btnExit.Location = new System.Drawing.Point(1322, 2);
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
            // pnlSearch
            // 
            this.pnlSearch.BackColor = System.Drawing.Color.Lavender;
            this.pnlSearch.BorderRadius = 5;
            this.pnlSearch.BottomSahddow = false;
            this.pnlSearch.color = System.Drawing.Color.Gray;
            this.pnlSearch.Controls.Add(this.txtLineNo);
            this.pnlSearch.Controls.Add(this.lblSearchOptionDetails);
            this.pnlSearch.Controls.Add(this.lblSearchOption);
            this.pnlSearch.Controls.Add(this.btnSearchOption);
            this.pnlSearch.Controls.Add(this.btnSearchClear);
            this.pnlSearch.LeftSahddow = false;
            this.pnlSearch.Location = new System.Drawing.Point(4, 32);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.RightSahddow = false;
            this.pnlSearch.ShadowDepth = 20;
            this.pnlSearch.Size = new System.Drawing.Size(1344, 37);
            this.pnlSearch.TabIndex = 309;
            // 
            // txtLineNo
            // 
            this.txtLineNo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtLineNo.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLineNo.Location = new System.Drawing.Point(170, 14);
            this.txtLineNo.Name = "txtLineNo";
            this.txtLineNo.Size = new System.Drawing.Size(60, 20);
            this.txtLineNo.TabIndex = 305;
            this.txtLineNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtLineNo.Visible = false;
            // 
            // lblSearchOptionDetails
            // 
            this.lblSearchOptionDetails.AutoSize = true;
            this.lblSearchOptionDetails.Cursor = System.Windows.Forms.Cursors.Default;
            this.lblSearchOptionDetails.Font = new System.Drawing.Font("Courier New", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearchOptionDetails.ForeColor = System.Drawing.Color.Blue;
            this.lblSearchOptionDetails.Location = new System.Drawing.Point(22, 32);
            this.lblSearchOptionDetails.Name = "lblSearchOptionDetails";
            this.lblSearchOptionDetails.Size = new System.Drawing.Size(50, 12);
            this.lblSearchOptionDetails.TabIndex = 321;
            this.lblSearchOptionDetails.Text = "-Details-";
            // 
            // lblSearchOption
            // 
            this.lblSearchOption.AutoSize = true;
            this.lblSearchOption.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblSearchOption.Font = new System.Drawing.Font("Courier New", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSearchOption.ForeColor = System.Drawing.Color.Navy;
            this.lblSearchOption.Location = new System.Drawing.Point(7, 7);
            this.lblSearchOption.Name = "lblSearchOption";
            this.lblSearchOption.Size = new System.Drawing.Size(90, 12);
            this.lblSearchOption.TabIndex = 320;
            this.lblSearchOption.Text = "(+)Search Options";
            this.lblSearchOption.Click += new System.EventHandler(this.lblSearchOption_Click);
            // 
            // btnSearchOption
            // 
            this.btnSearchOption.AutoSize = true;
            this.btnSearchOption.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnSearchOption.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnSearchOption.FlatAppearance.BorderSize = 0;
            this.btnSearchOption.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnSearchOption.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearchOption.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnSearchOption.Location = new System.Drawing.Point(1143, 7);
            this.btnSearchOption.Name = "btnSearchOption";
            this.btnSearchOption.Size = new System.Drawing.Size(99, 26);
            this.btnSearchOption.TabIndex = 0;
            this.btnSearchOption.Text = "SEARCH";
            this.btnSearchOption.UseVisualStyleBackColor = false;
            this.btnSearchOption.Click += new System.EventHandler(this.btnSearchOption_Click);
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
            this.btnSearchClear.Location = new System.Drawing.Point(1242, 7);
            this.btnSearchClear.Name = "btnSearchClear";
            this.btnSearchClear.Size = new System.Drawing.Size(95, 26);
            this.btnSearchClear.TabIndex = 312;
            this.btnSearchClear.Text = "CLEAR";
            this.btnSearchClear.UseVisualStyleBackColor = false;
            this.btnSearchClear.Click += new System.EventHandler(this.btnSearchClear_Click);
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
            this.pnlList.Location = new System.Drawing.Point(4, 70);
            this.pnlList.Name = "pnlList";
            this.pnlList.RightSahddow = false;
            this.pnlList.ShadowDepth = 20;
            this.pnlList.Size = new System.Drawing.Size(1344, 517);
            this.pnlList.TabIndex = 310;
            // 
            // lvwSearch
            // 
            this.lvwSearch.BackColor = System.Drawing.Color.GhostWhite;
            this.lvwSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvwSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwSearch.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvwSearch.ForeColor = System.Drawing.Color.Black;
            this.lvwSearch.FullRowSelect = true;
            this.lvwSearch.HideSelection = false;
            this.lvwSearch.Location = new System.Drawing.Point(0, 0);
            this.lvwSearch.MultiSelect = false;
            this.lvwSearch.Name = "lvwSearch";
            this.lvwSearch.Size = new System.Drawing.Size(1342, 515);
            this.lvwSearch.TabIndex = 105;
            this.lvwSearch.UseCompatibleStateImageBehavior = false;
            this.lvwSearch.View = System.Windows.Forms.View.Details;
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
            this.btnPreview.Location = new System.Drawing.Point(1239, 591);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(105, 26);
            this.btnPreview.TabIndex = 316;
            this.btnPreview.Text = "PREVIEW REPORT";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // bunifuDragControl2
            // 
            this.bunifuDragControl2.Fixed = true;
            this.bunifuDragControl2.Horizontal = true;
            this.bunifuDragControl2.TargetControl = this.pnlHeader;
            this.bunifuDragControl2.Vertical = true;
            // 
            // bunifuElipse1
            // 
            this.bunifuElipse1.ElipseRadius = 5;
            this.bunifuElipse1.TargetControl = this;
            // 
            // pnlNavigator
            // 
            this.pnlNavigator.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlNavigator.Controls.Add(this.txtPage);
            this.pnlNavigator.Controls.Add(this.btnNextPage);
            this.pnlNavigator.Controls.Add(this.btnPreviousPage);
            this.pnlNavigator.Controls.Add(this.btnLastPage);
            this.pnlNavigator.Controls.Add(this.btnFirstPage);
            this.pnlNavigator.Location = new System.Drawing.Point(4, 593);
            this.pnlNavigator.Name = "pnlNavigator";
            this.pnlNavigator.Size = new System.Drawing.Size(198, 27);
            this.pnlNavigator.TabIndex = 416;
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
            this.lblTotalCount.Location = new System.Drawing.Point(1018, 598);
            this.lblTotalCount.Name = "lblTotalCount";
            this.lblTotalCount.Size = new System.Drawing.Size(215, 14);
            this.lblTotalCount.TabIndex = 420;
            this.lblTotalCount.Text = "STATUS";
            this.lblTotalCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblTotalCount.Visible = false;
            // 
            // ucStatus
            // 
            this.ucStatus.Location = new System.Drawing.Point(207, 583);
            this.ucStatus.Name = "ucStatus";
            this.ucStatus.Size = new System.Drawing.Size(603, 33);
            this.ucStatus.TabIndex = 345;
            this.ucStatus.Visible = false;
            // 
            // frmFindME
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1352, 623);
            this.Controls.Add(this.lblTotalCount);
            this.Controls.Add(this.pnlNavigator);
            this.Controls.Add(this.btnPreview);
            this.Controls.Add(this.pnlSearch);
            this.Controls.Add(this.pnlList);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.ucStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "frmFindME";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmFindMasterEngine_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmFindME_KeyDown);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).EndInit();
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.pnlList.ResumeLayout(false);
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

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel pnlHeader;
        private Bunifu.Framework.UI.BunifuImageButton bunifuImageButton1;
        private Bunifu.Framework.UI.BunifuImageButton btnExit;
        private Bunifu.Framework.UI.BunifuCustomLabel lblHeader;
        private Bunifu.Framework.UI.BunifuCards pnlSearch;
        private System.Windows.Forms.TextBox txtLineNo;
        private System.Windows.Forms.Label lblSearchOptionDetails;
        private System.Windows.Forms.Label lblSearchOption;
        private System.Windows.Forms.Button btnSearchOption;
        private System.Windows.Forms.Button btnSearchClear;
        private Bunifu.Framework.UI.BunifuCards pnlList;
        private System.Windows.Forms.ListView lvwSearch;
        private System.Windows.Forms.Label lblSearchStatus;
        private System.Windows.Forms.Button btnPreview;
        private Bunifu.Framework.UI.BunifuDragControl bunifuDragControl2;
        private Bunifu.Framework.UI.BunifuElipse bunifuElipse1;
        private ucStatus ucStatus;
        private System.Windows.Forms.Panel pnlNavigator;
        private System.Windows.Forms.TextBox txtPage;
        private Bunifu.Framework.UI.BunifuImageButton btnNextPage;
        private Bunifu.Framework.UI.BunifuImageButton btnPreviousPage;
        private Bunifu.Framework.UI.BunifuImageButton btnLastPage;
        private Bunifu.Framework.UI.BunifuImageButton btnFirstPage;
        private System.Windows.Forms.Label lblTotalCount;
    }
}