using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace IM_DeJaVu_1
{
    public partial class SystemInfoControl : UserControl
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool GetDiskFreeSpace(string lpRootPathName,
   out uint lpSectorsPerCluster,
   out uint lpBytesPerSector,
   out uint lpNumberOfFreeClusters,
   out uint lpTotalNumberOfClusters);

        Label driveLabel = null;
        PerformanceCounter cpu_counter = new PerformanceCounter();
        public SystemInfoControl()
        {
            InitializeComponent();

            cpu_counter.CategoryName = "Processor";
            cpu_counter.CounterName = "% Processor Time";
            cpu_counter.InstanceName = "_Total";

            CheckState();

        }
        
        public void CheckState()
        {
            cpu_txt.Text = Math.Round(cpu_counter.NextValue(), 1).ToString() + "%";
            ram_txt.Text = Math.Round(Environment.WorkingSet/1e+6, 1) + "Mb";

            try
            {
                
                DriveInfo[] drives = DriveInfo.GetDrives();
                Console.WriteLine("checking drives..." + drives.Length);
                flp_drives.Controls.Clear();
                foreach (DriveInfo drive in drives)
                {
                    if (drive.IsReady==true)
                    {                        
                        string driveinfo = "";
                        StringBuilder stringBuilder = new StringBuilder(drive.Name);
                        driveLabel = new Label();
                        
                        driveLabel.AutoSize = true;
                        stringBuilder.Append(" free space: ");
                        stringBuilder.Append(Math.Round(drive.AvailableFreeSpace/1e+9, 1) + "Gb");
                        //stringBuilder.Append(drive.TotalSize / 1024 / 1024);
                        Console.WriteLine(stringBuilder.ToString());
                        driveLabel.Text = stringBuilder.ToString();
                        flp_drives.Controls.Add(driveLabel);
                        driveLabel.Dock = DockStyle.Top;
                    }

                }
            }
            catch (IOException iox)
            {
                Console.WriteLine(iox.Message);
            }
        }
    }
}
