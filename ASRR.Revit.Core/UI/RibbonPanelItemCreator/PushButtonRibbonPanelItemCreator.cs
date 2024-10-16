using Autodesk.Revit.UI;
using System;
using System.Reflection;
using System.Windows.Media;

namespace ASRR.Revit.Core.UI.RibbonPanelItemCreator
{
    public class PushButtonRibbonPanelItemCreator<T> : IRibbonPanelItemCreator where T : IExternalCommand
    {
        private readonly ImageSource _imageSource;
        private readonly string _title;
        private readonly string _description;

        public PushButtonRibbonPanelItemCreator(string title, ImageSource imageSource, string description = "")
        {
            _title = title;
            _imageSource = imageSource;
            _description = title;
            if (!string.IsNullOrEmpty(description))
                _description = string.Format("{0}\n{1}", _title, description)
                    .Trim();
        }

        public void Create(RibbonPanel panel)
        {
            //Get the type and assembly for the command
            var commandType = typeof(T);
            var commandName = commandType.FullName;

            var commandAssembly = Assembly.GetAssembly(commandType);
            #if NET48_OR_GREATER
            var commandAssemblyName = commandAssembly.CodeBase;
            #else
            var commandAssemblyName = commandAssembly.Location;
            #endif
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