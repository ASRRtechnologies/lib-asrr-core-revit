using System.Collections.Generic;
using System.IO;
using System.Linq;
using ASRR.Revit.Core.Exporter.GLTF.Core;
using ASRR.Revit.Core.Exporter.GLTF.Model;

namespace ASRR.Revit.Core.Exporter.GLTF.Export
{
    internal class GlbFile
    {
        public static void Create(Preferences preferences, List<GLTFBinaryData> binaryFileData, string json)
        {
            byte[] jsonChunk = GlbJsonInfo.Get(json);
            byte[] binChunk = GlbBinInfo.Get(binaryFileData, preferences.normals, preferences.batchId);
            byte[] headerChunk = GlbHeaderInfo.Get(jsonChunk, binChunk);

            string fileDirectory = string.Concat(preferences.path, ".glb");
            byte[] exportArray = headerChunk.Concat(jsonChunk).Concat(binChunk).ToArray();

            File.WriteAllBytes(fileDirectory, exportArray);
        }
    }
}
