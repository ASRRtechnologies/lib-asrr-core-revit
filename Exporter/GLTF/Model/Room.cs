using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Exporter.GLTF.Model
{
    public class Room
    {
        public string Name { get; set; }

        public Element Element { get; set; }

        public ElementId ElementId { get; set; }

        public List<ElementId> ElementList { get; set; }
    }
}
