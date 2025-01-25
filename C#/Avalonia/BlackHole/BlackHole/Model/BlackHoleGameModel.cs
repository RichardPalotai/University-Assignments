using System;
using System.IO;
using System.Threading.Tasks;
using BlackHole.Persistence;
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
        private ITimer _timer;

        #endregion

        #region Properties
        public Int32 MapSize => _map.MapSize;
        public BlackHoleMap.Field this[Int32 x, Int32 y] => _map[x, y];
        public Field CurrentPlayer { get { return _currentPlayer; } set { _currentPlayer = value; } }
        public Int32 RedShipsIn { get { return _map.RedShipsIn; } set { _map.RedShipsIn = value; } }
        public Int32 BlueShipsIn { get { return _map.BlueShipsIn; } set { _map.BlueShipsIn = value; } }
        public Int32 GameTime { get { return _gameTime; } }
        public Boolean IsGameOver { get { return (_gameTime == 0 || _map.RedShipsIn == (_map.MapSize - 1)/2 || _map.BlueShipsIn == (_map.MapSize - 1)/2); } }
        public BlackHoleMap Map { get { return _map; } }

        #endregion

        #region Events

        public event EventHandler<BlackHoleFieldEventArgs>? FieldChanged;
        public event EventHandler<BlackHoleEventArgs>? GameAdvanced;
        public event EventHandler<BlackHoleEventArgs>? GameOver;
        public event EventHandler<BlackHoleEventArgs>? GameCreated;

        #endregion

        #region Constructor
        public BlackHoleGameModel(IBlackHoleDataAccess dataAccess, ITimer timer)
        {
            _dataAccess = dataAccess;
            _map = new BlackHoleMap();

            _timer = timer;
            _timer.Interval = 1000;
            _timer.Elapsed += new EventHandler(Timer_Elapsed);
        }
        #endregion

        #region Public map accessoors

        public BlackHoleMap.Field GetFieldValue(Int32 x, Int32 y) => _map.GetFieldValue(x, y);
        public Boolean IsFieldEmpty(Int32 x, Int32 y) => _map.IsFieldEmpty(x, y);
        public Boolean IsFieldBlackHole(Int32 x, Int32 y) => _map.IsFieldBlackHole(x, y);

        #endregion

        #region Public game methods

        public void NewGame()
        {
            _map = new BlackHoleMap(MapSize);
            _currentPlayer = Field.BLUE;
            _map.RedShipsIn = 0;
            _map.BlueShipsIn = 0;
            _gameTime = 200 * _map.MapSize;
            GenerateFields();
            OnGameCreated();
        }

        public void PauseGame()
        {
            _timer.Stop();
        }

        public void ResumeGame()
        {
            if (!IsGameOver)
                _timer.Start();
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

            if (_map.RedShipsIn == (_map.MapSize-1)/2 || _map.BlueShipsIn == (_map.MapSize-1)/2)
                OnGameOver(true);
        }

        public async Task LoadGameAsync(String path)
        {
            await LoadGameAsync(File.OpenRead(path));
        }

        public async Task LoadGameAsync(Stream stream)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("Access to data is not provided.");
            
            _map = await _dataAccess.LoadAsync(stream);
            _currentPlayer = Field.BLUE;

            _gameTime = 200 * _map.MapSize;
            OnGameCreated();
        }

        public async Task SaveGameAsync(String path)
        {
            await SaveGameAsync(File.OpenWrite(path));
        }

        public async Task SaveGameAsync(Stream stream)
        {
            if (_dataAccess == null)
                throw new InvalidOperationException("Access to data is not provided.");
            
            await _dataAccess.SaveAsync(stream, _map);
        }

        #endregion

        #region Private game methods

        private void GenerateFields()
        {
            int mapSizeHalf = (int)Math.Ceiling(((double)_map.MapSize - 1) / 2);

            for (int i = 0; i < _map.MapSize; i++)
            {
                for (int j = 0; j < _map.MapSize; j++)
                {
                    _map.SetFieldValue(i, j, Field.EMPTY);
                }
            }

            for (int i = 0; i < _map.MapSize; i++)
            {
                for (int j = 0; j < _map.MapSize; j++)
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
            _timer.Stop();
            GameOver?.Invoke(this, new BlackHoleEventArgs(_gameTime, _map.RedShipsIn, _map.BlueShipsIn, _currentPlayer, isWon));
        }

        private void OnGameCreated()
        {
            _timer.Start();
            GameCreated?.Invoke(this, new BlackHoleEventArgs(_gameTime, _map.RedShipsIn, _map.BlueShipsIn, _currentPlayer, false));
        }

        #endregion

        #region Timer event handlers

        private void Timer_Elapsed(Object? sender, EventArgs e)
        {
            if (IsGameOver)
                return;

            _gameTime--;
            OnGameAdvanced();

            if (_gameTime == 0)
                OnGameOver(false);
        }

        #endregion
    }
}
