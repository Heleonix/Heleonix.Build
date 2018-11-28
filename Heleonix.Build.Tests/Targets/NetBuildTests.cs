// <copyright file="NetBuildTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Targets
{
    using System.IO;
    using Heleonix.Build.Tests.Common;
    using Heleonix.Testing.NUnit.Aaa;
    using NUnit.Framework;
    using static Heleonix.Testing.NUnit.Aaa.AaaSpec;

    /// <summary>
    /// Tests the BuildNet target.
    /// </summary>
    [ComponentTest(Type = typeof(NetBuildTests))]
    public static class NetBuildTests
    {
        /// <summary>
        /// Tests the <see cref="NetBuildTests"/>.
        /// </summary>
        [MemberTest(Name = nameof(NetBuildTests))]
        public static void Execute()
        {
            var succeeded = false;
            var artifactDir = NetStandardSimulatorPathHelper.GetArtifactDir("Hx_Net_Build");

            Act(() =>
            {
                succeeded = MSBuildHelper.RunTestTarget("Hx_Net_Build", NetStandardSimulatorPathHelper.SolutionDir);
            });

            Teardown(() =>
            {
                Directory.Delete(artifactDir, true);
            });

            When("target is executed", () =>
            {
                Should("succeed", () =>
                {
                    var sourceProjectOutputDir = Path.Combine(
                        artifactDir,
                        "NetStandardSimulator",
                        "bin",
                        PathHelper.Configuration);

                    Assert.That(succeeded, Is.True);
                    Assert.That(File.Exists(Path.Combine(artifactDir, "SharedProjectInfo.proj")));
                    Assert.That(File.Exists(Path.Combine(artifactDir, "NetStandardSimulator", "NetStandardSimulator.csproj")));
                    Assert.That(File.Exists(Path.Combine(artifactDir, "NetStandardSimulator.Tests", "NetStandardSimulator.Tests.csproj")));
                    Assert.That(File.Exists(Path.Combine(sourceProjectOutputDir, "NetStandardSimulator.1.0.0.nupkg")));
                    Assert.That(File.Exists(Path.Combine(sourceProjectOutputDir, "NetStandardSimulator.1.0.0.symbols.nupkg")));

                    foreach (var tfm in NetStandardSimulatorPathHelper.SourceProjectTargetFrameworks)
                    {
                        Assert.That(File.Exists(Path.Combine(sourceProjectOutputDir, tfm, "NetStandardSimulator.dll")));
                        Assert.That(File.Exists(Path.Combine(sourceProjectOutputDir, tfm, "NetStandardSimulator.xml")));
                        Assert.That(File.Exists(Path.Combine(sourceProjectOutputDir, tfm, "NetStandardSimulator.pdb")));
                        Assert.That(Directory.Exists(Path.Combine(sourceProjectOutputDir, tfm, "publish")));
                        Assert.That(File.Exists(Path.Combine(sourceProjectOutputDir, tfm, "publish", "NetStandardSimulator.dll")));
                        Assert.That(File.Exists(Path.Combine(sourceProjectOutputDir, tfm, "publish", "NetStandardSimulator.xml")));
                        Assert.That(File.Exists(Path.Combine(sourceProjectOutputDir, tfm, "publish", "NetStandardSimulator.pdb")));
                    }

                    var testProjectOutputDir = Path.Combine(
                        artifactDir,
                        "NetStandardSimulator.Tests",
                        "bin",
                        PathHelper.Configuration);

                    foreach (var tfm in NetStandardSimulatorPathHelper.TestProjectTargetFrameworks)
                    {
                        Assert.That(File.Exists(Path.Combine(testProjectOutputDir, tfm, "NetStandardSimulator.Tests.dll")));
                        Assert.That(File.Exists(Path.Combine(testProjectOutputDir, tfm, "NetStandardSimulator.Tests.xml")));
                        Assert.That(File.Exists(Path.Combine(testProjectOutputDir, tfm, "NetStandardSimulator.Tests.pdb")));
                        Assert.That(Directory.Exists(Path.Combine(testProjectOutputDir, tfm, "publish")));
                        Assert.That(File.Exists(Path.Combine(testProjectOutputDir, tfm, "publish", "NetStandardSimulator.Tests.dll")));
                        Assert.That(File.Exists(Path.Combine(testProjectOutputDir, tfm, "publish", "NetStandardSimulator.Tests.xml")));
                        Assert.That(File.Exists(Path.Combine(testProjectOutputDir, tfm, "publish", "NetStandardSimulator.Tests.pdb")));
                    }
                });
            });
        }
    }
}
