using ASRR.Revit.Core.Elements.Families;
using Autodesk.Revit.DB;
using System;
using NLog;

namespace ASRR.Revit.Core.Utilities
{
    public class WallUtilities
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static bool IsConstructive(Wall wall)
        {
            if (wall == null) throw new ArgumentNullException(nameof(wall));

            var utils = new RevitFamilyUtils();

            var family = utils.GetFamilyFromInstance(wall, false);
            if (family == null) return false;

            var wallType = wall.WallType;
            Log.Info($"Walltype for element {wall.Name} is {wallType.Name}");
            var parameter = wall.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_USAGE_PARAM);
            if (parameter == null) return false;

            Log.Info($"Found parameter for element {wall.Name}: {parameter.AsValueString()}");
            return parameter.AsValueString() == "Bearing";
        }

        public static int WallLength(Wall wall)
        {
            var parameter = wall.get_Parameter(BuiltInParameter.CURVE_ELEM_LENGTH).AsValueString();
            Log.Info($"wall length is: {parameter}");
            return Convert.ToInt32(parameter);
        }

        public static int WallHeight(Wall wall)
        {
            var parameter = wall.get_Parameter(BuiltInParameter.WALL_USER_HEIGHT_PARAM).AsValueString();
            Log.Info($"wall height is: {parameter}");
            return Convert.ToInt32(parameter);
        }

        public static string WallBaseConstraint(Wall wall)
        {
            var parameter = wall.get_Parameter(BuiltInParameter.WALL_BASE_CONSTRAINT).AsValueString();
            Log.Debug($"wall base constraint is: {parameter}");
            return parameter;
        }

        public static Curve GetCurve(Wall wall)
        {
            if (!(wall.Location is LocationCurve location)) throw new Exception("Could not cast wall to LocationCurve");
            return location.Curve;
        }

        public Tuple<double, double> GetWallDimensions(Wall wall)
        {
            var curve = GetCurve(wall);
            var length = curve.Length;
            var height = WallHeight(wall);
            return new Tuple<double, double>(length, height);
        }

        public Tuple<XYZ, XYZ> GetEndPoints(Wall wall)
        {
            var curve = GetCurve(wall);
            var start = curve.GetEndPoint(0);
            var end = curve.GetEndPoint(1);
            return new Tuple<XYZ, XYZ>(start, end);
        }

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