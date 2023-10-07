using System;
using System.Collections.Generic;
using System.Linq;
using tsm = Tekla.Structures.Model;
using tsg = Tekla.Structures.Geometry3d;
using tsd = Tekla.Structures.Drawing;

namespace TeklaDev
{
    public static class ExtPoint
    {
        public static tsg.Point MidPoint(this tsg.Point p1, tsg.Point p2)
        {
            var minPoint = p1;
            var maxPoint = p2;
            var midPoint = new tsg.Point((maxPoint.X + minPoint.X) * 0.5, (maxPoint.Y + minPoint.Y) * 0.5, (maxPoint.Z + minPoint.Z) * 0.5);
            return midPoint;
        }
        public static double DistancePToP(this tsg.Point p1, tsg.Point p2)
        {
            return Math.Sqrt(Math.Pow((p2.X - p1.X), 2) + Math.Pow((p2.Y - p1.Y), 2) + Math.Pow((p2.Z - p1.Z), 2));
        }
        public static tsg.Point Rotate(this tsg.Point p1, tsg.Point centerRotate, double angleDeg)
        {
            angleDeg = angleDeg * Math.PI / 180;
            var vt = p1 - centerRotate;
            var newPoint = new tsg.Point(
                vt.X * Math.Cos(angleDeg) - vt.Y * Math.Sin(angleDeg),
                vt.X * Math.Sin(angleDeg) + vt.Y * Math.Cos(angleDeg),
                p1.Z);
            return newPoint;
        }
        public static tsg.Point Tranform(this tsg.Point tranformPoint, tsg.Vector vectorTranform)
        {
            return tranformPoint + vectorTranform;
        }
        public static List<tsg.Point> TransformCoordinate(
            this List<tsg.Point> points, 
            tsg.CoordinateSystem pointsCoordinateSystem,
            tsm.TransformationPlane currentPlane)
        {
            var pointsTransformationPlane = new tsm.TransformationPlane(pointsCoordinateSystem);
            var results = points
                .Select(p=> currentPlane.TransformationMatrixToGlobal.Transform(p))
                .ToList();
            return results;
        }
        public static List<tsg.Point> TransformPointsCoordinateToCurrentPlane(
            this List<tsg.Point> points,
            tsg.CoordinateSystem pointsCoordinateSystem,
            tsm.TransformationPlane currentPlane)
        {
            var pointsTransformationPlane = new tsm.TransformationPlane(pointsCoordinateSystem);
            var results = points
                .Select(p => currentPlane.TransformationMatrixToLocal.Transform(pointsTransformationPlane.TransformationMatrixToGlobal.Transform(p)))
                .ToList();
            return results;
        }
        public static List<tsg.Point> TransformPointsInModelToViewDrawing(
            this List<tsg.Point> points,
            tsg.CoordinateSystem pointsCoordinateSystem,
            tsm.Model cmodel,
            tsd.View partView)
        {
            var savePlane = cmodel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
            cmodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new tsm.TransformationPlane());

            var pointsTransformationPlane = new tsm.TransformationPlane(pointsCoordinateSystem);
            var pointsGlobal = points
                .Select(p => pointsTransformationPlane.TransformationMatrixToGlobal.Transform(p))
                .Select(p => new tsg.Point(p.X, p.Y, 0))
                .ToList();
            var convMatrix = tsg.MatrixFactory.ToCoordinateSystem(partView.DisplayCoordinateSystem);
            var pointsDrawing = pointsGlobal
                .Select(p => convMatrix.Transform(p))
                .ToList();

            cmodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(savePlane);
            return pointsDrawing;
        }
        public static tsg.Point Get2PHasDistanceMax(this List<tsg.Point> polygons, out tsg.Point pResult2)
        {
            //polygons la tap hop cac diem khep kin
            var lines = new List<tsg.LineSegment>();
            var pointsCount = polygons.Count;
            for (int i = 0; i < pointsCount - 1; i++)
            {
                lines.Add(new tsg.LineSegment(polygons[i], polygons[i + 1]));
            }
            var lineResult = lines
                .OrderBy(x=>x.Length())
                .LastOrDefault();
            pResult2 = lineResult.Point2;
            return lineResult.Point1;
        }
    }
}
