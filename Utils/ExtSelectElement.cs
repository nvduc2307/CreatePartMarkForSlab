using System.Collections;
using tsmui = Tekla.Structures.Model.UI;
using tsd = Tekla.Structures.Drawing;
namespace TeklaDev
{
    public static class ExtSelectElement
    {
        public static void HightLightInModel(this ArrayList modelObjects)
        {
            var selector = new tsmui.ModelObjectSelector();
            selector.Select(modelObjects);
        }
        public static void HightLightInDrawing(this tsd.DrawingHandler dHandle, ArrayList drawingObjects)
        {
            dHandle.GetDrawingObjectSelector().SelectObjects(drawingObjects, false);
        }
    }
}
