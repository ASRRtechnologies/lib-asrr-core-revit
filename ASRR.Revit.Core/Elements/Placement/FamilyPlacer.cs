using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using NLog;

namespace ASRR.Core.Revit.Elements.Placement
{
    
    /// <summary>
    /// Class to interact with families
    /// </summary>
    public class FamilyPlacer
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// This method places a family based on reference plane, location and referenceDirection
        /// </summary>
        public void Place(Document doc, 
            Reference reference,
            XYZ location,
            XYZ referenceDirection,
            FamilySymbol symbol)
        {
            Log.Info($"Creating new placing instance at {location}");
            // TODO: make this work based on family name + family type
            doc.Create.NewFamilyInstance(reference, location, referenceDirection, symbol);
        }
        
        public IEnumerable<FamilyInstance> GetFamilyInstancesByFamilyName(Document doc, string familyName)
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

        public IEnumerable<FamilyInstance> GetFamilyInstancesByFamilyAndType(Document doc, string familyName, string typeName)
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
