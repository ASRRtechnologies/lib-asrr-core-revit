using Autodesk.Revit.DB;
using System;

namespace ASRR.Revit.Core.Utilities
{
    public class CoordinateUtilities
    {
        public static XYZ ConvertMmToFeet(XYZ vector)
        {
            return new XYZ
            (
                ConvertMmToFeet(vector.X),
                ConvertMmToFeet(vector.Y),
                ConvertMmToFeet(vector.Z)
            );
        }

        public static XYZ ConvertFeetToMm(XYZ vector)
        {
            return new XYZ
            (
                ConvertFeetToMm(vector.X),
                ConvertFeetToMm(vector.Y),
                ConvertFeetToMm(vector.Z)
            );
        }

        public static XYZ DynamicConvert(XYZ vector, ForgeTypeId current, ForgeTypeId desired)
        {
            return new XYZ
            (
                DynamicConvert(vector.X, current, desired),
                DynamicConvert(vector.Y, current, desired),
                DynamicConvert(vector.Z, current, desired)
            );
        }

        public static double DynamicConvert(double value, ForgeTypeId current, ForgeTypeId desired)
        {
            return UnitUtils.Convert(value, current, desired);
        }

        public static double ConvertMmToFeet(double millimeterValue)
        {
            return UnitUtils.Convert(millimeterValue, UnitTypeId.Millimeters,
                UnitTypeId.Feet);
        }

        public static double ConvertFeetToMm(double feetValue)
        {
            return UnitUtils.Convert(feetValue, UnitTypeId.Feet,
                UnitTypeId.Millimeters);
        }

        public static bool ApproximateEquals(XYZ position, XYZ otherPositionInMillimeters)
        {
            return Math.Abs(position.X - otherPositionInMillimeters.X) < 0.05 &&
                   Math.Abs(position.Y - otherPositionInMillimeters.Y) < 0.05 &&
                   Math.Abs(position.Z - otherPositionInMillimeters.Z) < 0.05;
        }

        public static bool ApproximateEquals(double rotation, double otherPositionInMillimeters)
        {
            return Math.Abs(rotation - otherPositionInMillimeters) < 0.05;
        }

        public static string WriteXyz(XYZ vector)
        {
            return $"({vector.X:0.##}, {vector.Y:0.##}, {vector.Z:0.##})";
        }


        public static string ToString(XYZ point)
        {
            return "(" + point.X + ", " + point.Y + ", " + point.Z + ")";
        }

        public static double SquareFootToSquareM(double sqFoot, int decimals = 2)
        {
            return Math.Round(sqFoot * 0.092903, decimals);
        }


        public static string CurveToString(Curve curve)
        {
            var result = "Start: ";
            result += ToString(curve.GetEndPoint(0));
            result += "End: ";
            result += ToString(curve.GetEndPoint(1));
            result += Environment.NewLine;
            return result;
        }
    }
}