using Autodesk.Revit.DB;
using System;
using ASRR.Revit.Core.Elements.Placement;
using System.Collections.Generic;

namespace ASRR.Revit.Core.Elements
{
    public static class GeometryUtils
    {
        private const double TOLERANCE = 0.001;

        public static XYZ GetInverse(XYZ point)
        {
            return new XYZ(-point.X, -point.Y, -point.Z);
        }

        public static XYZ GetCenter(PlanarFace face)
        {
            var bb = face.GetBoundingBox();
            var half = (bb.Max - bb.Min) / 2;

            return face.Origin + (face.XVector * half.U) + (face.YVector * half.V);
        }

        public static XYZ GetOrientation(Wall wall)
        {
            var locationCurve = wall.Location as LocationCurve;
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
            var difference = Math.Abs(a - b);
            if (difference > epsilon)
                return false;

            return true;
        }

        public static int CompareLengths(CurveLoop curveLoopA, CurveLoop curveLoopB)
        {
            if (curveLoopA.GetExactLength() > curveLoopB.GetExactLength())
                return 1;
            else if (Math.Abs(curveLoopA.GetExactLength() - curveLoopB.GetExactLength()) < TOLERANCE)
                return 0;
            else
                return -1;
        }

        public static XYZ GetWallCenter(Wall wall)
        {
            if (!(wall.Location is LocationCurve c))
                throw new Exception("Could not cast wall to LocationCurve");

            var xyzFeet =
                (c.Curve.GetEndPoint(0) + c.Curve.GetEndPoint(1)) /
                2;

            //TODO fix this 1500 value to check height +1500 on level wall excists on
            return xyzFeet.Add(new XYZ(0, 0, CoordinateUtilities.ConvertMmToFeet(1500)));
        }

        public static List<XYZ> GetWallClashPoints(Wall wall)
        {
            if (!(wall.Location is LocationCurve c))
                throw new Exception("Could not cast wall to LocationCurve");

            var xyzMidFeet = (c.Curve.GetEndPoint(0) + c.Curve.GetEndPoint(1)) / 2;
            var xyzQ1Feet = (c.Curve.GetEndPoint(0) + xyzMidFeet) / 2;
            var xyzQ3Feet = (xyzMidFeet + c.Curve.GetEndPoint(1)) / 2;
            var xyzQ11Feet = (c.Curve.GetEndPoint(0) + xyzQ1Feet) / 2;
            var xyzQ33Feet = (xyzQ3Feet + c.Curve.GetEndPoint(1)) / 2;

            //TODO fix this 1500 value to check height +1500 on level wall excists on
            xyzMidFeet.Add(new XYZ(0, 0, CoordinateUtilities.ConvertMmToFeet(1500)));
            xyzQ1Feet.Add(new XYZ(0, 0, CoordinateUtilities.ConvertMmToFeet(1500)));
            xyzQ3Feet.Add(new XYZ(0, 0, CoordinateUtilities.ConvertMmToFeet(1500)));
            xyzQ11Feet.Add(new XYZ(0, 0, CoordinateUtilities.ConvertMmToFeet(1500)));
            xyzQ33Feet.Add(new XYZ(0, 0, CoordinateUtilities.ConvertMmToFeet(1500)));

            return new List<XYZ>() {xyzQ1Feet, xyzMidFeet, xyzQ3Feet, xyzQ11Feet, xyzQ33Feet};
        }
    }
}