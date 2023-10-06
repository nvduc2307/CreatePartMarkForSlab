using System;
using System.Diagnostics;
using Tekla.Structures.Datatype;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace DirectionFloorPlugin.Utils
{
	public static class TeklaVectorExtension
	{
		public static readonly Vector GlobalX = new Vector(1, 0, 0);
		public static readonly Vector GlobalY = new Vector(0, 1, 0);
		public static readonly Vector GlobalZ = new Vector(0, 0, 1);
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

		public static Vector RayToPlane(this Vector vec, Vector vecRay, GeometricPlane plane)
		{
			var result = new Vector();

			var p1 = new Point();
			var p2 = p1.Add(vec, 1);
			var p1New = p1.RayToPlane(vecRay, plane);
			var p2New = p2.RayToPlane(vecRay, plane);
			if (p1New != null && p2New != null)
			{
				result = new Vector(p2New.Subtract(p1New)).GetNormal();
			}

			return result;
		}

		public static bool IsParallel(this Vector v1, Vector v2)
		{
			var result = false;

			var angle = v1.GetAngleBetween(v2);
			if (Math.Abs(angle) < 1.0e-6 || Math.Abs(angle - Math.PI) < 1.0e-6)
			{
				result = true;
			}

			return result;
		}
	}
}
