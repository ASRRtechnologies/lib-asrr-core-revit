using System.Collections.Generic;
using ASRR.Revit.Core.Model;
using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Exporter.Groups.Model
{
    /// <summary>
    /// Dto that contains all information about a modelgroup to properly copy it over to a new file
    /// </summary>
    public class GroupTypeSet
    {
        public GroupType ModelGroupType { get; set; }

        /// <summary>
        /// Stores the offset of the modelgroup origin point to the origin of the file
        /// </summary>
        public IPosition PositionOffset { get; set; }

        /// <summary>
        /// Stores the detailgroup type that is associated with this modelgroup's type
        /// </summary>
        public List<AttachedDetailGroupType> AttachedDetailGroupTypes { get; set; }
    }
}