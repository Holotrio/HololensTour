using System.Collections.Generic;
using TourBackend;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TourBackendUI
{
    public static class DeepCopy
    {
        /// <summary>
        /// Creates a deep copy of a Dictionary<int, CodeObject>
        /// </summary>
        /// <param name="dict"> Dictionary to be copied</param>
        /// <returns></returns>
        public static Dictionary<int, CodeObject> CopyInt(Dictionary<int, CodeObject> dict)
        {
            Dictionary<int, CodeObject> copy = new Dictionary<int, CodeObject>();

            foreach (KeyValuePair<int, CodeObject> p in dict)
            {
                copy.Add(p.Key, new CodeObject(p.Value));
            }

            return copy;
        }
    }


}
