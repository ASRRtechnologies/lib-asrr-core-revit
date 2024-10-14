using Autodesk.Revit.DB;
using Common_glTF_Exporter.Utils;

namespace ASRR.Revit.Core.Exporter.GLTF.Utils
{
    class IsDocumentRFA
    {
        public static bool Check(Document doc)
        {
            if (doc.IsFamilyDocument)
            {
                SettingsConfig.SetValue("isRFA", "true");
                return true;
            }
            else
            {
                SettingsConfig.SetValue("isRFA", "false");
                return false;
            }
        }
    }
}
