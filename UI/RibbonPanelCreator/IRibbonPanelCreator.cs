using System.Collections.Generic;
using Autodesk.Revit.UI;

namespace DigiBase.Modelleerversneller.Revit.AddIn.RibbonPanelCreator
{
    public interface IRibbonPanelCreator
    {
        IEnumerable<RibbonPanel> Create(UIControlledApplication application, string tabName);
    }
}