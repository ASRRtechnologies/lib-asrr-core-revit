using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Elements.Placement
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


    }
}
