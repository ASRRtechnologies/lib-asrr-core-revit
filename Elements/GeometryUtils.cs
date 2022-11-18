using Autodesk.Revit.DB;
using System;

namespace ASRR.Revit.Core.Elements
{
    public class GeometryUtils
    {

        public static XYZ GetInverse(XYZ point)
        {
            return new XYZ(-point.X, -point.Y, -point.Z);
        }

        public static XYZ GetCenter(PlanarFace face)
        {
            BoundingBoxUV bb = face.GetBoundingBox();
            UV half = (bb.Max - bb.Min) / 2;

            return face.Origin + (face.XVector * half.U) + (face.YVector * half.V);
        }

        public static XYZ GetOrientation(Wall wall)
        {
            LocationCurve locationCurve = wall.Location as LocationCurve;
            return (locationCurve.Curve.GetEndPoint(0) - locationCurve.Curve.GetEndPoint(1)).Normalize();
        }

        public static bool IsAlmostEqual(XYZ a, XYZ b)
        {
            return
                IsAlmostEqual(a.X, b.X) &&
                IsAlmostEqual(a.Y, b.Y) &&
                IsAlmostEqual(a.Z, b.Z);
        }

        public static bool IsAlmostEqual(double a, double b, double epsilon = 0.001)
        {
            double difference = Math.Abs(a - b);
            if (difference > epsilon)
                return false;

            return true;
        }

        public static int CompareLengths(CurveLoop curveLoopA, CurveLoop curveLoopB)
        {
            if (curveLoopA.GetExactLength() > curveLoopB.GetExactLength())
                return 1;
            else if (curveLoopA.GetExactLength() == curveLoopB.GetExactLength())
                return 0;
            else
                return -1;
        }
    }
}
