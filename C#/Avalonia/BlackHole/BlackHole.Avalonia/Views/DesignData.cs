using System;
using BlackHole.Model;
using BlackHole.Persistence;
using BlackHole.Avalonia.ViewModels;

namespace BlackHole.Avalonia.Views
{
    public static class DesignData
    {
        public static BlackHoleViewModel ViewModel
        {
            get
            {
                var model = new BlackHoleGameModel(new BlackHoleFileDataAccess(), new BlackHoleTimerInheritance());
                model.NewGame();
                model.PauseGame();
                return new BlackHoleViewModel(model);
            }
        }
    }
}

