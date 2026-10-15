using System.Collections.Generic;

namespace MIS
{
    partial class frmBankTemplateGen
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBankTemplateGen));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle19 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle20 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.bunifuImageButton1 = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnMinimize = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnExit = new Bunifu.Framework.UI.BunifuImageButton();
            this.lblHeader = new System.Windows.Forms.Label();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblTotalImported = new System.Windows.Forms.Label();
            this.lvwExcelTemplate = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lblValidation = new System.Windows.Forms.Label();
            this.pnlSource = new System.Windows.Forms.Panel();
            this.btnLoadFile = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtSourcePath = new System.Windows.Forms.TextBox();
            this.lblFieldsHeader = new System.Windows.Forms.Label();
            this.lblPath = new System.Windows.Forms.Label();
            this.pnlSourceHeader = new System.Windows.Forms.Panel();
            this.lblBulkEditHint = new System.Windows.Forms.Label();
            this.lblSourceHeader = new System.Windows.Forms.Label();
            this.grdBankFields = new System.Windows.Forms.DataGridView();
            this.colIrSelected = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colIrRequestPrioritization = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colIrPosSetup = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colIrPosType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colIrPosConnectionType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colIrArea1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.btnApplyToSelected = new System.Windows.Forms.Button();
            this.lblSelectedRows = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlBorderLeft = new System.Windows.Forms.Panel();
            this.pnlBorderRight = new System.Windows.Forms.Panel();
            this.pnlBorderBottom = new System.Windows.Forms.Panel();
            this.bunifuElipse1 = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.bunifuDragControl2 = new Bunifu.Framework.UI.BunifuDragControl(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrRequestId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrVendor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrRequestDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrRequestor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrRequestType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrTargetInstallationDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrRemarks = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrMid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrTid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrMerchantLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrCity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrArea2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrContactPerson = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrContactNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).BeginInit();
            this.pnlContent.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnlSource.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnLoadFile)).BeginInit();
            this.pnlSourceHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdBankFields)).BeginInit();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.SteelBlue;
            this.pnlHeader.Controls.Add(this.bunifuImageButton1);
            this.pnlHeader.Controls.Add(this.btnMinimize);
            this.pnlHeader.Controls.Add(this.btnExit);
            this.pnlHeader.Controls.Add(this.lblHeader);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1400, 35);
            this.pnlHeader.TabIndex = 0;
            // 
            // bunifuImageButton1
            // 
            this.bunifuImageButton1.BackColor = System.Drawing.Color.Transparent;
            this.bunifuImageButton1.Image = ((System.Drawing.Image)(resources.GetObject("bunifuImageButton1.Image")));
            this.bunifuImageButton1.ImageActive = null;
            this.bunifuImageButton1.Location = new System.Drawing.Point(9, 6);
            this.bunifuImageButton1.Name = "bunifuImageButton1";
            this.bunifuImageButton1.Size = new System.Drawing.Size(23, 22);
            this.bunifuImageButton1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.bunifuImageButton1.TabIndex = 417;
            this.bunifuImageButton1.TabStop = false;
            this.bunifuImageButton1.Zoom = 10;
            // 
            // btnMinimize
            // 
            this.btnMinimize.BackColor = System.Drawing.Color.Transparent;
            this.btnMinimize.Image = ((System.Drawing.Image)(resources.GetObject("btnMinimize.Image")));
            this.btnMinimize.ImageActive = null;
            this.btnMinimize.Location = new System.Drawing.Point(1344, 8);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(22, 20);
            this.btnMinimize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnMinimize.TabIndex = 415;
            this.btnMinimize.TabStop = false;
            this.btnMinimize.Zoom = 10;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageActive = null;
            this.btnExit.Location = new System.Drawing.Point(1369, 8);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(22, 20);
            this.btnExit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnExit.TabIndex = 414;
            this.btnExit.TabStop = false;
            this.btnExit.Zoom = 10;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.White;
            this.lblHeader.Location = new System.Drawing.Point(33, 8);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(212, 18);
            this.lblHeader.TabIndex = 1;
            this.lblHeader.Text = "BANK TEMPLATE GENERATOR";
            // 
            // pnlContent
            // 
            this.pnlContent.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlContent.Controls.Add(this.panel1);
            this.pnlContent.Controls.Add(this.lblValidation);
            this.pnlContent.Controls.Add(this.pnlSource);
            this.pnlContent.Controls.Add(this.grdBankFields);
            this.pnlContent.Controls.Add(this.pnlFooter);
            this.pnlContent.Location = new System.Drawing.Point(9, 42);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(1382, 751);
            this.pnlContent.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Location = new System.Drawing.Point(946, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(433, 393);
            this.panel1.TabIndex = 7;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Controls.Add(this.lvwExcelTemplate);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(433, 393);
            this.panel3.TabIndex = 9;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.lblTotal);
            this.panel2.Controls.Add(this.lblTotalImported);
            this.panel2.Location = new System.Drawing.Point(-1, 357);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(433, 35);
            this.panel2.TabIndex = 8;
            // 
            // lblTotal
            // 
            this.lblTotal.AutoSize = true;
            this.lblTotal.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblTotal.Location = new System.Drawing.Point(56, 9);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(14, 14);
            this.lblTotal.TabIndex = 3;
            this.lblTotal.Text = "0";
            // 
            // lblTotalImported
            // 
            this.lblTotalImported.AutoSize = true;
            this.lblTotalImported.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblTotalImported.Location = new System.Drawing.Point(7, 9);
            this.lblTotalImported.Name = "lblTotalImported";
            this.lblTotalImported.Size = new System.Drawing.Size(56, 14);
            this.lblTotalImported.TabIndex = 2;
            this.lblTotalImported.Text = "TOTAL: ";
            // 
            // lvwExcelTemplate
            // 
            this.lvwExcelTemplate.BackColor = System.Drawing.Color.GhostWhite;
            this.lvwExcelTemplate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvwExcelTemplate.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.lvwExcelTemplate.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvwExcelTemplate.ForeColor = System.Drawing.Color.Black;
            this.lvwExcelTemplate.FullRowSelect = true;
            this.lvwExcelTemplate.HideSelection = false;
            this.lvwExcelTemplate.Location = new System.Drawing.Point(-1, -1);
            this.lvwExcelTemplate.Name = "lvwExcelTemplate";
            this.lvwExcelTemplate.Size = new System.Drawing.Size(433, 359);
            this.lvwExcelTemplate.TabIndex = 314;
            this.lvwExcelTemplate.UseCompatibleStateImageBehavior = false;
            this.lvwExcelTemplate.View = System.Windows.Forms.View.Details;
            this.lvwExcelTemplate.DoubleClick += new System.EventHandler(this.lvwExcelTemplate_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "LINE#";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Filename";
            this.columnHeader2.Width = 133;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Filepath";
            this.columnHeader3.Width = 237;
            // 
            // lblValidation
            // 
            this.lblValidation.AutoSize = true;
            this.lblValidation.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.lblValidation.ForeColor = System.Drawing.Color.DimGray;
            this.lblValidation.Location = new System.Drawing.Point(4, 688);
            this.lblValidation.Name = "lblValidation";
            this.lblValidation.Size = new System.Drawing.Size(126, 14);
            this.lblValidation.TabIndex = 4;
            this.lblValidation.Text = "NO FILES SELECTED";
            // 
            // pnlSource
            // 
            this.pnlSource.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlSource.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSource.Controls.Add(this.btnLoadFile);
            this.pnlSource.Controls.Add(this.btnBrowse);
            this.pnlSource.Controls.Add(this.txtSourcePath);
            this.pnlSource.Controls.Add(this.lblFieldsHeader);
            this.pnlSource.Controls.Add(this.lblPath);
            this.pnlSource.Controls.Add(this.pnlSourceHeader);
            this.pnlSource.Location = new System.Drawing.Point(7, 5);
            this.pnlSource.Name = "pnlSource";
            this.pnlSource.Size = new System.Drawing.Size(937, 86);
            this.pnlSource.TabIndex = 0;
            // 
            // btnLoadFile
            // 
            this.btnLoadFile.BackColor = System.Drawing.Color.Transparent;
            this.btnLoadFile.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLoadFile.ErrorImage = null;
            this.btnLoadFile.Image = ((System.Drawing.Image)(resources.GetObject("btnLoadFile.Image")));
            this.btnLoadFile.ImageActive = null;
            this.btnLoadFile.Location = new System.Drawing.Point(714, 33);
            this.btnLoadFile.Name = "btnLoadFile";
            this.btnLoadFile.Size = new System.Drawing.Size(25, 21);
            this.btnLoadFile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnLoadFile.TabIndex = 293;
            this.btnLoadFile.TabStop = false;
            this.btnLoadFile.Zoom = 10;
            this.btnLoadFile.Click += new System.EventHandler(this.btnLoadFile_Click);
            // 
            // btnBrowse
            // 
            this.btnBrowse.BackColor = System.Drawing.Color.Navy;
            this.btnBrowse.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBrowse.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnBrowse.FlatAppearance.BorderSize = 0;
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnBrowse.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnBrowse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnBrowse.Location = new System.Drawing.Point(767, 30);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(162, 27);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "BROWSE FILE(S)";
            this.btnBrowse.UseVisualStyleBackColor = false;
            // 
            // txtSourcePath
            // 
            this.txtSourcePath.BackColor = System.Drawing.Color.White;
            this.txtSourcePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSourcePath.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSourcePath.Location = new System.Drawing.Point(80, 32);
            this.txtSourcePath.Name = "txtSourcePath";
            this.txtSourcePath.ReadOnly = true;
            this.txtSourcePath.Size = new System.Drawing.Size(628, 24);
            this.txtSourcePath.TabIndex = 2;
            // 
            // lblFieldsHeader
            // 
            this.lblFieldsHeader.AutoSize = true;
            this.lblFieldsHeader.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblFieldsHeader.Location = new System.Drawing.Point(4, 64);
            this.lblFieldsHeader.Name = "lblFieldsHeader";
            this.lblFieldsHeader.Size = new System.Drawing.Size(119, 14);
            this.lblFieldsHeader.TabIndex = 1;
            this.lblFieldsHeader.Text = "TEMPLATE PREVIEW";
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPath.Location = new System.Drawing.Point(4, 35);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(70, 14);
            this.lblPath.TabIndex = 1;
            this.lblPath.Text = "FILE PATH";
            // 
            // pnlSourceHeader
            // 
            this.pnlSourceHeader.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlSourceHeader.Controls.Add(this.lblBulkEditHint);
            this.pnlSourceHeader.Controls.Add(this.lblSourceHeader);
            this.pnlSourceHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSourceHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlSourceHeader.Name = "pnlSourceHeader";
            this.pnlSourceHeader.Size = new System.Drawing.Size(935, 25);
            this.pnlSourceHeader.TabIndex = 0;
            // 
            // lblBulkEditHint
            // 
            this.lblBulkEditHint.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblBulkEditHint.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblBulkEditHint.ForeColor = System.Drawing.Color.Navy;
            this.lblBulkEditHint.Location = new System.Drawing.Point(485, 0);
            this.lblBulkEditHint.Name = "lblBulkEditHint";
            this.lblBulkEditHint.Size = new System.Drawing.Size(450, 25);
            this.lblBulkEditHint.TabIndex = 1;
            this.lblBulkEditHint.Text = "BULK EDIT: APPLY ONE FIELD AT A TIME";
            this.lblBulkEditHint.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSourceHeader
            // 
            this.lblSourceHeader.AutoSize = true;
            this.lblSourceHeader.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblSourceHeader.Location = new System.Drawing.Point(4, 5);
            this.lblSourceHeader.Name = "lblSourceHeader";
            this.lblSourceHeader.Size = new System.Drawing.Size(105, 14);
            this.lblSourceHeader.TabIndex = 0;
            this.lblSourceHeader.Text = "SOURCE FILE(S)";
            // 
            // grdBankFields
            // 
            this.grdBankFields.AllowUserToAddRows = false;
            this.grdBankFields.AllowUserToDeleteRows = false;
            this.grdBankFields.AllowUserToResizeRows = false;
            dataGridViewCellStyle16.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle16.ForeColor = System.Drawing.Color.Black;
            this.grdBankFields.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle16;
            this.grdBankFields.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.grdBankFields.BackgroundColor = System.Drawing.Color.GhostWhite;
            dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle17.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle17.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle17.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle17.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle17.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle17.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdBankFields.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle17;
            this.grdBankFields.ColumnHeadersHeight = 64;
            this.grdBankFields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grdBankFields.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colIrSelected,
            this.colIrRequestId,
            this.colIrVendor,
            this.colIrRequestDate,
            this.colIrRequestor,
            this.colIrRequestType,
            this.colIrRequestPrioritization,
            this.colIrPosSetup,
            this.colIrPosType,
            this.colIrPosConnectionType,
            this.colIrTargetInstallationDate,
            this.colIrRemarks,
            this.colIrMid,
            this.colIrTid,
            this.colIrMerchantLocation,
            this.colIrAddress,
            this.colIrCity,
            this.colIrArea1,
            this.colIrArea2,
            this.colIrContactPerson,
            this.colIrContactNumber});
            dataGridViewCellStyle19.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle19.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle19.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle19.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle19.SelectionBackColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle19.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle19.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdBankFields.DefaultCellStyle = dataGridViewCellStyle19;
            this.grdBankFields.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.grdBankFields.EnableHeadersVisualStyles = false;
            this.grdBankFields.GridColor = System.Drawing.Color.Silver;
            this.grdBankFields.Location = new System.Drawing.Point(7, 89);
            this.grdBankFields.MultiSelect = false;
            this.grdBankFields.Name = "grdBankFields";
            this.grdBankFields.RowHeadersWidth = 45;
            this.grdBankFields.RowTemplate.Height = 30;
            this.grdBankFields.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.grdBankFields.Size = new System.Drawing.Size(937, 593);
            this.grdBankFields.TabIndex = 3;
            // 
            // colIrSelected
            // 
            this.colIrSelected.DataPropertyName = "IsSelected";
            this.colIrSelected.FalseValue = false;
            this.colIrSelected.Frozen = true;
            this.colIrSelected.HeaderText = "ALL";
            this.colIrSelected.Name = "colIrSelected";
            this.colIrSelected.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colIrSelected.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.colIrSelected.TrueValue = true;
            this.colIrSelected.Width = 45;
            // 
            // colIrRequestPrioritization
            // 
            this.colIrRequestPrioritization.DataPropertyName = "RequestPrioritization";
            this.colIrRequestPrioritization.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colIrRequestPrioritization.HeaderText = "Request Prioritization";
            this.colIrRequestPrioritization.Items.AddRange(new object[] {
            "REGULAR",
            "RUSH",
            "SCHEDULED"});
            this.colIrRequestPrioritization.Name = "colIrRequestPrioritization";
            this.colIrRequestPrioritization.Width = 180;
            // 
            // colIrPosSetup
            // 
            this.colIrPosSetup.DataPropertyName = "PosSetup";
            this.colIrPosSetup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colIrPosSetup.HeaderText = "POS Setup";
            this.colIrPosSetup.Items.AddRange(new object[] {
            "BANCNET ONLY",
            "CASHOUT",
            "RETAIL SETUP",
            "RETAIL WITH INSTALLMENT",
            "HOTEL SETUP"});
            this.colIrPosSetup.Name = "colIrPosSetup";
            this.colIrPosSetup.Width = 150;
            // 
            // colIrPosType
            // 
            this.colIrPosType.DataPropertyName = "PosType";
            this.colIrPosType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colIrPosType.HeaderText = "POS Type";
            this.colIrPosType.Items.AddRange(new object[] {
            "[NOT SPECIFIED]",
            "S1F1",
            "S1F2",
            "S1F2-A",
            "S1F3",
            "S1E MINI2"});
            this.colIrPosType.Name = "colIrPosType";
            this.colIrPosType.Width = 180;
            // 
            // colIrPosConnectionType
            // 
            this.colIrPosConnectionType.DataPropertyName = "PosConnectionType";
            this.colIrPosConnectionType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colIrPosConnectionType.HeaderText = "POS Connection Type";
            this.colIrPosConnectionType.Items.AddRange(new object[] {
            "WIFI AND SIM based",
            "WIFI ONLY",
            "SIM based"});
            this.colIrPosConnectionType.Name = "colIrPosConnectionType";
            this.colIrPosConnectionType.Width = 180;
            // 
            // colIrArea1
            // 
            this.colIrArea1.DataPropertyName = "Area1";
            this.colIrArea1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colIrArea1.HeaderText = "Area 1 (Metro Manila / Provincial)";
            this.colIrArea1.Items.AddRange(new object[] {
            "METRO MANILA",
            "PROVINCIAL"});
            this.colIrArea1.Name = "colIrArea1";
            this.colIrArea1.Width = 230;
            // 
            // pnlFooter
            // 
            this.pnlFooter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFooter.Controls.Add(this.btnApplyToSelected);
            this.pnlFooter.Controls.Add(this.lblSelectedRows);
            this.pnlFooter.Controls.Add(this.btnClose);
            this.pnlFooter.Controls.Add(this.btnGenerate);
            this.pnlFooter.Controls.Add(this.btnReset);
            this.pnlFooter.Controls.Add(this.lblStatus);
            this.pnlFooter.Location = new System.Drawing.Point(7, 708);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(1369, 36);
            this.pnlFooter.TabIndex = 4;
            // 
            // btnApplyToSelected
            // 
            this.btnApplyToSelected.Enabled = false;
            this.btnApplyToSelected.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnApplyToSelected.Location = new System.Drawing.Point(910, 5);
            this.btnApplyToSelected.Name = "btnApplyToSelected";
            this.btnApplyToSelected.Size = new System.Drawing.Size(234, 25);
            this.btnApplyToSelected.TabIndex = 4;
            this.btnApplyToSelected.Text = "APPLY CELL TO SELECTED";
            this.btnApplyToSelected.UseVisualStyleBackColor = true;
            // 
            // lblSelectedRows
            // 
            this.lblSelectedRows.AutoSize = true;
            this.lblSelectedRows.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblSelectedRows.ForeColor = System.Drawing.Color.Navy;
            this.lblSelectedRows.Location = new System.Drawing.Point(690, 10);
            this.lblSelectedRows.Name = "lblSelectedRows";
            this.lblSelectedRows.Size = new System.Drawing.Size(119, 14);
            this.lblSelectedRows.TabIndex = 5;
            this.lblSelectedRows.Text = "0 ROW(S) SELECTED";
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(103, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(92, 25);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "CLOSE";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Enabled = false;
            this.btnGenerate.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGenerate.Location = new System.Drawing.Point(1150, 5);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(214, 25);
            this.btnGenerate.TabIndex = 3;
            this.btnGenerate.Text = "GENERATE IR IMPORT TEMPLATE";
            this.btnGenerate.UseVisualStyleBackColor = true;
            // 
            // btnReset
            // 
            this.btnReset.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.Location = new System.Drawing.Point(5, 5);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(92, 25);
            this.btnReset.TabIndex = 1;
            this.btnReset.Text = "RESET";
            this.btnReset.UseVisualStyleBackColor = true;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Location = new System.Drawing.Point(201, 10);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(217, 14);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "NO INSTALLATION REQUEST LOADED";
            // 
            // pnlBorderLeft
            // 
            this.pnlBorderLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pnlBorderLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlBorderLeft.Location = new System.Drawing.Point(0, 35);
            this.pnlBorderLeft.Name = "pnlBorderLeft";
            this.pnlBorderLeft.Size = new System.Drawing.Size(1, 767);
            this.pnlBorderLeft.TabIndex = 2;
            // 
            // pnlBorderRight
            // 
            this.pnlBorderRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pnlBorderRight.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlBorderRight.Location = new System.Drawing.Point(1398, 35);
            this.pnlBorderRight.Name = "pnlBorderRight";
            this.pnlBorderRight.Size = new System.Drawing.Size(2, 765);
            this.pnlBorderRight.TabIndex = 0;
            // 
            // pnlBorderBottom
            // 
            this.pnlBorderBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.pnlBorderBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBorderBottom.Location = new System.Drawing.Point(1, 800);
            this.pnlBorderBottom.Name = "pnlBorderBottom";
            this.pnlBorderBottom.Size = new System.Drawing.Size(1399, 2);
            this.pnlBorderBottom.TabIndex = 1;
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
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "RequestId";
            this.dataGridViewTextBoxColumn1.HeaderText = "Request ID";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 170;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "Vendor";
            dataGridViewCellStyle20.BackColor = System.Drawing.Color.WhiteSmoke;
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle20;
            this.dataGridViewTextBoxColumn2.HeaderText = "Vendor";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.Width = 95;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "RequestDate";
            this.dataGridViewTextBoxColumn3.HeaderText = "Request Date";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Width = 145;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.DataPropertyName = "Requestor";
            this.dataGridViewTextBoxColumn4.HeaderText = "Requestor";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 210;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.DataPropertyName = "RequestType";
            this.dataGridViewTextBoxColumn5.HeaderText = "Request Type";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.Width = 210;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.DataPropertyName = "TargetInstallationDate";
            this.dataGridViewTextBoxColumn6.HeaderText = "Target Installation date";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.Width = 165;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.DataPropertyName = "Remarks";
            this.dataGridViewTextBoxColumn7.HeaderText = "RM Instruction / Remarks";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.Width = 260;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.DataPropertyName = "Mid";
            this.dataGridViewTextBoxColumn8.HeaderText = "Merchant ID (MID)";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            this.dataGridViewTextBoxColumn8.Width = 165;
            // 
            // dataGridViewTextBoxColumn9
            // 
            this.dataGridViewTextBoxColumn9.DataPropertyName = "Tid";
            this.dataGridViewTextBoxColumn9.HeaderText = "Terminal ID (TID)";
            this.dataGridViewTextBoxColumn9.Name = "dataGridViewTextBoxColumn9";
            this.dataGridViewTextBoxColumn9.Width = 155;
            // 
            // dataGridViewTextBoxColumn10
            // 
            this.dataGridViewTextBoxColumn10.DataPropertyName = "MerchantLocation";
            this.dataGridViewTextBoxColumn10.HeaderText = "Merchant Location/DBA Name";
            this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
            this.dataGridViewTextBoxColumn10.Width = 240;
            // 
            // dataGridViewTextBoxColumn11
            // 
            this.dataGridViewTextBoxColumn11.DataPropertyName = "Address";
            this.dataGridViewTextBoxColumn11.HeaderText = "Address";
            this.dataGridViewTextBoxColumn11.Name = "dataGridViewTextBoxColumn11";
            this.dataGridViewTextBoxColumn11.Width = 300;
            // 
            // dataGridViewTextBoxColumn12
            // 
            this.dataGridViewTextBoxColumn12.DataPropertyName = "City";
            this.dataGridViewTextBoxColumn12.HeaderText = "City";
            this.dataGridViewTextBoxColumn12.Name = "dataGridViewTextBoxColumn12";
            this.dataGridViewTextBoxColumn12.Width = 145;
            // 
            // dataGridViewTextBoxColumn13
            // 
            this.dataGridViewTextBoxColumn13.DataPropertyName = "Area2";
            this.dataGridViewTextBoxColumn13.HeaderText = "Area 2 (Region/Zone/Area code)";
            this.dataGridViewTextBoxColumn13.Name = "dataGridViewTextBoxColumn13";
            this.dataGridViewTextBoxColumn13.Width = 225;
            // 
            // dataGridViewTextBoxColumn14
            // 
            this.dataGridViewTextBoxColumn14.DataPropertyName = "ContactPerson";
            this.dataGridViewTextBoxColumn14.HeaderText = "Contact Person";
            this.dataGridViewTextBoxColumn14.Name = "dataGridViewTextBoxColumn14";
            this.dataGridViewTextBoxColumn14.Width = 190;
            // 
            // dataGridViewTextBoxColumn15
            // 
            this.dataGridViewTextBoxColumn15.DataPropertyName = "ContactNumber";
            this.dataGridViewTextBoxColumn15.HeaderText = "Contact Number";
            this.dataGridViewTextBoxColumn15.Name = "dataGridViewTextBoxColumn15";
            this.dataGridViewTextBoxColumn15.Width = 165;
            // 
            // colIrRequestId
            // 
            this.colIrRequestId.DataPropertyName = "RequestId";
            this.colIrRequestId.HeaderText = "Request ID";
            this.colIrRequestId.Name = "colIrRequestId";
            this.colIrRequestId.Width = 170;
            // 
            // colIrVendor
            // 
            this.colIrVendor.DataPropertyName = "Vendor";
            dataGridViewCellStyle18.BackColor = System.Drawing.Color.WhiteSmoke;
            this.colIrVendor.DefaultCellStyle = dataGridViewCellStyle18;
            this.colIrVendor.HeaderText = "Vendor";
            this.colIrVendor.Name = "colIrVendor";
            this.colIrVendor.ReadOnly = true;
            this.colIrVendor.Width = 95;
            // 
            // colIrRequestDate
            // 
            this.colIrRequestDate.DataPropertyName = "RequestDate";
            this.colIrRequestDate.HeaderText = "Request Date";
            this.colIrRequestDate.Name = "colIrRequestDate";
            this.colIrRequestDate.Width = 145;
            // 
            // colIrRequestor
            // 
            this.colIrRequestor.DataPropertyName = "Requestor";
            this.colIrRequestor.HeaderText = "Requestor";
            this.colIrRequestor.Name = "colIrRequestor";
            this.colIrRequestor.Width = 210;
            // 
            // colIrRequestType
            // 
            this.colIrRequestType.DataPropertyName = "RequestType";
            this.colIrRequestType.HeaderText = "Request Type";
            this.colIrRequestType.Name = "colIrRequestType";
            this.colIrRequestType.Width = 210;
            // 
            // colIrTargetInstallationDate
            // 
            this.colIrTargetInstallationDate.DataPropertyName = "TargetInstallationDate";
            this.colIrTargetInstallationDate.HeaderText = "Target Installation date";
            this.colIrTargetInstallationDate.Name = "colIrTargetInstallationDate";
            this.colIrTargetInstallationDate.Width = 165;
            // 
            // colIrRemarks
            // 
            this.colIrRemarks.DataPropertyName = "Remarks";
            this.colIrRemarks.HeaderText = "RM Instruction / Remarks";
            this.colIrRemarks.Name = "colIrRemarks";
            this.colIrRemarks.Width = 260;
            // 
            // colIrMid
            // 
            this.colIrMid.DataPropertyName = "Mid";
            this.colIrMid.HeaderText = "Merchant ID (MID)";
            this.colIrMid.Name = "colIrMid";
            this.colIrMid.Width = 165;
            // 
            // colIrTid
            // 
            this.colIrTid.DataPropertyName = "Tid";
            this.colIrTid.HeaderText = "Terminal ID (TID)";
            this.colIrTid.Name = "colIrTid";
            this.colIrTid.Width = 155;
            // 
            // colIrMerchantLocation
            // 
            this.colIrMerchantLocation.DataPropertyName = "MerchantLocation";
            this.colIrMerchantLocation.HeaderText = "Merchant Location/DBA Name";
            this.colIrMerchantLocation.Name = "colIrMerchantLocation";
            this.colIrMerchantLocation.Width = 240;
            // 
            // colIrAddress
            // 
            this.colIrAddress.DataPropertyName = "Address";
            this.colIrAddress.HeaderText = "Address";
            this.colIrAddress.Name = "colIrAddress";
            this.colIrAddress.Width = 300;
            // 
            // colIrCity
            // 
            this.colIrCity.DataPropertyName = "City";
            this.colIrCity.HeaderText = "City";
            this.colIrCity.Name = "colIrCity";
            this.colIrCity.Width = 145;
            // 
            // colIrArea2
            // 
            this.colIrArea2.DataPropertyName = "Area2";
            this.colIrArea2.HeaderText = "Area 2 (Region/Zone/Area code)";
            this.colIrArea2.Name = "colIrArea2";
            this.colIrArea2.Width = 225;
            // 
            // colIrContactPerson
            // 
            this.colIrContactPerson.DataPropertyName = "ContactPerson";
            this.colIrContactPerson.HeaderText = "Contact Person";
            this.colIrContactPerson.Name = "colIrContactPerson";
            this.colIrContactPerson.Width = 190;
            // 
            // colIrContactNumber
            // 
            this.colIrContactNumber.DataPropertyName = "ContactNumber";
            this.colIrContactNumber.HeaderText = "Contact Number";
            this.colIrContactNumber.Name = "colIrContactNumber";
            this.colIrContactNumber.Width = 165;
            // 
            // frmBankTemplateGen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(1400, 802);
            this.Controls.Add(this.pnlBorderRight);
            this.Controls.Add(this.pnlBorderBottom);
            this.Controls.Add(this.pnlBorderLeft);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "frmBankTemplateGen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BANK TEMPLATE GENERATOR";
            this.Load += new System.EventHandler(this.frmBankTemplateGen_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).EndInit();
            this.pnlContent.ResumeLayout(false);
            this.pnlContent.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.pnlSource.ResumeLayout(false);
            this.pnlSource.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnLoadFile)).EndInit();
            this.pnlSourceHeader.ResumeLayout(false);
            this.pnlSourceHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdBankFields)).EndInit();
            this.pnlFooter.ResumeLayout(false);
            this.pnlFooter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Panel pnlSource;
        private System.Windows.Forms.Label lblValidation;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtSourcePath;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.Panel pnlSourceHeader;
        private System.Windows.Forms.Label lblBulkEditHint;
        private System.Windows.Forms.Label lblSourceHeader;
        private System.Windows.Forms.Label lblFieldsHeader;
        private System.Windows.Forms.DataGridView grdBankFields;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colIrSelected;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Button btnApplyToSelected;
        private System.Windows.Forms.Label lblSelectedRows;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel pnlBorderLeft;
        private System.Windows.Forms.Panel pnlBorderRight;
        private System.Windows.Forms.Panel pnlBorderBottom;
        private Bunifu.Framework.UI.BunifuElipse bunifuElipse1;
        private Bunifu.Framework.UI.BunifuDragControl bunifuDragControl2;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Bunifu.Framework.UI.BunifuImageButton btnMinimize;
        private Bunifu.Framework.UI.BunifuImageButton btnExit;
        private Bunifu.Framework.UI.BunifuImageButton bunifuImageButton1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIrRequestId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIrVendor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIrRequestDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIrRequestor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIrRequestType;
        private System.Windows.Forms.DataGridViewComboBoxColumn colIrRequestPrioritization;
        private System.Windows.Forms.DataGridViewComboBoxColumn colIrPosSetup;
        private System.Windows.Forms.DataGridViewComboBoxColumn colIrPosType;
        private System.Windows.Forms.DataGridViewComboBoxColumn colIrPosConnectionType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIrTargetInstallationDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIrRemarks;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIrMid;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIrTid;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIrMerchantLocation;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIrAddress;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIrCity;
        private System.Windows.Forms.DataGridViewComboBoxColumn colIrArea1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIrArea2;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIrContactPerson;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIrContactNumber;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnClose;
        private Bunifu.Framework.UI.BunifuImageButton btnLoadFile;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblTotal;
        private System.Windows.Forms.Label lblTotalImported;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn9;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn11;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn12;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn13;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn14;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn15;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ListView lvwExcelTemplate;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}