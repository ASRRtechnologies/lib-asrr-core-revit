using Autodesk.Revit.DB;
using NLog;

namespace ASRR.Revit.Core.Elements.Rotation
{
    public class ElementRotator
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static bool RotateElement(Element element, double degrees, XYZ rotationAxis)
        {
            var rotated = false;
            // Rotate the element via its location curve.
            var curve = element.Location as LocationCurve;
            var elementPoint = element.Location as LocationPoint;
            if (null != elementPoint)
            {
                //Curve line = elementPoint.Point;
                var aa = rotationAxis;
                var cc = new XYZ(aa.X, aa.Y, aa.Z + 10);
                var axis = Line.CreateBound(aa, cc);
                rotated = elementPoint.Rotate(axis, ConvertToRadians(degrees));
                Log.Info($"Rotated around point {aa.X}, {aa.Y}, {aa.Z}");
                Log.Info($"C Rotated around point {cc.X}, {cc.Y}, {cc.Z}");
            }
            else
            {
                Log.Info("elementPoint is null");
            }

            Log.Info($"rotated : '{rotated}'");
            return rotated;
        }

        private static double ConvertToRadians(double angle)
        {
            return Math.PI / 180 * angle;
        }
    }
}