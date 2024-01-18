using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.Docker;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;

using Serilog;

using static Nuke.Common.Tools.Docker.DockerTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Compile);

    private readonly List<string> CreatedImages = [];

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("RID to use during publish")]
    readonly DotNetRuntimeIdentifier Runtime = DotNetRuntimeIdentifier.linux_x64;

    [GitVersion] readonly GitVersion GitVersion = default!;

    [Solution(GenerateProjects = true)] readonly Solution Solution = default!;

    GitHubActions GitHubActions => GitHubActions.Instance;

    AbsolutePath Artifacts => RootDirectory / "artifacts";

    AbsolutePath WorkerPubishOutput => Artifacts / "docker-backup";

    string DockerName => "huszky/docker-backup";
    string DockerTag => $"{DockerName}:{GitVersion.NuGetVersionV2}";
    string DockerLatestTag => $"{DockerName}:latest";

    string DebugContainerName => "docker-backup-debug";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
            Artifacts.CreateOrCleanDirectory());

    Target Restore => _ => _
        .DependsOn(Clean)
        .Executes(() =>
            DotNetRestore(s => s
                .SetProjectFile(Solution)));

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .EnableNoRestore()));

    Target PublishWorker => _ => _
        .DependsOn(Compile)
        .Executes(() =>
            DotNetPublish(s => s
                .SetProject(Solution.src.DockerBackup)
                .SetConfiguration(Configuration)
                .SetRuntime(Runtime)
                .SetProperty("UseAppHost", true)
                .SetOutput(WorkerPubishOutput)));

    Target SetDockerLogger => _ => _
        .DependentFor(BuildWorkerDockerImage)
        .Executes(() => DockerLogger = (_, log) => Log.Information(log));

    Target BuildWorkerDockerImage => _ => _
        .DependsOn(PublishWorker)
        .Executes(() =>
            DockerBuild(s =>
            {
                string[] tags = GitHubActions?.EventName != "workflow_dispatch"
                    ? [DockerTag, DockerLatestTag]
                    : [DockerTag];

                CreatedImages.AddRange(tags);

                return s
                    .SetFile(Solution.src.DockerBackup.Directory / "Dockerfile")
                    .SetPath(RootDirectory)
                    .SetTag(tags);
            }));

    Target RunWorkerDocker => _ => _
        .After(BuildWorkerDockerImage)
        .Executes(() =>
            DockerRun(s => s
                .SetName(DebugContainerName)
                .SetImage(DockerTag)));

    Target RemoveDockerContainer => _ => _
        .TriggeredBy(RunWorkerDocker)
        .AssuredAfterFailure()
        .Executes(() =>
        {
            DockerStop(s => s.SetContainers(DebugContainerName));
            DockerRm(s => s.SetContainers(DebugContainerName));
        });

    Target PushDockerImages => _ => _
        .After(BuildWorkerDockerImage)
        .Executes(() =>
            CreatedImages.ForEach(i =>
                DockerPush(s => s
                    .SetName(i))));
}
