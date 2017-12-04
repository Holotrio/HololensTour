using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proto;
using System.Drawing;

namespace TourBackend
{

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
