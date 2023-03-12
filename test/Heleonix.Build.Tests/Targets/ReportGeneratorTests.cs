// <copyright file="ReportGeneratorTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Targets;

/// <summary>
/// Tests the ReportGenerator target.
/// </summary>
[ComponentTest(Type = typeof(ReportGeneratorTests))]
public static class ReportGeneratorTests
{
    /// <summary>
    /// Tests the <see cref="ReportGeneratorTests"/>.
    /// </summary>
    [MemberTest(Name = nameof(ReportGeneratorTests))]
    public static void Execute()
    {
        var succeeded = false;
        IDictionary<string, ITaskItem[]> items = null;
        NetSimulatorHelper simulator = null;

        Arrange(() =>
        {
            simulator = new NetSimulatorHelper();

            items = new Dictionary<string, ITaskItem[]>
            {
                {
                    "Hx_ReportGenerator_CoverageResultFiles",
                    new ITaskItem[] { new TaskItem(PathHelper.OpenCoverResultFile) }
                },
            };
        });

        Act(() =>
        {
            succeeded = MSBuildHelper.RunTestTarget("Hx_ReportGenerator", simulator.SolutionDir, null, items);
        });

        Teardown(() =>
        {
            simulator.Clear();
        });

        When("target is executed", () =>
        {
            Should("succeed", () =>
            {
                var artifactsDir = simulator.GetArtifactsDir("Hx_ReportGenerator");

                Assert.That(succeeded, Is.True);
                Assert.That(File.Exists(Path.Combine(artifactsDir, "index.htm")), Is.True);
                Assert.That(File.Exists(Path.Combine(artifactsDir, "report.css")), Is.True);
            });
        });
    }
}
