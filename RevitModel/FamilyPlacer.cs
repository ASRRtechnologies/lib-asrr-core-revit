using ASRR.Revit.Core.Model;
using ASRR.Revit.Core.Utilities;
using ASRR.Revit.Core.Warnings;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace ASRR.Revit.Core.RevitModel
{
    public class FamilyPlacer
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, Family> _loadedFamilies = new Dictionary<string, Family>();

        public FamilyInstance PlaceFamily(Document doc, IPosition position, string familyFilePath, string typeName = null, Wall host = null)
        {
            familyFilePath = AddRfaExtensionIfMissing(familyFilePath);
            using (Transaction transaction = WarningDiscardFailuresPreprocessor.GetTransaction(doc))
            {
                transaction.Start("Place family instance");
                FamilySymbol familySymbol;
                var success = false;

                //If a typename is provided, only load that specific symbol
                if (typeName != null)
                {
                    success = doc.LoadFamilySymbol(familyFilePath, typeName, out familySymbol);
                    if (success)
                        AddOrReplace(familyFilePath, familySymbol.Family);
                    else
                    {
                        //Family was already loaded maybe, so find the right symbol
                        if (!_loadedFamilies.ContainsKey(familyFilePath))
                        {
                            _logger.Error($"Failed to load family from '{familyFilePath}' with type '{typeName}'");
                            return null;
                        }

                        ElementId familySymbolId = _loadedFamilies[familyFilePath].GetFamilySymbolIds().First(id => doc.GetElement(id).Name == typeName);
                        familySymbol = doc.GetElement(familySymbolId) as FamilySymbol;
                    }
                }
                //If a typename is not provided, load the entire family and pick the first
                else
                {
                    success = doc.LoadFamily(familyFilePath, out Family family);
                    if (success)
                    {
                        familySymbol = doc.GetElement(family.GetFamilySymbolIds().First()) as FamilySymbol;
                        AddOrReplace(familyFilePath, family);
                    }
                    else if (_loadedFamilies.ContainsKey(familyFilePath))
                    {
                        //Family was already loaded
                        family = _loadedFamilies[familyFilePath];
                        familySymbol = doc.GetElement(family.GetFamilySymbolIds().First()) as FamilySymbol;
                    }
                    else
                    {
                        _logger.Error($"Revit failed to load family from '{familyFilePath}'");
                        return null;
                    }
                }

                //Ensure the family symbol is activated
                if (familySymbol != null && !familySymbol.IsActive)
                    familySymbol.Activate();

                //Avoid bugs by refreshing first
                doc.Regenerate();

                FamilyInstance instance;

                if (host != null)
                {
                    if (!IsInsideHost(host, position))
                        return null;

                    IPosition offsetPosition = GetHostableOffsetPosition(doc, position, host);
                    instance = doc.Create.NewFamilyInstance(offsetPosition.PositionInFeet, familySymbol, host, StructuralType.NonStructural);
                    _logger.Trace($"Placed family instance '{instance.Name}' in wall '{host.Name}' at {LogUtilities.PrintXYZ(position.PositionInMillimeters)}");
                }
                else
                {
                    instance = doc.Create.NewFamilyInstance(position.PositionInFeet, familySymbol, StructuralType.NonStructural);
                    _logger.Trace($"Placed family instance '{instance.Name}' at {LogUtilities.PrintXYZ(position.PositionInMillimeters)}");
                }

                transaction.Commit();

                return instance;
            }

        }


        private bool IsInsideHost(Wall host, IPosition position)
        {
            BoundingBoxXYZ hostBoundingBox = host.get_BoundingBox(null);
            if (CoordinateUtilities.IsInsideBoundingBox(hostBoundingBox, position.PositionInFeet))
                return true;

            FeetPosition bbMin = new FeetPosition(hostBoundingBox.Min);
            FeetPosition bbMax = new FeetPosition(hostBoundingBox.Max);
            _logger.Error($"Failed to host family instance into host because instance is outside of host: ");
            _logger.Error($"Instance: {LogUtilities.PrintXYZ(position.PositionInMillimeters)} Host: {LogUtilities.PrintXYZ(bbMin.PositionInMillimeters)} / {LogUtilities.PrintXYZ(bbMax.PositionInMillimeters)}");
            return false;
        }

        private IPosition GetHostableOffsetPosition(Document doc, IPosition position, Wall host)
        {
            Level hostLevel = doc.GetElement(host.LevelId) as Level;
            _logger.Trace($"Level of host is at {CoordinateUtilities.ConvertFeetToMm(hostLevel.Elevation)}");

            XYZ offsetXYZ = new XYZ(position.PositionInFeet.X, position.PositionInFeet.Y, position.PositionInFeet.Z + hostLevel.Elevation);
            return new FeetPosition(offsetXYZ);
        }

        private void AddOrReplace(string familyFilePath, Family family)
        {
            if (_loadedFamilies.ContainsKey(familyFilePath))
                _loadedFamilies.Remove(familyFilePath);

            _loadedFamilies.Add(familyFilePath, family);
        }

        private string AddRfaExtensionIfMissing(string familyFilePath)
        {
            if (!familyFilePath.EndsWith(".rfa"))
                return familyFilePath + ".rfa";

            return familyFilePath;
        }
    }
}
