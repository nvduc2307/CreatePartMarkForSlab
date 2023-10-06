using System.Collections.Generic;
using Tekla.Structures.Geometry3d;

namespace DirectionFloorPlugin.Utils
{
	public class PointComprarer : IEqualityComparer<Point>
	{
		public bool Equals(Point x, Point y)
		{
			var result = false;
			if (y.Subtract(x).AsVector().GetLength() < 1.0e-6)
			{
				result = true;
			}
			return result;
		}

		public int GetHashCode(Point obj)
		{
			return obj.GetHashCode();
		}
	}
}
