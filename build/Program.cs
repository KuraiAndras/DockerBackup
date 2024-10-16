﻿using Bullseye;

using static Bullseye.Targets;
using static SimpleExec.Command;

internal static class Program
{
    /// <summary>Run or list targets.</summary>
    /// <param name="targets">A list of targets to run or list. If not specified, the "default" target will be run, or all targets will be listed.</param>
    /// <param name="clear">Clear the console before execution.</param>
    /// <param name="dryRun">Do a dry run without executing actions.</param>
    /// <param name="host">Force the mode for a specific host environment (normally auto-detected).</param>
    /// <param name="listDependencies">List all (or specified) targets and dependencies, then exit.</param>
    /// <param name="listInputs">List all (or specified) targets and inputs, then exit.</param>
    /// <param name="listTargets">List all (or specified) targets, then exit.</param>
    /// <param name="listTree">List all (or specified) targets and dependency trees, then exit.</param>
    /// <param name="noColor">Disable colored output.</param>
    /// <param name="noExtendedChars">Disable extended characters.</param>
    /// <param name="parallel">Run targets in parallel.</param>
    /// <param name="skipDependencies">Do not run targets' dependencies.</param>
    /// <param name="verbose">Enable verbose output.</param>
    /// <param name="configuration">.NET confiuration</param>
    private static async Task Main(
        string[] targets,
        bool clear,
        bool dryRun,
        Host? host,
        bool listDependencies,
        bool listInputs,
        bool listTargets,
        bool listTree,
        bool noColor,
        bool noExtendedChars,
        bool parallel,
        bool skipDependencies,
        bool verbose,
        string configuration = "Debug")
    {
        var options = new Options
        {
            Clear = clear,
            DryRun = dryRun,
            Host = host,
            ListDependencies = listDependencies,
            ListInputs = listInputs,
            ListTargets = listTargets,
            ListTree = listTree,
            NoColor = noColor,
            NoExtendedChars = noExtendedChars,
            Parallel = parallel,
            SkipDependencies = skipDependencies,
            Verbose = verbose,
        };

        Target("build", () => RunAsync("dotnet", $"build --configuration {configuration} --nologo --verbosity quiet"));
        Target("test", DependsOn("build"), () => RunAsync("dotnet", "test --configuration Release --no-build --nologo --verbosity quiet"));
        Target("default", DependsOn("test"));

        await RunTargetsWithoutExitingAsync(targets, options, messageOnly: ex => ex is SimpleExec.ExitCodeException);
    }
}
