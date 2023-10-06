using System;
using tsg = Tekla.Structures.Geometry3d;
namespace TeklaDev
{
    public static class ExtVector
    {
        public static tsg.Vector VectorNormalize(this tsg.Vector vector)
        {
            var distance = vector.Distance();
            return new tsg.Vector(vector.X/ distance, vector.Y / distance, vector.Z / distance);
        }
        public static double Distance(this tsg.Vector vector)
        {
            return Math.Sqrt(Math.Pow(vector.X,2) + Math.Pow(vector.Y, 2) + Math.Pow(vector.Z, 2));
        }
        public static tsg.Vector CreateVector(this tsg.Point p1, tsg.Point p2)
        {
            var vt = new tsg.Vector(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
            return vt;
        }
    }
}
