using Tekla.Structures.Drawing;
using tsd = Tekla.Structures.Drawing;
using tsg = Tekla.Structures.Geometry3d;
using tsm = Tekla.Structures.Model;

namespace TeklaDev
{
    public static class ExtDrawingPartMark
    {
        public static void CreatePartMark(
            this tsd.ModelObject modelObjectInDrawing,
            tsm.Model cmodel,
            tsd.ViewBase viewBase,
            PointInsertMark pointInsertMark,
            MarkType markType,
            string message = "")
        {
            //Get position mark
            tsg.Point PartTopLeft = null, 
                PartBotLeft = null, 
                PartTopRight = null, 
                PartBotRight = null,
                PartCenterPoint = null;
            GetPartPoints(
                cmodel, 
                viewBase, 
                modelObjectInDrawing, 
                out PartTopLeft, 
                out PartBotLeft,
                out PartTopRight,
                out PartBotRight, 
                out PartCenterPoint);

            var modelObject = GetModelObjectFromDrawingModelObject(cmodel, modelObjectInDrawing);

            var distance1 = PartTopLeft.DistancePToP(PartTopRight);
            var distance2 = PartTopLeft.DistancePToP(PartBotLeft);
            tsg.Point pointInsert = null;
            tsd.PlacingBase placingBase = null;
            if (modelObject  is tsm.ContourPlate)
            {
                switch (pointInsertMark)
                {
                    case PointInsertMark.TopPart:
                    case PointInsertMark.MiddlePart:
                    case PointInsertMark.BottomPart:
                        pointInsert = PartCenterPoint;

                        if (distance1 > distance2)
                        {
                            placingBase = new AlongLinePlacing(PartTopLeft, PartBotLeft);
                        }
                        else
                        {
                            placingBase = new AlongLinePlacing(PartTopLeft, PartTopRight);
                        }
                        break;
                }
            }
            else
            {
                switch (pointInsertMark)
                {
                    case PointInsertMark.TopPart:
                        if (distance1 > distance2)
                        {
                            pointInsert = GetInsertionPoint(PartTopLeft, PartTopRight);
                            placingBase = new AlongLinePlacing(PartTopLeft, PartTopRight);
                        }
                        else
                        {
                            pointInsert = GetInsertionPoint(PartTopLeft, PartBotLeft);
                            placingBase = new AlongLinePlacing(PartTopLeft, PartBotLeft);
                        }
                        break;
                    case PointInsertMark.MiddlePart:
                        pointInsert = PartCenterPoint;

                        if (distance1 > distance2)
                        {
                            placingBase = new AlongLinePlacing(PartTopLeft, PartTopRight);
                        }
                        else
                        {
                            placingBase = new AlongLinePlacing(PartTopLeft, PartBotLeft);
                        }
                        break;
                    case PointInsertMark.BottomPart:
                        if (distance1 > distance2)
                        {
                            pointInsert = GetInsertionPoint(PartBotLeft, PartBotRight);
                            placingBase = new AlongLinePlacing(PartBotLeft, PartBotRight);
                        }
                        else
                        {
                            pointInsert = GetInsertionPoint(PartBotLeft, PartTopLeft);
                            placingBase = new AlongLinePlacing(PartBotLeft, PartTopLeft);
                        }
                        break;
                }
            }

            //Setting mark ready use
            var parkMark = new tsd.Mark(modelObjectInDrawing);
            parkMark.ConfigMarkSetting(markType, message);
            parkMark.Placing = placingBase;
            parkMark.InsertionPoint = pointInsert;
            parkMark.Insert();
        }

        #region Private methods
        private static void GetPartPoints(
            tsm.Model cmodel,
            tsd.ViewBase partView,
            tsd.ModelObject modelObject,
            out tsg.Point PartTopLeft,
            out tsg.Point PartBotLeft, 
            out tsg.Point PartTopRight,
            out tsg.Point PartBotRight,
            out tsg.Point PartCenterPoint)
        {
            tsm.ModelObject modelPart = GetModelObjectFromDrawingModelObject(cmodel, modelObject);
            GetModelObjectStartAndEndPoint(
                cmodel, 
                modelPart, 
                (tsd.View)partView, 
                out PartTopLeft, 
                out PartBotLeft, 
                out PartTopRight,
                out PartBotRight);
            PartCenterPoint = GetInsertionPoint(PartBotLeft, PartTopRight);
        }

        private static tsm.ModelObject GetModelObjectFromDrawingModelObject(
            tsm.Model cmodel, 
            tsd.ModelObject partOfMark)
        {
            var modelObject = cmodel.SelectModelObject(partOfMark.ModelIdentifier);

            var modelPart = (tsm.Part)modelObject;

            return modelPart;
        }

        private static void GetModelObjectStartAndEndPoint(
            tsm.Model cmodel, 
            tsm.ModelObject modelObject, 
            tsd.View partView, 
            out tsg.Point PartTopLeft, 
            out tsg.Point PartBotLeft, 
            out tsg.Point PartTopRight,
            out tsg.Point PartBotRight)
        {
            var modelPart = (tsm.Part)modelObject;

            var savePlane = cmodel.GetWorkPlaneHandler().GetCurrentTransformationPlane();
            cmodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(new tsm.TransformationPlane());

            var partStartPoint = modelPart.GetSolid().MinimumPoint;
            partStartPoint.Z = 0.0;
            var PartEndPoint = modelPart.GetSolid().MaximumPoint;
            PartEndPoint.Z = 0.0;

            PartTopLeft = new tsg.Point(partStartPoint.X, PartEndPoint.Y, 0);
            PartBotLeft = partStartPoint;
            PartTopRight = PartEndPoint;
            PartBotRight = new tsg.Point(PartEndPoint.X, partStartPoint.Y, 0);

            var convMatrix = tsg.MatrixFactory.ToCoordinateSystem(partView.DisplayCoordinateSystem);
            PartTopLeft = convMatrix.Transform(PartTopLeft);
            PartBotLeft = convMatrix.Transform(PartBotLeft);
            PartTopRight = convMatrix.Transform(PartTopRight);
            PartBotRight = convMatrix.Transform(PartBotRight);

            cmodel.GetWorkPlaneHandler().SetCurrentTransformationPlane(savePlane);
        }

        private static tsg.Point GetInsertionPoint(
            tsg.Point partStartPoint, 
            tsg.Point partEndPoint)
        {
            var minPoint = partStartPoint;
            var maxPoint = partEndPoint;
            var insertionPoint = new tsg.Point((maxPoint.X + minPoint.X) * 0.5, (maxPoint.Y + minPoint.Y) * 0.5, (maxPoint.Z + minPoint.Z) * 0.5);
            insertionPoint.Z = 0;
            return insertionPoint;
        }

        public enum PointInsertMark
        {
            TopPart,
            MiddlePart,
            BottomPart,
        }
        #endregion
    }
}
