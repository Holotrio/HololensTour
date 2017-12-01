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
            Bitmap returnvalue = frames[currentIdx];
            if (currentIdx == frames.Length - 1)
            {
                currentIdx = 0;
            }
            else {
                currentIdx++;
            }
            return returnvalue;
        }

        public System.Drawing.Bitmap ReturnAndSetPreviousFrame()
        {
            Bitmap returnvalue = frames[currentIdx];
            if (currentIdx == 0)
            {
                currentIdx = frames.Length-1;
            }
            else
            {
                currentIdx--;
            }
            return returnvalue;
        }

        public System.Drawing.Bitmap GetCurrentFrame()
        {
            return frames[currentIdx];
        }

        public void Reset() {
            currentIdx = 0;
        }


    }
}
