
using ASRR.Core.FileManagement;
using ASRR.Revit.Core.Warnings;
using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Exporter.IFC.Service
{
    public class IfcExporter : IExporter
    {
        public void Export(Document document, string exportFolder, string exportFileName)
        {
            FileValidator.EnsureFolderExists(exportFolder);
            var ifcExportOptions = new IFCExportOptions();
            ifcExportOptions.FileVersion = IFCVersion.IFC4;
            ifcExportOptions.ExportBaseQuantities = true;
            ifcExportOptions.AddOption("Name", "ASRR Technologies (ASRR B.V.)");

            using (var transaction = WarningDiscardFailuresPreprocessor.GetTransaction(document))
            {

                transaction.Start("Exporting IFC");
                document.Export(exportFolder, exportFileName, ifcExportOptions);
                transaction.RollBack();
            }
        }

    }
}