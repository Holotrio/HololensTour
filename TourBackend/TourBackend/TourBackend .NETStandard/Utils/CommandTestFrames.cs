using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV;
using System.Drawing;

namespace TourBackend
{
    public class CommandTestFrames
    {
        public Bitmap[] frames;
        public int currentIdx;

        public CommandTestFrames(Bitmap[] _frames){
            frames = _frames;
            currentIdx = 0;
        }

        public System.Drawing.Bitmap ReturnAndSetNextFrame()
        {
            return null;
        }

        public System.Drawing.Bitmap ReturnAndSetPreviousFrame()
        {
            return null;
        }

        public System.Drawing.Bitmap GetCurrentFrame()
        {
            return null;
        }

        public void Reset() {

        }


    }
}
