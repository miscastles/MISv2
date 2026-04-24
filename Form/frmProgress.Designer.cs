namespace MIS
{
    partial class frmProgress
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmProgress));
            this.picGIF = new System.Windows.Forms.PictureBox();
            this.txtHeader2 = new System.Windows.Forms.RichTextBox();
            this.txtHeader1 = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.picGIF)).BeginInit();
            this.SuspendLayout();
            // 
            // picGIF
            // 
            this.picGIF.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picGIF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picGIF.ErrorImage = ((System.Drawing.Image)(resources.GetObject("picGIF.ErrorImage")));
            this.picGIF.Image = ((System.Drawing.Image)(resources.GetObject("picGIF.Image")));
            this.picGIF.Location = new System.Drawing.Point(3, 3);
            this.picGIF.Name = "picGIF";
            this.picGIF.Size = new System.Drawing.Size(173, 173);
            this.picGIF.TabIndex = 0;
            this.picGIF.TabStop = false;
            // 
            // txtHeader2
            // 
            this.txtHeader2.BackColor = System.Drawing.SystemColors.Control;
            this.txtHeader2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtHeader2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHeader2.ForeColor = System.Drawing.Color.Black;
            this.txtHeader2.Location = new System.Drawing.Point(12, 32);
            this.txtHeader2.Name = "txtHeader2";
            this.txtHeader2.Size = new System.Drawing.Size(158, 23);
            this.txtHeader2.TabIndex = 6;
            this.txtHeader2.Text = "Please wait...";
            // 
            // txtHeader1
            // 
            this.txtHeader1.BackColor = System.Drawing.SystemColors.Control;
            this.txtHeader1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtHeader1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHeader1.ForeColor = System.Drawing.Color.Black;
            this.txtHeader1.Location = new System.Drawing.Point(12, 12);
            this.txtHeader1.Name = "txtHeader1";
            this.txtHeader1.Size = new System.Drawing.Size(159, 23);
            this.txtHeader1.TabIndex = 5;
            this.txtHeader1.Text = "Loading...";
            // 
            // frmProgress
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(178, 180);
            this.Controls.Add(this.txtHeader2);
            this.Controls.Add(this.txtHeader1);
            this.Controls.Add(this.picGIF);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Name = "frmProgress";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Activated += new System.EventHandler(this.frmProgress_Activated);
            ((System.ComponentModel.ISupportInitialize)(this.picGIF)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picGIF;
        private System.Windows.Forms.RichTextBox txtHeader2;
        private System.Windows.Forms.RichTextBox txtHeader1;
    }
}