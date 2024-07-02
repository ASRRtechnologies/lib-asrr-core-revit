using System.Collections.Generic;
using ASRR.Revit.Core.Exporter.GLTF.Model;
using ASRR.Revit.Core.Exporter.GLTF.Utils;

namespace ASRR.Revit.Core.Exporter.GLTF.Transform
{
    public static class ModelScale
    {
        public static List<double> Get(Preferences preferences)
        {
            double scale = Util.ConvertFeetToUnitTypeId(preferences);
            return new List<double> { scale, scale, scale };
        }
    }
}
