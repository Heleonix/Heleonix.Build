// <copyright file="Hx_NetBuildTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Targets;

using System.Diagnostics;
using System.Reflection;

/// <summary>
/// Tests the BuildNet target.
/// </summary>
[ComponentTest(Type = typeof(Hx_NetBuildTests))]
public static class Hx_NetBuildTests
{
    /// <summary>
    /// Tests the <see cref="Hx_NetBuildTests"/>.
    /// </summary>
    [MemberTest(Name = nameof(Hx_NetBuildTests))]
    public static void Execute()
    {
        var succeeded = false;
        NetSimulatorHelper simulator = null;

        Arrange(() =>
        {
            simulator = new NetSimulatorHelper();

            Directory.CreateDirectory(simulator.GetArtifactsDir("Hx_ChangeLog"));
            File.WriteAllText(Path.Combine(simulator.GetArtifactsDir("Hx_ChangeLog"), "semver.txt"), "1.2.3\r\n");
            File.WriteAllText(
                Path.Combine(simulator.GetArtifactsDir("Hx_ChangeLog"), "ReleaseNotes.txt"),
                "-release note 1;\r\n-release note 2");
        });

        Act(() =>
        {
            succeeded = ToolHelper.RunTestTarget(
                "Hx_NetBuild",
                simulator.SolutionDir,
                new Dictionary<string, string>
                {
                    { "Hx_NetBuild_SnkFile", PathHelper.SnkPairFile },
                    { "GO_PIPELINE_COUNTER", "123" },
                });
        });

        Teardown(() =>
        {
            simulator.Clear();
        });

        When("target is executed", () =>
        {
            Should("succeed", () =>
            {
                var artifactsDir = simulator.GetArtifactsDir("Hx_NetBuild");

                var sourceProjectOutputDir = Path.Combine(
                    artifactsDir,
                    "NetSimulator",
                    "bin",
                    PathHelper.Configuration);

                Assert.That(succeeded, Is.True);

                Assert.That(File.Exists(Path.Combine(artifactsDir, "SharedProjectInfo.props")));

                foreach (var tfm in simulator.SourceProjectTargetFrameworks)
                {
                    var path = Path.Combine(sourceProjectOutputDir, tfm, "NetSimulator.dll");
                    var info = FileVersionInfo.GetVersionInfo(path);

                    Assert.That(info.FileVersion, Is.EqualTo("1.2.3.123"));
                    Assert.That(info.ProductVersion, Is.EqualTo("1.2.3"));

                    var name = AssemblyName.GetAssemblyName(path);

                    Assert.That(name.FullName, Contains.Substring("Version=1.2.3.123"));
                    Assert.That(name.FullName, Contains.Substring("PublicKeyToken=56430db2a23ca0d2"));

                    Assert.That(File.Exists(Path.Combine(sourceProjectOutputDir, tfm, "NetSimulator.xml")));
                    Assert.That(File.Exists(Path.Combine(sourceProjectOutputDir, tfm, "NetSimulator.pdb")));
                }

                var testProjectOutputDir = Path.Combine(
                    artifactsDir,
                    "NetSimulator.Tests",
                    "bin",
                    PathHelper.Configuration);

                foreach (var tfm in simulator.TestProjectTargetFrameworks)
                {
                    var path = Path.Combine(testProjectOutputDir, tfm, "NetSimulator.Tests.dll");
                    var info = FileVersionInfo.GetVersionInfo(path);

                    Assert.That(info.FileVersion, Is.EqualTo("1.2.3.123"));
                    Assert.That(info.ProductVersion, Is.EqualTo("1.2.3"));

                    var name = AssemblyName.GetAssemblyName(path);

                    Assert.That(name.FullName, Contains.Substring("Version=1.2.3.123"));
                    Assert.That(name.FullName, Contains.Substring("PublicKeyToken=56430db2a23ca0d2"));

                    Assert.That(File.Exists(Path.Combine(testProjectOutputDir, tfm, "NetSimulator.Tests.xml")));
                    Assert.That(File.Exists(Path.Combine(testProjectOutputDir, tfm, "NetSimulator.Tests.pdb")));
                }

                var sourceProjectObjDir = Path.Combine(
                    artifactsDir,
                    "NetSimulator",
                    "obj",
                    PathHelper.Configuration);

                Assert.That(Directory.GetDirectories(sourceProjectObjDir).Length, Is.GreaterThan(0));

                var testProjectObjDir = Path.Combine(
                    artifactsDir,
                    "NetSimulator.Tests",
                    "obj",
                    PathHelper.Configuration);

                Assert.That(Directory.GetDirectories(testProjectObjDir).Length, Is.GreaterThan(0));

                Assert.That(File.Exists(Path.Combine(artifactsDir, "NetSimulator.sln")));

                Assert.That(
                    File.Exists(Path.Combine(artifactsDir, "NetSimulator", "NetSimulator.csproj")));

                Assert.That(
                    File.Exists(Path.Combine(artifactsDir, "NetSimulator.Tests", "NetSimulator.Tests.csproj")));
            });
        });
    }
}
