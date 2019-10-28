using DirectShowLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;
using GitHub.secile.Avi.One;

using System.Drawing.Imaging;

namespace IM_DeJaVu_1
{
    public partial class dejavuForm : Form
    {
        System.Drawing.Imaging.Encoder imageEncoder;
        EncoderParameter myEncoderParameter;
        EncoderParameters myEncoderParameters;
        ImageCodecInfo myImageCodecInfo;

        private CameraManager cameraMan = null;
        public static PictureBox pictureBx = null;

        //private OpenH264Lib.Encoder encoder = null;
        //private H264Writer writer = null;
        private MjpegWriter writer = null;

        private List<byte[]> arrayList = new List<byte[]>();
        //private List<byte[]> arrayBytes = new List<byte[]>();

        private List<byte[]> buffer_2 = new List<byte[]>();

        Bitmap bitmap = null;
        bool recording = false;

        //List<byte[]> bytearray = new List<byte[]>();

        //TIMERS
        private Timer timer = null; //Main timer to control the main recording
        private Timer sensorTimer = null;
        private Timer snaptimer;
        private Timer fake5sec;
        private Timer resourceChecker;
        //

        private IRSensor iRSensor = null;

        //VARS
        private string destinationFileName = String.Concat(DateTime.Now.ToString("yyyyMMddHHmmss"), ".avi");
        int OneSecond = 1000;
        int fps = 15;
        int array_time_length = 10000;
        int period_before = 1;
        //int video_length = 20000;
        int sensor_steps = 1000;


        //Strings
        string destFile = String.Empty;
        string driveAddress = @"D:\CAMERA\";

        public dejavuForm()
        {
            InitializeComponent();

            destFile = Path.Combine(driveAddress, destinationFileName);
            Directory.CreateDirectory(driveAddress);
            //Console.WriteLine(destFile + " path");
            cameraMan = new CameraManager();
            cameraMan.mainForm = this;
            pictureBx = picBox;

            Console.WriteLine("Building graph...");
            cameraMan.GetInterfaces();

            Console.WriteLine("Start preview...");
            cameraMan.Init();
            DejavuForm_Resize(this, null);
            this.WindowState = FormWindowState.Maximized;
            /*
            //systemInfoControl1.Parent = picBox;

            // Get an ImageCodecInfo object that represents the JPEG codec.
            myImageCodecInfo = GetEncoderInfo("image/jpeg");

            imageEncoder = System.Drawing.Imaging.Encoder.Quality;
            // EncoderParameter object in the array.
            myEncoderParameters = new EncoderParameters(1);

            // Save the bitmap as a JPEG file with quality level 75.
            myEncoderParameter = new EncoderParameter(imageEncoder, 75L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            
            */

            //StartH264Encoder();

        }
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            Debug.WriteLine("Stop recording timers... END OF RECORDING \n Start checking the sensor");
            
            //Stops itself
            timer.Stop();                        
            timer.Enabled = false;

            fake5sec.Start();
            fake5sec.Enabled = true;
            recording = false;
            if (writer != null)
            {
                writer.Close();
            }

            //sensorTimer.Start();
            //sensorTimer.Enabled = true;
        }
        private void CheckSensor(object sender, EventArgs e)
        {
            
            uint sval = iRSensor.CheckSensor();
            Debug.WriteLine("Sensor check..." + sval);

            //SET value to 0 for testing purpose
            //1 is for PP
            if (sval == 0 && !timer.Enabled) //sensor true and 1 min elapsed
            {
                destinationFileName = String.Concat(DateTime.Now.ToString("yyyyMMddHHmmss"), ".avi");
                destFile = Path.Combine(driveAddress, destinationFileName);

                timer.Start();
                Debug.WriteLine("MOTION DETECTED...");
                sensorTimer.Stop();
                sensorTimer.Enabled = false;
                //the sensor returns true. Start recording

                //START the encoder
                StartH264Encoder();

                if (arrayList.Count > 0 && !fake5sec.Enabled)
                {
                    //Debug.WriteLine("FAKE 5 SEC TIMER OFF");
                    RecordBuffer2Disk();
                }
                else
                {
                    //recording = true;
                }
                
            }
            else
            {

            }

        }
        public void ResumeSensor()
        {
            //sensorTimer.Start();
        }
        
        private void DejavuForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            iRSensor.SensorClose();
            //ReleaseInstance(ref graphBuilder);
            timer.Stop();
            sensorTimer.Stop();
            snaptimer.Stop();
            fake5sec.Stop();
            recording = false;
            if (writer != null)
            {
                writer.Close();
            }
            cameraMan.ReleaseInterfaces();
        }

        private void DejavuForm_Load(object sender, EventArgs e)
        {
            //BuildGraph();

            //SETUP Timers
            sensorTimer = new Timer
            {
                Interval = sensor_steps
            };
            sensorTimer.Tick += CheckSensor;
            //sensorTimer.Start();

            fake5sec = new Timer
            {
                Interval = period_before * 1000
            };
            fake5sec.Tick += Fake5sec_Tick;
            fake5sec.Start();

            timer = new Timer
            {
                Interval = array_time_length //1min tick to finish recording and recheck the sensor
            };
            timer.Tick += Timer_Tick;

            snaptimer = new Timer()
            {
                Interval = OneSecond / fps,
            };
            snaptimer.Tick += Snaptimer_Elapsed;
            //

            resourceChecker = new Timer()
            {
                Interval = 5000
            };
            resourceChecker.Tick += ResourceChecker_Tick;
            resourceChecker.Start();

            //
            iRSensor = new IRSensor();

            snaptimer.Start();
        }

        private void ResourceChecker_Tick(object sender, EventArgs e)
        {
            systemInfoControl1.CheckState();
        }

        private void Fake5sec_Tick(object sender, EventArgs e)
        {
            fake5sec.Stop();
            fake5sec.Enabled = false;
            sensorTimer.Start();
        }

        private void Snaptimer_Elapsed(object sender, EventArgs e)
        {

            //Cursor.Current = Cursors.WaitCursor;
            // capture image

            //bitmap = cameraMan.GetBitmapImage;
            try {
                //arrayList.Add(cameraMan.GetRawBytes());
                //bitmap = cameraMan.BitmapFromRawData(cameraMan.GetRawBytes());
                writer.AddImage(cameraMan.GetRawBytes());
            }
            catch(NullReferenceException nrx)
            {

            }
            
            //
            //Console.WriteLine(arrayList[0]);
            //destinationFileName = String.Concat(DateTime.Now.ToString("yyyyMMddHHmmssfff"),  ".jpeg");
            //destFile = Path.Combine(driveAddress, destinationFileName);
            /*
            if (recording)
            {
                //Console.WriteLine(arrayList.Count + " recording");
                //bitmap.Save(@"dejavutestimage.jpeg");
                if(bitmap!=null)
                {
                    //encoder.Encode(bitmap);
                    writer.AddImage(bitmap);
                }                
            }
            else
            {
                //Console.WriteLine(arrayList.Count);
                if (bitmap != null)
                {
                    //Console.WriteLine(bitmap);
                    //bitmap.Save(destFile, myImageCodecInfo, myEncoderParameters);
                    //bitmap.Save(destFile);
                }

                //arrayList.Add(bitmap);                
                int arrcount = arrayList.Count;                
                int necessary_count = fps * array_time_length;
                if (arrcount > necessary_count)
                {
                    arrayList.RemoveRange(0, arrcount - necessary_count);
                }
                
            }
            */
            //bitmap.Dispose();
            //Cursor.Current = Cursors.Default;

        }
        void RecordBuffer2Disk()
        {
            Console.WriteLine(arrayList.Count);
            Bitmap bitmap = null;
            if (arrayList.Count > 0)
            {
                buffer_2.Clear();
                buffer_2.AddRange(arrayList);
                arrayList.Clear();

                Debug.WriteLine(buffer_2.Count + " - must be larger than this: " + (period_before * fps));
                if (buffer_2.Count > (period_before * fps))
                {
                    buffer_2.RemoveRange(0, buffer_2.Count - (period_before * fps));
                }
                int arcount = buffer_2.Count;

                for (int i = 0; i < arcount; i++)
                {
                    // writer frame
                    if(buffer_2[i]!=null)
                    {
                        //Console.WriteLine(buffer_2[i].Width);
                        //buffer_2[i].Save("testimage.jpeg");
                        //int hr = encoder.Encode((Bitmap)buffer_2[i]);
                        //writer.AddImage(buffer_2[i]);
                        //bitmap = cameraMan.BitmapFromRawData(buffer_2[i]);
                        writer.AddImage(cameraMan.GetRawBytes());
                    }
                    
                    //writer.AddImage((Bitmap)arrayList[i]);
                }
                //Debug.WriteLine("FRAME count now: {0} and it was {1}", buffer_2.Count, arcount);
                buffer_2.RemoveRange(0, arcount);
                //Debug.WriteLine("FRAME count: {0}", buffer_2.Count);
                if (arrayList.Count > 0)
                {
                    RecordBuffer2Disk();
                }
                else
                {
                    //recording = true;
                }
            }
            else
            {
                //recording = true;
            }
        }
        
        private void StartH264Encoder()
        {
            int fps = 15;
            //Bitmap firstFrame = new Bitmap(1280, 720);//set the values programmatically

            // AVIに出力するライターを作成(create AVI writer)
            var aviPath = destFile;
            var aviFile = System.IO.File.OpenWrite(aviPath);
            Debug.WriteLine(aviPath);
            //writer = new H264Writer(aviFile, firstFrame.Width, firstFrame.Height, fps);
            writer = new MjpegWriter(aviFile, 1280, 720, fps);

            // H264エンコーダーを作成(create H264 encoder)
            //encoder = new OpenH264Lib.Encoder("openh264-2.0.0-win32.dll");

            // 1フレームエンコードするごとにライターに書き込み(write frame data for each frame encoded)
            OpenH264Lib.Encoder.OnEncodeCallback onEncode = (data, length, frameType) =>
            {
                var keyFrame = (frameType == OpenH264Lib.Encoder.FrameType.IDR) || (frameType == OpenH264Lib.Encoder.FrameType.I);
                
                //arrayList.Add(data);
                int arrcount = arrayList.Count;
                int necessary_count = fps * array_time_length;
                if (arrcount > necessary_count)
                {
                    arrayList.RemoveRange(0, arrcount - necessary_count);
                }
                //writer.AddImage(data, keyFrame);
                //Console.WriteLine("Encode {0} frametype", frameType);
            };

            // H264エンコーダーの設定(encoder setup)
            int bps = 5000 * 1000;         // target bitrate. 5Mbps.
            float keyFrameInterval = 2.0f; // insert key frame interval. unit is second.
            
            //encoder.Setup(firstFrame.Width, firstFrame.Height, bps, fps, keyFrameInterval, onEncode);

            // 1フレームごとにエンコード実施(do encode)

            //encoder.Encode(firstFrame);
            writer.AddImage(cameraMan.GetRawBytes());

            //writer.Close();
            //MessageBox.Show(string.Format("{0}\n is created.", aviPath));
        }

        
        private void DejavuForm_Resize(object sender, EventArgs e)
        {            
            if(cameraMan!=null)
                cameraMan.videoWindow.SetWindowPosition(0, 0, picBox.Width, picBox.Height);
        }

        private void SystemInfoControl1_MouseClick(object sender, MouseEventArgs e)
        {
            systemInfoControl1.CheckState();
        }

        private void DejavuForm_MouseClick(object sender, MouseEventArgs e)
        {
            
            
        }

        private void SystemInfoControl1_Load(object sender, EventArgs e)
        {

        }

        private void PicBox_Click(object sender, EventArgs e)
        {
            Bitmap bitmap;
            bitmap = cameraMan.GetBitmapImage;
            bitmap.Save("tettete.bmp");
        }
    }
}
