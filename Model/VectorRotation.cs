using System;
using ASRR.Revit.Core.Utilities;
using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Model
{
    public class VectorRotation : IRotation
    {
        public VectorRotation(XYZ rotationVector = null)
        {
            RotationVector = rotationVector ?? new XYZ();
        }

        public XYZ RotationVector { get; set; }

        public double RotationInRadians => RotationVector.AngleOnPlaneTo(XYZ.BasisX, XYZ.BasisZ.Negate());

        public double RotationInDegrees => RotationInRadians * 180 / Math.PI;

        public bool IsAlmostEqualTo(IRotation other)
        {
            return CoordinateUtilities.ApproximateEquals(RotationInRadians, other.RotationInRadians);
        }
    }
}