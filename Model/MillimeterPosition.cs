using ASRR.Core.MathUtils;
using ASRR.Revit.Core.Elements.Placement;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ASRR.Revit.Core.Model
{
    public class MillimeterPosition : IPosition
    {
        public XYZ Position { get; set; }

        public MillimeterPosition(XYZ position)
        {
            Position = position;
        }

        public XYZ PositionInFeet => CoordinateUtilities.ConvertMmToFeet(Position);

        public XYZ PositionInMillimeters => Position;

        public bool IsAlmostEqualTo(IPosition other)
        {
            return CoordinateUtilities.ApproximateEquals(Position, other.PositionInMillimeters);
        }

        public static MillimeterPosition Zero => new MillimeterPosition(XYZ.Zero);
    }
}