namespace MIS.ControlObject
{
    partial class ucInfoListView
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
            this.panel46 = new System.Windows.Forms.Panel();
            this.label52 = new System.Windows.Forms.Label();
            this.panel47 = new System.Windows.Forms.Panel();
            this.lvwList = new System.Windows.Forms.ListView();
            this.panel44.SuspendLayout();
            this.panel45.SuspendLayout();
            this.panel46.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel44
            // 
            this.panel44.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel44.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel44.Controls.Add(this.panel45);
            this.panel44.Controls.Add(this.panel46);
            this.panel44.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel44.Location = new System.Drawing.Point(3, 3);
            this.panel44.Name = "panel44";
            this.panel44.Size = new System.Drawing.Size(499, 315);
            this.panel44.TabIndex = 486;
            // 
            // panel45
            // 
            this.panel45.Controls.Add(this.lvwList);
            this.panel45.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel45.Location = new System.Drawing.Point(0, 24);
            this.panel45.Name = "panel45";
            this.panel45.Size = new System.Drawing.Size(497, 289);
            this.panel45.TabIndex = 312;
            // 
            // panel46
            // 
            this.panel46.BackColor = System.Drawing.Color.Gainsboro;
            this.panel46.Controls.Add(this.label52);
            this.panel46.Controls.Add(this.panel47);
            this.panel46.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel46.Location = new System.Drawing.Point(0, 0);
            this.panel46.Name = "panel46";
            this.panel46.Size = new System.Drawing.Size(497, 24);
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
            this.panel47.Size = new System.Drawing.Size(497, 1);
            this.panel47.TabIndex = 0;
            // 
            // lvwList
            // 
            this.lvwList.BackColor = System.Drawing.Color.GhostWhite;
            this.lvwList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvwList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwList.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvwList.ForeColor = System.Drawing.Color.Black;
            this.lvwList.FullRowSelect = true;
            this.lvwList.HideSelection = false;
            this.lvwList.Location = new System.Drawing.Point(0, 0);
            this.lvwList.Name = "lvwList";
            this.lvwList.Size = new System.Drawing.Size(497, 289);
            this.lvwList.TabIndex = 118;
            this.lvwList.UseCompatibleStateImageBehavior = false;
            this.lvwList.View = System.Windows.Forms.View.Details;
            // 
            // ucInfoListView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel44);
            this.Name = "ucInfoListView";
            this.Size = new System.Drawing.Size(504, 320);
            this.panel44.ResumeLayout(false);
            this.panel45.ResumeLayout(false);
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
        private System.Windows.Forms.ListView lvwList;
    }
}
