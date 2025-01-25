using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BlackHole.Model;
using BlackHole.Persistence;
using static BlackHole.Persistence.BlackHoleMap;

namespace BlackHole.ViewModel
{
    public class BlackHoleViewModel : ViewModelBase
    {
        #region Fields

        private BlackHoleGameModel _model;

        #endregion

        #region Properties
        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand LoadGameCommand { get; private set; }
        public DelegateCommand SaveGameCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }
        public ObservableCollection<BlackHoleField> Fields { get; set; }
        public BlackHoleField? SelectedField { get; set; }
        public BlackHoleMap.Field CurrentPlayer { get { return _model.CurrentPlayer; } set { _model.CurrentPlayer = value; } }
        public String GameTime { get { return TimeSpan.FromSeconds(_model.GameTime).ToString("g"); } }
        public Int32 RedShipsIn { get { return _model.Map.RedShipsIn; } }
        public Int32 BlueShipsIn { get { return _model.Map.BlueShipsIn; } }
        public Int32 GetMapSize { get { return _model.Map.GetMapSize(); } }
        public Int32 ShipsInToWin { get { return (_model.Map.GetMapSize() - 1) / 2; } }
        #endregion

        #region Events
        public event EventHandler? NewGame;
        public event EventHandler? LoadGame;
        public event EventHandler? SaveGame;
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

            NewGameCommand = new DelegateCommand(param => OnNewGame());
            LoadGameCommand = new DelegateCommand(param => OnLoadGame());
            SaveGameCommand = new DelegateCommand(param => OnSaveGame());
            ExitCommand = new DelegateCommand(param => OnExitGame());

            Fields = new ObservableCollection<BlackHoleField>();
            StructureFields();
            foreach (BlackHoleField field in Fields)
            {
                field.FieldValue = !_model.Map.IsFieldEmpty(field.X, field.Y) ? _model.Map.GetFieldValue(field.X, field.Y) == BlackHoleMap.Field.RED ? BlackHoleMap.Field.RED : _model.Map.GetFieldValue(field.X, field.Y) == BlackHoleMap.Field.BLUE ? BlackHoleMap.Field.BLUE : BlackHoleMap.Field.BLACKHOLE : BlackHoleMap.Field.EMPTY;
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
                field.FieldValue = !_model.Map.IsFieldEmpty(field.X, field.Y) ? _model.Map.GetFieldValue(field.X, field.Y) == BlackHoleMap.Field.RED ? BlackHoleMap.Field.RED : _model.Map.GetFieldValue(field.X, field.Y) == BlackHoleMap.Field.BLUE ? BlackHoleMap.Field.BLUE : BlackHoleMap.Field.BLACKHOLE : BlackHoleMap.Field.EMPTY;
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
                        ClickCommand = new DelegateCommand(param =>
                        {
                            if (param is Tuple<Int32, Int32> position)
                            {
                                BlackHoleField clickedField = Fields.FirstOrDefault(field => field.X == position.Item1 && field.Y == position.Item2)!;
                                if (clickedField != null && Fields.All(field => !field.IsSelected) && clickedField.FieldValue == CurrentPlayer)
                                {
                                    SelectedField = clickedField;
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

            field.FieldValue = !_model.Map.IsFieldEmpty(field.X, field.Y) ? _model.Map.GetFieldValue(field.X, field.Y) == BlackHoleMap.Field.RED ? BlackHoleMap.Field.RED : _model.Map.GetFieldValue(field.X, field.Y) == BlackHoleMap.Field.BLUE ? BlackHoleMap.Field.BLUE : BlackHoleMap.Field.BLACKHOLE : BlackHoleMap.Field.EMPTY;
            OnPropertyChanged(nameof(CurrentPlayer));
        }

        private void Model_GameOver(object? sender, BlackHoleEventArgs e)
        {
            
        }

        private void Model_GameAdvanced(object? sender, BlackHoleEventArgs e)
        {
            OnPropertyChanged(nameof(GameTime));
            OnPropertyChanged(nameof(RedShipsIn));
            OnPropertyChanged(nameof(BlueShipsIn));
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
        private void OnExitGame()
        {
            ExitGame?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
