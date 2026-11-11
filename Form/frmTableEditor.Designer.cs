namespace MIS
{
    partial class frmTableEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTableEditor));
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnMinimize = new Bunifu.Framework.UI.BunifuImageButton();
            this.bunifuImageButton1 = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnExit = new Bunifu.Framework.UI.BunifuImageButton();
            this.lblHeader = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel4 = new System.Windows.Forms.Panel();
            this.tabDBData = new System.Windows.Forms.TabControl();
            this.TabTables = new System.Windows.Forms.TabPage();
            this.pnlSource = new System.Windows.Forms.Panel();
            this.lvwTableList = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabFunctions = new System.Windows.Forms.TabPage();
            this.lvwFunctionList = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabStoredProcedures = new System.Windows.Forms.TabPage();
            this.lvwRoutineList = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.rteQuery = new FastColoredTextBoxNS.FastColoredTextBox();
            this.pnlSourceHeader = new System.Windows.Forms.Panel();
            this.lblRuntime = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.barLoading = new System.Windows.Forms.ProgressBar();
            this.lblSelectedRow = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSourceHeader = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblPage = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tabReturns = new System.Windows.Forms.TabControl();
            this.tabDataReturn = new System.Windows.Forms.TabPage();
            this.dgvTableContents = new System.Windows.Forms.DataGridView();
            this.tabRawReturn = new System.Windows.Forms.TabPage();
            this.rteRawReturn = new System.Windows.Forms.RichTextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.bunifuElipse1 = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.bunifuDragControl1 = new Bunifu.Framework.UI.BunifuDragControl(this.components);
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tabDBData.SuspendLayout();
            this.TabTables.SuspendLayout();
            this.pnlSource.SuspendLayout();
            this.tabFunctions.SuspendLayout();
            this.tabStoredProcedures.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.rteQuery)).BeginInit();
            this.pnlSourceHeader.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabReturns.SuspendLayout();
            this.tabDataReturn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTableContents)).BeginInit();
            this.tabRawReturn.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pnlHeader.Controls.Add(this.btnMinimize);
            this.pnlHeader.Controls.Add(this.bunifuImageButton1);
            this.pnlHeader.Controls.Add(this.btnExit);
            this.pnlHeader.Controls.Add(this.lblHeader);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1680, 35);
            this.pnlHeader.TabIndex = 324;
            // 
            // btnMinimize
            // 
            this.btnMinimize.BackColor = System.Drawing.Color.Transparent;
            this.btnMinimize.Image = ((System.Drawing.Image)(resources.GetObject("btnMinimize.Image")));
            this.btnMinimize.ImageActive = null;
            this.btnMinimize.Location = new System.Drawing.Point(1624, 6);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(22, 23);
            this.btnMinimize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnMinimize.TabIndex = 301;
            this.btnMinimize.TabStop = false;
            this.btnMinimize.Zoom = 10;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // bunifuImageButton1
            // 
            this.bunifuImageButton1.BackColor = System.Drawing.Color.Transparent;
            this.bunifuImageButton1.Image = ((System.Drawing.Image)(resources.GetObject("bunifuImageButton1.Image")));
            this.bunifuImageButton1.ImageActive = null;
            this.bunifuImageButton1.Location = new System.Drawing.Point(8, 6);
            this.bunifuImageButton1.Name = "bunifuImageButton1";
            this.bunifuImageButton1.Size = new System.Drawing.Size(21, 23);
            this.bunifuImageButton1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.bunifuImageButton1.TabIndex = 300;
            this.bunifuImageButton1.TabStop = false;
            this.bunifuImageButton1.Zoom = 10;
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageActive = null;
            this.btnExit.Location = new System.Drawing.Point(1652, 6);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(21, 23);
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
            this.lblHeader.Location = new System.Drawing.Point(33, 9);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(142, 18);
            this.lblHeader.TabIndex = 7;
            this.lblHeader.Text = "DEV-TABLE-EDITOR";
            // 
            // tabControl
            // 
            this.tabControl.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl.Controls.Add(this.tabPage1);
            this.tabControl.Controls.Add(this.tabPage2);
            this.tabControl.Location = new System.Drawing.Point(4, 41);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(1676, 950);
            this.tabControl.TabIndex = 332;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel4);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Controls.Add(this.panel3);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1668, 920);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "TABLES";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.tabDBData);
            this.panel4.Location = new System.Drawing.Point(1388, 1);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(280, 919);
            this.panel4.TabIndex = 339;
            // 
            // tabDBData
            // 
            this.tabDBData.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabDBData.CausesValidation = false;
            this.tabDBData.Controls.Add(this.TabTables);
            this.tabDBData.Controls.Add(this.tabFunctions);
            this.tabDBData.Controls.Add(this.tabStoredProcedures);
            this.tabDBData.HotTrack = true;
            this.tabDBData.Location = new System.Drawing.Point(2, 1);
            this.tabDBData.Name = "tabDBData";
            this.tabDBData.SelectedIndex = 0;
            this.tabDBData.Size = new System.Drawing.Size(278, 921);
            this.tabDBData.TabIndex = 336;
            // 
            // TabTables
            // 
            this.TabTables.Controls.Add(this.pnlSource);
            this.TabTables.Location = new System.Drawing.Point(4, 26);
            this.TabTables.Name = "TabTables";
            this.TabTables.Padding = new System.Windows.Forms.Padding(3);
            this.TabTables.Size = new System.Drawing.Size(270, 891);
            this.TabTables.TabIndex = 0;
            this.TabTables.Text = "TABLES";
            this.TabTables.UseVisualStyleBackColor = true;
            // 
            // pnlSource
            // 
            this.pnlSource.BackColor = System.Drawing.Color.WhiteSmoke;
            this.pnlSource.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSource.Controls.Add(this.lvwTableList);
            this.pnlSource.Location = new System.Drawing.Point(0, 0);
            this.pnlSource.Name = "pnlSource";
            this.pnlSource.Size = new System.Drawing.Size(268, 893);
            this.pnlSource.TabIndex = 326;
            // 
            // lvwTableList
            // 
            this.lvwTableList.BackColor = System.Drawing.Color.GhostWhite;
            this.lvwTableList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvwTableList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvwTableList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwTableList.Font = new System.Drawing.Font("JetBrains Mono", 8.249999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvwTableList.ForeColor = System.Drawing.Color.Black;
            this.lvwTableList.FullRowSelect = true;
            this.lvwTableList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvwTableList.HideSelection = false;
            this.lvwTableList.Location = new System.Drawing.Point(0, 0);
            this.lvwTableList.Name = "lvwTableList";
            this.lvwTableList.Size = new System.Drawing.Size(266, 891);
            this.lvwTableList.TabIndex = 104;
            this.lvwTableList.UseCompatibleStateImageBehavior = false;
            this.lvwTableList.View = System.Windows.Forms.View.Details;
            this.lvwTableList.DoubleClick += new System.EventHandler(this.lvwTableList_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "ID";
            this.columnHeader1.Width = 0;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "TABLE NAME";
            this.columnHeader2.Width = 272;
            // 
            // tabFunctions
            // 
            this.tabFunctions.Controls.Add(this.lvwFunctionList);
            this.tabFunctions.Location = new System.Drawing.Point(4, 26);
            this.tabFunctions.Name = "tabFunctions";
            this.tabFunctions.Padding = new System.Windows.Forms.Padding(3);
            this.tabFunctions.Size = new System.Drawing.Size(270, 891);
            this.tabFunctions.TabIndex = 1;
            this.tabFunctions.Text = "FUNCTION";
            this.tabFunctions.UseVisualStyleBackColor = true;
            // 
            // lvwFunctionList
            // 
            this.lvwFunctionList.BackColor = System.Drawing.Color.GhostWhite;
            this.lvwFunctionList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvwFunctionList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.lvwFunctionList.Font = new System.Drawing.Font("JetBrains Mono", 8.249999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvwFunctionList.ForeColor = System.Drawing.Color.Black;
            this.lvwFunctionList.FullRowSelect = true;
            this.lvwFunctionList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvwFunctionList.HideSelection = false;
            this.lvwFunctionList.Location = new System.Drawing.Point(0, 0);
            this.lvwFunctionList.Name = "lvwFunctionList";
            this.lvwFunctionList.Size = new System.Drawing.Size(268, 893);
            this.lvwFunctionList.TabIndex = 105;
            this.lvwFunctionList.UseCompatibleStateImageBehavior = false;
            this.lvwFunctionList.View = System.Windows.Forms.View.Details;
            this.lvwFunctionList.DoubleClick += new System.EventHandler(this.lvwFunctionList_DoubleClick);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "ID";
            this.columnHeader3.Width = 0;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "FUNCTION";
            this.columnHeader4.Width = 272;
            // 
            // tabStoredProcedures
            // 
            this.tabStoredProcedures.Controls.Add(this.lvwRoutineList);
            this.tabStoredProcedures.Location = new System.Drawing.Point(4, 26);
            this.tabStoredProcedures.Name = "tabStoredProcedures";
            this.tabStoredProcedures.Padding = new System.Windows.Forms.Padding(3);
            this.tabStoredProcedures.Size = new System.Drawing.Size(270, 891);
            this.tabStoredProcedures.TabIndex = 2;
            this.tabStoredProcedures.Text = "ROUTINES";
            this.tabStoredProcedures.UseVisualStyleBackColor = true;
            // 
            // lvwRoutineList
            // 
            this.lvwRoutineList.BackColor = System.Drawing.Color.GhostWhite;
            this.lvwRoutineList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvwRoutineList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6});
            this.lvwRoutineList.Font = new System.Drawing.Font("JetBrains Mono", 8.249999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvwRoutineList.ForeColor = System.Drawing.Color.Black;
            this.lvwRoutineList.FullRowSelect = true;
            this.lvwRoutineList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvwRoutineList.HideSelection = false;
            this.lvwRoutineList.Location = new System.Drawing.Point(0, 0);
            this.lvwRoutineList.Name = "lvwRoutineList";
            this.lvwRoutineList.Size = new System.Drawing.Size(268, 893);
            this.lvwRoutineList.TabIndex = 106;
            this.lvwRoutineList.UseCompatibleStateImageBehavior = false;
            this.lvwRoutineList.View = System.Windows.Forms.View.Details;
            this.lvwRoutineList.DoubleClick += new System.EventHandler(this.lvwRoutineList_DoubleClick);
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "ID";
            this.columnHeader5.Width = 0;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "ROUTINE";
            this.columnHeader6.Width = 272;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.rteQuery);
            this.panel1.Controls.Add(this.pnlSourceHeader);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1389, 543);
            this.panel1.TabIndex = 335;
            // 
            // rteQuery
            // 
            this.rteQuery.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.rteQuery.AutoIndentCharsPatterns = "";
            this.rteQuery.AutoScrollMinSize = new System.Drawing.Size(0, 17);
            this.rteQuery.BackBrush = null;
            this.rteQuery.BackColor = System.Drawing.Color.Gainsboro;
            this.rteQuery.CharHeight = 17;
            this.rteQuery.CharWidth = 9;
            this.rteQuery.CommentPrefix = "--";
            this.rteQuery.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.rteQuery.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.rteQuery.Font = new System.Drawing.Font("JetBrains Mono", 9.75F, System.Drawing.FontStyle.Bold);
            this.rteQuery.IsReplaceMode = false;
            this.rteQuery.Language = FastColoredTextBoxNS.Language.SQL;
            this.rteQuery.LeftBracket = '(';
            this.rteQuery.LineNumberColor = System.Drawing.Color.DarkGray;
            this.rteQuery.Location = new System.Drawing.Point(-1, 61);
            this.rteQuery.Name = "rteQuery";
            this.rteQuery.Paddings = new System.Windows.Forms.Padding(0);
            this.rteQuery.RightBracket = ')';
            this.rteQuery.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.rteQuery.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("rteQuery.ServiceColors")));
            this.rteQuery.Size = new System.Drawing.Size(1389, 480);
            this.rteQuery.TabIndex = 339;
            this.rteQuery.WordWrap = true;
            this.rteQuery.Zoom = 100;
            this.rteQuery.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.rteQuery_TextChanged);
            // 
            // pnlSourceHeader
            // 
            this.pnlSourceHeader.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlSourceHeader.Controls.Add(this.lblRuntime);
            this.pnlSourceHeader.Controls.Add(this.label5);
            this.pnlSourceHeader.Controls.Add(this.label2);
            this.pnlSourceHeader.Controls.Add(this.barLoading);
            this.pnlSourceHeader.Controls.Add(this.lblSelectedRow);
            this.pnlSourceHeader.Controls.Add(this.label1);
            this.pnlSourceHeader.Controls.Add(this.lblSourceHeader);
            this.pnlSourceHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlSourceHeader.Name = "pnlSourceHeader";
            this.pnlSourceHeader.Size = new System.Drawing.Size(1388, 25);
            this.pnlSourceHeader.TabIndex = 335;
            // 
            // lblRuntime
            // 
            this.lblRuntime.AutoSize = true;
            this.lblRuntime.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblRuntime.Location = new System.Drawing.Point(1148, 5);
            this.lblRuntime.Name = "lblRuntime";
            this.lblRuntime.Size = new System.Drawing.Size(84, 14);
            this.lblRuntime.TabIndex = 339;
            this.lblRuntime.Text = "[ RUNTIME ]";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(1128, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 14);
            this.label5.TabIndex = 338;
            this.label5.Text = "-";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(1024, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 14);
            this.label2.TabIndex = 337;
            this.label2.Text = "QUERY RUNTIME";
            // 
            // barLoading
            // 
            this.barLoading.Location = new System.Drawing.Point(1249, -1);
            this.barLoading.Name = "barLoading";
            this.barLoading.Size = new System.Drawing.Size(139, 26);
            this.barLoading.TabIndex = 336;
            // 
            // lblSelectedRow
            // 
            this.lblSelectedRow.AutoSize = true;
            this.lblSelectedRow.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblSelectedRow.Location = new System.Drawing.Point(100, 5);
            this.lblSelectedRow.Name = "lblSelectedRow";
            this.lblSelectedRow.Size = new System.Drawing.Size(119, 14);
            this.lblSelectedRow.TabIndex = 2;
            this.lblSelectedRow.Text = "[ SELECTED ROW ]";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(80, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 14);
            this.label1.TabIndex = 1;
            this.label1.Text = "-";
            // 
            // lblSourceHeader
            // 
            this.lblSourceHeader.AutoSize = true;
            this.lblSourceHeader.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblSourceHeader.Location = new System.Drawing.Point(4, 5);
            this.lblSourceHeader.Name = "lblSourceHeader";
            this.lblSourceHeader.Size = new System.Drawing.Size(70, 14);
            this.lblSourceHeader.TabIndex = 0;
            this.lblSourceHeader.Text = "SQL QUERY";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.btnClear);
            this.panel2.Controls.Add(this.lblPage);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.btnReset);
            this.panel2.Controls.Add(this.btnRun);
            this.panel2.Controls.Add(this.btnNext);
            this.panel2.Controls.Add(this.btnBack);
            this.panel2.Location = new System.Drawing.Point(-1, 25);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1389, 36);
            this.panel2.TabIndex = 338;
            // 
            // btnClear
            // 
            this.btnClear.AutoSize = true;
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnClear.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClear.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnClear.FlatAppearance.BorderSize = 0;
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnClear.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnClear.Location = new System.Drawing.Point(1249, 2);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(81, 26);
            this.btnClear.TabIndex = 336;
            this.btnClear.Text = "CLEAR";
            this.btnClear.UseVisualStyleBackColor = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblPage
            // 
            this.lblPage.AutoSize = true;
            this.lblPage.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblPage.Location = new System.Drawing.Point(220, 10);
            this.lblPage.Name = "lblPage";
            this.lblPage.Size = new System.Drawing.Size(63, 14);
            this.lblPage.TabIndex = 337;
            this.lblPage.Text = "[ PAGE ]";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(176, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 14);
            this.label3.TabIndex = 336;
            this.label3.Text = "PAGE: ";
            // 
            // btnReset
            // 
            this.btnReset.AutoSize = true;
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReset.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnReset.FlatAppearance.BorderSize = 0;
            this.btnReset.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnReset.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnReset.Location = new System.Drawing.Point(85, 4);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(81, 26);
            this.btnReset.TabIndex = 332;
            this.btnReset.Text = "RESET";
            this.btnReset.UseVisualStyleBackColor = false;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnRun
            // 
            this.btnRun.AutoSize = true;
            this.btnRun.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnRun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRun.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnRun.FlatAppearance.BorderSize = 0;
            this.btnRun.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnRun.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRun.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnRun.Location = new System.Drawing.Point(3, 4);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(81, 26);
            this.btnRun.TabIndex = 333;
            this.btnRun.Text = "RUN";
            this.btnRun.UseVisualStyleBackColor = false;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnNext
            // 
            this.btnNext.AutoSize = true;
            this.btnNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnNext.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNext.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnNext.FlatAppearance.BorderSize = 0;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnNext.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNext.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnNext.Location = new System.Drawing.Point(1358, 2);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(26, 26);
            this.btnNext.TabIndex = 335;
            this.btnNext.Text = ">";
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnBack
            // 
            this.btnBack.AutoSize = true;
            this.btnBack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBack.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnBack.FlatAppearance.BorderSize = 0;
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnBack.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnBack.Location = new System.Drawing.Point(1332, 2);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(26, 26);
            this.btnBack.TabIndex = 334;
            this.btnBack.Text = "<";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.tabReturns);
            this.panel3.Location = new System.Drawing.Point(0, 443);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1389, 477);
            this.panel3.TabIndex = 337;
            // 
            // tabReturns
            // 
            this.tabReturns.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabReturns.Controls.Add(this.tabDataReturn);
            this.tabReturns.Controls.Add(this.tabRawReturn);
            this.tabReturns.Location = new System.Drawing.Point(-1, 99);
            this.tabReturns.Name = "tabReturns";
            this.tabReturns.SelectedIndex = 0;
            this.tabReturns.Size = new System.Drawing.Size(1392, 377);
            this.tabReturns.TabIndex = 332;
            // 
            // tabDataReturn
            // 
            this.tabDataReturn.Controls.Add(this.dgvTableContents);
            this.tabDataReturn.Location = new System.Drawing.Point(4, 26);
            this.tabDataReturn.Name = "tabDataReturn";
            this.tabDataReturn.Padding = new System.Windows.Forms.Padding(3);
            this.tabDataReturn.Size = new System.Drawing.Size(1384, 347);
            this.tabDataReturn.TabIndex = 0;
            this.tabDataReturn.Text = "DATA";
            this.tabDataReturn.UseVisualStyleBackColor = true;
            // 
            // dgvTableContents
            // 
            this.dgvTableContents.AllowUserToAddRows = false;
            this.dgvTableContents.AllowUserToDeleteRows = false;
            this.dgvTableContents.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.Disable;
            this.dgvTableContents.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgvTableContents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTableContents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTableContents.Location = new System.Drawing.Point(3, 3);
            this.dgvTableContents.MultiSelect = false;
            this.dgvTableContents.Name = "dgvTableContents";
            this.dgvTableContents.ReadOnly = true;
            this.dgvTableContents.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvTableContents.ShowCellErrors = false;
            this.dgvTableContents.ShowCellToolTips = false;
            this.dgvTableContents.ShowEditingIcon = false;
            this.dgvTableContents.ShowRowErrors = false;
            this.dgvTableContents.Size = new System.Drawing.Size(1378, 341);
            this.dgvTableContents.TabIndex = 331;
            this.dgvTableContents.SelectionChanged += new System.EventHandler(this.dgvTableContents_SelectionChanged);
            // 
            // tabRawReturn
            // 
            this.tabRawReturn.Controls.Add(this.rteRawReturn);
            this.tabRawReturn.Location = new System.Drawing.Point(4, 26);
            this.tabRawReturn.Name = "tabRawReturn";
            this.tabRawReturn.Padding = new System.Windows.Forms.Padding(3);
            this.tabRawReturn.Size = new System.Drawing.Size(1384, 347);
            this.tabRawReturn.TabIndex = 1;
            this.tabRawReturn.Text = "RAW";
            this.tabRawReturn.UseVisualStyleBackColor = true;
            // 
            // rteRawReturn
            // 
            this.rteRawReturn.AutoWordSelection = true;
            this.rteRawReturn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rteRawReturn.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rteRawReturn.Enabled = false;
            this.rteRawReturn.Font = new System.Drawing.Font("JetBrains Mono NL Light", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rteRawReturn.Location = new System.Drawing.Point(3, 3);
            this.rteRawReturn.Margin = new System.Windows.Forms.Padding(16);
            this.rteRawReturn.Name = "rteRawReturn";
            this.rteRawReturn.Size = new System.Drawing.Size(1378, 341);
            this.rteRawReturn.TabIndex = 335;
            this.rteRawReturn.Text = "";
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1668, 920);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "SCRIPTS";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // bunifuElipse1
            // 
            this.bunifuElipse1.ElipseRadius = 5;
            this.bunifuElipse1.TargetControl = this;
            // 
            // bunifuDragControl1
            // 
            this.bunifuDragControl1.Fixed = true;
            this.bunifuDragControl1.Horizontal = true;
            this.bunifuDragControl1.TargetControl = this.pnlHeader;
            this.bunifuDragControl1.Vertical = true;
            // 
            // frmTableEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1680, 990);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "frmTableEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmTableEditor";
            this.Load += new System.EventHandler(this.frmTableEditor_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmTableEditor_KeyDown);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.tabDBData.ResumeLayout(false);
            this.TabTables.ResumeLayout(false);
            this.pnlSource.ResumeLayout(false);
            this.tabFunctions.ResumeLayout(false);
            this.tabStoredProcedures.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.rteQuery)).EndInit();
            this.pnlSourceHeader.ResumeLayout(false);
            this.pnlSourceHeader.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.tabReturns.ResumeLayout(false);
            this.tabDataReturn.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTableContents)).EndInit();
            this.tabRawReturn.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlHeader;
        private Bunifu.Framework.UI.BunifuImageButton bunifuImageButton1;
        private Bunifu.Framework.UI.BunifuImageButton btnExit;
        private Bunifu.Framework.UI.BunifuCustomLabel lblHeader;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView dgvTableContents;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnlSourceHeader;
        private System.Windows.Forms.Label lblSourceHeader;
        private System.Windows.Forms.Label lblSelectedRow;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblPage;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ProgressBar barLoading;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TabControl tabDBData;
        private System.Windows.Forms.TabPage TabTables;
        private System.Windows.Forms.Panel pnlSource;
        private System.Windows.Forms.ListView lvwTableList;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.TabPage tabFunctions;
        private System.Windows.Forms.TabPage tabStoredProcedures;
        private System.Windows.Forms.ListView lvwFunctionList;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ListView lvwRoutineList;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.TabControl tabReturns;
        private System.Windows.Forms.TabPage tabDataReturn;
        private System.Windows.Forms.TabPage tabRawReturn;
        private System.Windows.Forms.RichTextBox rteRawReturn;
        private FastColoredTextBoxNS.FastColoredTextBox rteQuery;
        private Bunifu.Framework.UI.BunifuElipse bunifuElipse1;
        private System.Windows.Forms.Label lblRuntime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private Bunifu.Framework.UI.BunifuImageButton btnMinimize;
        private Bunifu.Framework.UI.BunifuDragControl bunifuDragControl1;
    }
}