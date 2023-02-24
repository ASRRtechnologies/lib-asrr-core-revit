
using ASRR.Core.FileManagement.Service;
using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Exporter.IFC.Service
{
    public class IFCExporter: IExporter
    {
        public void Export(Document document, string exportFolder, string exportFileName)
        {
            FileValidator.EnsureFolderExists(exportFolder);
            
            var ifcExportOptions = new IFCExportOptions();
            ifcExportOptions.FileVersion = IFCVersion.IFC4;
            ifcExportOptions.ExportBaseQuantities = true;
            ifcExportOptions.AddOption("Name", "ASRR Technologies (ASRR B.V.)");
            
            document.Export(exportFolder, exportFileName, ifcExportOptions);
        }
      
    }
}