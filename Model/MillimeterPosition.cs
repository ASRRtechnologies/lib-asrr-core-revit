using ASRR.Revit.Core.Elements.Placement;
using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Model
{
    public class MillimeterPosition : IPosition
    {
        public MillimeterPosition(XYZ position)
        {
            Position = position;
        }

        public XYZ Position { get; set; }

        public static MillimeterPosition Zero => new MillimeterPosition(XYZ.Zero);

        public XYZ PositionInFeet => CoordinateUtilities.ConvertMmToFeet(Position);

        public XYZ PositionInMillimeters => Position;

        public bool IsAlmostEqualTo(IPosition other)
        {
            return CoordinateUtilities.ApproximateEquals(Position, other.PositionInMillimeters);
        }
    }
}