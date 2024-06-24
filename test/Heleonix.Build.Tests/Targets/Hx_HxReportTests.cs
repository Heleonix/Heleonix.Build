// <copyright file="Hx_HxReportTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Targets;

[ComponentTest(Type = typeof(Hx_HxReportTests))]
public static class Hx_HxReportTests
{
    [MemberTest(Name = nameof(Hx_HxReportTests))]
    public static void Execute()
    {
        var succeeded = false;
        NetSimulatorHelper simulator = null;

        Arrange(() =>
        {
            simulator = new NetSimulatorHelper();

            var testResultDir = Path.Combine(simulator.GetArtifactsDir("Hx_NetTest"), Path.GetRandomFileName());

            Directory.CreateDirectory(testResultDir);

            File.Copy(
                PathHelper.TrxTestResultFile,
                Path.Combine(testResultDir, Path.GetFileName(PathHelper.TrxTestResultFile)));
        });

        Act(() =>
        {
            succeeded = ToolHelper.RunTestTarget("Hx_HxReport", simulator.SolutionDir, null);
        });

        Teardown(() =>
        {
            simulator.Clear();
        });

        When("Hx_HxReport target is executed", () =>
        {
            Should("succeed", () =>
            {
                var artifactsDir = simulator.GetArtifactsDir("Hx_HxReport");

                Assert.That(succeeded, Is.True);
                Assert.That(File.Exists(Path.Combine(artifactsDir, "Report.html")), Is.True);
            });
        });
    }
}
