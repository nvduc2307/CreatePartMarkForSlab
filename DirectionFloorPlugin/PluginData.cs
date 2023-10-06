using Tekla.Structures.Plugins;

namespace DirectionFloorPlugin
{
	public class PluginData
	{
		[StructuresField("angle")]
		public double Angle;
		[StructuresField("direction")]
		public int DirectionIndex;
		[StructuresField("type")]
		public int TypeIndex;
	}
}
