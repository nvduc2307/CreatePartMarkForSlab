using System.Collections.Generic;
using System.Linq;
using System.Windows.Interop;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;

namespace DirectionFloorPlugin.Utils
{
	public static class TeklaPointExtensions
	{
		public static Vector AsVector(this Point p)
		{
			return new Vector(p.X, p.Y, p.Z);
		}

		public static Point Divide(this Point p, double n)
		{
			return new Point(p.X / n, p.Y / n, p.Z / n);
		}

		public static Point Add(this Point p, Vector direction, double dist)
		{
			var vector = direction.GetNormal() * dist;
			return new Point(p.X + vector.X, p.Y + vector.Y, p.Z + vector.Z);
		}

		public static Point Subtract(this Point p2, Point p1)
		{
			return new Point(p2.X - p1.X, p2.Y - p1.Y, p2.Z - p1.Z);
		}

		public static Point GetCenter(this List<Point> ps)
		{
			var result = new Point();
			for (int i = 0; i < ps.Count; i++)
			{
				result += ps[i];
			}
			result = result.Divide(ps.Count);
			return result;
		}

		public static double GetDistancePointsByVector(this List<Point> ps, Vector vec)
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

		public static ControlLine InitControlLineWith(this Point p1, Point p2)
		{
			ControlLine controlLine = new ControlLine();
			LineSegment line = new LineSegment();
			line.Point1 = p1;
			line.Point2 = p2;
			controlLine.Line = line;
			controlLine.IsMagnetic = true;
			return controlLine;
		}

		public static List<ControlLine> InitControlLines(this List<Point> ps)
		{
			var controlLines = new List<ControlLine>();

			for (int i = 0; i < ps.Count - 1; i++)
			{
				var controlLine = ps[i].InitControlLineWith(ps[i + 1]);
				controlLines.Add(controlLine);
			}
			return controlLines;
		}

		public static ContourPlate InitContourPalte(this List<Point> polygons, string profile = "PL5")
		{
			var contourPoints = polygons.Select(p => new ContourPoint(new Point(p.X, p.Y, p.Z), null)).ToList();
			var cp = new ContourPlate();
			contourPoints.ForEach(c => cp.Contour.AddContourPoint(c));
			cp.Name = "Slab Direction";
			cp.Finish = "BooleanPart";
			cp.Profile.ProfileString = profile;
			cp.Material.MaterialString = "Steel_Undefined";
			cp.Class = BooleanPart.BooleanOperativeClassName;
			cp.Position.Depth = Position.DepthEnum.MIDDLE;
			return cp;
		}
	}
}
