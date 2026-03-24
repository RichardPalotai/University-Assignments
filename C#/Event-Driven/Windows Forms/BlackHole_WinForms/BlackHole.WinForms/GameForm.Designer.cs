namespace BlackHole.WinForms
{
    partial class GameForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            _menuStrip = new MenuStrip();
            _menuFile = new ToolStripMenuItem();
            _menuFileNewGame = new ToolStripMenuItem();
            _menuFileSaveGame = new ToolStripMenuItem();
            _menuFileLoadGame = new ToolStripMenuItem();
            _menuFileQuitGame = new ToolStripMenuItem();
            _menuSettings = new ToolStripMenuItem();
            _menuSettingsSetMapSize = new ToolStripMenuItem();
            _toolStripMapSizeTextBox = new ToolStripTextBox();
            _statusStrip = new StatusStrip();
            _toolStripTurnLabel = new ToolStripStatusLabel();
            _toolLabelGameTurn = new ToolStripStatusLabel();
            _toolStripTimeLabel = new ToolStripStatusLabel();
            _toolLabelGameTime = new ToolStripStatusLabel();
            _toolStripRedLabel = new ToolStripStatusLabel();
            _toolStripRedProgressBar = new ToolStripProgressBar();
            _toolStripBlueLabel = new ToolStripStatusLabel();
            _toolStripBlueProgressBar = new ToolStripProgressBar();
            _openFileDialog = new OpenFileDialog();
            _saveFileDialog = new SaveFileDialog();
            _menuStrip.SuspendLayout();
            _statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // _menuStrip
            // 
            _menuStrip.ImageScalingSize = new Size(32, 32);
            _menuStrip.Items.AddRange(new ToolStripItem[] { _menuFile, _menuSettings });
            _menuStrip.Location = new Point(0, 0);
            _menuStrip.Name = "_menuStrip";
            _menuStrip.Size = new Size(986, 40);
            _menuStrip.TabIndex = 0;
            _menuStrip.Text = "Game Menu";
            // 
            // _menuFile
            // 
            _menuFile.DropDownItems.AddRange(new ToolStripItem[] { _menuFileNewGame, _menuFileSaveGame, _menuFileLoadGame, _menuFileQuitGame });
            _menuFile.Name = "_menuFile";
            _menuFile.Size = new Size(71, 36);
            _menuFile.Text = "File";
            // 
            // _menuFileNewGame
            // 
            _menuFileNewGame.Name = "_menuFileNewGame";
            _menuFileNewGame.Size = new Size(267, 44);
            _menuFileNewGame.Text = "New Game";
            _menuFileNewGame.Click += MenuFileNewGame_Click;
            // 
            // _menuFileSaveGame
            // 
            _menuFileSaveGame.Name = "_menuFileSaveGame";
            _menuFileSaveGame.Size = new Size(267, 44);
            _menuFileSaveGame.Text = "Save Game";
            _menuFileSaveGame.Click += MenuFileSaveGame_Click;
            // 
            // _menuFileLoadGame
            // 
            _menuFileLoadGame.Name = "_menuFileLoadGame";
            _menuFileLoadGame.Size = new Size(267, 44);
            _menuFileLoadGame.Text = "Load Game";
            _menuFileLoadGame.Click += MenuFileLoadGame_Click;
            // 
            // _menuFileQuitGame
            // 
            _menuFileQuitGame.Name = "_menuFileQuitGame";
            _menuFileQuitGame.Size = new Size(267, 44);
            _menuFileQuitGame.Text = "Quit Game";
            _menuFileQuitGame.Click += MenuFileQuitGame_Click;
            // 
            // _menuSettings
            // 
            _menuSettings.DropDownItems.AddRange(new ToolStripItem[] { _menuSettingsSetMapSize });
            _menuSettings.Name = "_menuSettings";
            _menuSettings.Size = new Size(120, 36);
            _menuSettings.Text = "Settings";
            // 
            // _menuSettingsSetMapSize
            // 
            _menuSettingsSetMapSize.DropDownItems.AddRange(new ToolStripItem[] { _toolStripMapSizeTextBox });
            _menuSettingsSetMapSize.Name = "_menuSettingsSetMapSize";
            _menuSettingsSetMapSize.Size = new Size(286, 44);
            _menuSettingsSetMapSize.Text = "Set Map Size";
            _menuSettingsSetMapSize.Click += MenuSetMapSize_Click;
            // 
            // _toolStripMapSizeTextBox
            // 
            _toolStripMapSizeTextBox.Name = "_toolStripMapSizeTextBox";
            _toolStripMapSizeTextBox.Size = new Size(100, 39);
            // 
            // _statusStrip
            // 
            _statusStrip.ImageScalingSize = new Size(32, 32);
            _statusStrip.Items.AddRange(new ToolStripItem[] { _toolStripTurnLabel, _toolLabelGameTurn, _toolStripTimeLabel, _toolLabelGameTime, _toolStripRedLabel, _toolStripRedProgressBar, _toolStripBlueLabel, _toolStripBlueProgressBar });
            _statusStrip.Location = new Point(0, 710);
            _statusStrip.Name = "_statusStrip";
            _statusStrip.Size = new Size(986, 42);
            _statusStrip.TabIndex = 1;
            _statusStrip.Text = "Game Status";
            // 
            // _toolStripTurnLabel
            // 
            _toolStripTurnLabel.Name = "_toolStripTurnLabel";
            _toolStripTurnLabel.Size = new Size(68, 32);
            _toolStripTurnLabel.Text = "Turn:";
            // 
            // _toolLabelGameTurn
            // 
            _toolLabelGameTurn.Name = "_toolLabelGameTurn";
            _toolLabelGameTurn.Size = new Size(56, 32);
            _toolLabelGameTurn.Text = "N/A";
            // 
            // _toolStripTimeLabel
            // 
            _toolStripTimeLabel.Name = "_toolStripTimeLabel";
            _toolStripTimeLabel.Size = new Size(72, 32);
            _toolStripTimeLabel.Text = "Time:";
            // 
            // _toolLabelGameTime
            // 
            _toolLabelGameTime.Name = "_toolLabelGameTime";
            _toolLabelGameTime.Size = new Size(27, 32);
            _toolLabelGameTime.Text = "0";
            // 
            // _toolStripRedLabel
            // 
            _toolStripRedLabel.Name = "_toolStripRedLabel";
            _toolStripRedLabel.Size = new Size(62, 32);
            _toolStripRedLabel.Text = "RED:";
            // 
            // _toolStripRedProgressBar
            // 
            _toolStripRedProgressBar.ForeColor = Color.Red;
            _toolStripRedProgressBar.Name = "_toolStripRedProgressBar";
            _toolStripRedProgressBar.Size = new Size(100, 30);
            _toolStripRedProgressBar.Style = ProgressBarStyle.Continuous;
            // 
            // _toolStripBlueLabel
            // 
            _toolStripBlueLabel.Name = "_toolStripBlueLabel";
            _toolStripBlueLabel.Size = new Size(72, 32);
            _toolStripBlueLabel.Text = "BLUE:";
            // 
            // _toolStripBlueProgressBar
            // 
            _toolStripBlueProgressBar.ForeColor = Color.Blue;
            _toolStripBlueProgressBar.Name = "_toolStripBlueProgressBar";
            _toolStripBlueProgressBar.Size = new Size(100, 30);
            _toolStripBlueProgressBar.Style = ProgressBarStyle.Continuous;
            // 
            // _openFileDialog
            // 
            _openFileDialog.FileName = "openFileDialog1";
            _openFileDialog.Filter = "Black Hole Game (*.bhg)|*.bhg";
            _openFileDialog.Title = "Load Black Hole Game";
            // 
            // _saveFileDialog
            // 
            _saveFileDialog.Filter = "Black Hole Game (*.bhg)|*.bhg";
            _saveFileDialog.Title = "Save Black Hole Game";
            // 
            // GameForm
            // 
            AutoScaleMode = AutoScaleMode.Inherit;
            ClientSize = new Size(986, 752);
            Controls.Add(_statusStrip);
            Controls.Add(_menuStrip);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MainMenuStrip = _menuStrip;
            MaximizeBox = false;
            MinimumSize = new Size(700, 310);
            Name = "GameForm";
            Text = "Black hole Game";
            _menuStrip.ResumeLayout(false);
            _menuStrip.PerformLayout();
            _statusStrip.ResumeLayout(false);
            _statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip _menuStrip;
        private ToolStripMenuItem _menuFile;
        private ToolStripMenuItem _menuFileNewGame;
        private ToolStripMenuItem _menuFileSaveGame;
        private ToolStripMenuItem _menuFileLoadGame;
        private ToolStripMenuItem _menuSettings;
        private ToolStripMenuItem _menuSettingsSetMapSize;
        private StatusStrip _statusStrip;
        private ToolStripMenuItem _menuFileQuitGame;
        private ToolStripStatusLabel _toolStripTurnLabel;
        private ToolStripStatusLabel _toolStripTimeLabel;
        private ToolStripStatusLabel _toolLabelGameTurn;
        private ToolStripStatusLabel _toolLabelGameTime;
        private ToolStripStatusLabel _toolStripRedLabel;
        private ToolStripProgressBar _toolStripRedProgressBar;
        private ToolStripStatusLabel _toolStripBlueLabel;
        private ToolStripProgressBar _toolStripBlueProgressBar;
        private OpenFileDialog _openFileDialog;
        private SaveFileDialog _saveFileDialog;
        private ToolStripTextBox _toolStripMapSizeTextBox;
    }
}
