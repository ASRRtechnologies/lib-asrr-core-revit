using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Exporter.GLTF.Model
{
    public interface IObject
    {
        Category Category { get; set; }

        string FamilySymbol { get; set; }

        string ElementName { get; set; }

        ElementId EId { get; set; }

        Location Location { get; set; }
    }
}
