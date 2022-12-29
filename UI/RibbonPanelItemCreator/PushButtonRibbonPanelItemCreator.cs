using System;
using System.Reflection;
using Autodesk.Revit.UI;

namespace ASRR.Revit.Core.UI.RibbonPanelItemCreator
{
    public class PushButtonRibbonPanelItemCreator<T> : IRibbonPanelItemCreator where T : IExternalCommand
    {
        private readonly ImageSource _imageSource;
        private readonly string _title;

        public PushButtonRibbonPanelItemCreator(string title, ImageSource imageSource)
        {
            _title = title;
            _imageSource = imageSource;
        }

        public void Create(RibbonPanel panel)
        {
            //Get the type and assembly for the command
            var commandType = typeof(T);
            var commandName = commandType.FullName;

            var commandAssembly = Assembly.GetAssembly(commandType);
            var commandAssemblyName = commandAssembly.CodeBase;
            var uri = new UriBuilder(commandAssemblyName);
            var path = Uri.UnescapeDataString(uri.Path);

            //Create PushButtonData first
            var pushButtonData = new PushButtonData(_title, _title, path, commandName);

            //Now add the button
            var pushButton = panel.AddItem(pushButtonData) as PushButton;
            pushButton.LargeImage = _imageSource;
        }
    }
}