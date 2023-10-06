using DirectionFloorPlugin.Utils;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Model.Geometry;

namespace DirectionFloorPlugin.Models
{
	public class FloorInfo
	{
		public const double LENGHT_SHORT_DIRECTION = 1500;
		public const double LENGHT_LONG_DIRECTION = 3000;
		public const double LENGHT_HEAD_DIRECTION = 300;

		public ContourPlate Floor { get; private set; }
		public Vector LocalShort { get; private set; }
		public Vector LocalLong { get; private set; }
		public GeometricPlane Plane { get; private set; }
		public Vector Normal => LocalShort.Cross(LocalLong).GetNormal();

		public FloorInfo(ContourPlate contourPlate)
		{
			Floor = contourPlate;
			InitData();
		}

		public List<Point> GetProfileDirection(Vector dir, double len = LENGHT_SHORT_DIRECTION, double lenHead = LENGHT_HEAD_DIRECTION)
		{
			var pCenter = GetCenter();
			var p2 = pCenter.Add(dir.Reverse(), len / 2);
			var p3 = pCenter.Add(dir, len / 2);
			var dir45 = dir.Rotate(Normal, Math.PI / 4);
			var dir_45 = dir.Rotate(Normal, -3 * Math.PI / 4);
			var p1 = p2.Add(dir45, lenHead);
			var p4 = p3.Add(dir_45, lenHead);

			return new List<Point>() { p1, p2, p3, p4 };
		}

		public Point GetCenter()
		{
			var result = new Point();

			var points = Floor.Contour.ContourPoints.ToArray().Cast<Point>().ToList();
			for (int i = 0; i < points.Count; i++)
			{
				result += points[i];
			}
			result = result.Divide(points.Count);

			return result;
		}

		private void InitData()
		{
			var coordinate = Floor.GetCoordinateSystem();
			var localx = coordinate.AxisX;
			var localy = coordinate.AxisY;
			var points = Floor.Contour.ContourPoints.ToArray().Cast<Point>().ToList();

			var distX = GetDistancePointsByVector(points, localx);
			var distY = GetDistancePointsByVector(points, localy);

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
			Plane = new GeometricPlane(points.FirstOrDefault(), Normal);
		}
		private double GetDistancePointsByVector(List<Point> ps, Vector vec)
		{
			var result = double.NaN;

			var pMin = ps.OrderBy(p => p.AsVector().Dot(vec)).FirstOrDefault();
			var pMax = ps.OrderBy(p => p.AsVector().Dot(vec)).LastOrDefault();
			if (pMin != null && pMax != null)
			{
				result = pMax.AsVector().Dot(vec) - pMin.AsVector().Dot(vec);
			}

			return result;
		}
	}
}
