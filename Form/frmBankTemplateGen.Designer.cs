using System.Collections.Generic;

namespace MIS
{
    partial class frmBankTemplateGen
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBankTemplateGen));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.bunifuImageButton1 = new Bunifu.Framework.UI.BunifuImageButton();
            this.lblSubHeader = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.btnMinimize = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnExit = new Bunifu.Framework.UI.BunifuImageButton();
            this.lblHeader = new System.Windows.Forms.Label();
            this.tabGenerator = new System.Windows.Forms.TabControl();
            this.tabConvert = new System.Windows.Forms.TabPage();
            this.pnlSource = new System.Windows.Forms.Panel();
            this.lblValidation = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtSourcePath = new System.Windows.Forms.TextBox();
            this.lblPath = new System.Windows.Forms.Label();
            this.pnlSourceHeader = new System.Windows.Forms.Panel();
            this.lblSourceHeader = new System.Windows.Forms.Label();
            this.lblFieldsHeader = new System.Windows.Forms.Label();
            this.lblFieldsHint = new System.Windows.Forms.Label();
            this.grdBankFields = new System.Windows.Forms.DataGridView();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.pnlBorderLeft = new System.Windows.Forms.Panel();
            this.pnlBorderRight = new System.Windows.Forms.Panel();
            this.pnlBorderBottom = new System.Windows.Forms.Panel();
            this.bunifuElipse1 = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.bunifuDragControl2 = new Bunifu.Framework.UI.BunifuDragControl(this.components);
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.colIrRequestId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrVendor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrRequestDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrRequestor = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrRequestType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrRequestPrioritization = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colIrPosSetup = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colIrPosType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colIrPosConnectionType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colIrTargetInstallationDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrRemarks = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrMid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrTid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrMerchantLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrCity = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrArea1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colIrArea2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrContactPerson = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIrContactNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).BeginInit();
            this.tabGenerator.SuspendLayout();
            this.tabConvert.SuspendLayout();
            this.pnlSource.SuspendLayout();
            this.pnlSourceHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdBankFields)).BeginInit();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.Maroon;
            this.pnlHeader.Controls.Add(this.bunifuImageButton1);
            this.pnlHeader.Controls.Add(this.lblSubHeader);
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
            this.bunifuImageButton1.BackColor = System.Drawing.Color.Maroon;
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
            // lblSubHeader
            // 
            this.lblSubHeader.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubHeader.ForeColor = System.Drawing.Color.Yellow;
            this.lblSubHeader.Location = new System.Drawing.Point(914, 8);
            this.lblSubHeader.Name = "lblSubHeader";
            this.lblSubHeader.Size = new System.Drawing.Size(411, 20);
            this.lblSubHeader.TabIndex = 416;
            this.lblSubHeader.Text = "-";
            this.lblSubHeader.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnMinimize
            // 
            this.btnMinimize.BackColor = System.Drawing.Color.Transparent;
            this.btnMinimize.Image = ((System.Drawing.Image)(resources.GetObject("btnMinimize.Image")));
            this.btnMinimize.ImageActive = null;
            this.btnMinimize.Location = new System.Drawing.Point(1339, 8);
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
            this.btnExit.BackColor = System.Drawing.Color.Maroon;
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageActive = null;
            this.btnExit.Location = new System.Drawing.Point(1364, 8);
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
            // tabGenerator
            // 
            this.tabGenerator.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabGenerator.Controls.Add(this.tabConvert);
            this.tabGenerator.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold);
            this.tabGenerator.Location = new System.Drawing.Point(9, 42);
            this.tabGenerator.Name = "tabGenerator";
            this.tabGenerator.SelectedIndex = 0;
            this.tabGenerator.Size = new System.Drawing.Size(1382, 751);
            this.tabGenerator.TabIndex = 1;
            // 
            // tabConvert
            // 
            this.tabConvert.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabConvert.Controls.Add(this.pnlSource);
            this.tabConvert.Controls.Add(this.lblFieldsHeader);
            this.tabConvert.Controls.Add(this.lblFieldsHint);
            this.tabConvert.Controls.Add(this.grdBankFields);
            this.tabConvert.Controls.Add(this.pnlFooter);
            this.tabConvert.Location = new System.Drawing.Point(4, 28);
            this.tabConvert.Name = "tabConvert";
            this.tabConvert.Padding = new System.Windows.Forms.Padding(3);
            this.tabConvert.Size = new System.Drawing.Size(1374, 719);
            this.tabConvert.TabIndex = 0;
            this.tabConvert.Text = "Convert File";
            // 
            // pnlSource
            // 
            this.pnlSource.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlSource.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSource.Controls.Add(this.lblValidation);
            this.pnlSource.Controls.Add(this.btnBrowse);
            this.pnlSource.Controls.Add(this.txtSourcePath);
            this.pnlSource.Controls.Add(this.lblPath);
            this.pnlSource.Controls.Add(this.pnlSourceHeader);
            this.pnlSource.Location = new System.Drawing.Point(7, 5);
            this.pnlSource.Name = "pnlSource";
            this.pnlSource.Size = new System.Drawing.Size(1360, 91);
            this.pnlSource.TabIndex = 0;
            // 
            // lblValidation
            // 
            this.lblValidation.AutoSize = true;
            this.lblValidation.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.lblValidation.ForeColor = System.Drawing.Color.DimGray;
            this.lblValidation.Location = new System.Drawing.Point(69, 66);
            this.lblValidation.Name = "lblValidation";
            this.lblValidation.Size = new System.Drawing.Size(126, 14);
            this.lblValidation.TabIndex = 4;
            this.lblValidation.Text = "NO FILES SELECTED";
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
            this.btnBrowse.Location = new System.Drawing.Point(1163, 32);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(184, 27);
            this.btnBrowse.TabIndex = 3;
            this.btnBrowse.Text = "BROWSE FILE(S)";
            this.btnBrowse.UseVisualStyleBackColor = false;
            // 
            // txtSourcePath
            // 
            this.txtSourcePath.BackColor = System.Drawing.Color.White;
            this.txtSourcePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSourcePath.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtSourcePath.Location = new System.Drawing.Point(70, 35);
            this.txtSourcePath.Name = "txtSourcePath";
            this.txtSourcePath.ReadOnly = true;
            this.txtSourcePath.Size = new System.Drawing.Size(1085, 20);
            this.txtSourcePath.TabIndex = 2;
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPath.Location = new System.Drawing.Point(4, 38);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(56, 14);
            this.lblPath.TabIndex = 1;
            this.lblPath.Text = "FILES *";
            // 
            // pnlSourceHeader
            // 
            this.pnlSourceHeader.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlSourceHeader.Controls.Add(this.lblSourceHeader);
            this.pnlSourceHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSourceHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlSourceHeader.Name = "pnlSourceHeader";
            this.pnlSourceHeader.Size = new System.Drawing.Size(1358, 25);
            this.pnlSourceHeader.TabIndex = 0;
            // 
            // lblSourceHeader
            // 
            this.lblSourceHeader.AutoSize = true;
            this.lblSourceHeader.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblSourceHeader.Location = new System.Drawing.Point(4, 5);
            this.lblSourceHeader.Name = "lblSourceHeader";
            this.lblSourceHeader.Size = new System.Drawing.Size(133, 14);
            this.lblSourceHeader.TabIndex = 0;
            this.lblSourceHeader.Text = "SOURCE FILE(S)";
            // 
            // lblFieldsHeader
            // 
            this.lblFieldsHeader.AutoSize = true;
            this.lblFieldsHeader.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblFieldsHeader.Location = new System.Drawing.Point(8, 105);
            this.lblFieldsHeader.Name = "lblFieldsHeader";
            this.lblFieldsHeader.Size = new System.Drawing.Size(196, 14);
            this.lblFieldsHeader.TabIndex = 1;
            this.lblFieldsHeader.Text = "MCC IMPORT TEMPLATE PREVIEW";
            // 
            // lblFieldsHint
            // 
            this.lblFieldsHint.AutoSize = true;
            this.lblFieldsHint.Font = new System.Drawing.Font("Arial Narrow", 8.25F);
            this.lblFieldsHint.ForeColor = System.Drawing.Color.DimGray;
            this.lblFieldsHint.Location = new System.Drawing.Point(205, 104);
            this.lblFieldsHint.Name = "lblFieldsHint";
            this.lblFieldsHint.Size = new System.Drawing.Size(462, 15);
            this.lblFieldsHint.TabIndex = 2;
            this.lblFieldsHint.Text = "Each row represents one Installation Request. Edit the values and complete all re" +
    "quired fields before exporting.";
            // 
            // grdBankFields
            // 
            this.grdBankFields.AllowUserToAddRows = false;
            this.grdBankFields.AllowUserToDeleteRows = false;
            this.grdBankFields.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.grdBankFields.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.grdBankFields.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
            this.grdBankFields.BackgroundColor = System.Drawing.Color.GhostWhite;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdBankFields.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.grdBankFields.ColumnHeadersHeight = 64;
            this.grdBankFields.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grdBankFields.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
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
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.Navy;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdBankFields.DefaultCellStyle = dataGridViewCellStyle4;
            this.grdBankFields.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.grdBankFields.EnableHeadersVisualStyles = false;
            this.grdBankFields.GridColor = System.Drawing.Color.Silver;
            this.grdBankFields.Location = new System.Drawing.Point(8, 124);
            this.grdBankFields.MultiSelect = false;
            this.grdBankFields.Name = "grdBankFields";
            this.grdBankFields.RowHeadersWidth = 45;
            this.grdBankFields.RowTemplate.Height = 30;
            this.grdBankFields.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.grdBankFields.Size = new System.Drawing.Size(1359, 545);
            this.grdBankFields.TabIndex = 3;
            // 
            // pnlFooter
            // 
            this.pnlFooter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFooter.Controls.Add(this.btnGenerate);
            this.pnlFooter.Controls.Add(this.btnClose);
            this.pnlFooter.Controls.Add(this.btnReset);
            this.pnlFooter.Controls.Add(this.lblStatus);
            this.pnlFooter.Location = new System.Drawing.Point(7, 676);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(1360, 36);
            this.pnlFooter.TabIndex = 4;
            // 
            // btnGenerate
            // 
            this.btnGenerate.Enabled = false;
            this.btnGenerate.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnGenerate.Location = new System.Drawing.Point(1137, 5);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(214, 25);
            this.btnGenerate.TabIndex = 3;
            this.btnGenerate.Text = "GENERATE IR IMPORT TEMPLATE";
            this.btnGenerate.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnClose.Location = new System.Drawing.Point(1039, 5);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(92, 25);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "CLOSE";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnReset
            // 
            this.btnReset.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnReset.Location = new System.Drawing.Point(941, 5);
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
            this.lblStatus.Location = new System.Drawing.Point(7, 10);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(210, 14);
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
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.WhiteSmoke;
            this.colIrVendor.DefaultCellStyle = dataGridViewCellStyle3;
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
            this.Controls.Add(this.tabGenerator);
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
            this.tabGenerator.ResumeLayout(false);
            this.tabConvert.ResumeLayout(false);
            this.tabConvert.PerformLayout();
            this.pnlSource.ResumeLayout(false);
            this.pnlSource.PerformLayout();
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
        private System.Windows.Forms.TabControl tabGenerator;
        private System.Windows.Forms.TabPage tabConvert;
        private System.Windows.Forms.Panel pnlSource;
        private System.Windows.Forms.Label lblValidation;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtSourcePath;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.Panel pnlSourceHeader;
        private System.Windows.Forms.Label lblSourceHeader;
        private System.Windows.Forms.Label lblFieldsHeader;
        private System.Windows.Forms.Label lblFieldsHint;
        private System.Windows.Forms.DataGridView grdBankFields;
        private System.Windows.Forms.Panel pnlFooter;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Panel pnlBorderLeft;
        private System.Windows.Forms.Panel pnlBorderRight;
        private System.Windows.Forms.Panel pnlBorderBottom;
        private Bunifu.Framework.UI.BunifuElipse bunifuElipse1;
        private Bunifu.Framework.UI.BunifuDragControl bunifuDragControl2;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private Bunifu.Framework.UI.BunifuCustomLabel lblSubHeader;
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
    }
}
