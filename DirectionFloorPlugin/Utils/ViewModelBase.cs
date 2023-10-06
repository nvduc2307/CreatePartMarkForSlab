using System.ComponentModel;
using System.Runtime.CompilerServices;
using Tekla.Structures.Internal;

namespace DirectionFloorPlugin.Utils
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        //For Tekla
		public bool IsDefaultValue(double Value)
		{
			return Value == (double)StructuresDataStorage.DEFAULT_VALUE;
		}
		public bool IsDefaultValue(int Value)
		{
			return Value == StructuresDataStorage.DEFAULT_VALUE;
		}
	}
}
