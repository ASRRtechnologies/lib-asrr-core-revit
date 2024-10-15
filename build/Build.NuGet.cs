using System.Linq;
using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Utilities.Collections;
using Serilog;
using Nuke.Common.Tools.DotNet;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

sealed partial class Build 
{
    [Parameter] string NuGetFeed = "https://api.nuget.org/v3/index.json";
    [Parameter("NuGet API Key"), Secret] string NuGetApiKey;
    
    Target PublishToNuGet => _ => _
        .Description($"Publishing to NuGet with the version.")
        .Triggers(CreateRelease)
        .Requires(() => NuGetApiKey)
        .OnlyWhenStatic(() => IsLocalBuild && GitRepository.IsOnDevelopBranch())
        .Executes(() =>
        {
            Log.Information($"Pushing package to NuGet feed...");
            ArtifactsDirectory.GlobFiles(ArtifactsType)
                .Where(x => !x.ToString().EndsWith(ExcludedArtifactsType))
                .ForEach(x =>
                {
                    DotNetNuGetPush(s => s
                        .SetTargetPath(x)
                        .SetSource(NuGetFeed)
                        .SetApiKey(NuGetApiKey)
                        .EnableSkipDuplicate()
                    );
                });
        });
}