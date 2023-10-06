using System.Collections.Generic;
using tsd = Tekla.Structures.Drawing;
using tsm = Tekla.Structures.Model;

namespace TeklaDev
{
    public static class ExtBeam
    {
        public static List<tsm.Beam> GetBeamsInModel(this tsm.Model cmodel, tsm.Beam.BeamTypeEnum beamType)
        {
            var objEnumerator = cmodel.GetModelObjectSelector()
                    .GetAllObjectsWithType(tsm.ModelObject.ModelObjectEnum.BEAM);
            var beams = new List<tsm.Beam>();
            foreach (var item in objEnumerator)
            {
                if (item is tsm.Beam beam)
                {
                    if (beam.Type == beamType) beams.Add(beam);
                }
            }
            return beams;
        }
        public static List<tsd.Part> GetBeamsInDrawingAtView(this tsd.View view, tsm.Model cmodel, tsm.Beam.BeamTypeEnum beamType)
        {
            var results = new List<tsd.Part>();
            var enumPart = view.GetAllObjects(typeof(tsd.Part));
            while (enumPart.MoveNext())
            {
                var partInDrawing = enumPart.Current as tsd.Part;
                var idPart = partInDrawing.ModelIdentifier;
                var partInModel = cmodel.SelectModelObject(idPart);
                if (partInModel is tsm.Beam beam)
                {
                    if (beam.Type == beamType)
                    {
                        results.Add(partInDrawing);
                    }
                }
            }
            return results;
        }
    }
}
