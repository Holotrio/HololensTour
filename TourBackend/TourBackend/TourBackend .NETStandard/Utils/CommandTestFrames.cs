using System;
using System.Collections.Generic;
using System.Text;
using Emgu.CV;
using System.Drawing;

namespace TourBackend
{
    /// <summary>
    /// Used to easily breeze through an array of Bitmaps. [External Use]
    /// </summary>
    public class CommandTestFrames
    {
        public Mat[] frames;
        public int currentIdx;

        public CommandTestFrames(Mat[] _frames)
        {
            frames = _frames;
            currentIdx = 0;
        }

        /// <summary>
        /// Return the Bitmap at the current position in the array and set the current position to the next Bitmap. [External Use]
        /// </summary>
        /// <returns>Current Bitmap</returns>
        public Mat ReturnAndSetNextFrame()
        {
            Mat returnvalue = frames[currentIdx];
            if (currentIdx == frames.Length - 1)
            {
                currentIdx = 0;
            }
            else
            {
                currentIdx++;
            }
            return returnvalue;
        }

        /// <summary>
        /// Return the Bitmap at the current position in the array and set the current position to the previous Bitmap. [External Use]
        /// </summary>
        /// <returns>Current Bitmap</returns>
        public Mat ReturnAndSetPreviousFrame()
        {
            Mat returnvalue = frames[currentIdx];
            if (currentIdx == 0)
            {
                currentIdx = frames.Length - 1;
            }
            else
            {
                currentIdx--;
            }
            return returnvalue;
        }

        /// <summary>
        /// Return the Bitmap at the current position in the array and leave the position at the current Bitmap. [External Use]
        /// </summary>
        /// <returns>Current Bitmap</returns>
        public Mat GetCurrentFrame()
        {
            return frames[currentIdx];
        }

        /// <summary>
        /// Returns the position to the beginning of the array. [External Use]
        /// </summary>
        public void Reset()
        {
            currentIdx = 0;
        }


    }
}
