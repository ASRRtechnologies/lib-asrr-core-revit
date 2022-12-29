using System;
using ASRR.Revit.Core.Elements.Placement;

namespace ASRR.Revit.Core.Model
{
    public class DegreeRotation : IRotation
    {
        public DegreeRotation(double rotation)
        {
            Rotation = rotation;
        }

        public double Rotation { get; set; }

        public double RotationInRadians => Rotation / (180 / Math.PI);

        public double RotationInDegrees => Rotation;

        public bool IsAlmostEqualTo(IRotation other)
        {
            return CoordinateUtilities.ApproximateEquals(Rotation, other.RotationInDegrees);
        }
    }
}