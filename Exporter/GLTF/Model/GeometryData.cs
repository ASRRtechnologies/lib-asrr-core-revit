using System.Collections.Generic;

namespace ASRR.Revit.Core.Exporter.GLTF.Model
{
    /// <summary>
    /// Intermediate data format for 
    /// converting between Revit Polymesh
    /// and glTF buffers.
    /// </summary>
    public class GeometryData
    {
        public VertexLookupInt vertDictionary = new VertexLookupInt();
        public List<long> vertices = new List<long>();
        public List<double> normals = new List<double>();
        public List<double> uvs = new List<double>();
        public List<int> faces = new List<int>();
    }
}