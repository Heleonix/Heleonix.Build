// <copyright file="Hx_TrxReportTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

#pragma warning disable S125 // Sections of code should not be commented out
/*
namespace Heleonix.Build.Tests.Targets;

/// <summary>
/// Tests the Hx_TrxReport target.
/// </summary>
[ComponentTest(Type = typeof(Hx_TrxReportTests))]
public static class Hx_TrxReportTests
{
    /// <summary>
    /// Tests the <see cref="Hx_TrxReportTests"/>.
    /// </summary>
    [MemberTest(Name = nameof(Hx_TrxReportTests))]
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
                    "Hx_TrxReport_TestResultFile",
                    PathHelper.NUnitTestResultFile
                },
            };
        });

        Act(() =>
        {
            succeeded = ToolHelper.RunTestTarget("Hx_TrxReport", simulator.SolutionDir, properties);
        });

        Teardown(() =>
        {
            simulator.Clear();
        });

        When("target is executed", () =>
        {
            Should("succeed", () =>
            {
                var artifactsDir = simulator.GetArtifactsDir("Hx_TrxReport");

                Assert.That(succeeded, Is.True);
                Assert.That(File.Exists(Path.Combine(artifactsDir, "ReportUnit.html")), Is.True);
            });
        });
    }
}
*/
#pragma warning restore S125 // Sections of code should not be commented out