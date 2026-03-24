using BlackHole.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static BlackHole.Persistence.BlackHoleMap;

namespace BlackHole.Model
{
    public class BlackHoleGameModel
    {
        #region Fields

        private IBlackHoleDataAccess _dataAccess; // adatelérés
        private BlackHoleMap _map; // játéktábla
        private Field _currentPlayer;
        private Int32 _gameTime; // játékidő

        #endregion

        #region Properties
        public BlackHoleMap Map { get { return _map; } }
        public Field CurrentPlayer { get { return _currentPlayer; } set { _currentPlayer = value; } }
        public Int32 GameTime { get { return _gameTime; } }
        public Boolean IsGameOver { get { return (_gameTime == 0 || _map.RedShipsIn == (_map.GetMapSize() - 1)/2 || _map.BlueShipsIn == (_map.GetMapSize() - 1)/2); } }

        #endregion

        #region Events

        public event EventHandler<BlackHoleFieldEventArgs>? FieldChanged;
        public event EventHandler<BlackHoleEventArgs>? GameAdvanced;
        public event EventHandler<BlackHoleEventArgs>? GameOver;
        public event EventHandler<BlackHoleEventArgs>? GameCreated;

        #endregion

        #region Constructor
        public BlackHoleGameModel(IBlackHoleDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
            _map = new BlackHoleMap();
        }
        #endregion

        #region Public game methods

        public void NewGame(Int32 mapSize)
        {
            _map = new BlackHoleMap(mapSize);
            _currentPlayer = Field.BLUE;
            _map.RedShipsIn = 0;
            _map.BlueShipsIn = 0;
            _gameTime = 600 * _map.GetMapSize();
            GenerateFields();
            OnGameCreated();
        }

        public void NewGame()
        {
            _map = new BlackHoleMap();
            _currentPlayer = Field.BLUE;
            _map.RedShipsIn = 0;
            _map.BlueShipsIn = 0;
            _gameTime = 600 * _map.GetMapSize();
            GenerateFields();
            OnGameCreated();
        }

        public void AdvanceTime()
        {
            if (IsGameOver)
                return;

            _gameTime--;
            OnGameAdvanced();

            if (_gameTime == 0)
                OnGameOver(false);
        }

        public void Step(Int32 x, Int32 y, Directions direction)
        {
            if (IsGameOver)
                return;

            (Int32 nextX, Int32 nextY) = _map.StepFieldValue(x, y, direction);

            if (!_map.InBounds(nextX, nextY))
                return;

            OnFieldChanged(x, y);
            OnFieldChanged(nextX, nextY);

            _currentPlayer = _currentPlayer == Field.BLUE ? Field.RED : Field.BLUE;
            OnGameAdvanced();

            if (_map.RedShipsIn == (_map.GetMapSize()-1)/2 || _map.BlueShipsIn == (_map.GetMapSize()-1)/2)
                OnGameOver(true);
        }

        public async Task LoadGameAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("Access to data is not provided.");
            
            _map = await _dataAccess.LoadAsync(path);
            _currentPlayer = Field.BLUE;

            _gameTime = 600 * _map.GetMapSize();
            OnGameCreated();
        }

        public async Task SaveGameAsync(String path)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("Access to data is not provided.");
            
            await _dataAccess.SaveAsync(path, _map);
        }

        #endregion

        #region Private game methods

        private void GenerateFields()
        {
            int mapSizeHalf = (int)Math.Ceiling(((double)_map.GetMapSize() - 1) / 2);

            for (int i = 0; i < _map.GetMapSize(); i++)
            {
                for (int j = 0; j < _map.GetMapSize(); j++)
                {
                    _map.SetFieldValue(i, j, Field.EMPTY);
                }
            }

            for (int i = 0; i < _map.GetMapSize(); i++)
            {
                for (int j = 0; j < _map.GetMapSize(); j++)
                {
                    if (mapSizeHalf == i && mapSizeHalf == j)
                    {
                        _map.SetFieldValue(i, j, Field.BLACKHOLE);
                    }
                    else if (i == j && i < mapSizeHalf)
                    {
                        _map.SetFieldValue(i, j, Field.RED);
                        _map.SetFieldValue(i, j + 2*(mapSizeHalf - i), Field.RED);
                    }
                    else if (i == j && i > mapSizeHalf)
                    {
                        _map.SetFieldValue(i, j, Field.BLUE);
                        _map.SetFieldValue(i, j - 2*(i - mapSizeHalf), Field.BLUE);
                    }
                }
            }
        }

        #endregion

        #region Private event methods

        private void OnFieldChanged(Int32 x, Int32 y)
        {
            FieldChanged?.Invoke(this, new BlackHoleFieldEventArgs(x, y));
        }

        private void OnGameAdvanced()
        {
            GameAdvanced?.Invoke(this, new BlackHoleEventArgs(_gameTime, _map.RedShipsIn, _map.BlueShipsIn, _currentPlayer, false));
        }

        private void OnGameOver(Boolean isWon)
        {
            GameOver?.Invoke(this, new BlackHoleEventArgs(_gameTime, _map.RedShipsIn, _map.BlueShipsIn, _currentPlayer, isWon));
        }

        private void OnGameCreated()
        {
            GameCreated?.Invoke(this, new BlackHoleEventArgs(_gameTime, _map.RedShipsIn, _map.BlueShipsIn, _currentPlayer, false));
        }

        #endregion
    }
}
