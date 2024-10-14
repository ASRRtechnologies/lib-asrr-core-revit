using ASRR.Revit.Core.Exporter.GLTF.Model;

namespace ASRR.Revit.Core.Exporter.GLTF.Export
{
    public class Compression
    {
        /// <summary>
        /// Run compression.
        /// </summary>
        /// <param name="preferences">preferences.</param>
        public static void Run(Preferences preferences)
        {
            switch (preferences.compression)
            {
                case CompressionEnum.ZIP:
                    ZIP.Compress(preferences);
                    break;
                case CompressionEnum.Draco:
                    Draco.Compress(preferences);
                    break;
                case CompressionEnum.Meshopt:
                    MeshOpt.Compress(preferences);
                    break;
                default:
                    break;
            }
        }
    }
}
