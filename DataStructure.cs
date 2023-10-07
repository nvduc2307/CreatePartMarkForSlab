using Tekla.Structures.Plugins;
using TD = Tekla.Structures.Datatype;

namespace CreatePartMarkForSlab
{
    public class DataStructure
    {
        //config slab mark
        [StructuresField("slabmarktype")]
        public string slabmarktype;

        [StructuresField("slabprefix")]
        public string slabprefix;

        [StructuresField("slabextendmark")]
        public double slabextendmark;

        [StructuresField("slabanglemark")]
        public double slabanglemark;

        [StructuresField("isApplyFor")]
        public int isApplyFor;
    }
}
