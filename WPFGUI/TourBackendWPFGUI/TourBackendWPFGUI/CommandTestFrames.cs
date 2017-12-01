using System;
using System.Collections.Generic;
using System.Text;
//using Emgu.CV;
using System.Drawing;

namespace TourBackend
{
    public class CommandTestFrames
    {
        public Bitmap[] frames;
        public int currentIdx;
        public int length=0;

        public CommandTestFrames(IEnumerable<string> _frames)
        {
            length = System.Linq.Enumerable.Count(_frames);
            frames = new Bitmap[length];
            int i = 0;
            foreach (string item in _frames)
            {
                frames[i] = new Bitmap(item);
                i++;
            }
            //ToDo Fix currentIndex if you don't choose first item
            currentIdx = 0;
        }
       
        public CommandTestFrames(Bitmap[] _frames){
            frames = _frames;
            currentIdx = 0;
        }

        public Bitmap ReturnAndSetNextFrame()
        {
            currentIdx++;
            if (currentIdx >= length) currentIdx = 0;
            return frames[currentIdx];
        }

        public Bitmap ReturnAndSetPreviousFrame()
        {
            currentIdx--;
            if (currentIdx < 0) currentIdx = length-1;
            return frames[currentIdx];
        }

        public Bitmap GetCurrentFrame()
        {
            return frames[currentIdx];
        }

        public void Reset() {
            currentIdx = 0;
        }


    }
}
