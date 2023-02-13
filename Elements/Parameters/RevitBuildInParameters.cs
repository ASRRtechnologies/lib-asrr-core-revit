using ASRR.Revit.Core.Elements.Parameters.Dto;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using glTFRevitExport;
using NLog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media.Media3D;

namespace ASRR.Revit.Core.Elements.Parameters
{
    public static class RevitBuildInParameters
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();
        public static string GetBuildInParameterValueString(Element e, BuiltInParameter bip)
        {
            Parameter p = e.get_Parameter(bip);

            string s = string.Empty;

            if (null != p)
            {
                switch (p.StorageType)
                {
                    case StorageType.Integer:
                        s = p.AsInteger().ToString();
                        break;

                    case StorageType.ElementId:
                        s = p.AsElementId().IntegerValue.ToString();
                        break;

                    case StorageType.Double:
                        s = Util.RealString(p.AsDouble());
                        break;

                    case StorageType.String:
                        s = string.Format("{0} ({1})",
                          p.AsValueString(),
                          Util.RealString(p.AsDouble()));
                        break;

                    default:
                        s = "";
                        break;
                }
                s = ", " + bip.ToString() + "=" + s;
            }
            return s;
        }

        public static string GetAssemblyCode(Autodesk.Revit.DB.Element element)
        {
            try
            {
                string assemblyCode = element.get_Parameter(BuiltInParameter.UNIFORMAT_CODE).AsString();
                Log.Info($"assemblyCode is '{assemblyCode}'");
                return assemblyCode;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetAssemblyName(Autodesk.Revit.DB.Element element)
        {
            try
            {
                string assemblyName = element.get_Parameter(BuiltInParameter.UNIFORMAT_DESCRIPTION).AsString();
                Log.Info($"assemblyName is '{assemblyName}'");
                return assemblyName;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetMaterial(Document doc, Element element)
        {
            HostObject FPR = element as HostObject;
            //Type tp = FPR.GetType();
            if (FPR == null || FPR.GetType() == null)
            {
                return element.GetParameters("Structural Material")[0].AsValueString();
                //return RevitParameter.GetParameterValue(element, "Structural Material").ToString();
            }
            //string tpName = tp.Name;
            //logger.Info($"material tp : {tp.Name}");
            
            switch (FPR.GetType().Name)
            {
                case "Wall":
                    Wall wall = element as Wall;
                    return doc.GetElement(wall.WallType.GetCompoundStructure().GetMaterialId(0)).Name;// as Material;
                case "Floor":
                    Floor floor = element as Floor;
                    return doc.GetElement(floor.FloorType.GetCompoundStructure().GetMaterialId(0)).Name;// as Material;
                case "FootPrintRoof":
                    FootPrintRoof roof = element as FootPrintRoof;
                    return doc.GetElement(roof.RoofType.GetCompoundStructure().GetMaterialId(0)).Name;// as Material;
                case "Fascia":
                    Fascia fascia = element as Fascia;
                    return doc.GetElement(fascia.FasciaType.GetCompoundStructure().GetMaterialId(0)).Name;// as Material;
                default:
                    string mat = doc.GetElement(element.GetMaterialIds(true).First()).Name; // as Material;
                    logger.Info("returnd default value");
                    return mat;
            }
        }
    }
}
