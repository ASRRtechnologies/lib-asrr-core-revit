using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Elements
{
    public class UseDestinationHandler : IDuplicateTypeNamesHandler
    {
        public static CopyPasteOptions CopyPasteOptions
        {
            get
            {
                CopyPasteOptions copyPasteOptions = new CopyPasteOptions();
                copyPasteOptions.SetDuplicateTypeNamesHandler(new UseDestinationHandler());
                return copyPasteOptions;
            }
        }

        public DuplicateTypeAction OnDuplicateTypeNamesFound(DuplicateTypeNamesHandlerArgs args)
        {
            return DuplicateTypeAction.UseDestinationTypes;
        }
    }
}