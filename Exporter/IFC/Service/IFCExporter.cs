using ASRR.Core.FileManagement.Service;
using ASRR.Revit.Core.Warnings;
using Autodesk.Revit.DB;
using BIM.IFC.Export.UI;

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
            ifcExportOptions.SpaceBoundaryLevel = 0;
            ifcExportOptions.AddOption("Name", "ASRR Technologies (ASRR B.V.)");

            using (var transaction = WarningDiscardFailuresPreprocessor.GetTransaction(document))
            {
                transaction.Start("Exporting IFC");
                document.Export(exportFolder, exportFileName, ifcExportOptions);
                transaction.RollBack();
            }
        }

        public void Export(Document document, string exportFolder, string exportFileName,
            IFCExportConfiguration ifcExportConfiguration, ElementId viewId = null)
        {
            FileValidator.EnsureFolderExists(exportFolder);
            var ifcExportOptions = new IFCExportOptions();
            if (ifcExportConfiguration == null)
            {
                ifcExportOptions.FileVersion = IFCVersion.IFC4;
                ifcExportOptions.ExportBaseQuantities = true;
                ifcExportOptions.SpaceBoundaryLevel = 0;
                ifcExportOptions.FilterViewId = viewId;
                ifcExportOptions.AddOption("Name", "ASRR Technologies (ASRR B.V.)");
            }
            else
            {
                if (viewId == null) ifcExportConfiguration.VisibleElementsOfCurrentView = false;
                ifcExportConfiguration.UpdateOptions(ifcExportOptions, viewId);
            }

            using (var transaction = WarningDiscardFailuresPreprocessor.GetTransaction(document))
            {
                transaction.Start("Exporting IFC");
                document.Export(exportFolder, exportFileName, ifcExportOptions);
                transaction.RollBack();
            }
        }
    }
}