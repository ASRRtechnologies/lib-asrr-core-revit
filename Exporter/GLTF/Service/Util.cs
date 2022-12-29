using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace glTFRevitExport
{
    internal class Util
    {
        public static int[] GetVec3MinMax(List<int> vec3)
        {
            var minVertexX = int.MaxValue;
            var minVertexY = int.MaxValue;
            var minVertexZ = int.MaxValue;
            var maxVertexX = int.MinValue;
            var maxVertexY = int.MinValue;
            var maxVertexZ = int.MinValue;
            for (var i = 0; i < vec3.Count; i += 3)
            {
                if (vec3[i] < minVertexX) minVertexX = vec3[i];
                if (vec3[i] > maxVertexX) maxVertexX = vec3[i];

                if (vec3[i + 1] < minVertexY) minVertexY = vec3[i + 1];
                if (vec3[i + 1] > maxVertexY) maxVertexY = vec3[i + 1];

                if (vec3[i + 2] < minVertexZ) minVertexZ = vec3[i + 2];
                if (vec3[i + 2] > maxVertexZ) maxVertexZ = vec3[i + 2];
            }

            return new[] {minVertexX, maxVertexX, minVertexY, maxVertexY, minVertexZ, maxVertexZ};
        }

        public static long[] GetVec3MinMax(List<long> vec3)
        {
            var minVertexX = long.MaxValue;
            var minVertexY = long.MaxValue;
            var minVertexZ = long.MaxValue;
            var maxVertexX = long.MinValue;
            var maxVertexY = long.MinValue;
            var maxVertexZ = long.MinValue;
            for (var i = 0; i < vec3.Count / 3; i += 3)
            {
                if (vec3[i] < minVertexX) minVertexX = vec3[i];
                if (vec3[i] > maxVertexX) maxVertexX = vec3[i];

                if (vec3[i + 1] < minVertexY) minVertexY = vec3[i + 1];
                if (vec3[i + 1] > maxVertexY) maxVertexY = vec3[i + 1];

                if (vec3[i + 2] < minVertexZ) minVertexZ = vec3[i + 2];
                if (vec3[i + 2] > maxVertexZ) maxVertexZ = vec3[i + 2];
            }

            return new[] {minVertexX, maxVertexX, minVertexY, maxVertexY, minVertexZ, maxVertexZ};
        }

        public static float[] GetVec3MinMax(List<float> vec3)
        {
            var xValues = new List<float>();
            var yValues = new List<float>();
            var zValues = new List<float>();
            for (var i = 0; i < vec3.Count; i++)
            {
                if (i % 3 == 0) xValues.Add(vec3[i]);
                if (i % 3 == 1) yValues.Add(vec3[i]);
                if (i % 3 == 2) zValues.Add(vec3[i]);
            }

            var maxX = xValues.Max();
            var minX = xValues.Min();
            var maxY = yValues.Max();
            var minY = yValues.Min();
            var maxZ = zValues.Max();
            var minZ = zValues.Min();

            return new[] {minX, maxX, minY, maxY, minZ, maxZ};
        }

        public static int[] GetScalarMinMax(List<int> scalars)
        {
            var minFaceIndex = int.MaxValue;
            var maxFaceIndex = int.MinValue;
            for (var i = 0; i < scalars.Count; i++)
            {
                var currentMin = Math.Min(minFaceIndex, scalars[i]);
                if (currentMin < minFaceIndex) minFaceIndex = currentMin;

                var currentMax = Math.Max(maxFaceIndex, scalars[i]);
                if (currentMax > maxFaceIndex) maxFaceIndex = currentMax;
            }

            return new[] {minFaceIndex, maxFaceIndex};
        }

        /// <summary>
        ///     From Jeremy Tammik's RvtVa3c exporter:
        ///     https://github.com/va3c/RvtVa3c
        ///     Return a string for a real number
        ///     formatted to two decimal places.
        /// </summary>
        public static string RealString(double a)
        {
            return a.ToString("0.##");
        }

        /// <summary>
        ///     From Jeremy Tammik's RvtVa3c exporter:
        ///     https://github.com/va3c/RvtVa3c
        ///     Return a string for an XYZ point
        ///     or vector with its coordinates
        ///     formatted to two decimal places.
        /// </summary>
        public static string PointString(XYZ p)
        {
            return string.Format("({0},{1},{2})",
                RealString(p.X),
                RealString(p.Y),
                RealString(p.Z));
        }

        /// <summary>
        ///     From Jeremy Tammik's RvtVa3c exporter:
        ///     https://github.com/va3c/RvtVa3c
        ///     Return an integer value for a Revit Color.
        /// </summary>
        public static int ColorToInt(Color color)
        {
            return (color.Red << 16)
                   | (color.Green << 8)
                   | color.Blue;
        }

        /// <summary>
        ///     From Jeremy Tammik's RvtVa3c exporter:
        ///     https://github.com/va3c/RvtVa3c
        ///     Extract a true or false value from the given
        ///     string, accepting yes/no, Y/N, true/false, T/F
        ///     and 1/0. We are extremely tolerant, i.e., any
        ///     value starting with one of the characters y, n,
        ///     t or f is also accepted. Return false if no
        ///     valid Boolean value can be extracted.
        /// </summary>
        public static bool GetTrueOrFalse(string s, out bool val)
        {
            val = false;

            if (s.Equals(bool.TrueString,
                    StringComparison.OrdinalIgnoreCase))
            {
                val = true;
                return true;
            }

            if (s.Equals(bool.FalseString,
                    StringComparison.OrdinalIgnoreCase))
                return true;

            if (s.Equals("1"))
            {
                val = true;
                return true;
            }

            if (s.Equals("0")) return true;

            s = s.ToLower();

            if ('t' == s[0] || 'y' == s[0])
            {
                val = true;
                return true;
            }

            if ('f' == s[0] || 'n' == s[0]) return true;

            return false;
        }

        /// <summary>
        ///     From Jeremy Tammik's RvtVa3c exporter:
        ///     https://github.com/va3c/RvtVa3c
        ///     Return a string describing the given element:
        ///     .NET type name,
        ///     category name,
        ///     family and symbol name for a family instance,
        ///     element id and element name.
        /// </summary>
        public static string ElementDescription(Element e)
        {
            if (null == e) return "<null>";

            // For a wall, the element name equals the
            // wall type name, which is equivalent to the
            // family name ...

            var fi = e as FamilyInstance;

            var typeName = e.GetType().Name;

            var categoryName = null == e.Category
                ? string.Empty
                : e.Category.Name + " ";

            var familyName = null == fi
                ? string.Empty
                : fi.Symbol.Family.Name + " ";

            var symbolName = null == fi
                             || e.Name.Equals(fi.Symbol.Name)
                ? string.Empty
                : fi.Symbol.Name + " ";

            return string.Format("{0} {1}{2}{3}<{4} {5}>",
                typeName, categoryName, familyName,
                symbolName, e.Id.IntegerValue, e.Name);
        }

        /// <summary>
        ///     From Jeremy Tammik's RvtVa3c exporter:
        ///     https://github.com/va3c/RvtVa3c
        ///     Return a dictionary of all the given
        ///     element parameter names and values.
        /// </summary>
        public static Dictionary<string, string> GetElementProperties(Element e, bool includeType)
        {
            var parameters
                = e.GetOrderedParameters();

            var a = new Dictionary<string, string>(parameters.Count);

            // Add element category
            if (e.Category != null) a.Add("Element Category", e.Category.Name);


            foreach (var p in parameters)
            {
                var key = p.Definition.Name;

                if (!a.ContainsKey(key))
                {
                    string val;
                    if (StorageType.String == p.StorageType)
                        val = p.AsString();
                    else
                        val = p.AsValueString();

                    if (!string.IsNullOrEmpty(val)) a.Add(key, val);
                }
            }

            if (includeType)
            {
                var idType = e.GetTypeId();

                if (idType != null && ElementId.InvalidElementId != idType)
                {
                    var doc = e.Document;
                    var typ = doc.GetElement(idType);
                    parameters = typ.GetOrderedParameters();
                    foreach (var p in parameters)
                    {
                        var key = "Type " + p.Definition.Name;

                        if (!a.ContainsKey(key))
                        {
                            string val;
                            if (StorageType.String == p.StorageType)
                                val = p.AsString();
                            else
                                val = p.AsValueString();

                            if (!string.IsNullOrEmpty(val)) a.Add(key, val);
                        }
                    }
                }
            }

            if (a.Count == 0) return null;
            return a;
        }
    }
}