using ASRR.Revit.Core.UI.RibbonPanelItemCreator;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace DigiBase.Modelleerversneller.Revit.AddIn.RibbonPanelCreator
{
    public class DictionaryBasedRibbonPanelsCreator : IRibbonPanelCreator
    {
        private readonly Dictionary<string, List<IRibbonPanelItemCreator>> _itemCreatorsPerPanelName;

        public DictionaryBasedRibbonPanelsCreator(
            Dictionary<string, List<IRibbonPanelItemCreator>> itemCreatorsPerPanelName)
        {
            _itemCreatorsPerPanelName = itemCreatorsPerPanelName;
        }

        public IEnumerable<RibbonPanel> Create(UIControlledApplication application, string tabName)
        {
            // initialize the result
            var result = new List<RibbonPanel>();

            // iterate all items, create the panels and the items for each panel
            foreach (var panelName in _itemCreatorsPerPanelName.Keys)
            {
                var panel = application.CreateRibbonPanel(tabName, panelName);
                result.Add(panel);

                var panelItemCreators = _itemCreatorsPerPanelName[panelName];

                foreach (var panelItemCreator in panelItemCreators) panelItemCreator.Create(panel);
            }

            return result;
        }
    }
}