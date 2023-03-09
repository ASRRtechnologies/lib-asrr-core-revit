using ASRR.Revit.Core.Exporter.Groups.Model;
using ASRR.Revit.Core.Model;
using ASRR.Revit.Core.Utilities;
using ASRR.Revit.Core.Warnings;
using Autodesk.Revit.DB;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ASRR.Revit.Core.Exporter.Groups.Service
{
    /// <summary>
    ///     Copy-pastes grouptypes from one file to another by copy-pasting it's grouptypes and instantiating those
    /// </summary>
    public class GroupTypeCopyPaster
    {
        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public List<GroupTypeSet> CopyGroupTypeSets(Document sourceDoc, Document destinationDoc,
            IEnumerable<GroupTypeSet> groupTypeSets)
        {
            var copiedGroupTypeSets = new List<GroupTypeSet>();

            using (var transaction = WarningDiscardFailuresPreprocessor.GetTransaction(destinationDoc, _logger))
            {
                transaction.Start("Copy paste grouptypes");
                foreach (var groupTypeSet in groupTypeSets)
                {
                    var copied = CopyGroupTypeSetOrGetExisting(groupTypeSet, sourceDoc, destinationDoc,
                        ref copiedGroupTypeSets);
                    if (copied == false)
                    {
                        transaction.RollBack();
                        return null;
                    }
                }

                transaction.Commit();
            }

            return copiedGroupTypeSets;
        }

        //Create an instance of the modelgrouptype and enable the detailgroups in the appropriate views
        public Group PlaceModelGroup(Document doc, GroupTypeSet groupTypeSet, IPosition position,
            IRotation rotation = null)
        {
            if (doc == null || groupTypeSet?.ModelGroupType == null)
            {
                _logger.Error("Could not place modelgroup: input was null");
                return null;
            }

            Group modelGroup;

            using (var transaction = WarningDiscardFailuresPreprocessor.GetTransaction(doc, _logger))
            {
                transaction.Start("Instantiate modelgroup");

                modelGroup = doc.Create.PlaceGroup(groupTypeSet.PositionOffset.PositionInFeet,
                    groupTypeSet.ModelGroupType);

                if (rotation != null)
                    TransformUtilities.RotateGlobal(modelGroup, rotation);

                TransformUtilities.Move(modelGroup, position);

                if (groupTypeSet.AttachedDetailGroupTypes.Count > 0)
                    EnableAttachedDetailGroupsInFloorPlans(doc, modelGroup, groupTypeSet.AttachedDetailGroupTypes);

                transaction.Commit();
            }

            return modelGroup;
        }

        //If the grouptype(s) already exist in the destination file, create a grouptypeset with the existing grouptype(s)
        private GroupTypeSet CreateExistingGroupTypeSet(GroupTypeSet groupTypeSet, List<GroupType> existingGroupTypes)
        {
            var existingModelGroupType =
                existingGroupTypes.Find(gt => gt.Name == groupTypeSet.ModelGroupType.Name);

            if (existingModelGroupType == null)
                return null;

            var attachedDetailGroupTypes = new List<AttachedDetailGroupType>();
            foreach (var detailGroupType in groupTypeSet.AttachedDetailGroupTypes)
            {
                var existingDetailGroupType = existingGroupTypes
                    .Find(gt => gt.Name == detailGroupType.GroupType.Name);
                if (existingDetailGroupType == null)
                {
                    _logger.Error($"Could not find detailgroup '{detailGroupType.GroupType.Name}'");
                    continue;
                }

                attachedDetailGroupTypes.Add(new AttachedDetailGroupType
                {
                    GroupType = existingDetailGroupType,
                    FloorPlanName = detailGroupType.FloorPlanName
                });
            }

            return new GroupTypeSet
            {
                ModelGroupType = existingModelGroupType,
                AttachedDetailGroupTypes = attachedDetailGroupTypes,
                PositionOffset = groupTypeSet.PositionOffset
            };
        }

        private bool CopyGroupTypeSetOrGetExisting(GroupTypeSet groupTypeSet, Document sourceDoc,
            Document destinationDoc, ref List<GroupTypeSet> copiedGroupTypeSets)
        {
            var existingGroupTypes =
                Collector.GetAllOfType<GroupType>(destinationDoc);

            var existingGroupTypeSet = CreateExistingGroupTypeSet(groupTypeSet, existingGroupTypes);
            //If the modelgrouptype already exists in the destination document, use it instead of copying it in again
            if (existingGroupTypeSet != null)
            {
                copiedGroupTypeSets.Add(existingGroupTypeSet);
            }

            //It doesn't already exist, copy the types over
            else
            {
                var copiedGroupTypeSet = CopyGroupTypeSet(sourceDoc, destinationDoc, groupTypeSet);
                if (copiedGroupTypeSet == null) return false;

                copiedGroupTypeSets.Add(copiedGroupTypeSet);

                //Don't forget to add the new grouptype(s) to the set of existing ones so duplicates don't get copied over again
                existingGroupTypes.Add(copiedGroupTypeSet.ModelGroupType);
                existingGroupTypes.AddRange(
                    copiedGroupTypeSet.AttachedDetailGroupTypes.Select(detailGroupType =>
                        detailGroupType.GroupType));
            }

            return true;
        }

        private GroupTypeSet CopyGroupTypeSet(Document sourceDoc, Document destinationDoc, GroupTypeSet groupTypeSet)
        {
            ICollection<ElementId> copiedIds = new List<ElementId>();

            var copyOptions = DocumentUtilities.CopyPasteOptions();

            var ids = new List<ElementId> { groupTypeSet.ModelGroupType.Id };
            ids.AddRange(
                groupTypeSet.AttachedDetailGroupTypes.Select(detailGroupType => detailGroupType.GroupType.Id));

            try
            {
                copiedIds = ElementTransformUtils.CopyElements(sourceDoc, ids, destinationDoc, Transform.Identity,
                    copyOptions);
                //Can go wrong
            }
            catch (Exception e)
            {
                _logger.Error(
                    $"Failed to copy grouptypeset '{groupTypeSet.ModelGroupType.Name}', because: '{e.Message}'");
                return null;
            }

            var copiedGroupTypes =
                copiedIds.Select(id => destinationDoc.GetElement(id) as GroupType).ToList();

            return CreateCopiedGroupTypeSet(groupTypeSet, copiedGroupTypes);
        }

        private GroupTypeSet CreateCopiedGroupTypeSet(GroupTypeSet groupTypeSet, List<GroupType> copiedGroupTypes)
        {
            var copiedGroupType = FindGroupType(copiedGroupTypes, groupTypeSet.ModelGroupType.Name);
            if (copiedGroupType == null) return null;

            var attachedDetailGroupTypes =
                FindAttachedDetailGroupTypes(groupTypeSet.AttachedDetailGroupTypes, copiedGroupTypes);

            var copiedGroupTypeSet = new GroupTypeSet
            {
                ModelGroupType = copiedGroupType,
                PositionOffset = groupTypeSet.PositionOffset,
                AttachedDetailGroupTypes = attachedDetailGroupTypes
            };

            return copiedGroupTypeSet;
        }

        private GroupType FindGroupType(List<GroupType> copiedGroupTypes, string name)
        {
            if (copiedGroupTypes == null)
            {
                _logger.Error("Parameter 'copiedGroupTypes' was null");
                return null;
            }

            var groupType = copiedGroupTypes.Count == 1
                ? copiedGroupTypes.First()
                : copiedGroupTypes.FirstOrDefault(type => type.Name == name);

            if (groupType == null)
                _logger.Error("Found grouptype was null or grouptype could not be found");

            return groupType;
        }

        private List<AttachedDetailGroupType> FindAttachedDetailGroupTypes(
            List<AttachedDetailGroupType> detailGroupTypes, List<GroupType> groupTypes)
        {
            var attachedDetailGroupTypes = new List<AttachedDetailGroupType>();
            foreach (var detailGroupType in detailGroupTypes)
            {
                var copiedDetailGroupType = groupTypes.FirstOrDefault(type =>
                    type.Name == detailGroupType.GroupType.Name);

                if (copiedDetailGroupType == null)
                {
                    _logger.Error($"Could not find copied detail group '{detailGroupType.GroupType.Name}'");
                    continue;
                }

                attachedDetailGroupTypes.Add(new AttachedDetailGroupType
                {
                    GroupType = copiedDetailGroupType,
                    FloorPlanName = detailGroupType.FloorPlanName
                });
            }

            return attachedDetailGroupTypes;
        }

        private void EnableAttachedDetailGroupsInFloorPlans(Document doc, Group modelGroup,
            List<AttachedDetailGroupType> attachedDetailGroupTypes)
        {
            var allFloorPlans = Collector.GetAllOfType<ViewPlan>(doc)
                .Where(v => v.ViewType == ViewType.FloorPlan).ToList();

            foreach (var detailGroupType in attachedDetailGroupTypes)
            {
                if (detailGroupType.GroupType == null)
                {
                    _logger.Error("detail grouptype was null");
                    return;
                }

                var floorPlan = allFloorPlans.FirstOrDefault(v => v.Name == detailGroupType.FloorPlanName);

                if (modelGroup == null)
                    _logger.Error("Could not find modelgroup. It's likely that it couldn't be successfully placed");
                else if (floorPlan == null)
                    _logger.Error(
                        $"Could not enable attached detail group because viewplan '{detailGroupType.FloorPlanName}' does not exist");
                else
                    try
                    {
                        modelGroup.ShowAttachedDetailGroups(floorPlan, detailGroupType.GroupType.Id);
                    }
                    catch (Exception e)
                    {
                        _logger.Error($"Failed to enable attached detail group because: '{e.Message}'");
                    }
            }
        }
    }
}