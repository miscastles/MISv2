namespace MIS
{
    partial class ucLoading
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucLoading));
            this.objProgress = new Bunifu.Framework.UI.BunifuCircleProgressbar();
            this.txtHeader1 = new System.Windows.Forms.RichTextBox();
            this.txtHeader2 = new System.Windows.Forms.RichTextBox();
            this.picGIF = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picGIF)).BeginInit();
            this.SuspendLayout();
            // 
            // objProgress
            // 
            this.objProgress.animated = false;
            this.objProgress.animationIterval = 5;
            this.objProgress.animationSpeed = 300;
            this.objProgress.BackColor = System.Drawing.Color.Maroon;
            this.objProgress.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("objProgress.BackgroundImage")));
            this.objProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 26.25F);
            this.objProgress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.objProgress.LabelVisible = false;
            this.objProgress.LineProgressThickness = 8;
            this.objProgress.LineThickness = 5;
            this.objProgress.Location = new System.Drawing.Point(5, 5);
            this.objProgress.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.objProgress.MaxValue = 100;
            this.objProgress.Name = "objProgress";
            this.objProgress.ProgressBackColor = System.Drawing.Color.Gainsboro;
            this.objProgress.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.objProgress.Size = new System.Drawing.Size(61, 61);
            this.objProgress.TabIndex = 1;
            this.objProgress.Value = 0;
            // 
            // txtHeader1
            // 
            this.txtHeader1.BackColor = System.Drawing.Color.Maroon;
            this.txtHeader1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtHeader1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHeader1.ForeColor = System.Drawing.Color.White;
            this.txtHeader1.Location = new System.Drawing.Point(68, 14);
            this.txtHeader1.Name = "txtHeader1";
            this.txtHeader1.Size = new System.Drawing.Size(306, 23);
            this.txtHeader1.TabIndex = 2;
            this.txtHeader1.Text = "Loading...";
            // 
            // txtHeader2
            // 
            this.txtHeader2.BackColor = System.Drawing.Color.Maroon;
            this.txtHeader2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtHeader2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtHeader2.ForeColor = System.Drawing.Color.White;
            this.txtHeader2.Location = new System.Drawing.Point(68, 34);
            this.txtHeader2.Name = "txtHeader2";
            this.txtHeader2.Size = new System.Drawing.Size(306, 23);
            this.txtHeader2.TabIndex = 4;
            this.txtHeader2.Text = "Please wait...";
            // 
            // picGIF
            // 
            this.picGIF.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.picGIF.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picGIF.ErrorImage = ((System.Drawing.Image)(resources.GetObject("picGIF.ErrorImage")));
            this.picGIF.Image = ((System.Drawing.Image)(resources.GetObject("picGIF.Image")));
            this.picGIF.Location = new System.Drawing.Point(19, 78);
            this.picGIF.Name = "picGIF";
            this.picGIF.Size = new System.Drawing.Size(173, 173);
            this.picGIF.TabIndex = 5;
            this.picGIF.TabStop = false;
            // 
            // ucLoading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Maroon;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.picGIF);
            this.Controls.Add(this.txtHeader2);
            this.Controls.Add(this.txtHeader1);
            this.Controls.Add(this.objProgress);
            this.Name = "ucLoading";
            this.Size = new System.Drawing.Size(732, 342);
            this.Load += new System.EventHandler(this.ucLoading_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picGIF)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Bunifu.Framework.UI.BunifuCircleProgressbar objProgress;
        private System.Windows.Forms.RichTextBox txtHeader1;
        private System.Windows.Forms.RichTextBox txtHeader2;
        private System.Windows.Forms.PictureBox picGIF;
    }
}
