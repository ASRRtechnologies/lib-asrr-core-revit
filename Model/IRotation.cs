namespace ASRR.Revit.Core.Model
{
    public interface IRotation
    {
        double RotationInRadians { get; }

        double RotationInDegrees { get; }

        bool IsAlmostEqualTo(IRotation other);
    }
}