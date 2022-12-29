using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using NLog;

namespace ASRR.Revit.Core.Elements
{
    public class Utilities
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public static void ZoomToPoint(UIDocument uidoc, XYZ point, float zoom)
        {
            var doc = uidoc.Document;
            var view = doc.ActiveView;
            UIView uiView = null;

            var uiviews = uidoc.GetOpenUIViews();
            foreach (var uv in uiviews)
                if (uv.ViewId.Equals(view.Id))
                {
                    uiView = uv;
                    break;
                }

            if (uiView == null)
                return;

            var corners = uiView.GetZoomCorners();
            var bottomLeftCorner = corners[0];
            var topRightCorner = corners[1];
            var diagonal = (topRightCorner - bottomLeftCorner).Normalize();
            diagonal *= zoom;

            uiView.ZoomAndCenterRectangle(point - diagonal, point + diagonal);
        }

        public static View3D Get3dView(Document doc)
        {
            //TODO: zorgen voor altijd een juiste view, bijv voorkeur view met naam '{3D}' 
            var collector = new FilteredElementCollector(doc).OfClass(typeof(View3D));

            foreach (View3D v in collector)
                // Skip view template here because view templates are invisible in project browser
                if (!v.IsTemplate)
                    return v;

            return null;
        }


        public static IEnumerable<Room> GetRooms(Document doc)
        {
            var elements = GetElementsOfCategory(doc, BuiltInCategory.OST_Rooms);
            return elements.Select(e => e as Room);
        }

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


        public static bool SetParameter(Element element, string parameterName, int value)
        {
            var parameter = element.LookupParameter(parameterName);
            if (parameter != null)
                try
                {
                    switch (parameter.StorageType)
                    {
                        case StorageType.None:
                            return false;
                        case StorageType.Integer:
                            parameter.Set(value);
                            return true;
                        case StorageType.Double:
                            parameter.Set((double) value);
                            return true;
                        case StorageType.String:
                            return false;
                        case StorageType.ElementId:
                            parameter.Set(new ElementId(value));
                            return true;
                    }
                }
                catch
                {
                }

            return false;
        }

        public static void DrawLine(Document doc, XYZ start, XYZ end)
        {
            if (GeometryUtils.IsAlmostEqual(start, end))
                throw new ArgumentException("Expected two different points.");

            var line = Line.CreateBound(start, end);

            if (null == line)
                throw new Exception(
                    "Geometry line creation failed.");

            doc.Create.NewModelCurve(line, NewSketchPlanePassLine());

            SketchPlane NewSketchPlanePassLine()
            {
                XYZ norm;
                if (start.X == end.X)
                    norm = XYZ.BasisX;
                else if (start.Y == end.Y)
                    norm = XYZ.BasisY;
                else
                    norm = XYZ.BasisZ;

                var plane = Plane.CreateByNormalAndOrigin(norm, start);

                return SketchPlane.Create(doc, plane);
            }
        }

        public static string CurveToString(Curve curve)
        {
            var result = "Start: ";
            result += XYZToString(curve.GetEndPoint(0));
            result += "End: ";
            result += XYZToString(curve.GetEndPoint(1));
            result += Environment.NewLine;
            return result;
        }

        public static string XYZToString(XYZ point)
        {
            return "(" + point.X + ", " + point.Y + ", " + point.Z + ")";
        }

        public static double SquareFootToSquareM(double sqFoot, int decimals = 2)
        {
            return Math.Round(sqFoot * 0.092903, decimals);
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

        //Saves and closes, as well as removes annoying back-up files
        public static void SaveAndCloseDocument(Document doc, string destinationFilePath, bool overwrite = true)
        {
            var destinationDirectory = Path.GetDirectoryName(destinationFilePath);
            Directory.CreateDirectory(destinationDirectory);

            var saveAsOptions = new SaveAsOptions {OverwriteExistingFile = overwrite};
            doc.SaveAs(destinationFilePath, saveAsOptions);
            doc.Close();

            RemoveBackUpFilesFromDirectory(new FileInfo(destinationFilePath).Directory.FullName);
        }

        /// <summary>
        ///     Opens a Revit file and sets the main 3d view as the active view, with the visual style set to "Shaded"
        /// </summary>
        public static void OpenDocumentIntoShaded3DView(UIApplication uiApp, string filePath)
        {
            var uiDoc = uiApp.OpenAndActivateDocument(filePath);
            var doc = uiDoc.Document;
            var view = GetFirstOfType<View3D>(doc);

            if (view == null)
                return;

            //These UI commands must happen outside of a transaction
            uiDoc.ActiveView = view;
            //Close the view that opened when this document started if it's not the 3d view
            var openViews = uiDoc.GetOpenUIViews();
            foreach (var uiView in openViews)
                if (uiView.ViewId != view.Id)
                    uiView.Close();

            using (var transaction = new Transaction(doc))
            {
                transaction.Start("Set view graphics to Shaded");
                view.get_Parameter(BuiltInParameter.MODEL_GRAPHICS_STYLE).Set(3); //3 = Shaded
                transaction.Commit();
            }
        }

        public static void RemoveBackUpFilesFromDirectory(string directory)
        {
            //Remove annoying backup files
            foreach (var file in Directory.GetFiles(directory))
                for (var i = 1; i < 10; i++)
                    if (file.EndsWith($".000{i}.rvt"))
                        File.Delete(file);
        }

        public static string WriteXyz(XYZ vector)
        {
            return $"({vector.X:0.##}, {vector.Y:0.##}, {vector.Z:0.##})";
        }

        public static CopyPasteOptions UseDestinationOnDuplicateNameCopyPasteOptions()
        {
            var copyOptions = new CopyPasteOptions();
            copyOptions.SetDuplicateTypeNamesHandler(new UseDestinationHandler());
            return copyOptions;
        }
    }
}