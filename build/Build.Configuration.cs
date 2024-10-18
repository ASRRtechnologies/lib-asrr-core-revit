using Nuke.Common.IO;

sealed partial class Build
{
    readonly AbsolutePath ChangeLogPath = RootDirectory / "CHANGELOG.md";
    static AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    
    [Nuke.Common.Parameter("Artifacts Type")] readonly string ArtifactsType;
    [Nuke.Common.Parameter("Excluded Artifacts Type")] readonly string ExcludedArtifactsType;

    protected override void OnBuildInitialized()
    {
        Configurations =
        [
            "Release*",
        ];

        
    }
}