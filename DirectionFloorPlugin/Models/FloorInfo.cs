using DirectionFloorPlugin.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace DirectionFloorPlugin.Models
{
    public class FloorInfo
    {
        private List<Point> _pointsOnTopFace = new List<Point>();

        public const double LENGHT_SHORT_DIRECTION = 500;
        public const double LENGHT_LONG_DIRECTION = 1000;
        public const double LENGHT_WIDTH_DIRECTION = 5;
        public const double LENGHT_HOOK_DIRECTION = 100;
        public const double ANGLE_HOOK_DIRECTION = Math.PI / 6;

        public Part MainModelObject { get; private set; }
        public Vector LocalShort { get; private set; }
        public Vector LocalLong { get; private set; }
        public GeometricPlane Plane { get; private set; }
        public Vector Normal { get; private set; }

        public FloorInfo(Part modelObject)
        {
            MainModelObject = modelObject;
            InitData();
        }

        public List<Point> GetProfileDirection(Vector dir, double len = LENGHT_SHORT_DIRECTION, double lenHook = LENGHT_HOOK_DIRECTION, double wid = LENGHT_WIDTH_DIRECTION, double angleRad = ANGLE_HOOK_DIRECTION)
        {
            var vecx = dir.GetNormal();
            var vecy = vecx.Cross(Normal.Reverse()).GetNormal();

            var pcen = GetCenter();
            //part1
            var p2 = pcen.Add(vecy, wid / 2);
            p2 = p2.Add(vecx.Reverse(), len / 2 - wid / Math.Sin(angleRad));
            var dir45 = vecx.Rotate(Normal, angleRad).GetNormal(); ;
            var p1 = p2.Add(dir45, lenHook);
            var p3 = p2.Add(vecx, len - wid / Math.Sin(angleRad));
            var p4 = p3.Add(vecy.Reverse(), wid);
            var dir_45 = vecx.Rotate(Normal, -(Math.PI - angleRad)).GetNormal();
            var p5 = p4.Add(dir_45, lenHook + wid / Math.Tan(angleRad));

            //part2
            var p2_ = pcen.Add(vecy.Reverse(), wid / 2);
            p2_ = p2_.Add(vecx, len / 2 - wid / Math.Sin(angleRad));
            var dir45_ = vecx.Rotate(Normal, -(Math.PI - angleRad)).GetNormal(); ;
            var p1_ = p2_.Add(dir45_, lenHook);
            var p3_ = p2_.Add(vecx.Reverse(), len - wid / Math.Sin(angleRad));
            var p4_ = p3_.Add(vecy, wid);
            var dir_45_ = vecx.Rotate(Normal, angleRad).GetNormal();
            var p5_ = p4_.Add(dir_45_, lenHook + wid / Math.Tan(angleRad));

            return new List<Point>() { p1, p2, p3, p4, p5, p1_, p2_, p3_, p4_, p5_ };
        }

        public Point GetCenter()
        {
            return _pointsOnTopFace.GetCenter();
        }

        public static bool ModelObjectIsValid(ModelObject modelObject)
        {
            bool result = false;
            if (modelObject is ContourPlate)
            {
                result = true;
            }
            if (modelObject is Beam beam)
            {
                if (beam.GetCoordinateSystem().AxisX.IsParallel(new Vector(0, 0, 1)))
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }

        private void InitData()
        {
            if (MainModelObject is ContourPlate contourPlate)
            {
                var coordinate = MainModelObject.GetCoordinateSystem();
                var localx = coordinate.AxisX.GetNormal();
                var localy = coordinate.AxisY.GetNormal();
                var localz = localx.Cross(localy).GetNormal();

                Normal = localx.Cross(localy).GetNormal();
                var solid = contourPlate.GetSolid();
                var points = solid.GetAllPoints();
                var pointTop = points.OrderBy(p => p.AsVector().Dot(localz)).LastOrDefault();

                Plane = new GeometricPlane(pointTop, Normal);
                _pointsOnTopFace = contourPlate.Contour.ContourPoints
                    .ToArray()
                    .Cast<Point>()
                    .Where(p => p != null)
                    .ToList()
                    .Select(p => Projection
                    .PointToPlane(p, Plane))
                    .ToList();

                var distX = _pointsOnTopFace.GetDistancePointsByVector(localx);
                var distY = _pointsOnTopFace.GetDistancePointsByVector(localy);
                if (distX <= distY)
                {
                    LocalShort = localx.GetNormal();
                    LocalLong = localy.GetNormal();
                }
                else
                {
                    LocalShort = localy.GetNormal();
                    LocalLong = localx.GetNormal();
                }
            }
            else if (MainModelObject is Beam beam)
            {
                var coordinate = MainModelObject.GetCoordinateSystem();
                var localx = coordinate.AxisX.GetNormal();
                var localy = localx.Cross(TeklaVectorExtension.GlobalZ.Reverse()).GetNormal();
                var localz = localx.Cross(localy).GetNormal();

                Normal = localz.GetNormal();
                var solid = beam.GetSolid();
                var points = solid.GetAllPoints();
                var pointTop = points.OrderBy(p => p.AsVector().Dot(localz)).LastOrDefault();

                Plane = new GeometricPlane(pointTop, Normal);
                _pointsOnTopFace = points.Select(p => Projection.PointToPlane(p, Plane)).ToList();

                var distX = _pointsOnTopFace.GetDistancePointsByVector(localx);
                var distY = _pointsOnTopFace.GetDistancePointsByVector(localy);
                if (distX <= distY)
                {
                    LocalShort = localx.GetNormal();
                    LocalLong = localy.GetNormal();
                }
                else
                {
                    LocalShort = localy.GetNormal();
                    LocalLong = localx.GetNormal();
                }
            }
        }
    }
}
