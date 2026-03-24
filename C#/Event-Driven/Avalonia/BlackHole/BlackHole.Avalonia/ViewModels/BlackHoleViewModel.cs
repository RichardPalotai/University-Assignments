using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Avalonia.Threading;
using BlackHole.Model;
using BlackHole.Persistence;
using CommunityToolkit.Mvvm.Input;
using MsBox.Avalonia.Enums;

namespace BlackHole.Avalonia.ViewModels
{
	public class BlackHoleViewModel : ViewModelBase
	{
        #region Fields

        private BlackHoleGameModel _model;
        private string _mapSize = "";

        #endregion

        #region Properties

        public RelayCommand ApplyMapSizeCommand { get; private set; }
        public RelayCommand NewGameCommand { get; private set; }
        public RelayCommand LoadGameCommand { get; private set; }
        public RelayCommand SaveGameCommand { get; private set; }
        public RelayCommand ExitCommand { get; private set; }
        public ObservableCollection<BlackHoleField> Fields { get; set; }
        public BlackHoleField? FromSelectedField { get; set; }
        public BlackHoleField? ToSelectedField { get; set; }
        public BlackHoleMap.Field CurrentPlayer { get { return _model.CurrentPlayer; }  set { _model.CurrentPlayer = value; } }
        public String GameTime { get { return TimeSpan.FromSeconds(_model.GameTime).ToString("g"); } }
        public Int32 RedShipsIn { get { return _model.Map.RedShipsIn; } }
        public Int32 BlueShipsIn { get { return _model.Map.BlueShipsIn; } }
        public Int32 GetMapSize { get { return _model.Map.MapSize; } set { _model.Map.MapSize = value; } }
        public Int32 ShipsInToWin { get { return (_model.Map.MapSize - 1) / 2; } }
        public string MapSize
        {
            get => _mapSize;
            set
            {
                if(_mapSize != value)
                {
                    _mapSize = value;
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Events

        public event EventHandler? NewGame;
        public event EventHandler? LoadGame;
        public event EventHandler? SaveGame;
        public event EventHandler? ApplyMapSize;
        public event EventHandler? ExitGame;

        #endregion

        #region Constructors

        public BlackHoleViewModel(BlackHoleGameModel model)
        {
            _model = model;
            _model.FieldChanged += new EventHandler<BlackHoleFieldEventArgs>(Model_FieldChanged);
            _model.GameAdvanced += new EventHandler<BlackHoleEventArgs>(Model_GameAdvanced);
            _model.GameOver += new EventHandler<BlackHoleEventArgs>(Model_GameOver);
            _model.GameCreated += new EventHandler<BlackHoleEventArgs>(Model_GameCreated);

            NewGameCommand = new RelayCommand(OnNewGame);
            LoadGameCommand = new RelayCommand(OnLoadGame);
            SaveGameCommand = new RelayCommand(OnSaveGame);
            ExitCommand = new RelayCommand(OnExitGame);
            ApplyMapSizeCommand = new RelayCommand(OnApplyMapSize);

            Fields = new ObservableCollection<BlackHoleField>();
            StructureFields();
            foreach (BlackHoleField field in Fields)
            {
                field.FieldValue = !_model.IsFieldEmpty(field.X, field.Y) ? _model.GetFieldValue(field.X, field.Y) == BlackHoleMap.Field.RED ? BlackHoleMap.Field.RED : _model.GetFieldValue(field.X, field.Y) == BlackHoleMap.Field.BLUE ? BlackHoleMap.Field.BLUE : BlackHoleMap.Field.BLACKHOLE : BlackHoleMap.Field.EMPTY;
            }
        }

        #endregion

        #region Private methods

        private void RefreshMap()
        {
            OnPropertyChanged(nameof(GetMapSize));
            OnPropertyChanged(nameof(ShipsInToWin));
            Fields.Clear();
            StructureFields();

            foreach (BlackHoleField field in Fields)
            {
                field.FieldValue = !_model.IsFieldEmpty(field.X, field.Y) ? _model.GetFieldValue(field.X, field.Y) == BlackHoleMap.Field.RED ? BlackHoleMap.Field.RED : _model.GetFieldValue(field.X, field.Y) == BlackHoleMap.Field.BLUE ? BlackHoleMap.Field.BLUE : BlackHoleMap.Field.BLACKHOLE : BlackHoleMap.Field.EMPTY;
            }

            OnPropertyChanged(nameof(GameTime));
            OnPropertyChanged(nameof(CurrentPlayer));
            OnPropertyChanged(nameof(RedShipsIn));
            OnPropertyChanged(nameof(BlueShipsIn));
        }

        public void StepGame(Int32 x, Int32 y, BlackHoleMap.Directions direction)
        {
            _model.Step(x, y, direction);
            RefreshMap();
        }

        private (Boolean, BlackHoleMap.Directions) GetDirection(Int32 FromX, Int32 FromY, Int32 ToX, Int32 ToY)
        {
            if (ToX < FromX && FromY == ToY)
                return (true, BlackHoleMap.Directions.UP);

            if (ToX > FromX && FromY == ToY)
                return (true, BlackHoleMap.Directions.DOWN);

            if (ToY < FromY && FromX == ToX)
                return (true, BlackHoleMap.Directions.LEFT);

            if (ToY > FromY && FromX == ToX)
                return (true, BlackHoleMap.Directions.RIGHT);

            return (false, BlackHoleMap.Directions.RIGHT);
        }

        private void StructureFields()
        {
            for (Int32 i = 0; i < GetMapSize; i++)
            {
                for (Int32 j = 0; j < GetMapSize; j++)
                {
                    Fields.Add(new BlackHoleField
                    {
                        FieldValue = BlackHoleMap.Field.EMPTY,
                        X = i,
                        Y = j,
                        ClickCommand = new RelayCommand<Tuple<Int32, Int32>>(position =>
                        {
                            if (position != null)
                            {
                                BlackHoleField clickedField = Fields.FirstOrDefault(field => field.X == position.Item1 && field.Y == position.Item2)!;
                                if (clickedField != null && (Fields.All(field => !field.IsSelected) || Fields.Count(field => field.IsSelected) == 1) && clickedField.FieldValue == CurrentPlayer)
                                {
                                    if (FromSelectedField != null)
                                        FromSelectedField.IsSelected = false;
                                    FromSelectedField = clickedField;
                                    FromSelectedField.IsSelected = true;
                                }
                                else if (clickedField != null && Fields.Count(field => field.IsSelected) == 1 && (clickedField.FieldValue == BlackHoleMap.Field.EMPTY || clickedField.FieldValue == BlackHoleMap.Field.BLACKHOLE))
                                {
                                    ToSelectedField = clickedField;
                                    if (FromSelectedField != null && ToSelectedField != null)
                                    {
                                        var (isValid, direction) = GetDirection(FromSelectedField.X, FromSelectedField.Y, ToSelectedField.X, ToSelectedField.Y);
                                        if (isValid)
                                        {
                                            FromSelectedField.IsSelected = false;
                                            StepGame(FromSelectedField.X, FromSelectedField.Y, direction);
                                        }
                                    }
                                }
                            }
                        })
                    });
                }
            }
        }

        #endregion

        #region Game event handlers

        private void Model_FieldChanged(object? sender, BlackHoleFieldEventArgs e)
        {
            BlackHoleField field = Fields.Single(f => f.X == e.X && f.Y == e.Y);

            field.FieldValue = !_model.IsFieldEmpty(field.X, field.Y) ? _model.GetFieldValue(field.X, field.Y) == BlackHoleMap.Field.RED ? BlackHoleMap.Field.RED : _model.GetFieldValue(field.X, field.Y) == BlackHoleMap.Field.BLUE ? BlackHoleMap.Field.BLUE : BlackHoleMap.Field.BLACKHOLE : BlackHoleMap.Field.EMPTY;
            OnPropertyChanged(nameof(CurrentPlayer));
        }

        private void Model_GameOver(object? sender, BlackHoleEventArgs e)
        {

        }

        private void Model_GameAdvanced(object? sender, BlackHoleEventArgs e)
        {
            if (!Dispatcher.UIThread.CheckAccess()) // hamisat ad vissza, ha nem a dispatcher thread-en vagyunk
            {
                Dispatcher.UIThread.InvokeAsync(() => { Model_GameAdvanced(sender, e); });
                return;
            }

            OnPropertyChanged(nameof(GameTime));
        }

        private void Model_GameCreated(object? sender, BlackHoleEventArgs e)
        {
            RefreshMap();
        }

        #endregion

        #region Event methods

        private void OnNewGame()
        {
            NewGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnLoadGame()
        {
            LoadGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveGame()
        {
            SaveGame?.Invoke(this, EventArgs.Empty);
        }

        private void OnApplyMapSize()
        {
            ApplyMapSize?.Invoke(this, EventArgs.Empty);
        }

        private void OnExitGame()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}

