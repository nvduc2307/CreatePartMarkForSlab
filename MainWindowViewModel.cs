using System.Collections.Generic;
using System.Windows;
using Tekla.Structures.Dialog;
using TeklaDev;
using TD = Tekla.Structures.Datatype;

namespace CreatePartMarkForSlab
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields
        //config slab mark
        private string _slabmarktype;
        private string _slabprefix;
        private bool _slabprefixaction;
        private int _slabLocationIndex;
        private double _slabExtendMark;
        private double _slabAngleMark;
        #endregion

        #region Properties
        //config slab mark
        public List<string> Slabmarktypes
        {
            get => new List<string>()
            {
                MarkType.ASSEMBLY_POSITION.ToString(),
                MarkType.ASSEMBLY_POSITION_PROFILE.ToString(),
                MarkType.PROFILE.ToString(),
                MarkType.PANEL_NAME.ToString(),
                MarkType.TEXT.ToString(),
            };
        }

        [StructuresDialog("slabmarktype", typeof(TD.String))]
        public string Slabmarktype
        {
            get => _slabmarktype;
            set
            {
                _slabmarktype = value;
                if (string.IsNullOrEmpty(_slabmarktype))
                    _slabmarktype = MarkType.ASSEMBLY_POSITION_PROFILE.ToString();
                switch (_slabmarktype)
                {
                    case MarkSetting.TEXT:
                        Slabprefixaction = true;
                        break;
                    default:
                        Slabprefixaction = false;
                        break;
                }
                OnPropertyChanged();
            }
        }

        [StructuresDialog("slabprefix", typeof(TD.String))]
        public string Slabprefix
        {
            get => _slabprefix;
            set
            {
                _slabprefix = value;
                if (string.IsNullOrEmpty(_slabprefix))
                    _slabprefix = MarkType.TEXT.ToString();
                OnPropertyChanged();
            }
        }

        public bool Slabprefixaction
        {
            get => _slabprefixaction;
            set
            {
                _slabprefixaction = value;
                OnPropertyChanged();
            }
        }

        [StructuresDialog("slablocationindex", typeof(TD.Integer))]
        public int Slablocationindex
        {
            get { return _slabLocationIndex; }
            set
            {
                _slabLocationIndex = value;
                if (_slabLocationIndex.IsDefaultValue())
                    _slabLocationIndex = 1;
                OnPropertyChanged();
            }
        }

        [StructuresDialog("slabextendmark", typeof(TD.Double))]
        public double Slabextendmark
        {
            get { return _slabExtendMark; }
            set
            {
                _slabExtendMark = value;
                if (_slabExtendMark.IsDefaultValue())
                    _slabExtendMark = 0;
                OnPropertyChanged();
            }
        }

        [StructuresDialog("slabanglemark", typeof(TD.Double))]
        public double Slabanglemark
        {
            get { return _slabAngleMark; }
            set
            {
                _slabAngleMark = value;
                if (_slabAngleMark.IsDefaultValue())
                    _slabAngleMark = 0;
                OnPropertyChanged();
            }
        }
        #endregion
    }
}
