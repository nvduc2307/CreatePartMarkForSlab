using CreatePartMarkForSlab.Utils;
using Tekla.Structures.Drawing;
using tsd = Tekla.Structures.Drawing;
using tsg = Tekla.Structures.Geometry3d;
using tsm = Tekla.Structures.Model;

namespace TeklaDev
{
    public static class ExtDrawingPartMark
    {
        public static void CreatePartMark(
            this tsd.ViewBase viewBase,
            tsm.Model cmodel,
            MarkType markType,
            string prefix = "",
            double extend = 0.0,
            double angle = 0.0)
        {
            var booleanParts = cmodel.GetBooleanPartsInModel(tsm.BooleanPart.BooleanTypeEnum.BOOLEAN_CUT);
            booleanParts.ForEach(booleanPart =>
            {
                var modelPark = booleanPart.Father as tsm.Part;
                var drawingModelObjs = viewBase.GetModelObjects(modelPark.Identifier);
                while (drawingModelObjs.MoveNext())
                {
                    if (drawingModelObjs.Current is tsd.Part dModelObj)
                    {
                        tsg.CoordinateSystem pointsCoordinate = null;
                        var points = booleanPart.GetPointOnTopShellFace(cmodel, out pointsCoordinate);
                        var pointsDrawing = points.TransformPointsInModelToViewDrawing(pointsCoordinate, cmodel, viewBase as tsd.View);
                        if (pointsDrawing.Count == 6)
                        {
                            var p_mark1 = pointsDrawing[1];
                            var p_mark2 = pointsDrawing[2];
                            var midPoint = p_mark1.MidPoint(p_mark2);
                            var dir = p_mark1.CreateVector(p_mark2);
                            var normal = dir.Cross(new tsg.Vector(0,0,1));
                            var pointInsert = midPoint.Tranform(normal * extend * (-1));
                            var p_along_mark_1 = p_mark1.Rotate(pointInsert, angle);
                            var p_along_mark_2 = p_mark2.Rotate(pointInsert, angle);

                            var placingBase = new AlongLinePlacing(p_mark1, p_mark2);

                            //Setting mark ready use
                            var parkMark = new tsd.Mark(dModelObj);
                            parkMark.ConfigMarkSetting(markType, prefix);
                            parkMark.Placing = placingBase;
                            parkMark.InsertionPoint = pointInsert;
                            parkMark.Insert();
                        }
                    }
                }
            });
        }
        public static void CreatePartMark(
            this tsd.ModelObject modelObjectInDrawing,
            tsm.Model cmodel,
            tsd.ViewBase viewBase,
            LocationMark pointInsertMark,
            MarkType markType,
            string prefix = "",
            double extend = 0.0,
            double angle = 0.0)
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
            //var values = new Hashtable();
            //modelObject.GetStringUserProperties(ref values);

            var distance1 = PartTopLeft.DistancePToP(PartTopRight);
            var distance2 = PartTopLeft.DistancePToP(PartBotLeft);
            var vt1 = PartTopLeft.CreateVector(PartTopRight).VectorNormalize();
            var vt2 = PartTopLeft.CreateVector(PartBotLeft).VectorNormalize();
            tsg.Point pointInsert = null;
            tsd.PlacingBase placingBase = null;
            tsg.Point p_along_mark_1 = null;
            tsg.Point p_along_mark_2 = null;
            if (modelObject is tsm.ContourPlate)
            {
                switch (pointInsertMark)
                {
                    case LocationMark.TopPart:
                        if (distance1 > distance2)
                        {
                            var topmid = PartTopRight.MidPoint(PartBotRight);
                            pointInsert = topmid.Tranform(vt1 * extend);
                            p_along_mark_1 = PartTopRight.Rotate(pointInsert, angle);
                            p_along_mark_2 = PartBotRight.Rotate(pointInsert, angle);
                            placingBase = new AlongLinePlacing(p_along_mark_1, p_along_mark_2);
                        }
                        else
                        {
                            var topmid = PartTopLeft.MidPoint(PartTopRight);
                            pointInsert = topmid.Tranform(vt2 * extend * (-1));
                            p_along_mark_1 = PartTopLeft.Rotate(pointInsert, angle);
                            p_along_mark_2 = PartTopRight.Rotate(pointInsert, angle);
                            placingBase = new AlongLinePlacing(p_along_mark_1, p_along_mark_2);
                        }
                        break;
                    case LocationMark.MiddlePart:
                        if (distance1 > distance2)
                        {
                            pointInsert = PartCenterPoint.Tranform(vt1 * extend);
                            p_along_mark_1 = PartTopLeft.Rotate(pointInsert, angle);
                            p_along_mark_2 = PartBotLeft.Rotate(pointInsert, angle);
                            placingBase = new AlongLinePlacing(p_along_mark_1, p_along_mark_2);
                        }
                        else
                        {
                            pointInsert = PartCenterPoint.Tranform(vt2 * extend * (-1));
                            p_along_mark_1 = PartTopLeft.Rotate(pointInsert, angle);
                            p_along_mark_2 = PartTopRight.Rotate(pointInsert, angle);
                            placingBase = new AlongLinePlacing(p_along_mark_1, p_along_mark_2);
                        }
                        break;
                    case LocationMark.BottomPart:
                        if (distance1 > distance2)
                        {
                            var botmid = PartTopLeft.MidPoint(PartBotLeft);
                            pointInsert = botmid.Tranform(vt1 * extend);
                            p_along_mark_1 = PartTopLeft.Rotate(pointInsert, angle);
                            p_along_mark_2 = PartBotLeft.Rotate(pointInsert, angle);
                            placingBase = new AlongLinePlacing(p_along_mark_1, p_along_mark_2);
                        }
                        else
                        {
                            var botmid = PartBotLeft.MidPoint(PartBotRight);
                            pointInsert = botmid.Tranform(vt2 * extend * (-1));
                            p_along_mark_1 = PartBotLeft.Rotate(pointInsert, angle);
                            p_along_mark_2 = PartBotRight.Rotate(pointInsert, angle);
                            placingBase = new AlongLinePlacing(p_along_mark_1, p_along_mark_2);
                        }
                        break;
                }
            }
            else
            {
                switch (pointInsertMark)
                {
                    case LocationMark.TopPart:
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
                    case LocationMark.MiddlePart:
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
                    case LocationMark.BottomPart:
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
            parkMark.ConfigMarkSetting(markType, prefix);
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
        #endregion
    }
}
