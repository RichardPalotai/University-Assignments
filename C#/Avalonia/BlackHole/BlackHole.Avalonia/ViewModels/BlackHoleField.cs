using System;
using BlackHole.Persistence;
using CommunityToolkit.Mvvm.Input;

namespace BlackHole.Avalonia.ViewModels
{
	public class BlackHoleField : ViewModelBase
	{
		private BlackHoleMap.Field _fieldValue = BlackHoleMap.Field.EMPTY;

		public BlackHoleMap.Field FieldValue
		{
			get { return _fieldValue; }
			set
			{
				_fieldValue = value;
				OnPropertyChanged();
			}
		}

		public Boolean IsSelected { get; set; }
		public Int32 X { get; set; }
		public Int32 Y { get; set; }
		public Tuple<Int32, Int32> XY
		{
			get { return new(X, Y); }
		}
		public RelayCommand<Tuple<Int32, Int32>>? ClickCommand { get; set; }
	}
}

