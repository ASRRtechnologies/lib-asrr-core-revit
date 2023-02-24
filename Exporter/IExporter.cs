using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Exporter
{
    public interface IExporter
    {
        void Export(Document document, string exportFolder, string exportFileName);
    }
}