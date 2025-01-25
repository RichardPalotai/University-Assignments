using System;

namespace BlackHole.Model
{
    public class BlackHoleFieldEventArgs : EventArgs
    {
        private Int32 _changedFieldX;
        private Int32 _changedFieldY;

        public Int32 X { get { return _changedFieldX; } }
        public Int32 Y { get { return _changedFieldY; } }

        public BlackHoleFieldEventArgs(Int32 x, Int32 y)
        {
            _changedFieldX = x;
            _changedFieldY = y;
        }
    }
}
