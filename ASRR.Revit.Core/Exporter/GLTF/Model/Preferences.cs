using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Exporter.GLTF.Model
{
    public enum CompressionEnum
    {
        None,
        Meshopt,
        Draco,
        ZIP,
    }

    public enum FormatEnum
    {
        gltf,
        glb,
    }

    public class Preferences
    {
        public bool materials { get; set; } = true;

        public FormatEnum format { get; set; } = FormatEnum.glb;
        
        public bool normals { get; set; } = true;

        public bool levels { get; set; } = true;

        public bool grids { get; set; } = false;

        public bool batchId { get; set; } = true;

        public bool properties { get; set; } = false;

        public bool relocateTo0 { get; set; } = false;

        public bool flipAxis { get; set; } = true;

        public CompressionEnum compression { get; set; } = CompressionEnum.Draco;

        #if REVIT2019 || REVIT2020

        public DisplayUnitType units { get; set; }

        #else
        public ForgeTypeId units { get; set; } = UnitTypeId.Meters;

        #endif

        public string path { get; set; }

        public string fileName { get; set; }
    }
}
