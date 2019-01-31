// <copyright file="OpenCoverTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Targets
{
    using System.Collections.Generic;
    using System.IO;
    using Heleonix.Build.Tests.Common;
    using Heleonix.Testing.NUnit.Aaa;
    using NUnit.Framework;
    using static Heleonix.Testing.NUnit.Aaa.AaaSpec;

    /// <summary>
    /// Tests the OpenCover target.
    /// </summary>
    [ComponentTest(Type = typeof(OpenCoverTests))]
    public static class OpenCoverTests
    {
        /// <summary>
        /// Tests the <see cref="OpenCoverTests"/>.
        /// </summary>
        [MemberTest(Name = nameof(OpenCoverTests))]
        public static void Execute()
        {
            var succeeded = false;
            NetStandardSimulatorHelper simulatorHelper = null;
            IDictionary<string, string> properties = null;

            Arrange(() =>
            {
                simulatorHelper = new NetStandardSimulatorHelper();

                MSBuildHelper.RunTestTarget("Hx_Net_Build", simulatorHelper.SolutionDir);
            });

            Act(() =>
            {
                succeeded = MSBuildHelper.RunTestTarget("Hx_OpenCover", simulatorHelper.SolutionDir, properties);
            });

            Teardown(() =>
            {
                simulatorHelper.Clear();
            });

            When("target is executed", () =>
            {
                And("Minimum coverage is satisfied", () =>
                {
                    properties = new Dictionary<string, string>
                    {
                        { "Hx_OpenCover_MinClassCoverage", "57" },
                        { "Hx_OpenCover_MinBranchCoverage", "55" },
                        { "Hx_OpenCover_MinMethodCoverage", "57" },
                        { "Hx_OpenCover_MinLineCoverage", "40" }
                    };

                    Should("succeed", () =>
                    {
                        var artifactDir = simulatorHelper.GetArtifactDir("Hx_OpenCover");
                        var nunitArtifactsDir = simulatorHelper.GetArtifactDir("Hx_NUnit");

                        Assert.That(succeeded, Is.True);
                        Assert.That(File.Exists(Path.Combine(artifactDir, "OpenCover.xml")), Is.True);
                        Assert.That(File.Exists(Path.Combine(nunitArtifactsDir, "NUnit.xml")), Is.True);
                        Assert.That(File.Exists(Path.Combine(nunitArtifactsDir, "Errors.txt")), Is.True);
                        Assert.That(File.Exists(Path.Combine(nunitArtifactsDir, "Output.txt")), Is.True);
                    });
                });

                And("Minimum coverage is not satisfied", () =>
                {
                    properties = new Dictionary<string, string>
                    {
                        { "Hx_OpenCover_MinClassCoverage", "100" },
                        { "Hx_OpenCover_MinBranchCoverage", "100" },
                        { "Hx_OpenCover_MinMethodCoverage", "100" },
                        { "Hx_OpenCover_MinLineCoverage", "100" }
                    };

                    Should("fail", () =>
                    {
                        var artifactDir = simulatorHelper.GetArtifactDir("Hx_OpenCover");
                        var nunitArtifactsDir = simulatorHelper.GetArtifactDir("Hx_NUnit");

                        Assert.That(succeeded, Is.False);
                        Assert.That(File.Exists(Path.Combine(artifactDir, "OpenCover.xml")), Is.True);
                        Assert.That(File.Exists(Path.Combine(nunitArtifactsDir, "NUnit.xml")), Is.True);
                        Assert.That(File.Exists(Path.Combine(nunitArtifactsDir, "Errors.txt")), Is.True);
                        Assert.That(File.Exists(Path.Combine(nunitArtifactsDir, "Output.txt")), Is.True);
                    });

                    And("should continue on error", () =>
                    {
                        properties.Add("Hx_OpenCover_ContinueOnError", "true");

                        Should("succeed", () =>
                        {
                            var artifactDir = simulatorHelper.GetArtifactDir("Hx_OpenCover");
                            var nunitArtifactsDir = simulatorHelper.GetArtifactDir("Hx_NUnit");

                            Assert.That(succeeded, Is.True);
                            Assert.That(File.Exists(Path.Combine(artifactDir, "OpenCover.xml")), Is.True);
                            Assert.That(File.Exists(Path.Combine(nunitArtifactsDir, "NUnit.xml")), Is.True);
                            Assert.That(File.Exists(Path.Combine(nunitArtifactsDir, "Errors.txt")), Is.True);
                            Assert.That(File.Exists(Path.Combine(nunitArtifactsDir, "Output.txt")), Is.True);
                        });
                    });
                });
            });
        }
    }
}
