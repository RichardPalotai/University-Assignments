using System;
using static BlackHole.Persistence.BlackHoleMap;

namespace BlackHole.Persistence
{
    public class BlackHoleMap
    {
        #region Fields
        public enum Directions { UP, DOWN, LEFT, RIGHT }
        public enum Field { RED, BLUE, BLACKHOLE, EMPTY }
        private Int32 _mapSize;
        private Field[,] _fieldValues;
        private Int32 _redShipsIn;
        private Int32 _blueShipsIn;
        #endregion

        #region Properties
        public Int32 GetMapSize() { return _mapSize; }
        public Int32 RedShipsIn { get { return _redShipsIn; } set { _redShipsIn = value; } }
        public Int32 BlueShipsIn { get { return _blueShipsIn; } set { _blueShipsIn = value; } }
        public Field this[Int32 x, Int32 y] { get { return GetFieldValue(x, y); } }
        #endregion

        #region Constructors
        public BlackHoleMap() : this(3) { }
        public BlackHoleMap(Int32 mapSize)
        {
            if (mapSize < 3)
                throw new ArgumentOutOfRangeException(nameof(mapSize) + " is less than the minimal map size (at least 3)");
            if (mapSize % 2 == 0)
                throw new ArgumentOutOfRangeException(nameof(mapSize) + " is not odd, only odd map sizes are allowed");
            _mapSize = mapSize;
            _fieldValues = new Field[mapSize,mapSize];
        }
        #endregion

        #region Public methods

        public Field GetFieldValue(Int32 x, Int32 y)
        {
            if (x < 0 || x >= _fieldValues.GetLength(0))
                throw new ArgumentOutOfRangeException(x + " : The X coordinate is out of range");
            if (y < 0 || y >= _fieldValues.GetLength(1))
                throw new ArgumentOutOfRangeException(y + " : The Y coordinate is out of range");

            return _fieldValues[x, y];
        }

        public void SetFieldValue(Int32 x, Int32 y, Field value)
        {
            if (x < 0 || x >= _fieldValues.GetLength(0))
                throw new ArgumentOutOfRangeException(nameof(x) + " : The X coordinate is out of range");
            if (y < 0 || y >= _fieldValues.GetLength(1))
                throw new ArgumentOutOfRangeException(nameof(y) + " : The Y coordinate is out of range");
            if (GetFieldValue(x, y) == Field.BLACKHOLE)
                throw new ArgumentException(nameof(Field.BLACKHOLE) + " : The black hole field can not be changed");
            // CheckStep() függvény kell ide????

            _fieldValues[x, y] = value;
        }

        public (Int32, Int32) StepFieldValue(Int32 x, Int32 y, Directions direction)
        {
            Int32 shipX = x;
            Int32 shipY = y;
            Int32 stepX = 0;
            Int32 stepY = 0;

            switch (direction)
            {
                case Directions.UP:
                    stepX = -1; stepY = 0;
                    break;
                case Directions.DOWN:
                    stepX = 1; stepY = 0;
                    break;
                case Directions.LEFT:
                    stepX = 0; stepY = -1;
                    break;
                case Directions.RIGHT:
                    stepX = 0; stepY = 1;
                    break;
                default:
                    break;
            }

            while (InBounds(x + stepX, y + stepY) && !IsFieldShip(x + stepX, y + stepY))
            {
                if (CheckForBlackHole(x + stepX, y + stepY, shipX, shipY))
                {
                    x += stepX;
                    y += stepY;
                    break;
                }
                else
                {
                    x += stepX;
                    y += stepY;
                }

            }
            if (!InBounds(x + stepX, y + stepY))
                return (x + stepX, y + stepY);
            if (!IsFieldBlackHole(x, y))
            {
                SetFieldValue(x, y, GetFieldValue(shipX, shipY));
                SetFieldValue(shipX, shipY, Field.EMPTY);
            }

            return (x, y);
        }

        public Boolean IsFieldEmpty(Int32 x, Int32 y)
        {
            return GetFieldValue(x, y) == Field.EMPTY;
        }

        public Boolean IsFieldBlackHole(Int32 x, Int32 y)
        {
            return GetFieldValue(x, y) == Field.BLACKHOLE;
        }
        public Boolean InBounds(Int32 x, Int32 y)
        {
            return x >= 0 && x < _mapSize && y >= 0 && y < _mapSize;
        }

        #endregion

        #region Private methods

        private Boolean IsFieldShip(Int32 x, Int32 y)
        {
            Field[] shipTypes = { Field.RED, Field.BLUE };
            return shipTypes.Contains(GetFieldValue(x, y));
        }

        private Boolean CheckForBlackHole(Int32 x, Int32 y, Int32 shipX, Int32 shipY)
        {
            if (IsFieldBlackHole(x, y))
            {
                switch (GetFieldValue(shipX, shipY))
                {
                    case Field.RED:
                        _redShipsIn++;
                        break;
                    case Field.BLUE:
                        _blueShipsIn++;
                        break;
                    default:
                        break;
                }
                SetFieldValue(shipX, shipY, Field.EMPTY);
                return true;
            }

            return false;
        }

        #endregion
    }
}
