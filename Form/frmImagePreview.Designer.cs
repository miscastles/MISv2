namespace MIS
{
    partial class frmImagePreview
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmImagePreview));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.bunifuElipse1 = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.bunifuDragControl1 = new Bunifu.Framework.UI.BunifuDragControl(this.components);
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.bunifuImageButton1 = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnMinimize = new Bunifu.Framework.UI.BunifuImageButton();
            this.btnExit = new Bunifu.Framework.UI.BunifuImageButton();
            this.lblHeader = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.pBoxPreview = new System.Windows.Forms.PictureBox();
            this.panel164 = new System.Windows.Forms.Panel();
            this.dgvImageData = new System.Windows.Forms.DataGridView();
            this.panel162 = new System.Windows.Forms.Panel();
            this.label36 = new System.Windows.Forms.Label();
            this.panel163 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDownloadImage = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBoxPreview)).BeginInit();
            this.panel164.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvImageData)).BeginInit();
            this.panel162.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
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
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.Peru;
            this.pnlHeader.Controls.Add(this.bunifuImageButton1);
            this.pnlHeader.Controls.Add(this.btnMinimize);
            this.pnlHeader.Controls.Add(this.btnExit);
            this.pnlHeader.Controls.Add(this.lblHeader);
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1271, 29);
            this.pnlHeader.TabIndex = 300;
            // 
            // bunifuImageButton1
            // 
            this.bunifuImageButton1.BackColor = System.Drawing.Color.Transparent;
            this.bunifuImageButton1.Image = ((System.Drawing.Image)(resources.GetObject("bunifuImageButton1.Image")));
            this.bunifuImageButton1.ImageActive = null;
            this.bunifuImageButton1.Location = new System.Drawing.Point(3, 1);
            this.bunifuImageButton1.Name = "bunifuImageButton1";
            this.bunifuImageButton1.Size = new System.Drawing.Size(26, 25);
            this.bunifuImageButton1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.bunifuImageButton1.TabIndex = 342;
            this.bunifuImageButton1.TabStop = false;
            this.bunifuImageButton1.Zoom = 10;
            // 
            // btnMinimize
            // 
            this.btnMinimize.BackColor = System.Drawing.Color.Transparent;
            this.btnMinimize.Image = ((System.Drawing.Image)(resources.GetObject("btnMinimize.Image")));
            this.btnMinimize.ImageActive = null;
            this.btnMinimize.Location = new System.Drawing.Point(1097, 5);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(24, 21);
            this.btnMinimize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnMinimize.TabIndex = 340;
            this.btnMinimize.TabStop = false;
            this.btnMinimize.Zoom = 10;
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.Peru;
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageActive = null;
            this.btnExit.Location = new System.Drawing.Point(1124, 5);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(24, 21);
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
            this.lblHeader.Location = new System.Drawing.Point(35, 6);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(126, 18);
            this.lblHeader.TabIndex = 7;
            this.lblHeader.Text = "IMAGE-PREVIEW";
            // 
            // pBoxPreview
            // 
            this.pBoxPreview.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pBoxPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pBoxPreview.Location = new System.Drawing.Point(6, 35);
            this.pBoxPreview.Name = "pBoxPreview";
            this.pBoxPreview.Size = new System.Drawing.Size(824, 550);
            this.pBoxPreview.TabIndex = 479;
            this.pBoxPreview.TabStop = false;
            // 
            // panel164
            // 
            this.panel164.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel164.Controls.Add(this.dgvImageData);
            this.panel164.Location = new System.Drawing.Point(833, 56);
            this.panel164.Name = "panel164";
            this.panel164.Size = new System.Drawing.Size(315, 481);
            this.panel164.TabIndex = 482;
            // 
            // dgvImageData
            // 
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.dgvImageData.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvImageData.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvImageData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Courier New", 8.25F);
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvImageData.DefaultCellStyle = dataGridViewCellStyle7;
            this.dgvImageData.Location = new System.Drawing.Point(0, 0);
            this.dgvImageData.Name = "dgvImageData";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Courier New", 8.25F);
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvImageData.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dgvImageData.Size = new System.Drawing.Size(313, 480);
            this.dgvImageData.TabIndex = 482;
            // 
            // panel162
            // 
            this.panel162.BackColor = System.Drawing.Color.Gainsboro;
            this.panel162.Controls.Add(this.label36);
            this.panel162.Controls.Add(this.panel163);
            this.panel162.Location = new System.Drawing.Point(833, 35);
            this.panel162.Name = "panel162";
            this.panel162.Size = new System.Drawing.Size(315, 23);
            this.panel162.TabIndex = 477;
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label36.ForeColor = System.Drawing.Color.Black;
            this.label36.Location = new System.Drawing.Point(3, 4);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(98, 14);
            this.label36.TabIndex = 310;
            this.label36.Text = "IMAGE DETAILS";
            // 
            // panel163
            // 
            this.panel163.BackColor = System.Drawing.Color.Silver;
            this.panel163.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel163.Location = new System.Drawing.Point(0, 21);
            this.panel163.Name = "panel163";
            this.panel163.Size = new System.Drawing.Size(315, 2);
            this.panel163.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnDownloadImage);
            this.panel1.Location = new System.Drawing.Point(833, 543);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(315, 42);
            this.panel1.TabIndex = 483;
            // 
            // btnDownloadImage
            // 
            this.btnDownloadImage.AutoSize = true;
            this.btnDownloadImage.BackColor = System.Drawing.Color.DarkSlateGray;
            this.btnDownloadImage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDownloadImage.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnDownloadImage.FlatAppearance.BorderSize = 0;
            this.btnDownloadImage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDownloadImage.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDownloadImage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnDownloadImage.Location = new System.Drawing.Point(0, -1);
            this.btnDownloadImage.Name = "btnDownloadImage";
            this.btnDownloadImage.Size = new System.Drawing.Size(314, 41);
            this.btnDownloadImage.TabIndex = 482;
            this.btnDownloadImage.Text = "DOWNLOAD MIAGE";
            this.btnDownloadImage.UseVisualStyleBackColor = false;
            this.btnDownloadImage.Click += new System.EventHandler(this.btnDownloadImage_Click);
            // 
            // frmImagePreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1152, 592);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel164);
            this.Controls.Add(this.pBoxPreview);
            this.Controls.Add(this.panel162);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmImagePreview";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmImagePreview";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmImagePreview_KeyDown);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bunifuImageButton1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnMinimize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pBoxPreview)).EndInit();
            this.panel164.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvImageData)).EndInit();
            this.panel162.ResumeLayout(false);
            this.panel162.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Bunifu.Framework.UI.BunifuElipse bunifuElipse1;
        private Bunifu.Framework.UI.BunifuDragControl bunifuDragControl1;
        private System.Windows.Forms.Panel pnlHeader;
        private Bunifu.Framework.UI.BunifuImageButton btnMinimize;
        private Bunifu.Framework.UI.BunifuImageButton btnExit;
        private Bunifu.Framework.UI.BunifuCustomLabel lblHeader;
        private System.Windows.Forms.PictureBox pBoxPreview;
        private System.Windows.Forms.Panel panel164;
        private System.Windows.Forms.Panel panel162;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.Panel panel163;
        private System.Windows.Forms.DataGridView dgvImageData;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnDownloadImage;
        private Bunifu.Framework.UI.BunifuImageButton bunifuImageButton1;
    }
}