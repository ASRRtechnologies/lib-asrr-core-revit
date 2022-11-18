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
            Document doc = uidoc.Document;
            View view = doc.ActiveView;
            UIView uiView = null;

            IList<UIView> uiviews = uidoc.GetOpenUIViews();
            foreach (UIView uv in uiviews)
            {
                if (uv.ViewId.Equals(view.Id))
                {
                    uiView = uv;
                    break;
                }
            }

            if (uiView == null)
                return;

            IList<XYZ> corners = uiView.GetZoomCorners();
            XYZ bottomLeftCorner = corners[0];
            XYZ topRightCorner = corners[1];
            XYZ diagonal = (topRightCorner - bottomLeftCorner).Normalize();
            diagonal *= zoom;

            uiView.ZoomAndCenterRectangle(point - diagonal, point + diagonal);
        }

        public static View3D Get3dView(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc).OfClass(typeof(View3D));

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
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ElementCategoryFilter filter = new ElementCategoryFilter(category);
            return collector.WherePasses(filter).ToElements();
        }

        public static ICollection<Element> GetElementsOfCategories(Document doc,
            ICollection<BuiltInCategory> categories)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ElementMulticategoryFilter filter = new ElementMulticategoryFilter(categories);
            return collector.WherePasses(filter).ToElements();
        }


        public static bool SetParameter(Element element, string parameterName, int value)
        {
            Parameter parameter = element.LookupParameter(parameterName);
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

            Line line = Line.CreateBound(start, end);

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

                Plane plane = Plane.CreateByNormalAndOrigin(norm, start);

                return SketchPlane.Create(doc, plane);
            }
        }

        public static string CurveToString(Curve curve)
        {
            string result = "Start: ";
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