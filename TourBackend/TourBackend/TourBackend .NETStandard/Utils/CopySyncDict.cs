using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourBackend
{
    /// <summary>
    /// Used to create a deep copy of a CodeObject, since Dictionary does not implement ICloneable. [External Use]
    /// </summary>
    public static class CopySyncDict
    {
        /// <summary>
        /// Creates a deep copy of the given Dictionary. [External Use]
        /// </summary>
        /// <param name="_dict">Dictionary to be copied</param>
        /// <returns>Deep copy of the given Dictionay</returns>
        public static Dictionary<int, CodeObject> Copy(Dictionary<int, CodeObject> _dict) {
            //return _dict.ToDictionary(x=>x.Key, x=>new CodeObject(x.Value));
            Dictionary<int, CodeObject> copy = new Dictionary<int, CodeObject>();


            foreach(KeyValuePair<int, CodeObject> p in _dict){
                    copy.Add(p.Key, new CodeObject(p.Value));
            }

            return copy;
        }
    }
}
