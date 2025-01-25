using System;

namespace BlackHole.Model
{
    public class BlackHoleEventArgs : EventArgs
    {
        private Int32 _gameTimer;
        private Int32 _redShips;
        private Int32 _blueShips;
        private Persistence.BlackHoleMap.Field _currentPlayer;
        private Boolean _isWon;

        public Int32 GameTime { get { return _gameTimer; } }
        public Int32 RedShipsGone { get { return _redShips; } }
        public Int32 BlueShipsGone { get { return _blueShips; } }
        public Persistence.BlackHoleMap.Field CurrentPlayer { get { return _currentPlayer; } }
        public Boolean IsWon { get { return _isWon; } }

        public BlackHoleEventArgs(Int32 gameTimer, Int32 redShips, Int32 blueShips, Persistence.BlackHoleMap.Field currentPlayer, Boolean isWon)
        {
            _gameTimer = gameTimer;
            _redShips = redShips;
            _blueShips = blueShips;
            _currentPlayer = currentPlayer;
            _isWon = isWon;
        }
    }
}
