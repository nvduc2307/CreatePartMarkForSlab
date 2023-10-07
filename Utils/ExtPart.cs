using tsd = Tekla.Structures.Drawing;
using tsm = Tekla.Structures.Model;
namespace TeklaDev
{
    public static class ExtPart
    {
        public static tsd.Part GetDObjFormMObj(this tsm.Part mPart, tsd.ViewBase viewBase)
        {
            var dObjEnum = viewBase.GetModelObjects(mPart.Identifier);
            tsd.Part dObj = null;
            while (dObjEnum.MoveNext())
            {
                if (dObjEnum.Current != null)
                {
                    dObj = dObjEnum.Current as tsd.Part;
                }
            }
            return dObj;
        }
        public static tsm.Part GetMObjFormDObj(this tsd.Part dPart, tsm.Model cmodel)
        {
            var mObj = cmodel.SelectModelObject(dPart.ModelIdentifier) as tsm.Part;
            return mObj;
        }
    }
}
