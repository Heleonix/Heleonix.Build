// <copyright file="Hx_ExtentReportTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Targets;

[ComponentTest(Type = typeof(Hx_ExtentReportTests))]
public static class Hx_ExtentReportTests
{
    [MemberTest(Name = nameof(Hx_ExtentReportTests))]
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
                PathHelper.NUnitTestResultFile,
                Path.Combine(testResultDir, Path.GetFileName(PathHelper.NUnitTestResultFile)));
        });

        Act(() =>
        {
            succeeded = ToolHelper.RunTestTarget("Hx_ExtentReport", simulator.SolutionDir, null);
        });

        Teardown(() =>
        {
            simulator.Clear();
        });

        When("Hx_ExtentReport target is executed", () =>
        {
            Should("succeed", () =>
            {
                var artifactsDir = simulator.GetArtifactsDir("Hx_ExtentReport");

                Assert.That(succeeded, Is.True);
                Assert.That(File.Exists(Path.Combine(artifactsDir, "index.html")), Is.True);
            });
        });
    }
}
