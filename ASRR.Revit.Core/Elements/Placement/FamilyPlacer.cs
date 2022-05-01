using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using NLog;

namespace ASRR.Revit.Core.Elements.Placement
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
            string familyName,
            string type,
            XYZ location,
            ElementId levelId)
        {
            var symbols = GetFamilySymbol(doc, familyName, type);

            if (!symbols.Any())
            {
                Log.Error($"No matching family symbol for familyName '{familyName}' and type '{type}' ");
                return;
            }

            if (symbols.Count() > 1)
            {
                Log.Warn("Found more than one matching symbol, using first symbol found.");
            }

            var level = doc.GetElement(levelId) as Level;
            var symbol = symbols.First();
            Log.Info($"Found matching symbol '{symbol.Name}'");

            using (var transaction = new Transaction(doc)) {
                transaction.Start("Place family instance");
                Log.Info($"Creating new placing instance at {location} on level {level.Elevation}");
                doc.Create.NewFamilyInstance(location, symbol, level, StructuralType.NonStructural);
            }
        }

        public List<FamilySymbol> GetFamilySymbol(Document doc, string familyName, string typeName)
        {
            try
            {
                return new FilteredElementCollector(doc)
                    .OfClass(typeof(FamilySymbol))
                    .Cast<FamilySymbol>()
                    .Where(x => x.Family.Name.Equals(familyName)) // family
                    .Where(x => x.Name.Equals(typeName))
                    .ToList(); // family type         
            }
            catch (Exception)
            {
                return null;
            }
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
