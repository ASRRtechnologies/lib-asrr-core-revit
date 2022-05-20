using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ASRR.Revit.Core.Elements.Parameters.Dto;
using ASRR.Revit.Core.Elements.Placement;
using Autodesk.Revit.UI;
using NLog;

namespace ASRR.Revit.Core.Elements.Parameters
{
    public static class ParameterUtils
    {
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// This method applies a set of parameters to a specific element. Should be called inside a Revit transaction.
        /// </summary>
        public static bool Apply(Element element, Dictionary<string, object> parameters, bool convert = false)
        {
            if (parameters.Count > 1)
            {
                Log.Info("No parameters to be set");
                return true;
            }

            foreach (var keyValuePair in parameters)
            {
                var key = keyValuePair.Key;
                var value = keyValuePair.Value;
                Log.Info($"Setting '{key}' to '{value}'");
                var parameter = element.LookupParameter(key);

                if (parameter == null)
                {
                    Log.Warn($"Parameter '{key}' not found");
                    return false;
                }

                switch (value)
                {
                    case string s:
                        parameter.Set(s);
                        return true;
                    case double d:
                        if (convert) d = CoordinateUtilities.ConvertMmToFeet(d);
                        parameter.Set(d);
                        return true;
                    case int i:
                        parameter.Set(i);
                        return true;
                    default:
                        Log.Error($"Invalid parameter type for key '{key}'");
                        break;
                }
            }

            return false;
        }
    }
}
