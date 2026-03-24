using System;
using System.Drawing;
using System.Windows.Forms;
using BlackHole.Model;
using BlackHole.Persistence;

namespace BlackHole.WinForms
{
    public partial class GameForm : Form
    {
        #region Fields

        private BlackHoleGameModel _model = null!;
        private Button[,] _buttonGrid = null!;
        private System.Windows.Forms.Timer _timer = null!;

        #endregion

        #region Constructors
        public GameForm()
        {
            InitializeComponent();

            IBlackHoleDataAccess _dataAccess = new BlackHoleFileDataAccess();

            this.KeyPreview = true;
            //this.KeyDown += new KeyEventHandler(ButtonGrid_KeyDown);
            _toolStripMapSizeTextBox.KeyDown += new KeyEventHandler(MenuSetMapSizeTextBox_KeyDown);
            
            _model = new BlackHoleGameModel(_dataAccess);
            _model.FieldChanged += new EventHandler<BlackHoleFieldEventArgs>(Game_FieldChanged);
            _model.GameAdvanced += new EventHandler<BlackHoleEventArgs>(Game_GameAdvanced);
            _model.GameOver += new EventHandler<BlackHoleEventArgs>(Game_GameOver);

            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 1000;
            _timer.Tick += new EventHandler(Timer_Tick);

            SetupMenus();

            _model.NewGame(SetMapSize());

            GenerateMap();

            SetupMap();

            SetWindowSize();

            _timer.Start();
        }
        #endregion

        #region Game event handlers

        private void Game_FieldChanged(Object? sender, BlackHoleFieldEventArgs e)
        {
            if (_model.Map.IsFieldEmpty(e.X, e.Y))
                _buttonGrid[e.X, e.Y].BackColor = Color.White;
            else if (_model.Map.GetFieldValue(e.X, e.Y) == BlackHoleMap.Field.BLACKHOLE)
                _buttonGrid[e.X, e.Y].BackColor = Color.Black;
            else
                _buttonGrid[e.X, e.Y].BackColor = _model.Map.GetFieldValue(e.X, e.Y) == BlackHoleMap.Field.RED ? Color.Red : Color.Blue;
        }

        private void Game_GameAdvanced(Object? sender, BlackHoleEventArgs e)
        {
            _toolLabelGameTime.Text = TimeSpan.FromSeconds(e.GameTime).ToString();
            _toolLabelGameTurn.Text = e.CurrentPlayer.ToString();
            _toolStripRedProgressBar.Value = _model.Map.RedShipsIn;
            _toolStripBlueProgressBar.Value = _model.Map.BlueShipsIn;
        }

        private void Game_GameOver(Object? sender, BlackHoleEventArgs e)
        {
            _timer.Stop();

            foreach (Button button in _buttonGrid)
                button.Enabled = false;

            _menuFileSaveGame.Enabled = false;

            if (e.IsWon)
            {
                string winner = _model.Map.RedShipsIn > _model.Map.BlueShipsIn ? BlackHoleMap.Field.RED.ToString() : BlackHoleMap.Field.BLUE.ToString();
                Int32 shipsIn = _model.Map.RedShipsIn > _model.Map.BlueShipsIn ? _model.Map.RedShipsIn : _model.Map.BlueShipsIn;
                MessageBox.Show("Congratulations! " + Environment.NewLine + winner +
                    " won with " + shipsIn + " ships in at " +
                    TimeSpan.FromSeconds(e.GameTime).ToString("g") + ".",
                    "Black Hole Game",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Asterisk);
            }
            else
            {
                MessageBox.Show("Sorry none of you won the game, because the time is up!",
                    "Black Hole Game",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Asterisk);
            }
        }

        #endregion

        #region Grid event handlers
        private void ButtonGrid_MouseClick(Object? sender, MouseEventArgs e)
        {
            if (sender is Button button)
            {
                Int32 x = (button.TabIndex - 100) / _model.Map.GetMapSize();
                Int32 y = (button.TabIndex - 100) % _model.Map.GetMapSize();

                if (_model.Map.GetFieldValue(x, y) == _model.CurrentPlayer)
                {
                    _model.SelectedX = x;
                    _model.SelectedY = y;
                    _buttonGrid[x, y].KeyDown += new KeyEventHandler(ButtonGrid_KeyDown);
                }
            }
        }

        private void ButtonGrid_KeyDown(Object? sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                    _model.Step(_model.SelectedX, _model.SelectedY, BlackHoleMap.Directions.UP);
                    break;
                case Keys.S:
                    _model.Step(_model.SelectedX, _model.SelectedY, BlackHoleMap.Directions.DOWN);
                    break;
                case Keys.A:
                    _model.Step(_model.SelectedX, _model.SelectedY, BlackHoleMap.Directions.LEFT);
                    break;
                case Keys.D:
                    _model.Step(_model.SelectedX, _model.SelectedY, BlackHoleMap.Directions.RIGHT);
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Menu event handlers

        private void MenuFileNewGame_Click(Object? sender, EventArgs e)
        {
            _menuFileSaveGame.Enabled = true;
            if (SetMapSize() % 2 != 0)
                ClearMap();
            try
            {
                _model.NewGame(SetMapSize());
                SetupMenus();
                GenerateMap();
                SetupMap();
                SetWindowSize();

                _timer.Start();
            }
            catch (ArgumentOutOfRangeException)
            {
                _timer.Stop();
                _toolStripMapSizeTextBox.Clear();
                MessageBox.Show("Not valid map size", "Black Hole Game", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void MenuFileLoadGame_Click(Object sender, EventArgs e)
        {
            Boolean restartTimer = _timer.Enabled;
            _timer.Stop();
            
            ClearMap();

            if (_openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await _model.LoadGameAsync(_openFileDialog.FileName);
                    _menuFileSaveGame.Enabled = true;
                }
                catch (BlackHoleDataException)
                {
                    MessageBox.Show("Could not load the game!" + Environment.NewLine + "Wrong file path or file format", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    _model.NewGame(SetMapSize());
                    _menuFileSaveGame.Enabled = true;
                }

                GenerateMap();
                SetupMap();
                SetupMenus();
                SetWindowSize();
            }

            if (restartTimer)
                _timer.Start();
        }
        private async void MenuFileSaveGame_Click(Object sender, EventArgs e)
        {
            Boolean restartTimer = _timer.Enabled;
            _timer.Stop();

            if (_saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    await _model.SaveGameAsync(_saveFileDialog.FileName);
                }
                catch (BlackHoleDataException)
                {
                    MessageBox.Show("Could not save the game!" + Environment.NewLine + "Wrong file path or file format", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if (restartTimer)
                _timer.Start();
        }
        private void MenuFileQuitGame_Click(Object sender, EventArgs e)
        {
            Boolean restartTimer = _timer.Enabled;
            _timer.Stop();

            // megkérdezzük, hogy biztos ki szeretne-e lépni
            if (MessageBox.Show("Do you want to exit the game?", "Black hole game", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // ha igennel válaszol
                Close();
            }
            else
            {
                if (restartTimer)
                    _timer.Start();
            }
        }

        private void MenuSetMapSize_Click(Object sender, EventArgs e)
        {

        }

        private void MenuSetMapSizeTextBox_KeyDown(Object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                MenuFileNewGame_Click(sender, e);
        }

        #endregion

        #region Timer event handlers

        private void Timer_Tick(Object? sender, EventArgs e)
        {
            _model.AdvanceTime();
        }

        #endregion

        #region Private methods

        private void GenerateMap()
        {
            _buttonGrid = new Button[_model.Map.GetMapSize(), _model.Map.GetMapSize()];
            for (Int32 i = 0; i < _model.Map.GetMapSize(); i++)
                for (Int32 j = 0; j < _model.Map.GetMapSize(); j++)
                {
                    _buttonGrid[i, j] = new Button();
                    _buttonGrid[i, j].Location = new Point(5 + 50 * j, 35 + 50 * i); // elhelyezkedés
                    _buttonGrid[i, j].Size = new Size(50, 50); // méret
                    _buttonGrid[i, j].Font = new Font(FontFamily.GenericSansSerif, 25, FontStyle.Bold); // bet?típus
                    _buttonGrid[i, j].Enabled = false; // kikapcsolt állapot
                    _buttonGrid[i, j].TabIndex = 100 + i * _model.Map.GetMapSize() + j; // a gomb számát a TabIndex-ben tároljuk
                    _buttonGrid[i, j].FlatStyle = FlatStyle.Flat; // lapított stípus
                    _buttonGrid[i, j].MouseClick += new MouseEventHandler(ButtonGrid_MouseClick);
                    // közös eseménykezel? hozzárendelése minden gombhoz

                    Controls.Add(_buttonGrid[i, j]);
                    // felvesszük az ablakra a gombot
                }
        }

        private void ClearMap()
        {
            for (int i = 0; i < _model.Map.GetMapSize(); i++)
            {
                for (int j = 0; j < _model.Map.GetMapSize(); j++)
                {
                    Controls.Remove(_buttonGrid[i, j]);
                }
            }
        }

        private void SetupMap()
        {
            for (Int32 i = 0; i < _buttonGrid.GetLength(0); i++)
            {
                for (Int32 j = 0; j < _buttonGrid.GetLength(1); j++)
                {
                    if (_model.Map.IsFieldEmpty(i, j))
                    {
                        _buttonGrid[i, j].BackColor = Color.White;
                        _buttonGrid[i, j].Enabled = true;

                    }
                    else if (_model.Map.IsFieldBlackHole(i, j))
                    {
                        _buttonGrid[i, j].BackColor = Color.Black;
                        _buttonGrid[i, j].Enabled = false;
                    }
                    else if (_model.Map.GetFieldValue(i, j) == BlackHoleMap.Field.RED)
                    {
                        _buttonGrid[i, j].BackColor = Color.Red;
                        _buttonGrid[i, j].Enabled = true;
                    }
                    else if (_model.Map.GetFieldValue(i, j) == BlackHoleMap.Field.BLUE)
                    {
                        _buttonGrid[i, j].BackColor = Color.Blue;
                        _buttonGrid[i, j].Enabled = true;
                    }
                }
            }

            _toolLabelGameTurn.Text = _model.CurrentPlayer.ToString();
            _toolLabelGameTime.Text = TimeSpan.FromSeconds(_model.GameTime).ToString("g");
            _toolStripRedProgressBar.Value = _model.Map.RedShipsIn;
            _toolStripBlueProgressBar.Value = _model.Map.BlueShipsIn;
        }

        private void SetupMenus()
        {
            _toolStripMapSizeTextBox.Text = _model.Map.GetMapSize().ToString();
            _toolStripRedProgressBar.Enabled = true;
            _toolStripBlueProgressBar.Enabled = true;
            _toolStripRedProgressBar.Minimum = 0;
            _toolStripRedProgressBar.Maximum = (_model.Map.GetMapSize() - 1) / 2;
            _toolStripBlueProgressBar.Minimum = 0;
            _toolStripBlueProgressBar.Maximum = (_model.Map.GetMapSize() - 1) / 2;
        }

        private Int32 SetMapSize()
        {
            Int32 size;
            if(!Int32.TryParse(_toolStripMapSizeTextBox.Text, out size))
            {
                _toolStripMapSizeTextBox.Clear();
            }
            return size;
        }

        private void SetWindowSize()
        {
            this.ClientSize = new Size(_buttonGrid.GetLength(1) * 60, _buttonGrid.GetLength(0) * 70);
        }

        #endregion
    }
}
