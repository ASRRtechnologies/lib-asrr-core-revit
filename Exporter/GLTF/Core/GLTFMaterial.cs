namespace ASRR.Revit.Core.Exporter.GLTF.Core
{
    /// <summary>
    /// The glTF PBR Material format
    /// https://github.com/KhronosGroup/glTF/tree/master/specification/2.0#materials.
    /// </summary>
    public class GLTFMaterial
    {
        public string alphaMode { get; set; }

        public float? alphaCutoff { get; set; }

        public string name { get; set; }

        public GLTFPBR pbrMetallicRoughness { get; set; }

        public bool doubleSided { get; set; }
    }
}
