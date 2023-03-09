using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using NLog;
using System.Collections.Generic;
using System.Linq;

namespace ASRR.Revit.Core.Utilities
{
    public class Collector
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public static ICollection<Element> GetElementsOfCategory(Document doc, BuiltInCategory category)
        {
            var collector = new FilteredElementCollector(doc);
            var filter = new ElementCategoryFilter(category);
            return collector.WherePasses(filter).ToElements();
        }

        public static ICollection<Element> GetElementsOfCategories(Document doc,
            ICollection<BuiltInCategory> categories)
        {
            var collector = new FilteredElementCollector(doc);
            var filter = new ElementMulticategoryFilter(categories);
            return collector.WherePasses(filter).ToElements();
        }

        public static IEnumerable<Room> GetRooms(Document doc)
        {
            var elements = GetElementsOfCategory(doc, BuiltInCategory.OST_Rooms);
            return elements.Select(e => e as Room);
        }

        public static View3D Get3dView(Document doc)
        {
            var collector = new FilteredElementCollector(doc).OfClass(typeof(View3D));
            return collector.Cast<View3D>().FirstOrDefault(v => !v.IsTemplate);
        }

        public static List<T> GetAllOfType<T>(Document doc) where T : class
        {
            var collector = new FilteredElementCollector(doc);
            return collector.OfClass(typeof(T)).Select(e => e as T).ToList();
        }

        public static T GetFirstOfType<T>(Document doc) where T : class
        {
            return GetAllOfType<T>(doc).FirstOrDefault();
        }

        public static T FindElementByName<T>(Document doc, string elementName) where T : class
        {
            var allElements = GetAllOfType<T>(doc);

            var element = allElements.FirstOrDefault(e => (e as Element).Name == elementName);

            return element;
        }

        public static List<T> FindElementsByName<T>(Document doc, IEnumerable<string> elementNames) where T : class
        {
            var result = new List<T>();

            if (elementNames == null)
                return result;

            var allElements = GetAllOfType<T>(doc);
            foreach (var elementName in elementNames)
            {
                var element = allElements.FirstOrDefault(e => (e as Element).Name == elementName);
                if (element == null)
                {
                    _log.Error($"Could not find element with name '{elementName}'");
                    continue; //Don't add null when element can't be found
                }

                result.Add(element);
            }

            return result;
        }
    }
}