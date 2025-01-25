using BlackHole.Model;
using BlackHole.Persistence;
using BlackHole.View;
using BlackHole.ViewModel;
using Microsoft.Win32;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using static BlackHole.Persistence.BlackHoleMap;

namespace BlackHole
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Fields

        private BlackHoleGameModel _model = null!;
        private BlackHoleViewModel _viewModel = null!;
        private MainWindow _view = null!;
        private DispatcherTimer _timer = null!;

        #endregion

        #region Constructors

        public App()
        {
            Startup += new StartupEventHandler(App_Startup);
        }

        #endregion

        #region Application event handlers

        private void App_Startup(object? sender, StartupEventArgs e)
        {
            _model = new BlackHoleGameModel(new BlackHoleFileDataAccess());
            _model.GameOver += new EventHandler<BlackHoleEventArgs>(Model_GameOver);
            _model.NewGame();

            _viewModel = new BlackHoleViewModel(_model);
            _viewModel.NewGame += new EventHandler(ViewModel_NewGame);
            _viewModel.ExitGame += new EventHandler(ViewModel_ExitGame);
            _viewModel.LoadGame += new EventHandler(ViewModel_LoadGame);
            _viewModel.SaveGame += new EventHandler(ViewModel_SaveGame);

            _view = new MainWindow();
            _view.DataContext = _viewModel;
            _view.MapSizeTextBox.KeyDown += new KeyEventHandler(View_MapSizeTextBox);
            _view.KeyDown += new KeyEventHandler(View_KeyDown);
            _view.Closing += new System.ComponentModel.CancelEventHandler(View_Closing); // eseménykezelés a bezáráshoz
            _view.Show();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += new EventHandler(Timer_Tick);
            _timer.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            _model.AdvanceTime();
        }

        #endregion

        #region View event handlers

        private void View_Closing(object? sender, CancelEventArgs e)
        {
            Boolean restartTimer = _timer.IsEnabled;

            _timer.Stop();

            if (MessageBox.Show("Biztos, hogy ki akar lépni?", "Black Hole", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            {
                e.Cancel = true;

                if (restartTimer)
                    _timer.Start();
            }
        }

        private void View_MapSizeTextBox(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ViewModel_NewGame(sender, e);
        }

        private void View_KeyDown(object? sender, KeyEventArgs e)
        {
            if (_viewModel.SelectedField != null)
            {
                BlackHoleField field = _viewModel.SelectedField;
                switch (e.Key)
                {
                    case Key.W:
                        _viewModel.StepGame(field.X, field.Y, BlackHoleMap.Directions.UP);
                        break;
                    case Key.A:
                        _viewModel.StepGame(field.X, field.Y, BlackHoleMap.Directions.LEFT);
                        break;
                    case Key.S:
                        _viewModel.StepGame(field.X, field.Y, BlackHoleMap.Directions.DOWN);
                        break;
                    case Key.D:
                        _viewModel.StepGame(field.X, field.Y, BlackHoleMap.Directions.RIGHT);
                        break;
                    default:
                        break;
                }
                _viewModel.SelectedField = null;
            }
        }

        #endregion

        #region ViewModel event handlers

        private void ViewModel_NewGame(object? sender, EventArgs e)
        {
            if (SetMapSize().Item1)
            {
                _model.NewGame(SetMapSize().Item2);
                _timer.Start();
            }
            else
            {
                MessageBox.Show("Not valid map size", "Black Hole Game", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private async void ViewModel_LoadGame(object? sender, EventArgs e)
        {
            Boolean restartTimer = _timer.IsEnabled;

            _timer.Stop();

            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Black Hole tábla betöltése";
                openFileDialog.Filter = "Black Hole tábla|*.bhg";
                if (openFileDialog.ShowDialog() == true)
                {
                    await _model.LoadGameAsync(openFileDialog.FileName);

                    _timer.Start();
                }
            }
            catch (BlackHoleDataException)
            {
                MessageBox.Show("Could not load the game!" + Environment.NewLine + "Wrong file path or file format", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (restartTimer)
                _timer.Start();
        }

        private async void ViewModel_SaveGame(object? sender, EventArgs e)
        {
            Boolean restartTimer = _timer.IsEnabled;

            _timer.Stop();

            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Black Hole tábla betöltése";
                saveFileDialog.Filter = "Black Hole tábla|*.bhg";
                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        await _model.SaveGameAsync(saveFileDialog.FileName);
                    }
                    catch (BlackHoleDataException)
                    {
                        MessageBox.Show("Could not save the game!" + Environment.NewLine + "Wrong file path or file format", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (BlackHoleDataException)
            {
                MessageBox.Show("Could not save the game!", "Black Hole", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (restartTimer)
                _timer.Start();
        }

        private void ViewModel_ExitGame(object? sender, EventArgs e)
        {
            _view.Close();
        }

        #endregion

        #region Model event handlers

        private void Model_GameOver(object? sender, BlackHoleEventArgs e)
        {
            _timer.Stop();

            if (e.IsWon)
            {
                string winner = _model.Map.RedShipsIn > _model.Map.BlueShipsIn ? BlackHoleMap.Field.RED.ToString() : BlackHoleMap.Field.BLUE.ToString();
                Int32 shipsIn = _model.Map.RedShipsIn > _model.Map.BlueShipsIn ? _model.Map.RedShipsIn : _model.Map.BlueShipsIn;
                MessageBox.Show("Congratulations! " + Environment.NewLine + winner +
                                " won with " + shipsIn + " ships in at " +
                                TimeSpan.FromSeconds(e.GameTime).ToString("g") + ".",
                                "Black Hole Game",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
            }
            else
            {
                MessageBox.Show("Sorry none of you won the game, because the time is up!",
                                "Black Hole Game",
                                MessageBoxButton.OK,
                                MessageBoxImage.Asterisk);
            }
        }

        #endregion

        #region Private methods

        private (Boolean, Int32) SetMapSize()
        {
            Int32 size;
            if (!Int32.TryParse(_view.MapSizeTextBox.Text, out size))
            {
                _view.MapSizeTextBox.Clear();
                return (false, 0);
            }
            else if (size % 2 == 0)
            {
                _view.MapSizeTextBox.Clear();
                return (false, size);
            }
            return (true, size);
        }

        #endregion
    }

}