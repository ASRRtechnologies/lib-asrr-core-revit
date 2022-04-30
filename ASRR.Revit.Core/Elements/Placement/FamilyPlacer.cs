using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace ASRR.Core.Revit.Elements.Placement
{
    public class FamilyPlacer
    {



        public static IEnumerable<FamilyInstance> GetFamilyInstancesByFamilyName(Document doc, string familyName)
        {
            try
            {
                IEnumerable<FamilyInstance> familyInstances = new FilteredElementCollector(doc)
                    .OfClass(typeof(FamilyInstance))
                    .Cast<FamilyInstance>()
                    .Where(x => x.Symbol.Family.Name.Contains(familyName));
                return familyInstances;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static IEnumerable<FamilyInstance> GetFamilyInstancesByFamilyAndType(Document doc, string familyName, string typeName)
        {
            try
            {
                return new FilteredElementCollector(doc)
                    .OfClass(typeof(FamilyInstance))
                    .Cast<FamilyInstance>()
                    .Where(x => x.Symbol.Family.Name.Equals(familyName)) // family
                    .Where(x => x.Name.Equals(typeName)); // family type         
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
