using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using Tekla.Structures.Geometry3d;
using t3d = Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.Operations;
using Tekla.Structures.Model.UI;
using Tekla.Structures.Plugins;
using DirectionFloorPlugin.Models;
using DirectionFloorPlugin.Utils;

namespace DirectionFloorPlugin
{
	[Plugin("DirectionFloorPlugin")]
	[PluginUserInterface("DirectionFloorPlugin.MainWindow")]
	public class DirectionFloorPlugin : PluginBase
	{
		private double _angle = 0.0;
		private int _directionIndex = 0;
		private int _typeIndex = 0;

		public Model Model { get; set; }
		public PluginData Data { get; set; }
		public DirectionFloorPlugin(PluginData data)
		{
			Model = new Model();
			Data = data;
		}

		public override List<InputDefinition> DefineInput()
		{
			List<InputDefinition> inputs = new List<InputDefinition>();

			Picker Picker = new Picker();
			var obj = Picker.PickObject(Picker.PickObjectEnum.PICK_ONE_OBJECT, "Pick object");
			inputs.Add(new InputDefinition(obj.Identifier));

			return inputs;
		}

		public override bool Run(List<InputDefinition> Input)
		{
			var result = false;

			try
			{
				GetValuesFromDialog();
				var id = Input[0].GetInput();
				var ob = Model.SelectModelObject(id as Tekla.Structures.Identifier);
				var floor = ob as ContourPlate;
				if (floor != null)
				{
					var floorInfor = new FloorInfo(floor);
					t3d.Vector direction = null;
					switch (_directionIndex)
					{
						case 0:
							direction = floorInfor.LocalShort;
							break;
						case 1:
							direction = floorInfor.LocalLong;
							break;
						case 2://global X
							direction = new t3d.Vector(1, 0, 0).ProjectToPlane(floorInfor.Plane);
							break;
						case 3://global y
							direction = new t3d.Vector(0, 1, 0).ProjectToPlane(floorInfor.Plane);
							break;
					}

					direction = direction.Rotate(floorInfor.Normal, (_angle / 180) * Math.PI);
					var profile = floorInfor.GetProfileDirection(direction, FloorInfo.LENGHT_SHORT_DIRECTION, FloorInfo.LENGHT_HEAD_DIRECTION);
					var controlLines = profile.InitControlLines();
					controlLines.ForEach(x => x.Insert());

					if (_typeIndex == 1) //add long direction
					{
						var directionLong = direction.Cross(floorInfor.Normal).GetNormal();
						var profileLong = floorInfor.GetProfileDirection(directionLong, FloorInfo.LENGHT_LONG_DIRECTION, FloorInfo.LENGHT_HEAD_DIRECTION);
						var controlLineLongs = profileLong.InitControlLines();
						controlLineLongs.ForEach(x => x.Insert());
					}

					result = true;
				}
				else
				{
					MessageBox.Show("Part must be ContourPlate");
				}
				Operation.DisplayPrompt("Selected components");
			}
			catch (Exception Exc)
			{
				MessageBox.Show(Exc.ToString());
			}

			return result;
		}

		private void GetValuesFromDialog()
		{
			_directionIndex = Data.DirectionIndex;
			_typeIndex = Data.TypeIndex;
			_angle = Data.Angle;

			if (IsDefaultValue(_directionIndex))
				_directionIndex = 0;
			if (IsDefaultValue(_typeIndex))
				_typeIndex = 0;
			if (IsDefaultValue(_angle))
				_angle = 0.0;
		}
	}
}
