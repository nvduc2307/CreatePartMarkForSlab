using System.Runtime.CompilerServices;
using Tekla.Structures.Model;

namespace DirectionFloorPlugin.Utils
{
	public static class TeklaPartCutExtension
	{
		public static BooleanPart InitPartCut(this Part partTobeCut, Part partCut)
		{
			var booleanPart = new BooleanPart();
			booleanPart.Father = partTobeCut;
			booleanPart.SetOperativePart(partCut);
			return booleanPart;
		}
	}
}
