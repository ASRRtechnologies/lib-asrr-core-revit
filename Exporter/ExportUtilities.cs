using NLog;

namespace ASRR.Revit.Core
{
    public class ExportUtilities
    {
        // todo: maak static class om png te exporten
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        
        public static void ExportPNG(string exportPath)
        {
            Log.Info("Exporting PNG");
        }
    }
}