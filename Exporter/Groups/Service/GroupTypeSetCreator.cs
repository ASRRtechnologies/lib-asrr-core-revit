using System.Collections.Generic;
using System.Linq;
using ASRR.Revit.Core.Elements;
using ASRR.Revit.Core.Exporter.Groups.Model;
using ASRR.Revit.Core.Model;
using Autodesk.Revit.DB;

namespace ASRR.Revit.Core.Exporter.Groups.Service
{
    /// <summary>
    /// Creates a grouptypeset dto from a given modelgroup
    /// </summary>
    public class GroupTypeSetCreator
    {
        /// <param name="doc"></param>
        /// <param name="modelGroup"></param>
        /// <param name="includeOffset">Indicates whether the offset of the group relative to the origin of the document should be accounted for</param>
        /// <returns></returns>
        public GroupTypeSet Create(Document doc, Group modelGroup, bool includeOffset)
        {
            List<AttachedDetailGroupType> detailGroupTypes = GetAttachedDetailGroupTypes(doc, modelGroup);

            GroupTypeSet groupTypeSet = new GroupTypeSet
            {
                ModelGroupType = modelGroup.GroupType,
                PositionOffset = CreatePosition(modelGroup, includeOffset),
                AttachedDetailGroupTypes = detailGroupTypes
            };

            return groupTypeSet;
        }

        private static IPosition CreatePosition(Group modelGroup, bool includeOffset)
        {
            IPosition position = TransformUtilities.GetPosition(modelGroup);

            if (!includeOffset)
                return new MillimeterPosition(new XYZ(0, 0, position.PositionInMillimeters.Z));

            return position;
        }

        private List<AttachedDetailGroupType> GetAttachedDetailGroupTypes(Document doc, Group modelGroup)
        {
            List<AttachedDetailGroupType> detailGroupTypes = new List<AttachedDetailGroupType>();
            List<Group> detailGroups = FindAttachedDetailGroups(doc, modelGroup);
            List<ViewPlan> allFloorPlans = Utilities.GetAllOfType<ViewPlan>(doc)
                .Where(v => v.ViewType == ViewType.FloorPlan).ToList();

            foreach (Group detailGroup in detailGroups)
            {
                ElementId ownerViewId = detailGroup.OwnerViewId;
                ViewPlan ownerFloorPlan = allFloorPlans.FirstOrDefault(floorplan => floorplan.Id == ownerViewId);

                if (ownerFloorPlan != null)
                    detailGroupTypes.Add(new AttachedDetailGroupType
                    {
                        GroupType = detailGroup.GroupType,
                        FloorPlanName = ownerFloorPlan.Name
                    });
            }

            return detailGroupTypes;
        }

        private List<Group> FindAttachedDetailGroups(Document doc, Group modelGroup)
        {
            ElementFilter familyCategoryFilter = new ElementCategoryFilter(BuiltInCategory.OST_IOSAttachedDetailGroups);
            ICollection<Element> detailGroups =
                new FilteredElementCollector(doc).WherePasses(familyCategoryFilter).ToElements();

            return detailGroups.Where(dg => HasGroupAsParent(dg, modelGroup)).Select(dg => dg as Group).ToList();
        }

        private bool HasGroupAsParent(Element detailGroup, Element modelGroup)
        {
            Parameter groupNameParameter = detailGroup.get_Parameter(BuiltInParameter.GROUP_ATTACHED_PARENT_NAME);
            if (groupNameParameter == null)
                return false;

            return modelGroup.Name == groupNameParameter.AsString();
        }
    }
}