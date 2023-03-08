using ASRR.Revit.Core.Elements.Placement;
using ASRR.Revit.Core.Utilities;
using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Model
{
    public class FeetPosition : IPosition
    {
        public FeetPosition(XYZ position)
        {
            Position = position;
        }

        public XYZ Position { get; set; }

        public XYZ PositionInFeet => Position;

        public XYZ PositionInMillimeters => CoordinateUtilities.ConvertFeetToMm(Position);

        public bool IsAlmostEqualTo(IPosition other)
        {
            return CoordinateUtilities.ApproximateEquals(Position, other.PositionInFeet);
        }
    }
}