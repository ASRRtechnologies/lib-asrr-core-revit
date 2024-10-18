﻿using ASRR.Revit.Core.Elements;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;

namespace ASRR.Revit.Core.Utilities
{
    public class DocumentUtilities
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        //Saves and closes, as well as removes annoying back-up files
        public static void SaveAndCloseDocument(Document doc, string destinationFilePath, bool overwrite = true)
        {
            var destinationDirectory = Path.GetDirectoryName(destinationFilePath);
            Directory.CreateDirectory(destinationDirectory);

            var saveAsOptions = new SaveAsOptions {OverwriteExistingFile = overwrite};
            doc.SaveAs(destinationFilePath, saveAsOptions);
            doc.Close(true);

            FileUtilities.RemoveBackUpFilesFromDirectory(new FileInfo(destinationFilePath).Directory.FullName);
        }

        /// <summary>
        ///     Opens a Revit file and sets the main 3d view as the active view, with the visual style set to "Shaded"
        /// </summary>
        public static void OpenDocumentIntoShaded3DView(UIApplication uiApp, string filePath)
        {
            var uiDoc = uiApp.OpenAndActivateDocument(filePath);
            var doc = uiDoc.Document;
            var view = Collector.GetFirstOfType<View3D>(doc);

            if (view == null)
                return;

            //These UI commands must happen outside of a transaction
            uiDoc.ActiveView = view;
            //Close the view that opened when this document started if it's not the 3d view
            var openViews = uiDoc.GetOpenUIViews();
            foreach (var uiView in openViews)
                if (uiView.ViewId != view.Id)
                    uiView.Close();

            using (var transaction = new Transaction(doc))
            {
                transaction.Start("Set view graphics to Shaded");
                view.get_Parameter(BuiltInParameter.MODEL_GRAPHICS_STYLE).Set(3); //3 = Shaded
                transaction.Commit();
            }
        }

        public static void DrawLine(Document doc, XYZ start, XYZ end)
        {
            if (GeometryUtils.IsAlmostEqual(start, end))
                throw new ArgumentException("Expected two different points.");

            var line = Line.CreateBound(start, end);

            if (null == line)
                throw new Exception(
                    "Geometry line creation failed.");

            doc.Create.NewModelCurve(line, NewSketchPlanePassLine());

            SketchPlane NewSketchPlanePassLine()
            {
                XYZ norm;
                if (start.X == end.X)
                    norm = XYZ.BasisX;
                else if (start.Y == end.Y)
                    norm = XYZ.BasisY;
                else
                    norm = XYZ.BasisZ;

                var plane = Plane.CreateByNormalAndOrigin(norm, start);

                return SketchPlane.Create(doc, plane);
            }
        }

        public static List<Element> GetSelection(UIDocument uiDoc)
        {
            var selection = uiDoc.Selection;
            var selectedIds = selection.GetElementIds();

            _log.Debug($"Selected {selectedIds.Count} elements");

            if (selectedIds.Count != 0) return Collector.GetElementsByIds(uiDoc.Document, selectedIds);

            TaskDialog.Show("Error", "Please select elements to export");
            return null;
        }

        public static CopyPasteOptions CopyPasteOptions()
        {
            var copyPasteOptions = new CopyPasteOptions();
            copyPasteOptions.SetDuplicateTypeNamesHandler(new UseDestinationHandler());
            return copyPasteOptions;
        }

        public DuplicateTypeAction OnDuplicateTypeNamesFound()
        {
            return DuplicateTypeAction.UseDestinationTypes;
        }
    }
}