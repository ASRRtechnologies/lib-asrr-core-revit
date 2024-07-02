using System.Collections.Generic;

namespace ASRR.Revit.Core.Exporter.GLTF.Model
{
    public class RevitGridParametersObject
    {
        public List<double> origin { get; set; }

        public List<double> direction { get; set; }

        public double length { get; set; }
    }
}
