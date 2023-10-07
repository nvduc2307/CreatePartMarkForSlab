using CreatePartMarkForSlab.Utils;
using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing.Tools;
using Tekla.Structures.Plugins;
using TeklaDev;
using tsd = Tekla.Structures.Drawing;
using tsg = Tekla.Structures.Geometry3d;
using tsm = Tekla.Structures.Model;

namespace CreatePartMarkForSlab
{
    [Plugin("CreatePartMarkForSlab")]
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
                var tuple = pick.PickObject("Pick Part In View Action");
                var objectdrawing = tuple.Item1;
                while (!objectdrawing.GetType().Equals(typeof(tsd.Part)))
                {
                    tuple = pick.PickObject("Pick Part In View Action");
                    objectdrawing = tuple.Item1;
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

                var viewBase = InputDefinitionFactory.GetView(inputs[0]) as tsd.ViewBase;
                var view = viewBase as tsd.View;
                if (view != null)
                {
                    var typeMark = _slabmarktype.TransformTextToMarkType();
                    var prefix = _slabprefix;
                    var extendMark = _slabExtendMark;
                    var angleMark = _slabAngleMark;

                    var booleanParts = cmodel.GetBooleanPartsInModel(tsm.BooleanPart.BooleanTypeEnum.BOOLEAN_CUT);
                    var booleanPartsCount = booleanParts.Count;
                    if (booleanPartsCount > 0)
                    {
                        booleanParts.ForEach(booleanPart =>
                        {
                            var modelPark = booleanPart.Father as tsm.Part;
                            var drawingObjEnum = viewBase.GetModelObjects(modelPark.Identifier);
                            tsd.Part drawingModel = null;
                            while (drawingObjEnum.MoveNext()) 
                            {
                                if (drawingObjEnum.Current != null)
                                {
                                    drawingModel = drawingObjEnum.Current as tsd.Part;
                                }
                            }
                            tsg.CoordinateSystem pointsCoordinate = null;
                            var points = booleanPart.GetPointOnTopShellFace(cmodel, out pointsCoordinate);
                            var pointsDrawing = points.TransformPointsInModelToViewDrawing(pointsCoordinate, cmodel, viewBase as tsd.View);
                            if (pointsDrawing.Count == 6)
                            {
                                var p_mark1 = pointsDrawing[2];
                                var p_mark2 = pointsDrawing[3];
                                drawingModel?.CreatePartMark(booleanPart, p_mark1, p_mark2, typeMark, prefix, extendMark, angleMark);
                            }
                            else
                            {
                                var p_mark1 = pointsDrawing[13];
                                var p_mark2 = pointsDrawing[7];
                                drawingModel?.CreatePartMark(booleanPart, p_mark1, p_mark2, typeMark, prefix, extendMark, angleMark);
                            }
                        });
                    }
                }
                cmodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(savePlane);
            }
            catch (Exception)
            {
                //MessageBox.Show(Exc.ToString());
            }

            return true;
        }
        #endregion

        #region Private methods
        private void GetValuesFromDialog()
        {
            _slabmarktype = _data.slabmarktype;
            _slabprefix = _data.slabprefix;
            _slabExtendMark = _data.slabextendmark;
            _slabAngleMark = _data.slabanglemark;

            //slab
            if (IsDefaultValue(_slabmarktype))
                _slabmarktype = MarkSetting.PROFILE;
            if (IsDefaultValue(_slabprefix))
                _slabprefix = MarkSetting.TEXT;
            if (IsDefaultValue(_slabExtendMark))
                _slabExtendMark = 0.0;
            if (IsDefaultValue(_slabAngleMark))
                _slabAngleMark = 0.0;
        }
        #endregion
    }
}
