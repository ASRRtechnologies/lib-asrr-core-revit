using ASRR.Core.MathUtils;
using ASRR.Revit.Core.Elements.Placement;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ASRR.Revit.Core.Model
{
    public class FeetPosition : IPosition
    {
        public XYZ Position { get; set; }

        public FeetPosition(XYZ position)
        {
            Position = position;
        }

        public XYZ PositionInFeet => Position;

        public XYZ PositionInMillimeters => CoordinateUtilities.ConvertFeetToMm(Position);

        public bool IsAlmostEqualTo(IPosition other)
        {
            return CoordinateUtilities.ApproximateEquals(Position, other.PositionInFeet);
        }
    }
}