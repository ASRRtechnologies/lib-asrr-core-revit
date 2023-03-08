using System;
using ASRR.Revit.Core.Elements.Placement;
using ASRR.Revit.Core.Model;
using ASRR.Revit.Core.Utilities;
using Autodesk.Revit.DB;

namespace Form.Revit.Common
{
    public class VectorRotation : IRotation
    {
        public VectorRotation(XYZ rotationVector)
        {
            RotationVector = rotationVector;
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