// <copyright file="NugetPublishTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Targets;

/// <summary>
/// Tests the NugetDeploy target.
/// </summary>
[ComponentTest(Type = typeof(NugetPublishTests))]
public static class NugetPublishTests
{
    /// <summary>
    /// Tests the <see cref="NugetPublishTests"/>.
    /// </summary>
    [MemberTest(Name = nameof(NugetPublishTests))]
    public static void Execute()
    {
        string sourceDir = null;
        var succeeded = false;
        IDictionary<string, string> properties = null;
        NetSimulatorHelper simulator = null;

        Arrange(() =>
        {
            simulator = new NetSimulatorHelper();

            Directory.CreateDirectory(simulator.GetArtifactsDir("Hx_ChangeLog"));
            File.WriteAllText(Path.Combine(simulator.GetArtifactsDir("Hx_ChangeLog"), "semver.txt"), "1.2.3\r\n");
            File.WriteAllText(
                Path.Combine(simulator.GetArtifactsDir("Hx_ChangeLog"), "ReleaseNotes.txt"),
                "-release note 1; -release note 2");

            MSBuildHelper.RunTestTarget("Hx_NetBuild", simulator.SolutionDir);

            sourceDir = PathHelper.GetRandomFileNameInCurrentDir();

            Directory.CreateDirectory(sourceDir);

            ExeHelper.Execute(PathHelper.NugetExe, $"init \"{sourceDir}\"");

            properties = new Dictionary<string, string>
            {
                { "Hx_NugetPublish_SourceURL", sourceDir },
            };
        });

        Act(() =>
        {
            succeeded = MSBuildHelper.RunTestTarget("Hx_NugetPublish", simulator.SolutionDir, properties);
        });

        Teardown(() =>
        {
            Directory.Delete(sourceDir, true);
            simulator.Clear();
        });

        When("target is executed", () =>
        {
            Should("succeed", () =>
            {
                var artifactsDir = simulator.GetArtifactsDir("Hx_NugetPublish");

                Assert.That(succeeded, Is.True);
                Assert.That(File.Exists(Path.Combine(artifactsDir, "NetSimulator.1.2.3.nupkg")), Is.True);
                Assert.That(File.Exists(Path.Combine(artifactsDir, "NetSimulator.1.2.3.snupkg")), Is.True);
                Assert.That(File.Exists(Path.Combine(sourceDir, "NetSimulator.1.2.3.nupkg")), Is.True);
            });
        });
    }
}
