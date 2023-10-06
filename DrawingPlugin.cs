using System;
using System.Collections.Generic;
using Tekla.Structures.Drawing.Tools;
using Tekla.Structures.Plugins;
using TeklaDev;
using tsd = Tekla.Structures.Drawing;
using tsm = Tekla.Structures.Model;
using TD = Tekla.Structures.Datatype;
using System.Linq;

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
        private string _slabmarktype = MarkType.ASSEMBLY_POSITION_PROFILE.ToString();
        private string _slabprefix = MarkType.TEXT.ToString();
        private bool _slabprefixaction = true;
        private int _slabLocationIndex = 0;
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
                var viewBase = InputDefinitionFactory.GetView(inputs[0]) as tsd.ViewBase;
                var view = viewBase as tsd.View;
                if (view != null)
                {
                    //var beams = view.GetBeamsInDrawingAtView(cmodel, tsm.Beam.BeamTypeEnum.BEAM);
                    //var walls = view.GetBeamsInDrawingAtView(cmodel, tsm.Beam.BeamTypeEnum.PANEL);
                    var slabs = view.GetSlabsInDrawingAtView(cmodel, tsm.ContourPlate.ContourPlateTypeEnum.SLAB);
                    //beams.ForEach(beam =>
                    //{
                    //    beam.CreatePartMark(cmodel, viewBase, ExtDrawingPartMark.PointInsertMark.MiddlePart, MarkType.ASSEMBLY_POSITION_PROFILE);
                    //});
                    //walls.ForEach(wall =>
                    //{
                    //    wall.CreatePartMark(cmodel, viewBase, ExtDrawingPartMark.PointInsertMark.MiddlePart, MarkType.ASSEMBLY_POSITION_PROFILE);
                    //});
                    var typeMark = _slabmarktype.TransformTextToMarkType();
                    var prefix = _slabprefix;
                    var locationMark = _slabLocationIndex.TransformIntToLocationMark();
                    var extendMark = _slabExtendMark;
                    var angleMark = _slabAngleMark;

                    viewBase.CreatePartMark(cmodel, typeMark, prefix, extendMark, angleMark);
                    //slabs.ForEach(slab =>
                    //{
                    //    slab.CreatePartMark(cmodel, viewBase, locationMark, typeMark, prefix, extendMark, angleMark);
                    //});
                    cmodel.CommitChanges();
                }
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
            _slabLocationIndex = _data.slablocationindex;

            //slab
            if (IsDefaultValue(_slabmarktype))
                _slabmarktype = MarkType.ASSEMBLY_POSITION_PROFILE.ToString();
            if (IsDefaultValue(_slabprefix))
                _slabprefix = MarkType.TEXT.ToString();
            if (IsDefaultValue(_slabLocationIndex))
                _slabLocationIndex = 0;
            if (IsDefaultValue(_slabExtendMark))
                _slabExtendMark = 0.0;
            if (IsDefaultValue(_slabAngleMark))
                _slabAngleMark = 0.0;
        }
        #endregion
    }
}
