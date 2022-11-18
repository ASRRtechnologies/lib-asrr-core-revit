using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Elements.Families
{
    public class RevitFamilyUtils
    {
        public Family GetFamilyFromInstance(Element element, bool silentFail = true)
        {
            var familyInstance = element as FamilyInstance;
            var family = familyInstance?.Symbol?.Family;

            if (family == null && !silentFail)
            {
                throw new System.Exception("Could not get family from element");
            }

            return family;
        }

        public FamilySymbol GetFamilySymbolFromInstance(Element element, bool silentFail = true)
        {
            var familyInstance = element as FamilyInstance;
            var familySymbol = familyInstance?.Symbol;

            if (familySymbol == null && !silentFail)
            {
                throw new System.Exception("Could not get family symbol from element");
            }

            return familySymbol;
        }
    }
}