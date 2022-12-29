using System;
using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Exporter.GLTF.Model
{
    /// <summary>
    ///     From Jeremy Tammik's RvtVa3c exporter:
    ///     https://github.com/va3c/RvtVa3c
    ///     An integer-based 3D point class.
    /// </summary>
    public class PointInt : IComparable<PointInt>
    {
        /// <summary>
        ///     Consider a Revit length zero
        ///     if is smaller than this.
        /// </summary>
        private const double _eps = 1.0e-9;

        /// <summary>
        ///     Conversion factor from feet to millimetres.
        /// </summary>
        private const double _feet_to_mm = 25.4 * 12;

        public PointInt(XYZ p, bool switch_coordinates)
        {
            X = ConvertFeetToMillimetres(p.X);
            Y = ConvertFeetToMillimetres(p.Y);
            Z = ConvertFeetToMillimetres(p.Z);

            if (switch_coordinates)
            {
                X = -X;
                var tmp = Y;
                Y = Z;
                Z = tmp;
            }
        }

        public long X { get; set; }
        public long Y { get; set; }
        public long Z { get; set; }

        public int CompareTo(PointInt a)
        {
            var d = X - a.X;
            if (0 == d)
            {
                d = Y - a.Y;
                if (0 == d) d = Z - a.Z;
            }

            return 0 == d ? 0 : 0 < d ? 1 : -1;
        }

        /// <summary>
        ///     Conversion a given length value
        ///     from feet to millimetre.
        /// </summary>
        public static long ConvertFeetToMillimetres(double d)
        {
            if (0 < d)
                return _eps > d
                    ? 0
                    : (long) (_feet_to_mm * d + 0.5);
            return _eps > -d
                ? 0
                : (long) (_feet_to_mm * d - 0.5);
        }
    }
}