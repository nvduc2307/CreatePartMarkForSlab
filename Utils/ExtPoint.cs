using System;
using tsg = Tekla.Structures.Geometry3d;

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
            return Math.Sqrt(Math.Pow((p2.X - p1.X),2) + Math.Pow((p2.Y - p1.Y), 2) + Math.Pow((p2.Z - p1.Z), 2));
        }
    }
}
