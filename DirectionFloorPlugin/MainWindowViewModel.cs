using td = Tekla.Structures.Datatype;
using System.ComponentModel;
using Tekla.Structures.Dialog;
using DirectionFloorPlugin.Utils;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;

namespace DirectionFloorPlugin
{
	public class MainWindowViewModel : ViewModelBase
	{
		private double _angle = 0;
		[StructuresDialog("angle", typeof(td.Double))]
		public double Angle
		{
			get { return _angle; }
			set { _angle = value; OnPropertyChanged(); }
		}

		private int _directionIndex = 0;
		[StructuresDialog("direction", typeof(td.Integer))]
		public int DirectionIndex
		{
			get { return _directionIndex; }
			set { _directionIndex = value; OnPropertyChanged(); }
		}

		private int _typeIndex = 0;
		[StructuresDialog("type", typeof(td.Integer))]
		public int TypeIndex
		{
			get { return _typeIndex; }
			set { _typeIndex = value; OnPropertyChanged(); }
		}

		private string _slabName = "";
		[StructuresDialog("slabname", typeof(td.String))]
		public string SlabName
		{
			get { return _slabName; }
			set { _slabName = value; OnPropertyChanged(); }
		}

	}
}
