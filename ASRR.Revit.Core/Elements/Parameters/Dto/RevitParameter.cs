
using Autodesk.Revit.DB;
using NLog;

namespace ASRR.Revit.Core.Elements.Parameters.Dto
{
    public static class RevitParameter
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public static dynamic GetParameterValue(Element element, string parameterName)
        {
            try
            {
                var parameterValue = element.GetParameters(parameterName);
                dynamic parameterV = null;

                logger.Info($"GetParameterValue : '{parameterValue[0].AsValueString()}' for parameterName : '{parameterName}', elementId '{element.Id}'");
                parameterV = _getParameterValueByCorrectStorageType(parameterValue[0]);

                return parameterV;
            }
            catch (Exception e)
            {
                logger.Warn($"Can't get parameter with parameterName '{parameterName}', elementId '{element.Id}'");
                logger.Warn($"e '{e}'");
                return null;
            }
        }
        public static dynamic GetTypeParameterValue(Document doc, Element element, string parameterName)
        {
            try
            {
                ElementType type = doc.GetElement(element.GetTypeId()) as ElementType;
                dynamic value;
                if (type?.LookupParameter(parameterName) != null)
                {
                    value = _getParameterValueByCorrectStorageType(type.LookupParameter(parameterName));
                    // logger.Info($"GetParameterValue : '{value}' for parameterName : '{parameterName}', elementId '{element.Id}'");
                    return value;
                }
                // logger.Warn($"Can't get Type parameter with parameterName '{parameterName}', elementId '{element.Id}'");
                return null;
            }
            catch (Exception e)
            {
                // logger.Warn($"Can't get Type parameter with parameterName '{parameterName}', elementId '{element.Id}'");
                // logger.Warn($"e '{e}'");
                return null;
            }
        }
        private static dynamic _getParameterValueByCorrectStorageType(Parameter parameter)
        {
            return parameter.StorageType switch
            {
                StorageType.ElementId => parameter.AsElementId().IntegerValue,
                StorageType.Integer => parameter.AsInteger(),
                StorageType.None => parameter.AsString(),
                StorageType.Double => parameter.AsDouble(),
                StorageType.String => parameter.AsValueString(),
                _ => ""
            };
        }


        public static void SetParameterValue(Element element, string parameterName, dynamic parameterValue)
        {
            try
            {
                var parameter = element.LookupParameter(parameterName);
                logger.Info($"SetParameterValue : '{parameterValue}', for parameter '{parameterName}', elementId '{element.Id}'");
                _setParameterValueByCorrectStorageType(element.LookupParameter(parameterName), parameterValue);
            }
            catch (Exception e)
            {
                logger.Warn($"Can't set parameter with parameterName '{parameterName}', and parameterValue '{parameterValue}', elementId '{element.Id}'");
                logger.Error($"e : '{e}'");
            }


        }
        public static void SetTypeParameterValue(Document doc, Element element, string parameterName, dynamic parameterValue)
        {
            try
            {
                ElementType type = doc.GetElement(element.GetTypeId()) as ElementType;
                var parameter = type.LookupParameter(parameterName);

                logger.Info($"SetParameterValue : '{parameterValue}', for parameter '{parameterName}', elementId '{element.Id}'");
                _setParameterValueByCorrectStorageType(element.LookupParameter(parameterName), parameterValue);
            }
            catch (Exception e)
            {
                logger.Warn($"Can't set parameter with parameterName '{parameterName}', and parameterValue '{parameterValue}', elementId '{element.Id}'");
                logger.Error($"e : '{e}'");
            }

        }
        private static dynamic _setParameterValueByCorrectStorageType(Parameter parameter, dynamic parameterValue)
        {
            return parameter.StorageType switch
            {
                StorageType.Double => parameter.Set(Convert.ToDouble(parameterValue)),
                StorageType.Integer => parameter.Set(Convert.ToInt32(Convert.ToDouble(parameterValue))),
                StorageType.String => parameter.Set(Convert.ToString(parameterValue)),
                StorageType.None => parameter.Set(Convert.ToString(parameterValue)),
                StorageType.ElementId => parameter.Set(parameterValue),
                _ => ""
            };
        }
    }
}