namespace MIS
{
    partial class frmProcess
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
            this.ucLoadingX = new MIS.ucLoading();
            this.SuspendLayout();
            // 
            // ucLoadingX
            // 
            this.ucLoadingX.BackColor = System.Drawing.Color.Maroon;
            this.ucLoadingX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ucLoadingX.Location = new System.Drawing.Point(205, 189);
            this.ucLoadingX.Name = "ucLoadingX";
            this.ucLoadingX.Size = new System.Drawing.Size(391, 73);
            this.ucLoadingX.TabIndex = 390;
            // 
            // frmProcess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.ucLoadingX);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmProcess";
            this.Text = "frmProcess";
            this.ResumeLayout(false);

        }

        #endregion

        private ucLoading ucLoadingX;
    }
}