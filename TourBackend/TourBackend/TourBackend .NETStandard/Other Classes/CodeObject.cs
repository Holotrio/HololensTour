using System;
using System.Collections.Generic;
using System.Text;

namespace TourBackend
{
    public class CodeObject
    {

        public int id = -1;

        public bool isActive = true;
        public double[] position;
        public double[] rotation;
        public double[] scaling;

        public CodeObject(CodeObject codeobj)
        {
            if (codeobj != null)
            {
                position = new double[3];
                rotation = new double[9];

                id = codeobj.id;
                codeobj.position.CopyTo(position, 0);
                codeobj.rotation.CopyTo(rotation, 0);
                isActive = codeobj.isActive;
            }
        }

        public CodeObject()
        {
            isActive = true;
        }

        public CodeObject(int _objectid, double[] _position, double[] _rotation, bool _isActive)
        {
            id = _objectid;
            position = _position;
            rotation = _rotation;
            isActive = _isActive;
        }

        public CodeObject(int _objectid, double[] _position, double[] _rotation)
        {
            id = _objectid;
            position = _position;
            rotation = _rotation;
            isActive = true;
        }
    }
}
