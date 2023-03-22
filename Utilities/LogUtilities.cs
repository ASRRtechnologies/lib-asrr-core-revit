using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Utilities
{
    public static class LogUtilities
    {
        public static string PrintXYZ(XYZ xyz)
        {
            return $"X: {xyz.X}, Y: {xyz.Y}, Z: {xyz.Z}";
        }
    }
}