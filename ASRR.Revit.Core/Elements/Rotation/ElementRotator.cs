using System;
using Autodesk.Revit.DB;
using NLog;

namespace ASRR.Revit.Core.Elements.Rotation
{
    public class ElementRotator
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static bool RotateElement(Element element, double degrees)
        {
            bool rotated = false;
            // Rotate the element via its location curve.
            LocationCurve curve = element.Location as LocationCurve;
            LocationPoint elementPoint = element.Location as LocationPoint;
            if (null != elementPoint)
            {
                //Curve line = elementPoint.Point;
                XYZ aa = elementPoint.Point;
                XYZ cc = new XYZ(aa.X, aa.Y, aa.Z + 10);
                Line axis = Line.CreateBound(aa, cc);
                rotated = elementPoint.Rotate(axis, ConvertToRadians(degrees));
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
            return (Math.PI / 180) * angle;
        }
    }
}