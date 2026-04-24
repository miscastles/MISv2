namespace MIS
{
    partial class ucStatus
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rchStatus = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rchStatus
            // 
            this.rchStatus.BackColor = System.Drawing.Color.WhiteSmoke;
            this.rchStatus.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rchStatus.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rchStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.rchStatus.Location = new System.Drawing.Point(4, 4);
            this.rchStatus.Name = "rchStatus";
            this.rchStatus.ReadOnly = true;
            this.rchStatus.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.rchStatus.Size = new System.Drawing.Size(597, 26);
            this.rchStatus.TabIndex = 324;
            this.rchStatus.Text = "";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Controls.Add(this.rchStatus);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(601, 19);
            this.panel1.TabIndex = 341;
            // 
            // ucStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.Controls.Add(this.panel1);
            this.Name = "ucStatus";
            this.Size = new System.Drawing.Size(607, 28);
            this.Load += new System.EventHandler(this.ucStatus_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rchStatus;
        private System.Windows.Forms.Panel panel1;
    }
}
