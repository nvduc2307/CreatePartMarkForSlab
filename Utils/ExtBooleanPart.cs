using System.Linq;
using System.Collections.Generic;
using tsm = Tekla.Structures.Model;
using tsd = Tekla.Structures.Drawing;
using tsg = Tekla.Structures.Geometry3d;
using tss = Tekla.Structures.Solid;
using TeklaDev;
using Tekla.Structures.Model;
using static Tekla.Structures.ModelInternal.Operation;

namespace CreatePartMarkForSlab.Utils
{
    public static class ExtBooleanPart
    {
        public static List<tsm.BooleanPart> GetBooleanPartsInModel(
            this tsm.Model cmodel,
            tsm.BooleanPart.BooleanTypeEnum booleanTypeEnum,
            string promFilter = "slab")
        {
            var objEnumerator = cmodel.GetModelObjectSelector()
                    .GetAllObjects();
            var booleanParts = new List<tsm.BooleanPart>();
            foreach (var item in objEnumerator)
            {
                if (item is tsm.BooleanPart booleanPart)
                {
                    if (booleanPart.Type == booleanTypeEnum)
                    {
                        var father = booleanPart.Father as tsm.Part;
                        if (father is tsm.ContourPlate)
                        {
                            booleanParts.Add(booleanPart);
                        }
                        else if (father is tsm.Beam beam)
                        {
                            if (beam.Name.ToUpper().Contains(promFilter.ToUpper()))
                            {
                                booleanParts.Add(booleanPart);
                            }
                        }
                    }
                }
            }
            return booleanParts;
        }
        /// <summary>
        /// modelObjTobeCut is father BooleanPart
        /// modelObjCut is BooleanOperativeOBJ (cut)
        /// Shell là phần bị cắt
        /// GetPointOnTopShellFace() => số lượng phần tử chẵn
        /// </summary>
        /// <param name="modelObjTobeCut"></param>
        /// <param name="modelObjCut"></param>
        public static tsm.BooleanPart CreatePartCut(
            this tsm.Part modelObjTobeCut, 
            tsm.Part modelObjCut)
        {
            var booleanPart = new tsm.BooleanPart();
            booleanPart.Father = modelObjTobeCut;
            booleanPart.SetOperativePart(modelObjCut);
            booleanPart.Insert();
            modelObjCut.Delete();
            return booleanPart;
        }
        public static List<tsg.Point> GetPointsOfBooleanPart(this tsm.BooleanPart booleanPart)
        {
            var father = booleanPart.Father as tsm.Part;
            var solid1 = father.GetSolid(tsm.Solid.SolidCreationTypeEnum.RAW);
            var solid2 = father.GetSolid(tsm.Solid.SolidCreationTypeEnum.NORMAL);

            var shells = solid1.GetCutPart(solid2);

            var results = new List<tsg.Point>();
            while (shells.MoveNext())
            {
                var shell = shells.Current as tss.Shell;
                if (shell != null)
                {
                    var edges = shell.GetEdgeEnumerator();
                    while (edges.MoveNext())
                    {
                        if (edges.Current is tss.Edge edge)
                            results.Add(edge.StartPoint);
                    }
                }
            }
            return results;
        }
        public static List<tsg.Point> GetPointOnTopShellFace(
            this tsm.BooleanPart booleanPart, 
            tsm.Model cmodel, 
            out tsg.CoordinateSystem booleanPartCoordinateSystem)
        {
            booleanPartCoordinateSystem = booleanPart.GetCoordinateSystem();
            var savePlane = cmodel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
            cmodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new tsm.TransformationPlane(booleanPart.GetCoordinateSystem()));
            booleanPart.Select();

            var father = booleanPart.Father as tsm.Part;
            var solid1 = father.GetSolid(tsm.Solid.SolidCreationTypeEnum.RAW);
            var solid2 = father.GetSolid(tsm.Solid.SolidCreationTypeEnum.NORMAL);

            var shells = solid1.GetCutPart(solid2);
            tss.Face topFace;
            var results = new List<tsg.Point>();
            while (shells.MoveNext())
            {
                var shell = shells.Current as tss.Shell;
                if (shell != null)
                {
                    var faces = shell.GetFaceEnumerator();
                    while (faces.MoveNext())
                    {
                        if (faces.Current is tss.Face face)
                        {
                            var normal = face.Normal;
                            if (normal.Dot(new tsg.Vector(0, 0, 1)) > 0)
                            {
                                topFace = face;
                                var loops = face.GetLoopEnumerator();
                                while (loops.MoveNext())
                                {
                                    if (loops.Current is tss.Loop loop)
                                    {
                                        if (loop != null)
                                        {
                                            var Vertexs = loop.GetVertexEnumerator();
                                            while (Vertexs.MoveNext())
                                            {
                                                if (Vertexs.Current is tsg.Point point)
                                                {
                                                    if (point != null)
                                                    {
                                                        results.Add(point);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }
            cmodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(savePlane);
            return results;
        }
        public static List<tsg.Point> GeneratePointsOnTopShellFace(
            this tsm.BooleanPart booleanPart,
            tsm.Model cmodel,
            out tsg.CoordinateSystem booleanPartCoordinateSystem)
        {
            var PointsOnTopShellFace = booleanPart.GetPointOnTopShellFace(cmodel, out booleanPartCoordinateSystem);
            var results = new List<tsg.Point>();
            var count = PointsOnTopShellFace.Count;
            if (count % 2 == 0)
            {
                for (int i = 0; i < count - 1; i += 2)
                {
                    results.Add(PointsOnTopShellFace[i].MidPoint(PointsOnTopShellFace[i + 1]));
                }
            }
            return results;
        }
    }
}
