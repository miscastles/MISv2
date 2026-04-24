namespace MIS
{
    partial class frmTools
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmTools));
            this.bunifuElipse1 = new Bunifu.Framework.UI.BunifuElipse(this.components);
            this.pnlDrag = new System.Windows.Forms.Panel();
            this.txtTerminalModelID = new System.Windows.Forms.TextBox();
            this.txtTerminalTypeID = new System.Windows.Forms.TextBox();
            this.lblHeader = new Bunifu.Framework.UI.BunifuCustomLabel();
            this.btnExit = new Bunifu.Framework.UI.BunifuImageButton();
            this.bunifuDragControl2 = new Bunifu.Framework.UI.BunifuDragControl(this.components);
            this.txtInput = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtParse = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnParse = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.pnlDrag.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).BeginInit();
            this.SuspendLayout();
            // 
            // bunifuElipse1
            // 
            this.bunifuElipse1.ElipseRadius = 5;
            this.bunifuElipse1.TargetControl = this;
            // 
            // pnlDrag
            // 
            this.pnlDrag.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.pnlDrag.Controls.Add(this.txtTerminalModelID);
            this.pnlDrag.Controls.Add(this.txtTerminalTypeID);
            this.pnlDrag.Controls.Add(this.lblHeader);
            this.pnlDrag.Controls.Add(this.btnExit);
            this.pnlDrag.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlDrag.Location = new System.Drawing.Point(0, 0);
            this.pnlDrag.Name = "pnlDrag";
            this.pnlDrag.Size = new System.Drawing.Size(870, 35);
            this.pnlDrag.TabIndex = 264;
            // 
            // txtTerminalModelID
            // 
            this.txtTerminalModelID.BackColor = System.Drawing.Color.White;
            this.txtTerminalModelID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTerminalModelID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtTerminalModelID.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtTerminalModelID.Location = new System.Drawing.Point(262, 6);
            this.txtTerminalModelID.MaxLength = 45;
            this.txtTerminalModelID.Name = "txtTerminalModelID";
            this.txtTerminalModelID.ReadOnly = true;
            this.txtTerminalModelID.Size = new System.Drawing.Size(31, 20);
            this.txtTerminalModelID.TabIndex = 338;
            this.txtTerminalModelID.Visible = false;
            // 
            // txtTerminalTypeID
            // 
            this.txtTerminalTypeID.BackColor = System.Drawing.Color.White;
            this.txtTerminalTypeID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTerminalTypeID.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtTerminalTypeID.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtTerminalTypeID.Location = new System.Drawing.Point(225, 6);
            this.txtTerminalTypeID.MaxLength = 45;
            this.txtTerminalTypeID.Name = "txtTerminalTypeID";
            this.txtTerminalTypeID.ReadOnly = true;
            this.txtTerminalTypeID.Size = new System.Drawing.Size(31, 20);
            this.txtTerminalTypeID.TabIndex = 337;
            this.txtTerminalTypeID.Visible = false;
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold);
            this.lblHeader.ForeColor = System.Drawing.Color.White;
            this.lblHeader.Location = new System.Drawing.Point(3, 6);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(164, 18);
            this.lblHeader.TabIndex = 8;
            this.lblHeader.Text = "DEVELOPMENT TOOLS";
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(46)))), ((int)(((byte)(59)))));
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageActive = null;
            this.btnExit.Location = new System.Drawing.Point(845, 6);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(21, 23);
            this.btnExit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.btnExit.TabIndex = 0;
            this.btnExit.TabStop = false;
            this.btnExit.Zoom = 10;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // bunifuDragControl2
            // 
            this.bunifuDragControl2.Fixed = true;
            this.bunifuDragControl2.Horizontal = true;
            this.bunifuDragControl2.TargetControl = this.pnlDrag;
            this.bunifuDragControl2.Vertical = true;
            // 
            // txtInput
            // 
            this.txtInput.BackColor = System.Drawing.Color.White;
            this.txtInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInput.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtInput.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtInput.Location = new System.Drawing.Point(5, 64);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtInput.Size = new System.Drawing.Size(381, 137);
            this.txtInput.TabIndex = 346;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(3, 47);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 14);
            this.label7.TabIndex = 347;
            this.label7.Text = "VALUE";
            // 
            // txtParse
            // 
            this.txtParse.BackColor = System.Drawing.Color.White;
            this.txtParse.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtParse.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtParse.Font = new System.Drawing.Font("Courier New", 8.25F);
            this.txtParse.Location = new System.Drawing.Point(6, 221);
            this.txtParse.Multiline = true;
            this.txtParse.Name = "txtParse";
            this.txtParse.ReadOnly = true;
            this.txtParse.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtParse.Size = new System.Drawing.Size(381, 374);
            this.txtParse.TabIndex = 348;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 204);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 14);
            this.label1.TabIndex = 349;
            this.label1.Text = "PARSE";
            // 
            // btnParse
            // 
            this.btnParse.AutoSize = true;
            this.btnParse.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnParse.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btnParse.FlatAppearance.BorderSize = 0;
            this.btnParse.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnParse.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnParse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.btnParse.Location = new System.Drawing.Point(5, 597);
            this.btnParse.Name = "btnParse";
            this.btnParse.Size = new System.Drawing.Size(184, 26);
            this.btnParse.TabIndex = 400;
            this.btnParse.Text = "PARSE";
            this.btnParse.UseVisualStyleBackColor = false;
            this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
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
            this.btnClear.Location = new System.Drawing.Point(195, 597);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(170, 26);
            this.btnClear.TabIndex = 401;
            this.btnClear.Text = "CLEAR";
            this.btnClear.UseVisualStyleBackColor = false;
            // 
            // frmTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(870, 629);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnParse);
            this.Controls.Add(this.txtParse);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.pnlDrag);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "frmTools";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frmTools_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmTools_KeyDown);
            this.pnlDrag.ResumeLayout(false);
            this.pnlDrag.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.btnExit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Bunifu.Framework.UI.BunifuElipse bunifuElipse1;
        private System.Windows.Forms.Panel pnlDrag;
        private System.Windows.Forms.TextBox txtTerminalModelID;
        private System.Windows.Forms.TextBox txtTerminalTypeID;
        private Bunifu.Framework.UI.BunifuCustomLabel lblHeader;
        private Bunifu.Framework.UI.BunifuImageButton btnExit;
        private Bunifu.Framework.UI.BunifuDragControl bunifuDragControl2;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtParse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnParse;
    }
}