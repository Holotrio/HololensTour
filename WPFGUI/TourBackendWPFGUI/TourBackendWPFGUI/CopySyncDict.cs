using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#if !UNITY_EDITOR && UNITY_METRO
using System.Threading.Tasks;
#endif

namespace TourBackend
{
    public static class CopySyncDict
    {
        public static Dictionary<string, CodeObject> Copy(Dictionary<string, CodeObject> _dict) {
            Dictionary<string, CodeObject> copy = new Dictionary<string, CodeObject>();

            foreach(KeyValuePair<string, CodeObject> p in _dict){
                copy.Add(p.Key, new CodeObject(p.Value));
            }

            return copy;
        }

        public static Dictionary<int, CodeObject> CopyInt(Dictionary<int, CodeObject> _dict)
        {
            Dictionary<int, CodeObject> copy = new Dictionary<int, CodeObject>();

            foreach (KeyValuePair<int, CodeObject> p in _dict)
            {
                copy.Add(p.Key, new CodeObject(p.Value));
            }

            return copy;
        }
    }
}
