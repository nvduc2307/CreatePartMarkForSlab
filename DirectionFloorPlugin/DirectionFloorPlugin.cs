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
using System.Windows.Controls;

namespace DirectionFloorPlugin
{
    [Plugin("Slab Direction")]
    [PluginUserInterface("DirectionFloorPlugin.MainWindow")]
    public class DirectionFloorPlugin : PluginBase
    {
        private const string UDA_SLABNAME = "PANEL NAME";

        private double _angle = 0.0;
        private int _directionIndex = 0;
        private int _typeIndex = 0;
        private string _slabName = "";


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
            var obj = Picker.PickObject(Picker.PickObjectEnum.PICK_ONE_OBJECT, "Pick slab");
            inputs.Add(new InputDefinition(obj.Identifier));

            return inputs;
        }

        public override bool Run(List<InputDefinition> Input)
        {
            try
            {
                GetValuesFromDialog();

                var id = Input[0].GetInput();
                var ob = Model.SelectModelObject(id as Tekla.Structures.Identifier);
                if (!FloorInfo.ModelObjectIsValid(ob))
                {
                    MessageBox.Show("Part must be ContourPlate/Beam/Panel");
                    return false;
                }
                var floorInfor = new FloorInfo(ob as Part);
                t3d.Vector directionShort = null;
                switch (_directionIndex)
                {
                    case 0:
                        directionShort = floorInfor.LocalShort;
                        break;
                    case 1:
                        directionShort = floorInfor.LocalLong;
                        break;
                    case 2://Global X
                        directionShort = TeklaVectorExtension.GlobalX.RayToPlane(TeklaVectorExtension.GlobalZ, floorInfor.Plane);
                        break;
                    case 3://Global Y
                        directionShort = TeklaVectorExtension.GlobalY.RayToPlane(TeklaVectorExtension.GlobalZ, floorInfor.Plane);
                        break;
                }

                directionShort = directionShort.Rotate(floorInfor.Normal, (_angle / 180) * Math.PI);
                var profile = floorInfor.GetProfileDirection(directionShort, FloorInfo.LENGHT_SHORT_DIRECTION);
                var floor1 = profile.InitContourPalte();
                floor1.Insert();
                var partcut1 = floorInfor.MainModelObject.InitPartCut(floor1);
                partcut1.Insert();
                floor1.Delete();
                var resultSetUDA = floorInfor.MainModelObject.SetUserProperty(UDA_SLABNAME, _slabName);

                if (_typeIndex == 1) //Add long direction
                {
                    var directionLong = directionShort.Cross(floorInfor.Normal).GetNormal();
                    var profileLong = floorInfor.GetProfileDirection(directionLong, FloorInfo.LENGHT_LONG_DIRECTION);
                    var floor2 = profileLong.InitContourPalte();
                    floor2.Insert();
                    var partcut2 = floorInfor.MainModelObject.InitPartCut(floor2);
                    partcut2.Insert();
                    floor2.Delete();
                }

                Operation.DisplayPrompt("Selected components");
                return true;
            }
            catch (Exception Exc)
            {
                MessageBox.Show(Exc.ToString());
                return false;
            }
        }

        private void GetValuesFromDialog()
        {
            _directionIndex = Data.DirectionIndex;
            if (IsDefaultValue(_directionIndex))
                _directionIndex = 0;

            _typeIndex = Data.TypeIndex;
            if (IsDefaultValue(_typeIndex))
                _typeIndex = 0;

            _angle = Data.Angle;
            if (IsDefaultValue(_angle))
                _angle = 0.0;

            _slabName = Data.SlabName;
            if (IsDefaultValue(_slabName))
                _slabName = "";
        }
    }
}
