// <copyright file="Hx_NetNugetPushTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Targets;

/// <summary>
/// Tests the NugetDeploy target.
/// </summary>
[ComponentTest(Type = typeof(Hx_NetNugetPushTests))]
public static class Hx_NetNugetPushTests
{
    /// <summary>
    /// Tests the <see cref="Hx_NetNugetPushTests"/>.
    /// </summary>
    [MemberTest(Name = nameof(Hx_NetNugetPushTests))]
    public static void Execute()
    {
        string sourceDir = null;
        var succeeded = false;
        IDictionary<string, string> properties = null;
        NetSimulatorHelper simulator = null;

        simulator = new NetSimulatorHelper();

        try
        {
            Directory.CreateDirectory(simulator.GetArtifactsDir("Hx_ChangeLog"));
            File.WriteAllText(Path.Combine(simulator.GetArtifactsDir("Hx_ChangeLog"), "semver.txt"), "1.2.3\r\n");
            File.WriteAllText(
                Path.Combine(simulator.GetArtifactsDir("Hx_ChangeLog"), "ReleaseNotes.txt"),
                "-release note 1; -release note 2");

            ToolHelper.RunTestTarget("Hx_NetBuild", simulator.SolutionDir, null);

            Arrange(() =>
            {
                sourceDir = PathHelper.GetRandomFileNameInCurrentDir();

                Directory.CreateDirectory(sourceDir);

                properties = new Dictionary<string, string>
                {
                    { "Hx_NetNugetPush_SourceURL", sourceDir },
                };
            });

            Act(() =>
            {
                succeeded = ToolHelper.RunTestTarget("Hx_NetNugetPush", simulator.SolutionDir, properties);
            });

            Teardown(() =>
            {
                Directory.Delete(sourceDir, true);
            });

            When("target is executed", () =>
            {
                Should("succeed", () =>
                {
                    var artifactsDir = simulator.GetArtifactsDir("Hx_NetNugetPush");

                    Assert.That(succeeded, Is.True);
                    Assert.That(File.Exists(Path.Combine(artifactsDir, "NetSimulator.1.2.3.nupkg")), Is.True);
                    Assert.That(File.Exists(Path.Combine(artifactsDir, "NetSimulator.1.2.3.snupkg")), Is.True);
                    Assert.That(File.Exists(Path.Combine(sourceDir, "NetSimulator.1.2.3.nupkg")), Is.True);
                });
            });
        }
        finally
        {
            simulator.Clear();
        }
    }
}
