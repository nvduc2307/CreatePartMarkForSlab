using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Tekla.Structures.Model;

namespace DirectionFloorPlugin.Utils
{
	public static class TeklaModelWorkPlaneExtension
	{
		private static TransformationPlane _currentTransformationPlane;
		public static void SetGlobalWorkPlane(this Model model)
		{
			_currentTransformationPlane = model.GetWorkPlaneHandler().GetCurrentTransformationPlane();
			model.GetWorkPlaneHandler().SetCurrentTransformationPlane(new TransformationPlane());
		}

		public static void RestoreWorkPlane(this Model model)
		{
			if (_currentTransformationPlane != null)
			{
				model.GetWorkPlaneHandler().SetCurrentTransformationPlane(_currentTransformationPlane);
				_currentTransformationPlane = null;
			}
		}
	}
}
