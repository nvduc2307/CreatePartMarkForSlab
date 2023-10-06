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

		public const double LENGHT_SHORT_DIRECTION = 1500;
		public const double LENGHT_LONG_DIRECTION = 3000;
		public const double LENGHT_HOOK_DIRECTION = 200;
		public const double LENGHT_WIDTH_DIRECTION = 50;

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

		public List<Point> GetProfileDirection(Vector dir, double len = LENGHT_SHORT_DIRECTION, double lenHook = LENGHT_HOOK_DIRECTION, double lenWid = LENGHT_WIDTH_DIRECTION)
		{
			var vecx = dir.GetNormal();
			var vecy = vecx.Cross(Normal.Reverse()).GetNormal();

			//part1
			var p1 = GetCenter();
			p1 = p1.Add(vecy, lenWid / 2);
			p1 = p1.Add(vecx, len / 2);
			var p2 = p1.Add(vecx.Reverse(), len - lenWid);
			var p3 = p2.Add(vecy, lenHook);

			//part2
			var p1_1 = GetCenter();
			p1_1 = p1_1.Add(vecy.Reverse(), lenWid / 2);
			p1_1 = p1_1.Add(vecx.Reverse(), len / 2);
			var p1_2 = p1_1.Add(vecx, len - lenWid);
			var p1_3 = p1_2.Add(vecy.Reverse(), lenHook);

			return new List<Point>() { p1, p2, p3, p1_1, p1_2, p1_3 };
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
				var localy = localx.Cross(new Vector(0, 0, -1)).GetNormal();
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
