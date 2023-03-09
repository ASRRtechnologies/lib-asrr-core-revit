using ASRR.Revit.Core.Model;
using ASRR.Revit.Core.Utilities;
using Autodesk.Revit.DB;
using NLog;

namespace ASRR.Revit.Core
{
    public class TransformUtilities
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public static IPosition GetPosition(Element element)
        {
            var locationPoint = element.Location as LocationPoint;
            var xyz = locationPoint.Point;
            return new FeetPosition(xyz);
        }

        /// <summary>
        ///     Moves element to the given position
        /// </summary>
        /// <param name="ignoreZ"> Allows to ignore Z (up/down) to avoid moving elements vertically</param>
        /// <returns></returns>
        public static bool SetPosition(Element element, IPosition position, bool ignoreZ = true)
        {
            var location = element?.Location;
            if (location == null)
            {
                _logger.Error("Could not set rotation because element or location is null");
                return false;
            }

            if (!(element.Location is LocationPoint locationPoint))
            {
                _logger.Error("Can't set transform of element that does not have a LocationPoint");
                return false;
            }

            var z = ignoreZ ? 0 : position.PositionInFeet.Z - locationPoint.Point.Z;

            var positionOffset = new XYZ(
                position.PositionInFeet.X - locationPoint.Point.X,
                position.PositionInFeet.Y - locationPoint.Point.Y,
                z);

            Move(element, new FeetPosition(positionOffset));

            return true;
        }

        /// <summary>
        ///     Moves element by the given offset
        /// </summary>
        public static bool Move(Element element, IPosition positionOffset)
        {
            var location = element?.Location;
            if (location == null)
            {
                _logger.Error("Could not set rotation because element or location is null");
                return false;
            }

            location.Move(positionOffset.PositionInFeet);
            return true;
        }

        public static IRotation GetRotation(Element element)
        {
            var locationPoint = element.Location as LocationPoint;
            var rotationInRadians = locationPoint.Rotation;

            return new RadianRotation(rotationInRadians);
        }

        /// <summary>
        ///     Exactly set the rotation to the given rotation
        /// </summary>
        public static bool SetRotation(Element element, IRotation rotation)
        {
            var location = element?.Location;
            if (location == null)
            {
                _logger.Error("Could not set rotation because element or location is null");
                return false;
            }

            if (!(element.Location is LocationPoint locationPoint))
            {
                _logger.Error("Can't set transform of element that does not have a LocationPoint");
                return false;
            }

            //Find out how much we need to rotate by to get to the desired rotation
            var rotationOffset = new RadianRotation(rotation.RotationInRadians - locationPoint.Rotation);
            Rotate(element, rotationOffset);

            return true;
        }

        /// <summary>
        ///     Rotates element by the given rotation
        /// </summary>
        public static bool Rotate(Element element, IRotation rotation)
        {
            var location = element?.Location;
            if (location == null)
            {
                _logger.Error("Could not rotate because element or location is null");
                return false;
            }

            if (!(element.Location is LocationPoint locationPoint))
            {
                _logger.Error("Can't set transform of element that does not have a LocationPoint");
                return false;
            }

            //Only rotate if it's not 0
            if (!CoordinateUtilities.ApproximateEquals(rotation.RotationInRadians, 0))
                location.Rotate(Line.CreateBound(locationPoint.Point, locationPoint.Point + XYZ.BasisZ),
                    rotation.RotationInRadians);

            return true;
        }

        /// <summary>
        ///     Rotates element by the given rotation around the origin point of its document
        /// </summary>
        public static bool RotateGlobal(Element element, IRotation rotation)
        {
            var location = element?.Location;
            if (location == null)
            {
                _logger.Error("Could not rotate because element or location is null");
                return false;
            }

            //Only rotate if it's not 0
            if (!CoordinateUtilities.ApproximateEquals(rotation.RotationInRadians, 0))
                location.Rotate(Line.CreateBound(XYZ.Zero, XYZ.BasisZ), rotation.RotationInRadians);

            return true;
        }

        /// <summary>
        ///     Adds two positions together and returns the result
        /// </summary>
        public static IPosition AddPositions(IPosition a, IPosition b)
        {
            var xyz = a.PositionInMillimeters + b.PositionInMillimeters;
            return new MillimeterPosition(xyz);
        }
    }
}