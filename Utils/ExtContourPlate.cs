﻿using System.Linq;
using System.Collections.Generic;
using tsm = Tekla.Structures.Model;
using tsd = Tekla.Structures.Drawing;
using tsg = Tekla.Structures.Geometry3d;

namespace TeklaDev
{
    public static class ExtContourPlate
    {
        public static tsm.ContourPlate CreateContourBooleanPart(this List<tsg.Point> polygons, string profile = "PL200")
        {
            var contourPoints = polygons
                .Select(p => new tsm.ContourPoint(new tsg.Point(p.X, p.Y, p.Z), null));

            var cp = new tsm.ContourPlate();
            cp.Finish = "BooleanPart";
            cp.Profile.ProfileString = profile;
            cp.Material.MaterialString = "Steel_Undefined";
            cp.Class = tsm.BooleanPart.BooleanOperativeClassName;
            cp.Position.Depth = tsm.Position.DepthEnum.FRONT;
            bool result = false;
            result = cp.Insert();
            return cp;
        }
        public static List<tsm.ContourPlate> GetSlabsInModel(this tsm.Model cmodel, tsm.ContourPlate.ContourPlateTypeEnum contourType, string name = "", string prefix = "")
        {
            var objEnumerator = cmodel.GetModelObjectSelector()
                    .GetAllObjectsWithType(tsm.ModelObject.ModelObjectEnum.CONTOURPLATE);
            var contours = new List<tsm.ContourPlate>();
            foreach (var item in objEnumerator)
            {
                if (item is tsm.ContourPlate contourPlate)
                {
                    if (contourPlate.Type == contourType)
                    {
                        contours.Add(contourPlate);
                    }
                }
            }
            if (!string.IsNullOrEmpty(name))
                contours = contours.Where(x => x.Name == name).ToList();
            if (!string.IsNullOrEmpty(prefix))
                contours = contours.Where(x => x.Name == prefix).ToList();
            return contours;
        }
        public static List<tsd.Part> GetSlabsInDrawingAtView(this tsd.View view, tsm.Model cmodel, tsm.ContourPlate.ContourPlateTypeEnum contourType)
        {
            var results = new List<tsd.Part>();
            var enumPart = view.GetAllObjects(typeof(tsd.Part));
            while (enumPart.MoveNext())
            {
                var partInDrawing = enumPart.Current as tsd.Part;
                var idPart = partInDrawing.ModelIdentifier;
                var partInModel = cmodel.SelectModelObject(idPart);
                if (partInModel is tsm.ContourPlate contourPlate)
                {
                    if (contourPlate.Type == contourType)
                    {
                        results.Add(partInDrawing);
                    }
                }
            }
            return results;
        }
    }
}
