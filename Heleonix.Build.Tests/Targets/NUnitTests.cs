// <copyright file="NUnitTests.cs" company="Heleonix - Hennadii Lutsyshyn">
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
    /// Tests the NUnit target.
    /// </summary>
    [ComponentTest(Type = typeof(NUnitTests))]
    public static class NUnitTests
    {
        /// <summary>
        /// Tests the <see cref="NUnitTests"/>.
        /// </summary>
        [MemberTest(Name = nameof(NUnitTests))]
        public static void Execute()
        {
            var succeeded = false;
            var artifactDir = NetStandardSimulatorPathHelper.GetArtifactDir("Hx_NUnit");
            IDictionary<string, string> properties = null;

            Arrange(() =>
            {
                MSBuildHelper.RunTestTarget("Hx_Net_Build", NetStandardSimulatorPathHelper.SolutionDir);
            });

            Act(() =>
            {
                succeeded = MSBuildHelper.RunTestTarget(
                    "Hx_NUnit",
                    NetStandardSimulatorPathHelper.SolutionDir,
                    properties);
            });

            Teardown(() =>
            {
                Directory.Delete(artifactDir, true);
                Directory.Delete(NetStandardSimulatorPathHelper.GetArtifactDir("Hx_Net_Build"), true);
            });

            When("target is executed", () =>
            {
                Should("succeed", () =>
                {
                    Assert.That(succeeded, Is.True);
                    Assert.That(File.Exists(Path.Combine(artifactDir, "NUnit.xml")), Is.True);
                    Assert.That(File.Exists(Path.Combine(artifactDir, "Errors.txt")), Is.True);
                    Assert.That(File.Exists(Path.Combine(artifactDir, "Output.txt")), Is.True);
                });

                And("should fail on failed tests", () =>
                {
                    properties = new Dictionary<string, string>
                    {
                        { "Hx_NUnit_FailOnFailedTests", "true" }
                    };

                    Should("fail", () =>
                    {
                        Assert.That(succeeded, Is.False);
                        Assert.That(File.Exists(Path.Combine(artifactDir, "NUnit.xml")), Is.True);
                        Assert.That(File.Exists(Path.Combine(artifactDir, "Errors.txt")), Is.True);
                        Assert.That(File.Exists(Path.Combine(artifactDir, "Output.txt")), Is.True);
                    });
                });
            });
        }
    }
}
