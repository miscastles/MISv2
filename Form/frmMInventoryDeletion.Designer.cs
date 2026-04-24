namespace MIS
{
    partial class frmMInventoryDeletion
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMInventoryDeletion));
            this.bunifuElipse1 = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnMinimize = new Bunifu.Framework.UI.BunifuImageButton();
            this.bunifuImageButton1 = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnExit = new Bunifu.Framework.UI.BunifuImageButton();
            this.bunifuCustomLabel1 = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.panel22 = new System.Windows.Forms.Panel();
            this.panel23 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.panel24 = new System.Windows.Forms.Panel();
            this.tabDeletion = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRefactor = new System.Windows.Forms.Button();
            this.btnValidate = new System.Windows.Forms.Button();
            this.rtbSNList = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.btnCopyClipboard = new System.Windows.Forms.Button();
            this.btnProceedDeletion = new System.Windows.Forms.Button();
            this.lvwList = new System.Windows.Forms.ListView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtTUniqueInput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtReady = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTInput = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtRestricted = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTNotFound = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnClear = new System.Windows.Forms.Button();
            this.bunifuDragControl2 = new Bunifu.Framework.UI.BunifuDragControl(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.cboInventoryType = new System.Windows.Forms.ComboBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).BeginInit();
            this.panel22.SuspendLayout();
            this.panel23.SuspendLayout();
            this.tabDeletion.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel5.SuspendLayout();
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
            this.pnlHeader.Controls.Add(this.btnMinimize);
            this.pnlHeader.Controls.Add(this.bunifuImageButton1);
            this.pnlHeader.Controls.Add(this.btnExit);
            this.pnlHeader.Controls.Add(this.bunifuCustomLabel1);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(939, 29);
            this.pnlHeader.TabIndex = 308;
            // 
            // btnMinimize
            // 
            this.btnMinimize.BackColor = System.Drawing.Color.Transparent;
            this.btnMinimize.Image = ((System.Drawing.Image)(resources.GetObject("btnMinimize.Image")));
            this.btnMinimize.ImageActive = null;
            this.btnMinimize.Location = new System.Drawing.Point(876, 2);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(25, 25);
            this.btnMinimize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnMinimize.TabIndex = 341;
            this.btnMinimize.TabStop = false;
            this.btnMinimize.Zoom = 10;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
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
            this.bunifuImageButton1.TabIndex = 298;
            this.bunifuImageButton1.TabStop = false;
            this.bunifuImageButton1.Zoom = 10;
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Maroon;
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageActive = null;
            this.btnExit.Location = new System.Drawing.Point(907, 2);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(25, 25);
            this.btnExit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnExit.TabIndex = 8;
            this.btnExit.TabStop = false;
            this.btnExit.Zoom = 10;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // bunifuCustomLabel1
            // 
            this.bunifuCustomLabel1.AutoSize = true;
            this.bunifuCustomLabel1.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold);
            this.bunifuCustomLabel1.ForeColor = System.Drawing.Color.White;
            this.bunifuCustomLabel1.Location = new System.Drawing.Point(35, 4);
            this.bunifuCustomLabel1.Name = "bunifuCustomLabel1";
            this.bunifuCustomLabel1.Size = new System.Drawing.Size(223, 18);
            this.bunifuCustomLabel1.TabIndex = 7;
            this.bunifuCustomLabel1.Text = "TOOLS - INVENTORY DELETION";
            // 
            // panel22
            // 
            this.panel22.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel22.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel22.Controls.Add(this.panel23);
            this.panel22.Controls.Add(this.tabDeletion);
            this.panel22.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel22.Location = new System.Drawing.Point(3, 33);
            this.panel22.Name = "panel22";
            this.panel22.Size = new System.Drawing.Size(679, 540);
            this.panel22.TabIndex = 531;
            // 
            // panel23
            // 
            this.panel23.BackColor = System.Drawing.Color.Gainsboro;
            this.panel23.Controls.Add(this.label9);
            this.panel23.Controls.Add(this.label24);
            this.panel23.Controls.Add(this.panel24);
            this.panel23.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel23.Location = new System.Drawing.Point(0, 0);
            this.panel23.Name = "panel23";
            this.panel23.Size = new System.Drawing.Size(677, 26);
            this.panel23.TabIndex = 311;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(3, 6);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(84, 14);
            this.label9.TabIndex = 311;
            this.label9.Text = "INFORMATION";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.ForeColor = System.Drawing.Color.Black;
            this.label24.Location = new System.Drawing.Point(3, 4);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(0, 14);
            this.label24.TabIndex = 310;
            // 
            // panel24
            // 
            this.panel24.BackColor = System.Drawing.Color.Silver;
            this.panel24.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel24.Location = new System.Drawing.Point(0, 24);
            this.panel24.Name = "panel24";
            this.panel24.Size = new System.Drawing.Size(677, 2);
            this.panel24.TabIndex = 0;
            // 
            // tabDeletion
            // 
            this.tabDeletion.Controls.Add(this.tabPage1);
            this.tabDeletion.Controls.Add(this.tabPage2);
            this.tabDeletion.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabDeletion.Location = new System.Drawing.Point(-1, 27);
            this.tabDeletion.Name = "tabDeletion";
            this.tabDeletion.SelectedIndex = 0;
            this.tabDeletion.Size = new System.Drawing.Size(680, 512);
            this.tabDeletion.TabIndex = 533;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.btnRefactor);
            this.tabPage1.Controls.Add(this.btnValidate);
            this.tabPage1.Controls.Add(this.rtbSNList);
            this.tabPage1.Location = new System.Drawing.Point(4, 23);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(672, 485);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "PASTE/LOAD SN";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Courier New", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Blue;
            this.label2.Location = new System.Drawing.Point(383, 452);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(286, 30);
            this.label2.TabIndex = 537;
            this.label2.Text = "*Note: Copy/paste SN from excel or comma delimeted SN.";
            // 
            // btnRefactor
            // 
            this.btnRefactor.BackColor = System.Drawing.Color.Blue;
            this.btnRefactor.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnRefactor.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefactor.ForeColor = System.Drawing.Color.White;
            this.btnRefactor.Location = new System.Drawing.Point(3, 450);
            this.btnRefactor.Name = "btnRefactor";
            this.btnRefactor.Size = new System.Drawing.Size(171, 31);
            this.btnRefactor.TabIndex = 535;
            this.btnRefactor.Text = "REFACTOR";
            this.btnRefactor.UseVisualStyleBackColor = false;
            this.btnRefactor.Click += new System.EventHandler(this.btnRefactor_Click);
            // 
            // btnValidate
            // 
            this.btnValidate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnValidate.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnValidate.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnValidate.ForeColor = System.Drawing.Color.White;
            this.btnValidate.Location = new System.Drawing.Point(180, 450);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(171, 31);
            this.btnValidate.TabIndex = 534;
            this.btnValidate.Text = "VALIDATE LIST";
            this.btnValidate.UseVisualStyleBackColor = false;
            this.btnValidate.Click += new System.EventHandler(this.btnProcess_Click);
            // 
            // rtbSNList
            // 
            this.rtbSNList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbSNList.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbSNList.Location = new System.Drawing.Point(3, 3);
            this.rtbSNList.Name = "rtbSNList";
            this.rtbSNList.Size = new System.Drawing.Size(666, 443);
            this.rtbSNList.TabIndex = 532;
            this.rtbSNList.Text = "";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label8);
            this.tabPage2.Controls.Add(this.btnCopyClipboard);
            this.tabPage2.Controls.Add(this.btnProceedDeletion);
            this.tabPage2.Controls.Add(this.lvwList);
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(672, 485);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "PREVIEW RESULTS";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Courier New", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Blue;
            this.label8.Location = new System.Drawing.Point(421, 452);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(245, 30);
            this.label8.TabIndex = 538;
            this.label8.Text = "*Note: Ready to delete result will only be deleted.";
            // 
            // btnCopyClipboard
            // 
            this.btnCopyClipboard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnCopyClipboard.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCopyClipboard.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnCopyClipboard.ForeColor = System.Drawing.Color.White;
            this.btnCopyClipboard.Location = new System.Drawing.Point(178, 450);
            this.btnCopyClipboard.Name = "btnCopyClipboard";
            this.btnCopyClipboard.Size = new System.Drawing.Size(171, 31);
            this.btnCopyClipboard.TabIndex = 536;
            this.btnCopyClipboard.Text = "COPY CLIPBOARD";
            this.btnCopyClipboard.UseVisualStyleBackColor = false;
            this.btnCopyClipboard.Click += new System.EventHandler(this.btnCopyClipboard_Click);
            // 
            // btnProceedDeletion
            // 
            this.btnProceedDeletion.BackColor = System.Drawing.Color.Green;
            this.btnProceedDeletion.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnProceedDeletion.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnProceedDeletion.ForeColor = System.Drawing.Color.White;
            this.btnProceedDeletion.Location = new System.Drawing.Point(1, 450);
            this.btnProceedDeletion.Name = "btnProceedDeletion";
            this.btnProceedDeletion.Size = new System.Drawing.Size(171, 31);
            this.btnProceedDeletion.TabIndex = 535;
            this.btnProceedDeletion.Text = "PROCEED DELETION";
            this.btnProceedDeletion.UseVisualStyleBackColor = false;
            this.btnProceedDeletion.Click += new System.EventHandler(this.btnProceedDeletion_Click);
            // 
            // lvwList
            // 
            this.lvwList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvwList.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvwList.HideSelection = false;
            this.lvwList.Location = new System.Drawing.Point(1, 3);
            this.lvwList.Name = "lvwList";
            this.lvwList.Size = new System.Drawing.Size(669, 444);
            this.lvwList.TabIndex = 315;
            this.lvwList.UseCompatibleStateImageBehavior = false;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txtTUniqueInput);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtReady);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txtTInput);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.txtRestricted);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.txtTNotFound);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(685, 102);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(251, 166);
            this.panel1.TabIndex = 532;
            // 
            // txtTUniqueInput
            // 
            this.txtTUniqueInput.BackColor = System.Drawing.Color.White;
            this.txtTUniqueInput.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTUniqueInput.Location = new System.Drawing.Point(154, 60);
            this.txtTUniqueInput.MaxLength = 4;
            this.txtTUniqueInput.Name = "txtTUniqueInput";
            this.txtTUniqueInput.ReadOnly = true;
            this.txtTUniqueInput.Size = new System.Drawing.Size(91, 22);
            this.txtTUniqueInput.TabIndex = 531;
            this.txtTUniqueInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(2, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 14);
            this.label1.TabIndex = 530;
            this.label1.Text = "TOTAL UNIQUE INPUT:";
            // 
            // txtReady
            // 
            this.txtReady.BackColor = System.Drawing.Color.White;
            this.txtReady.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReady.ForeColor = System.Drawing.Color.Green;
            this.txtReady.Location = new System.Drawing.Point(155, 87);
            this.txtReady.MaxLength = 4;
            this.txtReady.Name = "txtReady";
            this.txtReady.ReadOnly = true;
            this.txtReady.Size = new System.Drawing.Size(91, 22);
            this.txtReady.TabIndex = 529;
            this.txtReady.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 14);
            this.label3.TabIndex = 528;
            this.label3.Text = "READY TO DELETE:";
            // 
            // txtTInput
            // 
            this.txtTInput.BackColor = System.Drawing.Color.White;
            this.txtTInput.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTInput.Location = new System.Drawing.Point(154, 30);
            this.txtTInput.MaxLength = 10;
            this.txtTInput.Name = "txtTInput";
            this.txtTInput.ReadOnly = true;
            this.txtTInput.Size = new System.Drawing.Size(91, 22);
            this.txtTInput.TabIndex = 527;
            this.txtTInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(1, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 14);
            this.label4.TabIndex = 526;
            this.label4.Text = "TOTAL INPUT:";
            // 
            // txtRestricted
            // 
            this.txtRestricted.BackColor = System.Drawing.Color.White;
            this.txtRestricted.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRestricted.ForeColor = System.Drawing.Color.Red;
            this.txtRestricted.Location = new System.Drawing.Point(154, 138);
            this.txtRestricted.MaxLength = 2;
            this.txtRestricted.Name = "txtRestricted";
            this.txtRestricted.ReadOnly = true;
            this.txtRestricted.Size = new System.Drawing.Size(93, 22);
            this.txtRestricted.TabIndex = 525;
            this.txtRestricted.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(2, 142);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(147, 14);
            this.label5.TabIndex = 524;
            this.label5.Text = "IN USE / RESTRICTED:";
            // 
            // txtTNotFound
            // 
            this.txtTNotFound.BackColor = System.Drawing.Color.White;
            this.txtTNotFound.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTNotFound.ForeColor = System.Drawing.Color.Blue;
            this.txtTNotFound.Location = new System.Drawing.Point(154, 112);
            this.txtTNotFound.MaxLength = 16;
            this.txtTNotFound.Name = "txtTNotFound";
            this.txtTNotFound.ReadOnly = true;
            this.txtTNotFound.Size = new System.Drawing.Size(92, 22);
            this.txtTNotFound.TabIndex = 330;
            this.txtTNotFound.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(1, 116);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 14);
            this.label6.TabIndex = 329;
            this.label6.Text = "NOT FOUND:";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Gainsboro;
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(249, 26);
            this.panel3.TabIndex = 311;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(3, 4);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 14);
            this.label7.TabIndex = 310;
            this.label7.Text = "SUMMARY";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Silver;
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 24);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(249, 2);
            this.panel4.TabIndex = 0;
            // 
            // btnClear
            // 
            this.btnClear.BackColor = System.Drawing.Color.Red;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnClear.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.Location = new System.Drawing.Point(685, 534);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(247, 31);
            this.btnClear.TabIndex = 533;
            this.btnClear.Text = "CLEAR";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // bunifuDragControl2
            // 
            this.bunifuDragControl2.Fixed = true;
            this.bunifuDragControl2.Horizontal = true;
            this.bunifuDragControl2.TargetControl = this.pnlHeader;
            this.bunifuDragControl2.Vertical = true;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.cboInventoryType);
            this.panel2.Controls.Add(this.panel5);
            this.panel2.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel2.Location = new System.Drawing.Point(684, 33);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(251, 63);
            this.panel2.TabIndex = 534;
            // 
            // cboInventoryType
            // 
            this.cboInventoryType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboInventoryType.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboInventoryType.FormattingEnabled = true;
            this.cboInventoryType.Items.AddRange(new object[] {
            "[NOT SPECIFIED]",
            "TERMINAL",
            "SIM"});
            this.cboInventoryType.Location = new System.Drawing.Point(3, 32);
            this.cboInventoryType.Name = "cboInventoryType";
            this.cboInventoryType.Size = new System.Drawing.Size(246, 24);
            this.cboInventoryType.TabIndex = 312;
            this.cboInventoryType.SelectedIndexChanged += new System.EventHandler(this.cboInventoryType_SelectedIndexChanged);
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.Gainsboro;
            this.panel5.Controls.Add(this.label10);
            this.panel5.Controls.Add(this.panel6);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel5.Location = new System.Drawing.Point(0, 0);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(249, 26);
            this.panel5.TabIndex = 311;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.Black;
            this.label10.Location = new System.Drawing.Point(3, 4);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(84, 14);
            this.label10.TabIndex = 310;
            this.label10.Text = "SELECT TYPE";
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.Silver;
            this.panel6.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel6.Location = new System.Drawing.Point(0, 24);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(249, 2);
            this.panel6.TabIndex = 0;
            // 
            // frmMInventoryDeletion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(939, 579);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel22);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmMInventoryDeletion";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.TopMost = true;
            this.Activated += new System.EventHandler(this.frmMInventoryDeletion_Activated);
            this.Load += new System.EventHandler(this.frmMInventoryDeletion_Load);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).EndInit();
            this.panel22.ResumeLayout(false);
            this.panel23.ResumeLayout(false);
            this.panel23.PerformLayout();
            this.tabDeletion.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Bunifu.Framework.UI.BunifuElipse bunifuElipse1;
        private System.Windows.Forms.Panel pnlHeader;
        private Bunifu.Framework.UI.BunifuImageButton bunifuImageButton1;
        private Bunifu.Framework.UI.BunifuImageButton btnExit;
        private Bunifu.Framework.UI.BunifuCustomLabel bunifuCustomLabel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtReady;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtTInput;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtRestricted;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtTNotFound;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel22;
        private System.Windows.Forms.RichTextBox rtbSNList;
        private System.Windows.Forms.Panel panel23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Panel panel24;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TabControl tabDeletion;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.ListView lvwList;
        private System.Windows.Forms.Button btnValidate;
        private System.Windows.Forms.Button btnProceedDeletion;
        private System.Windows.Forms.Button btnRefactor;
        private Bunifu.Framework.UI.BunifuDragControl bunifuDragControl2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.ComboBox cboInventoryType;
        private System.Windows.Forms.TextBox txtTUniqueInput;
        private System.Windows.Forms.Label label1;
        private Bunifu.Framework.UI.BunifuImageButton btnMinimize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCopyClipboard;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
    }
}