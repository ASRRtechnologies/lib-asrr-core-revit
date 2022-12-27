using System;
using ASRR.Revit.Core.Elements.Placement;
using ASRR.Revit.Core.Model;

namespace Form.Revit.Common
{
    public class RadianRotation : IRotation
    {
        public double Rotation { get; set; }

        public RadianRotation(double rotation)
        {
            Rotation = rotation;
        }

        public double RotationInRadians => Rotation;

        public double RotationInDegrees => Rotation * 180 / Math.PI;

        public bool IsAlmostEqualTo(IRotation other)
        {
            return CoordinateUtilities.ApproximateEquals(Rotation, other.RotationInRadians);
        }
    }
}