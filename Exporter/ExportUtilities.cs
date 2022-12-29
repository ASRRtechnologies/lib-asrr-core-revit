using Autodesk.Revit.DB;
using NLog;

namespace ASRR.Revit.Core
{
    public class ExportUtilities
    {
        // todo: maak static class om png te exporten
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        public static void ExportPNG(Document document, string exportPath)
        {
            var opt = new ImageExportOptions
            {
                ZoomType = ZoomFitType.FitToPage,
                PixelSize = 128,
                FilePath = exportPath,
                FitDirection = FitDirectionType.Horizontal,
                HLRandWFViewsFileType = ImageFileType.JPEGLossless,
                ImageResolution = ImageResolution.DPI_600,
            };
            document.ExportImage(opt);

            Log.Info("Exporting PNG");
        }
    }
}