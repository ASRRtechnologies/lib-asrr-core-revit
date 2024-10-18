using ASRR.Revit.Core.Exporter.GLTF.Core;
using ASRR.Revit.Core.Exporter.GLTF.Model;
using Autodesk.Revit.DB;
using Common_glTF_Exporter.Utils;

namespace ASRR.Revit.Core.Exporter.GLTF.Utils
{
    public class MaterialUtils
    {
        public static Material GetMeshMaterial(Document doc, Mesh mesh)
        {
            ElementId materialId = mesh.MaterialElementId;

            if (materialId != null)
            {
                return doc.GetElement(materialId) as Material;
            }
            else
            {
                return null;
            }
        }

        public static void SetMaterial(Document doc, Preferences preferences, Mesh mesh, IndexedDictionary<GLTFMaterial> materials, bool doubleSided)
        {
            GLTFMaterial gl_mat = new GLTFMaterial();

            Material material = MaterialUtils.GetMeshMaterial(doc, mesh);

            if (preferences.materials)
            {
                if (material == null)
                {
                    material = Collectors.GetRandomMaterial(doc);
                }

                gl_mat = GLTFExportUtils.GetGLTFMaterial(materials.List, material, doubleSided);

                materials.AddOrUpdateCurrentMaterial(material.UniqueId, gl_mat, doubleSided);
            }
        }
    }
}
