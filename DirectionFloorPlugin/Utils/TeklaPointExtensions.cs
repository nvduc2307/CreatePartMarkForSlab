using System.Collections.Generic;
using Tekla.Structures.Drawing;
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
	}
}
