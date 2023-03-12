// <copyright file="NUnitTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Targets;

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
        IDictionary<string, string> properties = null;
        NetSimulatorHelper simulator = null;

        Arrange(() =>
        {
            simulator = new NetSimulatorHelper();

            MSBuildHelper.RunTestTarget("Hx_NetBuild", simulator.SolutionDir);
        });

        Act(() =>
        {
            succeeded = MSBuildHelper.RunTestTarget("Hx_NUnit", simulator.SolutionDir, properties);
        });

        Teardown(() =>
        {
            simulator.Clear();
        });

        When("target is executed", () =>
        {
            Should("fail", () =>
            {
                var artifactsDir = simulator.GetArtifactsDir("Hx_NUnit");

                Assert.That(succeeded, Is.False);
                Assert.That(File.Exists(Path.Combine(artifactsDir, "NUnit.xml")), Is.True);
                Assert.That(File.Exists(Path.Combine(artifactsDir, "Output.txt")), Is.True);
            });

            And("should continue on error", () =>
            {
                properties = new Dictionary<string, string>
                {
                    { "Hx_NUnit_ContinueOnError", "true" },
                };

                Should("succeed", () =>
                {
                    var artifactsDir = simulator.GetArtifactsDir("Hx_NUnit");

                    Assert.That(succeeded, Is.True);
                    Assert.That(File.Exists(Path.Combine(artifactsDir, "NUnit.xml")), Is.True);
                    Assert.That(File.Exists(Path.Combine(artifactsDir, "Output.txt")), Is.True);
                });
            });
        });
    }
}
