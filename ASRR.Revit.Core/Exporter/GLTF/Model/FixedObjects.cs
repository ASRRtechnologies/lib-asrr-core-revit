using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Exporter.GLTF.Model
{
    public class FixedObjects
    {
        public FixedObjects()
        {
            this.ObjectsList = new List<FixedObject>();
        }

        public List<FixedObject> ObjectsList { get; set; }

        public int Count
        {
            get { return this.Count; }
            set { this.Count = this.ObjectsList.Count(); }
        }

        public Category Category
        {
            get { return this.Category; }
            set { this.Category = this.ObjectsList.FirstOrDefault().Category; }
        }
    }
}
