using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace GitHub.secile.Avi.One
{
    class MjpegWriter
    {
        private AviWriter aviWriter;

        public MjpegWriter(System.IO.Stream outputAvi, int width, int height, int fps)
        {
            aviWriter = new AviWriter(outputAvi, "MJPG", width, height, fps);
        }

        public void AddImage(byte[] frame)
        {
            
            aviWriter.AddImage(frame);
            
        }

        public void Close()
        {
            aviWriter.Close();
        }
    }
}
