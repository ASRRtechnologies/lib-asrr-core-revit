using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Exporter.GLTF.Model
{
    public interface IObjects<T>
    {
        List<T> ObjectsList { get; set; }

        int Count { get; set; }

        Category Category { get; set; }
    }
}
