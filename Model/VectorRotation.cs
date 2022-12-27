using Autodesk.Revit.DB;
using System;
using ASRR.Revit.Core.Elements.Placement;
using ASRR.Revit.Core.Model;

namespace Form.Revit.Common
{
    public class VectorRotation : IRotation
    {
        public XYZ RotationVector { get; set; }

        public VectorRotation(XYZ rotationVector)
        {
            RotationVector = rotationVector;
        }

        public double RotationInRadians => RotationVector.AngleOnPlaneTo(XYZ.BasisX, XYZ.BasisZ.Negate());

        public double RotationInDegrees => RotationInRadians * 180 / Math.PI;

        public bool IsAlmostEqualTo(IRotation other)
        {
            return CoordinateUtilities.ApproximateEquals(RotationInRadians, other.RotationInRadians);
        }
    }
}