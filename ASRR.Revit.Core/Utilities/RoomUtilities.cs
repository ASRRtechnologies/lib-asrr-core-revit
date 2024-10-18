using ASRR.Revit.Core.Elements;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using System.Collections.Generic;
using System.Linq;

namespace ASRR.Revit.Core.Utilities
{
    public class RoomUtilities
    {

        public static bool RoomIntersects(Wall wall, bool mirrored, List<Room> rooms)
        {
            //deze checked alleen het midpoint van de wall op een room. Hier kan dus perongeluk een element staat bijv een wall waardoor de room niet gevonden wordt
            var inverseNormal = mirrored ? wall.Orientation : GeometryUtils.GetInverse(wall.Orientation);
            var center = GeometryUtils.GetWallCenter(wall);

            //TODO: deze 500 variabele vangen in settings
            var addition = (wall.Width + CoordinateUtilities.ConvertMmToFeet(500));
            var pointInFrontOfFace = center + (inverseNormal * addition);

            return rooms.Any(room => room.IsPointInRoom(pointInFrontOfFace));
        }

        public static bool RoomIntersects(Wall wall, XYZ normal, List<Room> rooms)
        {
            //deze checked alleen het midpoint van de wall op een room. Hier kan dus perongeluk een element staat bijv een wall waardoor de room niet gevonden wordt
            var center = GeometryUtils.GetWallCenter(wall);

            //TODO: deze 500 variabele vangen in settings
            var addition = (wall.Width + CoordinateUtilities.ConvertMmToFeet(500));
            var pointInFrontOfFace = center + (normal * addition);

            return rooms.Any(room => room.IsPointInRoom(pointInFrontOfFace));
        }
    }
}