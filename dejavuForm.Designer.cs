namespace IM_DeJaVu_1
{
    partial class dejavuForm
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
            this.picBox = new System.Windows.Forms.PictureBox();
            this.systemInfoControl1 = new IM_DeJaVu_1.SystemInfoControl();
            ((System.ComponentModel.ISupportInitialize)(this.picBox)).BeginInit();
            this.SuspendLayout();
            // 
            // picBox
            // 
            this.picBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picBox.Enabled = false;
            this.picBox.Location = new System.Drawing.Point(0, 0);
            this.picBox.Name = "picBox";
            this.picBox.Size = new System.Drawing.Size(800, 450);
            this.picBox.TabIndex = 0;
            this.picBox.TabStop = false;
            this.picBox.Click += new System.EventHandler(this.PicBox_Click);
            // 
            // systemInfoControl1
            // 
            this.systemInfoControl1.Location = new System.Drawing.Point(13, 13);
            this.systemInfoControl1.Name = "systemInfoControl1";
            this.systemInfoControl1.Size = new System.Drawing.Size(155, 166);
            this.systemInfoControl1.TabIndex = 1;
            // 
            // dejavuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.systemInfoControl1);
            this.Controls.Add(this.picBox);
            this.Name = "dejavuForm";
            this.Text = "DeJaVu";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DejavuForm_FormClosing);
            this.Load += new System.EventHandler(this.DejavuForm_Load);
            this.Click += new System.EventHandler(this.PicBox_Click);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.DejavuForm_MouseClick);
            this.Resize += new System.EventHandler(this.DejavuForm_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.picBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox picBox;
        private SystemInfoControl systemInfoControl1;
    }
}

