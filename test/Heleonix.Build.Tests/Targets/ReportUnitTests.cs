// <copyright file="ReportUnitTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Targets;

/// <summary>
/// Tests the ReportUnit target.
/// </summary>
[ComponentTest(Type = typeof(ReportUnitTests))]
public static class ReportUnitTests
{
    /// <summary>
    /// Tests the <see cref="ReportUnitTests"/>.
    /// </summary>
    [MemberTest(Name = nameof(ReportUnitTests))]
    public static void Execute()
    {
        var succeeded = false;
        IDictionary<string, string> properties = null;
        NetSimulatorHelper simulator = null;

        Arrange(() =>
        {
            simulator = new NetSimulatorHelper();

            properties = new Dictionary<string, string>
            {
                {
                    "Hx_ReportUnit_TestResultFile",
                    PathHelper.NUnitTestResultFile
                },
            };
        });

        Act(() =>
        {
            succeeded = MSBuildHelper.RunTestTarget("Hx_ReportUnit", simulator.SolutionDir, properties);
        });

        Teardown(() =>
        {
            simulator.Clear();
        });

        When("target is executed", () =>
        {
            Should("succeed", () =>
            {
                var artifactsDir = simulator.GetArtifactsDir("Hx_ReportUnit");

                Assert.That(succeeded, Is.True);
                Assert.That(File.Exists(Path.Combine(artifactsDir, "ReportUnit.html")), Is.True);
            });
        });
    }
}
