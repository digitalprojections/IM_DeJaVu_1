using DirectShowLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace IM_DeJaVu_1
{
    class CameraManager
    {
        public dejavuForm mainForm;        

        public enum PlayState : int
        {
            Stopped,
            Paused,
            Running,
            Init
        }

        private IGraphBuilder pGraph;
        //private Guid CLSID_SampleGrabber = new Guid("{C1F400A0-3F08-11D3-9F0B-006008039E37}"); //qedit.dll
        private PlayState CurrentState = PlayState.Stopped;
        private int WM_GRAPHNOTIFY = Convert.ToInt32("0X8000", 16) + 1;
        public IVideoWindow videoWindow = null;
        private IMediaControl mediaControl = null;        
        private IMediaEventEx mediaEventEx = null;
        private IGraphBuilder graph = null;
        private ICaptureGraphBuilder2 pGraphBuilder = null;
        private IBaseFilter pUSB = null;
        private IBaseFilter renderFilter = null;
        private NullRenderer nullRender = null;
        private IAMStreamConfig streamConfig = null;
        private SampleGrabber pSampleGrabber = null;
        
        private VideoInfoHeader format = null;
        private AMMediaType pmt = null;
        private ISampleGrabber i_grabber = null;

        IBaseFilter pSmartTee = null;
        IBaseFilter pAVIMux = null;

        public void GetInterfaces()
        {
            graph = (IGraphBuilder)(new FilterGraph());
            pGraphBuilder = (ICaptureGraphBuilder2)(new CaptureGraphBuilder2());
            mediaControl = (IMediaControl)graph;
            videoWindow = (IVideoWindow)graph;
            mediaEventEx = (IMediaEventEx)graph;
            renderFilter = (IBaseFilter)new VideoMixingRenderer9();            
            pSampleGrabber = new SampleGrabber();
            i_grabber = pSampleGrabber as ISampleGrabber;
            var mt = new AMMediaType();
            mt.majorType = MediaType.Video;
            mt.subType = MediaSubType.RGB24;
            i_grabber.SetMediaType(mt);
            //send notification messages to the control window
            int hr = mediaEventEx.SetNotifyWindow(IntPtr.Zero, WM_GRAPHNOTIFY, IntPtr.Zero);
            DsError.ThrowExceptionForHR(hr);
            DsUtils.FreeAMMediaType(mt);

        }
        

        void SetFormat()
        {
            int hr = 0;
            pmt = new AMMediaType();
            pmt.majorType = MediaType.Video;
            pmt.subType = MediaSubType.MJPG;
            pmt.formatType = FormatType.VideoInfo;
            pmt.fixedSizeSamples = false; //true for 640x480
            pmt.formatSize = 88;
            pmt.sampleSize = 2764800; //2764800 614400
            pmt.temporalCompression = false;
            //////////////////////////////////
            format = new VideoInfoHeader();
            format.SrcRect = new DsRect();
            format.TargetRect = new DsRect();
            format.BitRate = 5000000;
            format.AvgTimePerFrame = 666666;
            //////////////////////////////////            
            format.BmiHeader = new BitmapInfoHeader();
            format.BmiHeader.Size = 40;
            format.BmiHeader.Width = 1280;
            format.BmiHeader.Height = 720;
            format.BmiHeader.Planes = 1;
            format.BmiHeader.BitCount = 24;
            format.BmiHeader.Compression = 1196444237; //1196444237 //844715353
            format.BmiHeader.ImageSize = 2764800; //2764800 614400
            pmt.formatPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(format));
            Marshal.StructureToPtr(format, pmt.formatPtr, false);
            //Debug.WriteLine(getCatName(pUSB) + " at line 130");
            streamConfig = (IAMStreamConfig)GetPin(pUSB, getCatName(pUSB));
            hr = streamConfig.SetFormat(pmt);
            DsUtils.FreeAMMediaType(pmt);
            if (hr < 0)
            {
                CapTest.CustomMessage.ShowMessage("Can`t set format");
                DsError.ThrowExceptionForHR(hr);
            }
        }

        //The following is called for building the PREVIEW graph
        #region PREVIEW ONLY
        public void Init()
        {           

            Debug.WriteLine("VIDEO FOR PREVIEW");
            pGraph = graph;
            int hr = 0;

            hr = pGraphBuilder.SetFiltergraph(pGraph);
            DsError.ThrowExceptionForHR(hr);

            pUSB = FindCaptureDevice();

            hr = pGraph.AddFilter(pUSB, "WebCamControl Video");
            DsError.ThrowExceptionForHR(hr);
                       
            hr = pGraph.AddFilter(pSampleGrabber as IBaseFilter, "SampleGrabber");
            checkHR(hr, "Can't add SampleGrabber to graph");
            i_grabber = (ISampleGrabber)pSampleGrabber;
            i_grabber.SetBufferSamples(true);
                        
            IVMRAspectRatioControl9 ratioControl9 = (IVMRAspectRatioControl9)renderFilter;
            hr = ratioControl9.SetAspectRatioMode(VMRAspectRatioMode.LetterBox);
            DsError.ThrowExceptionForHR(hr);

            hr = pGraph.AddFilter(renderFilter, "My Render Filter");
            DsError.ThrowExceptionForHR(hr);
            //SetFormat();

            hr = pGraphBuilder.RenderStream(PinCategory.Preview, MediaType.Video, pUSB,(IBaseFilter) pSampleGrabber, renderFilter);
            DsError.ThrowExceptionForHR(hr);
            Debug.WriteLine(DsError.GetErrorText(hr) + " is error in rendering");

            IEnumFilters enumFilters = null;
            IBaseFilter[] baseFilters = { null };
            IntPtr fetched = IntPtr.Zero;
            hr = pGraph.EnumFilters(out enumFilters);
            int r = 0;
            while (r == 0)
            {
                try
                {
                    r = enumFilters.Next(baseFilters.Length, baseFilters, fetched);
                    DsError.ThrowExceptionForHR(hr);
                    baseFilters[0].QueryFilterInfo(out FilterInfo filterInfo);
                    Debug.WriteLine(filterInfo.achName + " -filtername");
                }
                catch
                {
                    r = 1;
                    continue;
                }

            }

            SafeReleaseComObject(pUSB);
            SafeReleaseComObject(pAVIMux);
            SafeReleaseComObject(pGraph);
            SafeReleaseComObject(pGraphBuilder);
            SafeReleaseComObject(pSampleGrabber);
            //SafeReleaseComObject(sampleGrabber);
            SafeReleaseComObject(pSmartTee);
            SafeReleaseComObject(renderFilter);
            SafeReleaseComObject(nullRender);
            SafeReleaseComObject(mediaEventEx);
            SafeReleaseComObject(ratioControl9);
            SafeReleaseComObject(streamConfig);
            SafeReleaseComObject(mediaControl);
            //SafeReleaseComObject(format);
            //SafeReleaseComObject(i_grabber);
            SafeReleaseComObject(graph);

            SetupVideoWindow();
        }
        #endregion
        //List<byte> cambytes = new List<byte>();
       
        private Bitmap GetBitmap(ISampleGrabber i_grabber, int width, int height, int stride)
        {
            // サンプルグラバから画像を取得するためには
            // まずサイズ0でGetCurrentBufferを呼び出しバッファサイズを取得し
            // バッファ確保して再度GetCurrentBufferを呼び出す。
            // 取得した画像は逆になっているので反転させる必要がある。
            int sz = 0;
            i_grabber.GetCurrentBuffer(ref sz, IntPtr.Zero); // IntPtr.Zeroで呼び出してバッファサイズ取得
            if (sz == 0) return null;

            // メモリ確保し画像データ取得
            var ptr = Marshal.AllocCoTaskMem(sz);
            i_grabber.GetCurrentBuffer(ref sz, ptr);

            // 画像データをbyte配列に入れなおす
            var data = new byte[sz];
            Marshal.Copy(ptr, data, 0, sz);
            
            Bitmap result = null;

            try
            {
                // 画像を作成
                result = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                var bmp_data = result.LockBits(new Rectangle(Point.Empty, result.Size), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

                
                //cambytes.AddRange(data);
                //Console.WriteLine(cambytes.Count);
                // 上下反転させながら1行ごとコピー
                for (int y = 0; y < height; y++)
                {
                    var src_idx = sz - (stride * (y + 1)); // 最終行から
                    //Console.WriteLine(src_idx);
                    var dst = new IntPtr(bmp_data.Scan0.ToInt32() + (stride * y));
                    Marshal.Copy(data, src_idx, dst, stride);
                    //
                    //Console.WriteLine(src_idx.ToString() + " data length");
                }
                
                result.UnlockBits(bmp_data);
                Marshal.FreeCoTaskMem(ptr);
            }
            catch (ArgumentException ax)
            {
                //System.Windows.Forms.MessageBox.Show(ax.Message);
                Console.WriteLine(ax.Message);
            }

            return result;
        }

        public Bitmap BitmapFromRawData(byte[] data)
        {
            var mt = new AMMediaType();
            i_grabber.GetConnectedMediaType(mt);
            var header = (VideoInfoHeader)Marshal.PtrToStructure(mt.formatPtr, typeof(VideoInfoHeader));
            var width = header.BmiHeader.Width;
            var height = header.BmiHeader.Height;
            var stride = width * (header.BmiHeader.BitCount / 8);
            DeleteMediaType(ref mt);

            Bitmap result = null;

            try
            {
                // 画像を作成
                result = new Bitmap(width, height, PixelFormat.Format24bppRgb);
                var bmp_data = result.LockBits(new Rectangle(Point.Empty, result.Size), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);


                //cambytes.AddRange(data);
                //Console.WriteLine(cambytes.Count);
                // 上下反転させながら1行ごとコピー
                for (int y = 0; y < height; y++)
                {
                    var src_idx = data.Length - (stride * (y + 1)); // 最終行から
                    //Console.WriteLine(src_idx);
                    var dst = new IntPtr(bmp_data.Scan0.ToInt32() + (stride * y));
                    Marshal.Copy(data, src_idx, dst, stride);
                    //
                    //Console.WriteLine(src_idx.ToString() + " data length");
                    //Marshal.FreeCoTaskMem(dst);
                }
                result.UnlockBits(bmp_data);
                //
            }
            catch (ArgumentException ax)
            {
                //System.Windows.Forms.MessageBox.Show(ax.Message);
                Console.WriteLine(ax.Message);
            }
            return result;
        }

        public byte[] GetRawBytes()
        {
            int sz = 0;
            i_grabber.GetCurrentBuffer(ref sz, IntPtr.Zero); // IntPtr.Zeroで呼び出してバッファサイズ取得
            if (sz == 0) return null;

            // メモリ確保し画像データ取得
            var ptr = Marshal.AllocCoTaskMem(sz);
            i_grabber.GetCurrentBuffer(ref sz, ptr);

            // 画像データをbyte配列に入れなおす
            var data = new byte[sz];
            Marshal.Copy(ptr, data, 0, sz);
            Marshal.FreeCoTaskMem(ptr);
            return data;
        }

        

        public Bitmap GetBitmapImage
        {
            get
            {
                var mt = new AMMediaType();
                i_grabber.GetConnectedMediaType(mt);
                var header = (VideoInfoHeader)Marshal.PtrToStructure(mt.formatPtr, typeof(VideoInfoHeader));
                var width = header.BmiHeader.Width;
                var height = header.BmiHeader.Height;
                var stride = width * (header.BmiHeader.BitCount / 8);
                DeleteMediaType(ref mt);

                Bitmap result = GetBitmap(i_grabber, width, height, stride);

                //result.Save("testImage.jpeg");

                //Debug.WriteLine(result + " Must be the IMAGE");           
                return result;
            }
        }

        public static void DeleteMediaType(ref AMMediaType mt)
        {
            if (mt.sampleSize != 0) Marshal.FreeCoTaskMem(mt.formatPtr);
            if (mt.unkPtr != IntPtr.Zero) Marshal.FreeCoTaskMem(mt.unkPtr);
            mt = null;
        }

        
        static void checkHR(int hr, string msg)
        {
            if (hr < 0)
            {
                Console.WriteLine(msg);
                DsError.ThrowExceptionForHR(hr);
            }
        }
        private void SetupVideoWindow()
        {
            int hr = 0;

            //set the video window to be a child of the main window
            //putowner : Sets the owning parent window for the video playback window. 
            hr = videoWindow.put_Owner(dejavuForm.pictureBx.Handle);
            DsError.ThrowExceptionForHR(hr);

            hr = videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren);
            DsError.ThrowExceptionForHR(hr);

            
            //Make the video window visible, now that it is properly positioned
            //put_visible : This method changes the visibility of the video window. 
            hr = videoWindow.put_Visible(OABool.True);
            DsError.ThrowExceptionForHR(hr);

            hr = mediaControl.Run();

            DsError.ThrowExceptionForHR(hr);
            HandleGraphEvent();

            CurrentState = PlayState.Running;

            mainForm.ResumeSensor();
            //mainForm.WebCamControl_Resize(this, null);
            //videoControl.GetMaxAvailableFrameRate(pUSB.FindPin(""));
        }
        public void ReleaseInterfaces()
        {
            if (mediaControl != null)
                mediaControl.StopWhenReady();

            CurrentState = PlayState.Stopped;

            // stop notifications of events
            if (mediaEventEx != null)
                mediaEventEx.SetNotifyWindow(IntPtr.Zero, WM_GRAPHNOTIFY, IntPtr.Zero);

            //// below we relinquish ownership (IMPORTANT!) of the video window.
            //// Failing to call put_Owner can lead to assert failures within
            //// the video renderer, as it still assumes that it has a valid
            //// parent window.
            if (videoWindow != null)
            {
                videoWindow.put_Visible(OABool.False);
                videoWindow.put_Owner(IntPtr.Zero);
            }

            // Release DirectShow interfaces
            if (mediaControl != null)
            {
                Marshal.ReleaseComObject(mediaControl);
                mediaControl = null;
            }


            if (mediaEventEx != null)
            {
                Marshal.ReleaseComObject(mediaEventEx);
                mediaEventEx = null;
            }

            if (videoWindow != null)
            {
                Marshal.ReleaseComObject(videoWindow);
                videoWindow = null;
            }

            if (graph != null)
            {
                Marshal.ReleaseComObject(graph);
                graph = null;
            }

            if (pGraphBuilder != null)
            {
                Marshal.ReleaseComObject(pGraphBuilder);
                pGraphBuilder = null;
            }

        }
        private static void SafeReleaseComObject(object obj)
        {
            if (obj != null)
            {
                Marshal.ReleaseComObject(obj);
            }
        }

        //カメラデバイスにアクセス
        private IBaseFilter FindCaptureDevice()
        {
            ArrayList devs = new ArrayList();
            IEnumMoniker classEnum = null;
            IMoniker[] moniker = { null };

            object source = null;
            ICreateDevEnum devEnum = (ICreateDevEnum)(new CreateDevEnum());

            int hr = devEnum.CreateClassEnumerator(FilterCategory.VideoInputDevice, out classEnum, CDef.None);
            DsError.ThrowExceptionForHR(hr);

            if (classEnum == null)
            {
                throw new ApplicationException("No video capture device was detected.\\r\\n\\r\\n" + "This sample requires a video capture device, such as a USB WebCam,\\r\\nto be installed and working properly.  The sample will now close.");
            }

            IntPtr none = IntPtr.Zero;
            while (classEnum.Next(1, moniker, none) == 0)
            {

                Guid iid = typeof(IBaseFilter).GUID;
                moniker[0].BindToObject(null, null, ref iid, out source);
                Marshal.ReleaseComObject(moniker[0]);
                devs.Add(source);
            }

            Debug.WriteLine(devs.Count + " ==== cameras found");

            Marshal.ReleaseComObject(devEnum);
            Marshal.ReleaseComObject(classEnum);

            //we can now select cameras
            return (IBaseFilter)devs[0];
        }
        //カメラのPINにアクセス
        static IPin GetPin(IBaseFilter filter, string pinname)
        {
            IEnumPins epins;
            int hr = filter.EnumPins(out epins);
            if (hr < 0)
            {
                CapTest.CustomMessage.ShowMessage("Can't enumerate pins");
                DsError.ThrowExceptionForHR(hr);
            }

            IntPtr fetched = Marshal.AllocCoTaskMem(4);
            IPin[] pins = new IPin[1];
            while (epins.Next(1, pins, fetched) == 0)
            {
                PinInfo pinfo;
                pins[0].QueryPinInfo(out pinfo);
                bool found = (pinfo.name == pinname);
                CapTest.CustomMessage.ShowMessage(pinfo.name + " is PIN NAME");
                DsUtils.FreePinInfo(pinfo);
                if (found)
                    return pins[0];
            }

            CapTest.CustomMessage.ShowMessage("Pin not found");
            DsError.ThrowExceptionForHR(hr);

            return null;
        }

        string getCatName(IBaseFilter filter)
        {
            string retval = "";
            IEnumPins epins;
            int hr = filter.EnumPins(out epins);
            if (hr < 0)
            {
                CapTest.CustomMessage.ShowMessage("Can't enumerate pins");
                DsError.ThrowExceptionForHR(hr);
            }

            IntPtr fetched = Marshal.AllocCoTaskMem(4);
            IPin[] pins = new IPin[1];
            while (epins.Next(1, pins, fetched) == 0)
            {
                PinInfo pinfo;
                pins[0].QueryPinInfo(out pinfo);
                bool found = (pinfo.name == "Capture" || pinfo.name == "キャプチャ");
                CapTest.CustomMessage.ShowMessage(pinfo.name + " is pinname on getCatName");
                DsUtils.FreePinInfo(pinfo);
                if (found)
                    retval = pinfo.name;
            }
            CapTest.CustomMessage.ShowMessage("Pin found " + retval);
            return retval;
        }


        //デバッグ用イベント情報
        private void HandleGraphEvent()
        {
            int hr = 0;
            EventCode evCode = 0;
            IntPtr evParam1 = IntPtr.Zero;
            IntPtr evParam2 = IntPtr.Zero;

            while (mediaEventEx != null && mediaEventEx.GetEvent(out evCode, out evParam1, out evParam2, 0) == 0)
            {
                // Free event parameters to prevent memory leaks associated with
                // event parameter data.  While this application is not interested
                // in the received events, applications should always process them.
                hr = mediaEventEx.FreeEventParams(evCode, evParam1, evParam2);
                DsError.ThrowExceptionForHR(hr);

                Console.WriteLine(evCode + " -evCode " + evParam1 + " " + evParam2);
                // Insert event processing code here, if desired (see http://msdn2.microsoft.com/en-us/library/ms783649.aspx)
            }
        }
    }
}
