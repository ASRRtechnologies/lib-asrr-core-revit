using ASRR.Revit.Core.Elements.Placement;
using Autodesk.Revit.DB;
using System;

namespace ASRR.Revit.Core.Utilities
{
    public class WallUtilities
    {
        public static double GetNormalAxisProjection(Wall wall, double theta)
        {
            if (!(wall.Location is LocationCurve c))
                throw new Exception("Could not cast wall to LocationCurve");

            var xyzFeet = c.Curve.GetEndPoint(0); // 0 and 1 will give same output due to projection on normal axis
            var xyz = CoordinateUtilities.ConvertFeetToMm(xyzFeet);

            return Math.Cos(theta) * xyz.X + Math.Sin(theta) * xyz.Y;
        }

        public static bool IsExterior(WallType wallType)
        {
            var p = wallType.get_Parameter(
                BuiltInParameter.FUNCTION_PARAM);

            var f = (WallFunction)p.AsInteger();

            return WallFunction.Exterior == f;
        }

    }
}