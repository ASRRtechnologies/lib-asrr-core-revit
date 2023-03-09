using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace DigiBase.Modelleerversneller.Revit.AddIn.RibbonPanelCreator
{
    public interface IRibbonPanelCreator
    {
        IEnumerable<RibbonPanel> Create(UIControlledApplication application, string tabName);
    }
}