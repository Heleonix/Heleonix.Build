// <copyright file="Hx_ReportGeneratorTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Targets;

/// <summary>
/// Tests the ReportGenerator target.
/// </summary>
[ComponentTest(Type = typeof(Hx_ReportGeneratorTests))]
public static class Hx_ReportGeneratorTests
{
    /// <summary>
    /// Tests the <see cref="Hx_ReportGeneratorTests"/>.
    /// </summary>
    [MemberTest(Name = nameof(Hx_ReportGeneratorTests))]
    public static void Execute()
    {
        var succeeded = false;
        NetSimulatorHelper simulator = null;

        Arrange(() =>
        {
            simulator = new NetSimulatorHelper();

            Directory.CreateDirectory(simulator.GetArtifactsDir("Hx_NetTest"));

            File.Copy(
                PathHelper.CoverletResultFile,
                Path.Combine(simulator.GetArtifactsDir("Hx_NetTest"), Path.GetFileName(PathHelper.CoverletResultFile)));
        });

        Act(() =>
        {
            succeeded = ToolHelper.RunTestTarget("Hx_ReportGenerator", simulator.SolutionDir, null);
        });

        Teardown(() =>
        {
            simulator.Clear();
        });

        When("Hx_ReportGenerator target is executed", () =>
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
