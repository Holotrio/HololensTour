using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using System.Drawing;
using Emgu.CV;

namespace TourBackend
{
    /// <summary>
    /// This message notifies either the ControlActor or the RecognitionManager that a new frame has arrived and is ready to be processed. [Internal Use]
    /// </summary>
    public class NewFrameArrived
    {
        public string id;
        public Mat frame;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_id">Message ID</param>
        /// <param name="_frame">Frame which arrived</param>
        public NewFrameArrived(string _id, Mat _frame)
        {
                id = _id;
                frame = _frame;
        }
    }

}
