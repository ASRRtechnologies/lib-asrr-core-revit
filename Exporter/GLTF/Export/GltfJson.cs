using System.Collections.Generic;
using System.Linq;
using ASRR.Revit.Core.Exporter.GLTF.Core;
using Newtonsoft.Json;

namespace ASRR.Revit.Core.Exporter.GLTF.Export
{
    public static class GltfJson
    {
        public static string Get(
            List<GLTFScene> scenes,
            List<GLTFNode> nodes,
            List<GLTFMesh> meshes,
            List<GLTFMaterial> materials,
            List<GLTFBuffer> buffers,
            List<GLTFBufferView> bufferViews,
            List<GLTFAccessor> accessors,
            Preferences preferences)
        {
            // Package the properties into a serializable container
            Core.GLTF model = new Core.GLTF
            {
                asset = new GLTFVersion(),
                scenes = scenes,
                nodes = nodes,
                meshes = meshes,
            };

            if (materials.Any())
            {
                model.materials = materials;
            }

            model.buffers = buffers;
            model.bufferViews = bufferViews;
            model.accessors = accessors;

            // Write the *.gltf file
            string serializedModel = JsonConvert.SerializeObject(
                model,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });


            if (!preferences.batchId)
            {
                serializedModel = serializedModel.Replace(",\"_BATCHID\":0", string.Empty);
            }

            if (!preferences.normals)
            {
                serializedModel = serializedModel.Replace(",\"NORMAL\":0", string.Empty);
            }

            return serializedModel;
        }
    }
}
