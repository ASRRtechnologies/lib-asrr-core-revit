using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nuke.Common;
using Nuke.Common.ChangeLog;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.Git;
using Nuke.Common.Tools.GitHub;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using Octokit;
using Octokit.Internal;
using Serilog;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[GitHubActions(
    name: "continuous",
    image: GitHubActionsImage.WindowsLatest,
    AutoGenerate = true,
    FetchDepth = 0,
    OnPushBranches = ["dev", "release/**"],
    OnPullRequestBranches = ["release/**"],
    InvokedTargets = [ nameof(Pack) ],
    EnableGitHubToken = true,
    ImportSecrets = [nameof(NuGetApiKey)]
)]
sealed partial class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Pack);

    [Nuke.Common.Parameter("Copyright Details")] readonly string Copyright;
    
    [Solution(GenerateProjects = true)] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion(NoFetch = true)] readonly GitVersion GitVersion;
    
    static GitHubActions GitHubActions => GitHubActions.Instance;

    const string PackageContentType = "application/octet-stream";
    static string ChangeLogFile => RootDirectory / "CHANGELOG.md";

    static string GitHubNuGetFeed => GitHubActions != null
        ? $"https://nuget.pkg.github.com/{GitHubActions.RepositoryOwner}/index.json"
        : null;

    string[] Configurations;
    
    Target Print => _ => _
        .Before(Clean)
        .Executes(() =>
        {
            Log.Information("GitVersion = {Value}", GitVersion?.MajorMinorPatch ?? "Not available");
            Log.Information("GitHub NuGet feed = {Value}", GitHubNuGetFeed);
            Log.Information("GitVer = {Value}", GitVersion);
            Log.Information("NuGet feed = {Value}", NuGetFeed);
            Log.Information("GitHub Repo = {Value}", GitRepository);
            Log.Information("Artifacts = {Value}", ArtifactsDirectory);
            Log.Information("GitHub Actions = {Value}", GitHubActions);
            Log.Information("GlobConfigs = {Value}", GlobBuildConfigurations());

            var c = BuildChangelog();
            Log.Information(c.ToString());
            WriteCompareUrl(c);
            Log.Information(c.ToString());
        });

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            DotNetClean(c => c.SetProject(Solution.ASRR_Revit_Core));
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution.ASRR_Revit_Core)
            );
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            foreach(var configuration in GlobBuildConfigurations())
            {
                DotNetBuild(s => s
                    .SetConfiguration(configuration)
                    .SetVerbosity(DotNetVerbosity.minimal)
                );
            }
        });
    
    List<string> GlobBuildConfigurations()
    {
        var configurations = Solution.Configurations
            .Select(pair => pair.Key)
            .Select(config => config.Remove(config.LastIndexOf('|')))
            .Where(config => Configurations.Any(wildcard => FileSystemName.MatchesSimpleExpression(wildcard, config)))
            .ToList();
        
        Assert.NotEmpty(configurations,
            $"No solution configurations have been found. Pattern: {string.Join(" | ", Configurations)}");
        return configurations;
    }
    
    Target PublishToGitHub => _ => _
        .DependsOn(Pack)
        .Requires(() => GitHubActions.Token)
        .Requires(() => GitRepository)
        .OnlyWhenStatic(() => IsServerBuild && (GitRepository.IsOnDevelopBranch() || GitRepository.IsOnReleaseBranch()))
        .Executes(async () =>
        {
            GitHubTasks.GitHubClient = new GitHubClient(new ProductHeaderValue(Solution.Name))
            {
                Credentials = new Credentials(GitHubActions.Token)
            };

            var gitHubName = GitRepository.GetGitHubName();
            var gitHubOwner = GitRepository.GetGitHubOwner();

            ValidateRelease();

            var artifacts = Directory.GetFiles(ArtifactsDirectory, "*");
            var changelog = CreateGithubChangelog();
            Assert.NotEmpty(artifacts, "No artifacts were found to create the Release");

            var version = GitVersion.MajorMinorPatch;
            var newRelease = new NewRelease(GitVersion.MajorMinorPatch)
            {
                Name = version,
                Body = changelog,
                TargetCommitish = GitRepository.Commit,
                Prerelease = version.Contains("-beta") ||
                             version.Contains("-dev") ||
                             version.Contains("-preview")
            };

            var release = await GitHubTasks.GitHubClient.Repository.Release.Create(gitHubOwner, gitHubName, newRelease);
            await UploadArtifactsAsync(release, artifacts);
        });

    static async Task UploadArtifactsAsync(Release createdRelease, IEnumerable<string> artifacts)
    {
        foreach (var file in artifacts)
        {
            var releaseAssetUpload = new ReleaseAssetUpload
            {
                ContentType = "application/x-binary",
                FileName = Path.GetFileName(file),
                RawData = File.OpenRead(file)
            };

            await GitHubTasks.GitHubClient.Repository.Release.UploadAsset(createdRelease, releaseAssetUpload);
            Log.Information("Artifact: {Path}", file);
        }
    }
    
    StringBuilder BuildChangelog()
    {
        const string separator = "# ";

        var hasEntry = false;
        var changelog = new StringBuilder();
        foreach (var line in File.ReadLines(ChangeLogPath))
        {
            if (hasEntry)
            {
                if (line.StartsWith(separator)) break;

                changelog.AppendLine(line);
                continue;
            }

            if (line.StartsWith(separator) && line.Contains(GitVersion.MajorMinorPatch))
            {
                hasEntry = true;
            }
        }

        TrimEmptyLines(changelog);
        return changelog;
    }

    static void TrimEmptyLines(StringBuilder builder)
    {
        if (builder.Length == 0) return;

        while (builder[^1] == '\r' || builder[^1] == '\n')
        {
            builder.Remove(builder.Length - 1, 1);
        }

        while (builder[0] == '\r' || builder[0] == '\n')
        {
            builder.Remove(0, 1);
        }
    }
    
    void WriteCompareUrl(StringBuilder changelog)
    {
        var tags = GitTasks.Git("describe --tags --abbrev=0 --always", logInvocation: false, logOutput: false);
        var latestTag = tags.First().Text;
        if (latestTag == GitRepository.Commit) return;

        if (changelog[^1] != '\r' || changelog[^1] != '\n') changelog.AppendLine(Environment.NewLine);
        changelog.Append("Full changelog: ");
        changelog.Append(GitRepository.GetGitHubCompareTagsUrl(GitVersion.MajorMinorPatch, latestTag));
    }
    
    void ValidateRelease()
    {
        var tags = GitTasks.Git("describe --tags --abbrev=0 --always", logInvocation: false, logOutput: false);
        var latestTag = tags.First().Text;
        if (latestTag == GitRepository.Commit) return;

        Assert.False(latestTag == GitVersion.MajorMinorPatch, $"A Release with the specified tag already exists in the repository: {GitVersion.MajorMinorPatch}");
        Log.Information("Version: {Version}", GitVersion.MajorMinorPatch);
    }
    
    string CreateGithubChangelog()
    {
        Assert.True(File.Exists(ChangeLogPath), $"Unable to locate the changelog file: {ChangeLogPath}");
        Log.Information("Changelog: {Path}", ChangeLogPath);

        var changelog = BuildChangelog();
        Assert.True(changelog.Length > 0, $"No version entry exists in the changelog: {GitVersion.MajorMinorPatch}");

        WriteCompareUrl(changelog);
        return changelog.ToString();
    }
}