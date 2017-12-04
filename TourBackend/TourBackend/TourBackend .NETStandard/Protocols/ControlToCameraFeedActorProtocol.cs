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

        public NewFrameArrived(string _id, Bitmap _bitmap)
        {
                id = _id;
                bitmap = _bitmap;
        }
    }

}
