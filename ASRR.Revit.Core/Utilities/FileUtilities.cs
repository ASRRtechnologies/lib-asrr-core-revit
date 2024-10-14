using System.IO;

namespace ASRR.Revit.Core.Utilities
{
    public class FileUtilities
    {
        /// <summary>
        ///     Removes all backup files from a directory
        ///     <para>Example: "C:\Users\Public\Documents\Autodesk\Revit\Addins\2019\ASRR\ASRR.rvt.0001.rvt"</para>
        ///     <para>Example: "C:\Users\Public\Documents\Autodesk\Revit\Addins\2019\ASRR\ASRR.rvt.0002.rvt"</para>
        ///  </summary>
        public static void RemoveBackUpFilesFromDirectory(string directory)
        {
            foreach (var file in Directory.GetFiles(directory))
                for (var i = 1; i < 10; i++)
                    if (file.EndsWith($".000{i}.rvt"))
                        File.Delete(file);
        }
    }
}