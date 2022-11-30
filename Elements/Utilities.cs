using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System;
using System.Linq;
using Autodesk.Revit.ApplicationServices;

namespace ASRR.Revit.Core.Elements
{
    public class Utilities
    {
        public static void ZoomToPoint(UIDocument uidoc, XYZ point, float zoom)
        {
            var doc = uidoc.Document;
            var view = doc.ActiveView;
            UIView uiView = null;

            var uiviews = uidoc.GetOpenUIViews();
            foreach (var uv in uiviews)
            {
                if (uv.ViewId.Equals(view.Id))
                {
                    uiView = uv;
                    break;
                }
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
            {
                // Skip view template here because view templates are invisible in project browser
                if (!v.IsTemplate)
                    return v;
            }

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
            {
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
                            parameter.Set((double)value);
                            return true;
                        case StorageType.String:
                            return false;
                        case StorageType.ElementId:
                            parameter.Set(new ElementId(value));
                            return true;
                        default:
                            break;
                    }
                }
                catch
                {
                }
            }

            return false;
        }

        public static void DrawLine(Document doc, XYZ start, XYZ end)
        {
            if (GeometryUtils.IsAlmostEqual(start, end))
                throw new ArgumentException("Expected two different points.");

            var line = Line.CreateBound(start, end);

            if (null == line)
            {
                throw new Exception(
                    "Geometry line creation failed.");
            }

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
    }
}