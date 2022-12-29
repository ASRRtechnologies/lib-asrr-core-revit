using System.Collections.Generic;

namespace ASRR.Revit.Core.Exporter.GLTF.Model
{
    /// <summary>
    /// From Jeremy Tammik's RvtVa3c exporter:
    /// https://github.com/va3c/RvtVa3c
    /// A vertex lookup class to eliminate 
    /// duplicate vertex definitions.
    /// </summary>
    public class VertexLookupInt : Dictionary<PointInt, int>
    {
        /// <summary>
        /// Define equality for integer-based PointInt.
        /// </summary>
        class PointIntEqualityComparer : IEqualityComparer<PointInt>
        {
            public bool Equals(PointInt p, PointInt q)
            {
                return 0 == p.CompareTo(q);
            }

            public int GetHashCode(PointInt p)
            {
                return (p.X.ToString()
                        + "," + p.Y.ToString()
                        + "," + p.Z.ToString())
                    .GetHashCode();
            }
        }

        public VertexLookupInt() : base(new PointIntEqualityComparer())
        {
        }

        /// <summary>
        /// Return the index of the given vertex,
        /// adding a new entry if required.
        /// </summary>
        public int AddVertex(PointInt p)
        {
            return ContainsKey(p)
                ? this[p]
                : this[p] = Count;
        }
    }
}