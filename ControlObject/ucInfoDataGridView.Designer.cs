namespace MIS.ControlObject
{
    partial class ucInfoDataGridView
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
            this.panel44 = new System.Windows.Forms.Panel();
            this.panel45 = new System.Windows.Forms.Panel();
            this.grdData = new System.Windows.Forms.DataGridView();
            this.panel46 = new System.Windows.Forms.Panel();
            this.label52 = new System.Windows.Forms.Label();
            this.panel47 = new System.Windows.Forms.Panel();
            this.panel44.SuspendLayout();
            this.panel45.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).BeginInit();
            this.panel46.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel44
            // 
            this.panel44.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel44.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel44.Controls.Add(this.panel45);
            this.panel44.Controls.Add(this.panel46);
            this.panel44.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel44.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel44.Location = new System.Drawing.Point(0, 0);
            this.panel44.Name = "panel44";
            this.panel44.Size = new System.Drawing.Size(445, 327);
            this.panel44.TabIndex = 485;
            // 
            // panel45
            // 
            this.panel45.Controls.Add(this.grdData);
            this.panel45.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel45.Location = new System.Drawing.Point(0, 24);
            this.panel45.Name = "panel45";
            this.panel45.Size = new System.Drawing.Size(443, 301);
            this.panel45.TabIndex = 312;
            // 
            // grdData
            // 
            this.grdData.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grdData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdData.Location = new System.Drawing.Point(0, 0);
            this.grdData.Name = "grdData";
            this.grdData.ReadOnly = true;
            this.grdData.Size = new System.Drawing.Size(443, 301);
            this.grdData.TabIndex = 310;
            // 
            // panel46
            // 
            this.panel46.BackColor = System.Drawing.Color.Gainsboro;
            this.panel46.Controls.Add(this.label52);
            this.panel46.Controls.Add(this.panel47);
            this.panel46.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel46.Location = new System.Drawing.Point(0, 0);
            this.panel46.Name = "panel46";
            this.panel46.Size = new System.Drawing.Size(443, 24);
            this.panel46.TabIndex = 311;
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Font = new System.Drawing.Font("Century Gothic", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label52.ForeColor = System.Drawing.Color.Black;
            this.label52.Location = new System.Drawing.Point(3, 4);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(136, 15);
            this.label52.TabIndex = 310;
            this.label52.Text = "SELECTED INFORMATION";
            // 
            // panel47
            // 
            this.panel47.BackColor = System.Drawing.Color.Silver;
            this.panel47.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel47.Location = new System.Drawing.Point(0, 23);
            this.panel47.Name = "panel47";
            this.panel47.Size = new System.Drawing.Size(443, 1);
            this.panel47.TabIndex = 0;
            // 
            // ucInfoDataGridView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel44);
            this.Name = "ucInfoDataGridView";
            this.Size = new System.Drawing.Size(445, 327);
            this.panel44.ResumeLayout(false);
            this.panel45.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdData)).EndInit();
            this.panel46.ResumeLayout(false);
            this.panel46.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel44;
        private System.Windows.Forms.Panel panel45;
        private System.Windows.Forms.Panel panel46;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.Panel panel47;
        private System.Windows.Forms.DataGridView grdData;
    }
}
