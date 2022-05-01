using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Elements.Rotation
{
    public class ElementRotator
    {
        public static bool RotateElement(Element element, double rotation)
        {
            bool rotated = false;
            // Rotate the element via its location curve.
            //LocationCurve curve = element.Location as LocationCurve;
            //logger.Info($"curve : '{curve}'");
            // LocationPoint elementPoint = element.Location as LocationPoint;
            // logger.Info($"elementPoint : x:'{MmToFeet(elementPoint.Point.X)}', y:'{MmToFeet(elementPoint.Point.Y)}', z:'{MmToFeet(elementPoint.Point.Z)}'");
            // if (null != elementPoint)
            // {
            //     //Curve line = elementPoint.Point;
            //     XYZ aa = elementPoint.Point;
            //     logger.Info($"aa : x:'{MmToFeet(aa.X)}', y:'{MmToFeet(aa.Y)}', z:'{MmToFeet(aa.Z)}'");
            //     XYZ cc = new XYZ(aa.X, aa.Y, aa.Z + 10);
            //     logger.Info($"cc : x:'{MmToFeet(cc.X)}', y:'{MmToFeet(cc.Y)}', z:'{MmToFeet(cc.Z)}'");
            //     Line axis = Line.CreateBound(aa, cc);
            //     rotated = elementPoint.Rotate(axis, rotation);
            // }
            // else
            // {
            //     logger.Info("elementPoint is null");
            // }
            // logger.Info($"rotated : '{rotated}'");
            return rotated;
        }
    }
}
