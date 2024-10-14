using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Exporter.GLTF.Model
{
    public class MovableObjects
    {
        public MovableObjects()
        {
            this.ObjectsList = new List<MovableObject>();
        }

        public List<MovableObject> ObjectsList { get; set; }

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
