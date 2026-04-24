namespace MIS
{
    partial class frmReportViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmReportViewer));
            this.myViewer = new CrystalDecisions.Windows.Forms.CrystalReportViewer();
            this.lvwList = new System.Windows.Forms.ListView();
            this.lblRecordCount = new System.Windows.Forms.Label();
            this.btnExport = new Bunifu.Framework.UI.BunifuFlatButton();
            this.btnListView = new Bunifu.Framework.UI.BunifuFlatButton();
            this.btnReportView = new Bunifu.Framework.UI.BunifuFlatButton();
            this.label78 = new System.Windows.Forms.Label();
            this.btnSearch = new Bunifu.Framework.UI.BunifuImageButton();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.ucStatusDisplay = new MIS.ControlObject.ucDisplayStatus();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).BeginInit();
            this.pnlFooter.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // myViewer
            // 
            this.myViewer.ActiveViewIndex = -1;
            this.myViewer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.myViewer.Cursor = System.Windows.Forms.Cursors.Default;
            this.myViewer.Location = new System.Drawing.Point(8, 14);
            this.myViewer.Name = "myViewer";
            this.myViewer.Size = new System.Drawing.Size(277, 138);
            this.myViewer.TabIndex = 0;
            // 
            // lvwList
            // 
            this.lvwList.BackColor = System.Drawing.Color.GhostWhite;
            this.lvwList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvwList.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvwList.ForeColor = System.Drawing.Color.Black;
            this.lvwList.FullRowSelect = true;
            this.lvwList.HideSelection = false;
            this.lvwList.Location = new System.Drawing.Point(336, 14);
            this.lvwList.Name = "lvwList";
            this.lvwList.Size = new System.Drawing.Size(262, 138);
            this.lvwList.TabIndex = 312;
            this.lvwList.UseCompatibleStateImageBehavior = false;
            this.lvwList.View = System.Windows.Forms.View.Details;
            this.lvwList.DoubleClick += new System.EventHandler(this.lvwList_DoubleClick);
            // 
            // lblRecordCount
            // 
            this.lblRecordCount.Font = new System.Drawing.Font("Century Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecordCount.ForeColor = System.Drawing.Color.Black;
            this.lblRecordCount.Location = new System.Drawing.Point(507, 1);
            this.lblRecordCount.Name = "lblRecordCount";
            this.lblRecordCount.Size = new System.Drawing.Size(244, 33);
            this.lblRecordCount.TabIndex = 312;
            this.lblRecordCount.Text = "-";
            this.lblRecordCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnExport
            // 
            this.btnExport.Activecolor = System.Drawing.Color.Blue;
            this.btnExport.BackColor = System.Drawing.Color.Blue;
            this.btnExport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnExport.BorderRadius = 0;
            this.btnExport.ButtonText = "EXPORT TO EXCEL";
            this.btnExport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnExport.DisabledColor = System.Drawing.Color.Gray;
            this.btnExport.Iconcolor = System.Drawing.Color.Transparent;
            this.btnExport.Iconimage = ((System.Drawing.Image)(resources.GetObject("btnExport.Iconimage")));
            this.btnExport.Iconimage_right = null;
            this.btnExport.Iconimage_right_Selected = null;
            this.btnExport.Iconimage_Selected = null;
            this.btnExport.IconMarginLeft = 0;
            this.btnExport.IconMarginRight = 0;
            this.btnExport.IconRightVisible = true;
            this.btnExport.IconRightZoom = 0D;
            this.btnExport.IconVisible = true;
            this.btnExport.IconZoom = 90D;
            this.btnExport.IsTab = false;
            this.btnExport.Location = new System.Drawing.Point(396, 3);
            this.btnExport.Name = "btnExport";
            this.btnExport.Normalcolor = System.Drawing.Color.Blue;
            this.btnExport.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(129)))), ((int)(((byte)(77)))));
            this.btnExport.OnHoverTextColor = System.Drawing.Color.White;
            this.btnExport.selected = false;
            this.btnExport.Size = new System.Drawing.Size(169, 34);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "EXPORT TO EXCEL";
            this.btnExport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnExport.Textcolor = System.Drawing.Color.White;
            this.btnExport.TextFont = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnListView
            // 
            this.btnListView.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.btnListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.btnListView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnListView.BorderRadius = 0;
            this.btnListView.ButtonText = "LIST VIEW";
            this.btnListView.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnListView.DisabledColor = System.Drawing.Color.Gray;
            this.btnListView.Iconcolor = System.Drawing.Color.Transparent;
            this.btnListView.Iconimage = ((System.Drawing.Image)(resources.GetObject("btnListView.Iconimage")));
            this.btnListView.Iconimage_right = null;
            this.btnListView.Iconimage_right_Selected = null;
            this.btnListView.Iconimage_Selected = null;
            this.btnListView.IconMarginLeft = 0;
            this.btnListView.IconMarginRight = 0;
            this.btnListView.IconRightVisible = true;
            this.btnListView.IconRightZoom = 0D;
            this.btnListView.IconVisible = true;
            this.btnListView.IconZoom = 90D;
            this.btnListView.IsTab = false;
            this.btnListView.Location = new System.Drawing.Point(226, 3);
            this.btnListView.Name = "btnListView";
            this.btnListView.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.btnListView.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(129)))), ((int)(((byte)(77)))));
            this.btnListView.OnHoverTextColor = System.Drawing.Color.White;
            this.btnListView.selected = false;
            this.btnListView.Size = new System.Drawing.Size(169, 34);
            this.btnListView.TabIndex = 2;
            this.btnListView.Text = "LIST VIEW";
            this.btnListView.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnListView.Textcolor = System.Drawing.Color.White;
            this.btnListView.TextFont = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnListView.Click += new System.EventHandler(this.btnListView_Click);
            // 
            // btnReportView
            // 
            this.btnReportView.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.btnReportView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.btnReportView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnReportView.BorderRadius = 0;
            this.btnReportView.ButtonText = "REPORT VIEW";
            this.btnReportView.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnReportView.DisabledColor = System.Drawing.Color.Gray;
            this.btnReportView.Iconcolor = System.Drawing.Color.Transparent;
            this.btnReportView.Iconimage = ((System.Drawing.Image)(resources.GetObject("btnReportView.Iconimage")));
            this.btnReportView.Iconimage_right = null;
            this.btnReportView.Iconimage_right_Selected = null;
            this.btnReportView.Iconimage_Selected = null;
            this.btnReportView.IconMarginLeft = 0;
            this.btnReportView.IconMarginRight = 0;
            this.btnReportView.IconRightVisible = true;
            this.btnReportView.IconRightZoom = 0D;
            this.btnReportView.IconVisible = true;
            this.btnReportView.IconZoom = 90D;
            this.btnReportView.IsTab = false;
            this.btnReportView.Location = new System.Drawing.Point(56, 3);
            this.btnReportView.Name = "btnReportView";
            this.btnReportView.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.btnReportView.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(129)))), ((int)(((byte)(77)))));
            this.btnReportView.OnHoverTextColor = System.Drawing.Color.White;
            this.btnReportView.selected = false;
            this.btnReportView.Size = new System.Drawing.Size(169, 34);
            this.btnReportView.TabIndex = 1;
            this.btnReportView.Text = "REPORT VIEW";
            this.btnReportView.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReportView.Textcolor = System.Drawing.Color.White;
            this.btnReportView.TextFont = new System.Drawing.Font("Century Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReportView.Click += new System.EventHandler(this.btnReportView_Click);
            // 
            // label78
            // 
            this.label78.AutoSize = true;
            this.label78.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label78.ForeColor = System.Drawing.Color.Black;
            this.label78.Location = new System.Drawing.Point(7, 8);
            this.label78.Name = "label78";
            this.label78.Size = new System.Drawing.Size(69, 19);
            this.label78.TabIndex = 311;
            this.label78.Text = "SEARCH";
            // 
            // btnSearch
            // 
            this.btnSearch.BackColor = System.Drawing.Color.Transparent;
            this.btnSearch.Enabled = false;
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.ImageActive = null;
            this.btnSearch.Location = new System.Drawing.Point(463, -2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(38, 39);
            this.btnSearch.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnSearch.TabIndex = 169;
            this.btnSearch.TabStop = false;
            this.btnSearch.Zoom = 10;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.BackColor = System.Drawing.Color.White;
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtSearch.Font = new System.Drawing.Font("Courier New", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.txtSearch.Location = new System.Drawing.Point(86, 1);
            this.txtSearch.Margin = new System.Windows.Forms.Padding(3, 10, 3, 3);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(376, 35);
            this.txtSearch.TabIndex = 160;
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlFooter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlFooter.Controls.Add(this.ucStatusDisplay);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlFooter.Location = new System.Drawing.Point(3, 842);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(1370, 29);
            this.pnlFooter.TabIndex = 313;
            // 
            // ucStatusDisplay
            // 
            this.ucStatusDisplay.Location = new System.Drawing.Point(3, 0);
            this.ucStatusDisplay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ucStatusDisplay.Name = "ucStatusDisplay";
            this.ucStatusDisplay.Size = new System.Drawing.Size(643, 23);
            this.ucStatusDisplay.TabIndex = 347;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.pnlHeader, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pnlFooter, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 5.269461F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 94.73054F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1376, 874);
            this.tableLayoutPanel1.TabIndex = 451;
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.Gainsboro;
            this.pnlHeader.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlHeader.Controls.Add(this.panel2);
            this.pnlHeader.Controls.Add(this.lblRecordCount);
            this.pnlHeader.Controls.Add(this.label78);
            this.pnlHeader.Controls.Add(this.btnSearch);
            this.pnlHeader.Controls.Add(this.txtSearch);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlHeader.Location = new System.Drawing.Point(3, 3);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1370, 38);
            this.pnlHeader.TabIndex = 452;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lvwList);
            this.panel1.Controls.Add(this.myViewer);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 47);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1370, 789);
            this.panel1.TabIndex = 453;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnExport);
            this.panel2.Controls.Add(this.btnReportView);
            this.panel2.Controls.Add(this.btnListView);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(801, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(567, 36);
            this.panel2.TabIndex = 313;
            // 
            // frmReportViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1376, 874);
            this.Controls.Add(this.tableLayoutPanel1);
            this.KeyPreview = true;
            this.Name = "frmReportViewer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Report Preview";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Activated += new System.EventHandler(this.frmReportViewer_Activated);
            this.Load += new System.EventHandler(this.frmReportViewer_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmReportViewer_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.btnSearch)).EndInit();
            this.pnlFooter.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private CrystalDecisions.Windows.Forms.CrystalReportViewer myViewer;
        private Bunifu.Framework.UI.BunifuFlatButton btnListView;
        private Bunifu.Framework.UI.BunifuFlatButton btnReportView;
        private System.Windows.Forms.ListView lvwList;
        private Bunifu.Framework.UI.BunifuFlatButton btnExport;
        private System.Windows.Forms.TextBox txtSearch;
        private Bunifu.Framework.UI.BunifuImageButton btnSearch;
        private System.Windows.Forms.Label label78;
        private System.Windows.Forms.Label lblRecordCount;
        private System.Windows.Forms.Panel pnlFooter;
        private ControlObject.ucDisplayStatus ucStatusDisplay;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
    }
}