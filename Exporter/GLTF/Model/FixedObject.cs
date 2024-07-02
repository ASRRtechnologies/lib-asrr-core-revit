﻿using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Exporter.GLTF.Model
{
    public class FixedObject : IObject
    {
        public Category Category { get; set; }

        public string FamilySymbol { get; set; }

        public string ElementName { get; set; }

        public ElementId EId { get; set; }

        public Location Location { get; set; }
    }
}
