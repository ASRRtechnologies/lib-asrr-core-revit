using System;
using System.IO;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.Tools.DotNet;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

sealed partial class Build 
{
    Target Pack => _ => _
        .Produces(ArtifactsDirectory / ArtifactsType)
        .DependsOn(Compile)
        .Triggers(PublishToGithub, PublishToNuGet)
        .OnlyWhenStatic(() => IsLocalBuild || GitRepository.IsOnDevelopBranch())
        .Executes(() =>
        {
            var readme = CreateNugetReadme();
            foreach (var configuration in GlobBuildConfigurations())
            {
                DotNetPack(settings => settings
                    .SetProject(Solution.ASRR_Revit_Core)
                    .SetConfiguration(configuration)
                    .SetVersion(GitVersion.MajorMinorPatch)
                    .SetOutputDirectory(ArtifactsDirectory)
                    .SetVerbosity(DotNetVerbosity.minimal)
                );
            }
            RestoreReadme(readme);
        });
    
    string CreateNugetReadme()
    {
        var readmePath = Solution.Directory / "README.md";
        var readme = File.ReadAllText(readmePath);
        Log.Information("Readme: {Readme}", readme);

        const string startSymbol = "<p";
        const string endSymbol = "</p>\r\n\r\n";
        var logoStartIndex = readme.IndexOf(startSymbol, StringComparison.Ordinal);
        var logoEndIndex = readme.IndexOf(endSymbol, StringComparison.Ordinal);
        
        var nugetReadme = readme.Remove(logoStartIndex, logoEndIndex - logoStartIndex + endSymbol.Length);
        File.WriteAllText(readmePath, readme);

        return readme;
    }
    
    void RestoreReadme(string readme)
    {
        var readmePath = Solution.Directory / "README.md";
        File.WriteAllText(readmePath, readme);
    }
}