namespace IM_DeJaVu_1
{
    partial class SystemInfoControl
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
            this.cpu_txt = new System.Windows.Forms.Label();
            this.ram_txt = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBoxDrives = new System.Windows.Forms.GroupBox();
            this.flp_drives = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBoxDrives.SuspendLayout();
            this.SuspendLayout();
            // 
            // cpu_txt
            // 
            this.cpu_txt.AutoSize = true;
            this.cpu_txt.Dock = System.Windows.Forms.DockStyle.Top;
            this.cpu_txt.Location = new System.Drawing.Point(3, 15);
            this.cpu_txt.Name = "cpu_txt";
            this.cpu_txt.Size = new System.Drawing.Size(23, 12);
            this.cpu_txt.TabIndex = 0;
            this.cpu_txt.Text = "15%";
            this.cpu_txt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ram_txt
            // 
            this.ram_txt.AutoSize = true;
            this.ram_txt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ram_txt.Location = new System.Drawing.Point(3, 15);
            this.ram_txt.Name = "ram_txt";
            this.ram_txt.Size = new System.Drawing.Size(38, 12);
            this.ram_txt.TabIndex = 0;
            this.ram_txt.Text = "760Mb";
            this.ram_txt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.groupBoxDrives);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(115, 177);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Resource View";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cpu_txt);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(3, 15);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(109, 37);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "CPU";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.ram_txt);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(3, 52);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(109, 37);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "RAM";
            // 
            // groupBoxDrives
            // 
            this.groupBoxDrives.Controls.Add(this.flp_drives);
            this.groupBoxDrives.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBoxDrives.Location = new System.Drawing.Point(3, 89);
            this.groupBoxDrives.Name = "groupBoxDrives";
            this.groupBoxDrives.Size = new System.Drawing.Size(109, 69);
            this.groupBoxDrives.TabIndex = 2;
            this.groupBoxDrives.TabStop = false;
            this.groupBoxDrives.Text = "Drives";
            // 
            // flp_drives
            // 
            this.flp_drives.AutoScroll = true;
            this.flp_drives.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flp_drives.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flp_drives.Location = new System.Drawing.Point(3, 15);
            this.flp_drives.Name = "flp_drives";
            this.flp_drives.Size = new System.Drawing.Size(103, 51);
            this.flp_drives.TabIndex = 1;
            // 
            // SystemInfoControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "SystemInfoControl";
            this.Size = new System.Drawing.Size(121, 164);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBoxDrives.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label cpu_txt;
        private System.Windows.Forms.Label ram_txt;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBoxDrives;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.FlowLayoutPanel flp_drives;
    }
}
