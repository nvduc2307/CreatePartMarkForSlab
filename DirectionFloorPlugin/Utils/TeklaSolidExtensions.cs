using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Tekla.Structures.Geometry3d;
using Tekla.Structures.Model;
using Tekla.Structures.Solid;

namespace DirectionFloorPlugin.Utils
{
	public static class TeklaSolidExtensions
	{
		public static List<Point> GetAllPoints(this Solid solid)
		{
			var result = new List<Point>();

			var edges = solid.GetEdgeEnumerator();
			while (edges.MoveNext())
			{
				var edge = edges.Current as Edge;
				if (edge != null)
				{
					result.Add(edge.StartPoint);
					result.Add(edge.EndPoint);
				}
			}
			result = result.Distinct(new PointComprarer()).ToList();

			return result;
		}
	}
}
