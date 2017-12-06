using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using System.Drawing;

namespace TourBackend
{
    /// <summary>
    /// This message notifies either the ControlActor or the RecognitionManager that a new frame has arrived and is ready to be processed. [Internal Use]
    /// </summary>
    public class NewFrameArrived
    {
        public string id;
        public Bitmap bitmap;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_id">Message ID</param>
        /// <param name="_bitmap">Bitmap which arrived</param>
        public NewFrameArrived(string _id, Bitmap _bitmap)
        {
                id = _id;
                bitmap = _bitmap;
        }
    }

    /// <summary>
    /// with this message type we are able to respond to the command NewFrameArrived in the sense that
    /// the work with the frame was successfully done
    /// </summary>
    public class RespondNewFrameArrived
    {
        public string messageID;

        public RespondNewFrameArrived(string _messageID)
        {
            messageID = _messageID;
        }
    }
}
