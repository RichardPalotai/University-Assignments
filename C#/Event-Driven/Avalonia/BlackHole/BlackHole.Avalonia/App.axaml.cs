using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using BlackHole.Model;
using BlackHole.Persistence;
using BlackHole.Avalonia.ViewModels;
using BlackHole.Avalonia.Views;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.ComponentModel;
using System.IO;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Avalonia.Platform;

namespace BlackHole.Avalonia;

public partial class App : Application
{
    #region Fields

    private BlackHoleGameModel _model = null!;
    private BlackHoleViewModel _viewModel = null!;

    #endregion

    #region Properties

    private TopLevel? TopLevel
    {
        get
        {
            return ApplicationLifetime switch
            {
                IClassicDesktopStyleApplicationLifetime desktop => TopLevel.GetTopLevel(desktop.MainWindow),
                ISingleViewApplicationLifetime singleViewPlatform => TopLevel.GetTopLevel(singleViewPlatform.MainView),
                _ => null
            };
        }
    }

    #endregion

    #region Application methods

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        BindingPlugins.DataValidators.RemoveAt(0);

        _model = new BlackHoleGameModel(new BlackHoleFileDataAccess(), new BlackHoleTimerInheritance());
        _model.GameOver += new EventHandler<BlackHoleEventArgs>(Model_GameOver);
        _model.NewGame();

        _viewModel = new BlackHoleViewModel(_model);
        _viewModel.NewGame += new EventHandler(ViewModel_NewGame);
        _viewModel.LoadGame += new EventHandler(ViewModel_LoadGame);
        _viewModel.SaveGame += new EventHandler(ViewModel_SaveGame);
        _viewModel.ApplyMapSize += new EventHandler(ViewModel_SetMapSize);

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = _viewModel
            };

            desktop.Startup += async (s, e) =>
            {
                _model.NewGame();

                try
                {
                    await _model.LoadGameAsync(
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SudokuSuspendedGame"));
                }
                catch { }
            };

            desktop.Exit += async (s, e) =>
            {
                try
                {
                    await _model.SaveGameAsync(
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SudokuSuspendedGame"));
                }
                catch { }
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = _viewModel
            };

            if (Application.Current?.TryGetFeature<IActivatableLifetime>() is { } activatableLifetime)
            {
                activatableLifetime.Activated += async (sender, args) =>
                {
                    if (args.Kind == ActivationKind.Background)
                    {
                        try
                        {
                            await _model.LoadGameAsync(
                                Path.Combine(AppContext.BaseDirectory, "SuspendedGame"));
                        }
                        catch
                        {
                        }
                    }
                };
                activatableLifetime.Deactivated += async (sender, args) =>
                {
                    if (args.Kind == ActivationKind.Background)
                    {
                        try
                        {
                            await _model.SaveGameAsync(
                                Path.Combine(AppContext.BaseDirectory, "SuspendedGame"));
                        }
                        catch
                        {
                        }
                    }
                };
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    #endregion

    #region ViewModel event handlers

    private void ViewModel_NewGame(object? sender, EventArgs e)
    {
        _model.NewGame();
    }

    private async void ViewModel_LoadGame(object? sender, System.EventArgs e)
    {
        if (TopLevel == null)
        {
            await MessageBoxManager.GetMessageBoxStandard(
                    "BlackHole Game",
                    "File not supported!",
                    ButtonEnum.Ok, Icon.Error)
                .ShowAsync();
            return;
        }

        Boolean restartTimer = !_model.IsGameOver;
        _model.PauseGame();

        try
        {
            var files = await TopLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Loading BlackHole Game",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("BlackHole Map")
                    {
                        Patterns = new[] { "*.bhg" }
                    }
                }
            });

            if (files.Count > 0)
            {
                using (var stream = await files[0].OpenReadAsync())
                {
                    await _model.LoadGameAsync(stream);
                }
            }
        }
        catch (BlackHoleDataException)
        {
            await MessageBoxManager.GetMessageBoxStandard(
                    "BlackHole Game",
                    "File couldn't be loaded!",
                    ButtonEnum.Ok, Icon.Error)
                .ShowAsync();
        }

        if (restartTimer)
            _model.ResumeGame();
    }

    private async void ViewModel_SaveGame(object? sender, EventArgs e)
    {
        if (TopLevel == null)
        {
            await MessageBoxManager.GetMessageBoxStandard(
                    "BlackHole Game",
                    "File not supported!",
                    ButtonEnum.Ok, Icon.Error)
                .ShowAsync();
            return;
        }

        Boolean restartTimer = !_model.IsGameOver;
        _model.PauseGame();

        try
        {
            var file = await TopLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions()
            {
                Title = "Saving BlackHole Game",
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("BlackHole Map")
                    {
                        Patterns = new[] { "*.bhg" }
                    }
                }
            });

            if (file != null)
            {
                using (var stream = await file.OpenWriteAsync())
                {
                    await _model.SaveGameAsync(stream);
                }
            }
        }
        catch (Exception ex)
        {
            await MessageBoxManager.GetMessageBoxStandard(
                    "BlackHole Game",
                    "File couldn't be saved!" + ex.Message,
                    ButtonEnum.Ok, Icon.Error)
                .ShowAsync();
        }

        if (restartTimer)
            _model.ResumeGame();
    }

    private async void ViewModel_SetMapSize(object? sender, EventArgs e)
    {
        if (int.TryParse(_viewModel.MapSize, out int size) && size % 2 != 0)
        {
            _viewModel.GetMapSize = size;
            _model.NewGame();
        }
        else
        {
            await MessageBoxManager.GetMessageBoxStandard(
                    "BlackHole Game",
                    "Not valid map size!",
                    ButtonEnum.Ok, Icon.Error)
                .ShowAsync();
            return;
        }
    }

    #endregion

    #region Model event handlers

    private async void Model_GameOver(object? sender, BlackHoleEventArgs e)
    {
        if (e.IsWon)
        {
            string winner = _model.Map.RedShipsIn > _model.Map.BlueShipsIn ? BlackHoleMap.Field.RED.ToString() : BlackHoleMap.Field.BLUE.ToString();
            Int32 shipsIn = _model.Map.RedShipsIn > _model.Map.BlueShipsIn ? _model.Map.RedShipsIn : _model.Map.BlueShipsIn;
            await MessageBoxManager.GetMessageBoxStandard(
                            "Black Hole Game",
                            "Congratulations! " + Environment.NewLine +
                            winner + " won with " + shipsIn + " ships in at " +
                            TimeSpan.FromSeconds(e.GameTime).ToString("g") + ".",
                            ButtonEnum.Ok, Icon.Info)
                        .ShowAsync();
        }
        else
        {
            await MessageBoxManager.GetMessageBoxStandard(
                            "Black Hole Game",
                            "Sorry none of you won the game, because the time is up!",
                            ButtonEnum.Ok, Icon.Info)
                        .ShowAsync();
        }
    }

    #endregion
}
