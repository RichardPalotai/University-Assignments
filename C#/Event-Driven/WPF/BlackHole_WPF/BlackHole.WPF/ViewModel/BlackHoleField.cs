using System;
using System.Windows.Input;
using BlackHole.Persistence;

namespace BlackHole.ViewModel
{
    public class BlackHoleField : ViewModelBase
    {
        private BlackHoleMap.Field _fieldValue = BlackHoleMap.Field.EMPTY;

        public BlackHoleMap.Field FieldValue
        {
            get { return _fieldValue; }
            set
            {
                if (_fieldValue != value)
                {
                    _fieldValue = value;
                    OnPropertyChanged();
                }
            }
        }

        public Boolean IsSelected { get; set; }
        public Int32 X { get; set; }
        public Int32 Y { get; set; }
        public Tuple<Int32, Int32> XY
        {
            get { return new(X, Y); }
        }
        public DelegateCommand? ClickCommand { get; set; }
    }
}
