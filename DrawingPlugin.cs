using CreatePartMarkForSlab.Utils;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing.Tools;
using Tekla.Structures.Plugins;
using TeklaDev;
using tsd = Tekla.Structures.Drawing;
using tsg = Tekla.Structures.Geometry3d;
using tsm = Tekla.Structures.Model;
using tsdui = Tekla.Structures.Drawing.UI;

namespace CreatePartMarkForSlab
{
    [Plugin("Slab Mark Symbol")]
    [PluginUserInterface("CreatePartMarkForSlab.MainWindow")]
    public class CreatePartMarkForSlab : DrawingPluginBase
    {
        #region Fields
        private tsm.Model _cmodel;
        private tsd.DrawingHandler _dHandler;
        private DataStructure _data;

        //config slab mark
        private string _slabmarktype = MarkSetting.PROFILE;
        private string _slabprefix = MarkSetting.TEXT;
        private double _slabExtendMark = 0.0;
        private double _slabAngleMark = 0.0;
        private int _isApplyFor = 0;

        #endregion

        #region Properties
        private tsm.Model Cmodel
        {
            get => _cmodel;
        }
        private tsd.DrawingHandler DHandle
        {
            get => _dHandler;
        }
        #endregion

        #region Constructor
        public CreatePartMarkForSlab(DataStructure data)
        {
            _cmodel = new tsm.Model();
            _dHandler = new tsd.DrawingHandler();
            _data = data;
        }
        #endregion

        #region Overrides
        public override List<InputDefinition> DefineInput()
        {
            try
            {
                var dHandle = new tsd.DrawingHandler();
                var pick = dHandle.GetPicker();
                var tuple = pick.PickObject("Pick Slab In View Action");
                var objectdrawing = tuple.Item1;

                while (!objectdrawing.GetType().Equals(typeof(tsd.Part)))
                {
                    tuple = pick.PickObject("Pick Slab In View Action");
                    objectdrawing = tuple.Item1;
                }


                while (objectdrawing.GetType().Equals(typeof(tsd.Part)))
                {
                    var dObj = objectdrawing as tsd.Part;
                    var mObj = dObj.GetMObjFormDObj(new tsm.Model());
                    if (mObj.GetType() != typeof(tsm.ContourPlate))
                    {
                        tuple = pick.PickObject("Pick Slab In View Action");
                        objectdrawing = tuple.Item1;
                    }
                    else
                    {
                        break;
                    }
                }
                return new List<InputDefinition>
                {
                    InputDefinitionFactory.CreateInputDefinition(tuple)
                };
            }
            catch (Exception)
            {
                return new List<InputDefinition>();
            }
            
        }

        public override bool Run(List<InputDefinition> inputs)
        {
            try
            {
                GetValuesFromDialog();
                var cmodel = new tsm.Model();

                var savePlane = cmodel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
                cmodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new tsm.TransformationPlane());

                var viewBase = InputDefinitionFactory.GetView(inputs[0]);
                if (viewBase == null) return false;

                var dObj = InputDefinitionFactory.GetDrawingObject(inputs[0]) as tsd.Part;
                var typeMark = _slabmarktype.TransformTextToMarkType();
                var prefix = _slabprefix;
                var extendMark = _slabExtendMark;
                var angleMark = _slabAngleMark;

                var booleanParts = new List<tsm.BooleanPart>();
                if (_isApplyFor == 0)
                {
                    var mObj = dObj.GetMObjFormDObj(cmodel);
                    booleanParts = mObj.GetBooleanPartsInModel();
                }
                else
                {
                    booleanParts = cmodel.GetBooleanPartsInModel(tsm.BooleanPart.BooleanTypeEnum.BOOLEAN_CUT);
                }
                var booleanPartsCount = booleanParts.Count;
                if (booleanPartsCount > 0)
                {
                    booleanParts.ForEach(booleanPart =>
                    {
                        var modelPart = booleanPart.Father as tsm.Part;
                        var drawingObjEnum = viewBase.GetModelObjects(modelPart.Identifier);
                        tsd.Part drawingModelObj = null;
                        while (drawingObjEnum.MoveNext())
                        {
                            if (drawingObjEnum.Current != null)
                            {
                                drawingModelObj = drawingObjEnum.Current as tsd.Part;
                            }
                        }
                        tsg.CoordinateSystem pointsCoordinate = null;
                        var points = ExtBooleanPart.GetPointOnTopShellFace(cmodel, booleanPart, out pointsCoordinate);
                        var pointsDrawing = points.TransformPointsInModelToViewDrawing(pointsCoordinate, cmodel, viewBase as tsd.View);

                        tsg.Point p_mark1, p_mark2;
                        p_mark1 = pointsDrawing.Get2PHasDistanceMax(out p_mark2);
                        drawingModelObj?.CreatePartMark(p_mark1, p_mark2, typeMark, prefix, extendMark, angleMark);
                    });
                }
                cmodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(savePlane);
                return true;
            }
            catch (Exception)
            {
                //MessageBox.Show(Exc.ToString());
                return false;
            }

        }
        #endregion

        #region Private methods
        private void GetValuesFromDialog()
        {
            _slabmarktype = _data.slabmarktype;
            _slabprefix = _data.slabprefix;
            _slabExtendMark = _data.slabextendmark;
            _slabAngleMark = _data.slabanglemark;
            _isApplyFor = _data.isApplyFor;

            //slab
            if (IsDefaultValue(_slabmarktype))
                _slabmarktype = MarkSetting.PROFILE;
            if (IsDefaultValue(_slabprefix))
                _slabprefix = MarkSetting.TEXT;
            if (IsDefaultValue(_slabExtendMark))
                _slabExtendMark = 0.0;
            if (IsDefaultValue(_slabAngleMark))
                _slabAngleMark = 0.0;
            if (IsDefaultValue(_isApplyFor))
                _isApplyFor = 0;
        }
        #endregion
    }
}
