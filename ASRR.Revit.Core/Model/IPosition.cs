using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Model
{
    public interface IPosition
    {
        XYZ PositionInFeet { get; }

        XYZ PositionInMillimeters { get; }

        bool IsAlmostEqualTo(IPosition other);
    }
}