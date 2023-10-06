using System;
using Tekla.Structures.Datatype;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace DirectionFloorPlugin.Utils
{
	public static class TeklaVectorExtension
	{
		public static Vector Reverse(this Vector vector)
		{
			return new Vector(-vector.X, -vector.Y, -vector.Z);
		}

		public static Vector Add(this Vector vec1, Vector vec2)
		{
			return new Vector(vec1.X + vec2.X, vec1.Y + vec2.Y, vec1.Z + vec2.Z);
		}

		public static Vector Rotate(this Vector vec, Vector axis, double angleRad)
		{
			var axisNormal = axis.GetNormal();
			Vector vxp = axisNormal.Cross(vec);
			Vector vxvxp = axisNormal.Cross(vxp);
			return vec.Add(Math.Sin(angleRad) * vxp).Add((1 - Math.Cos(angleRad)) * vxvxp);
		}

		public static Vector ProjectToPlane(this Vector vec, GeometricPlane plane)
		{
			var p1 = new Point();
			var p2 = p1.Add(vec, 1);
			var p1New = Projection.PointToPlane(p1, plane);
			var p2New = Projection.PointToPlane(p2, plane);
			return new Vector(p2New.Subtract(p1New)).GetNormal();
		}
	}
}
