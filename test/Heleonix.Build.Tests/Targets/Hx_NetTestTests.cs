// <copyright file="Hx_NetTestTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Targets;

[ComponentTest(Type = typeof(Hx_NetTestTests))]
public static class Hx_NetTestTests
{
    [MemberTest(Name = nameof(Hx_NetTestTests))]
    public static void Execute()
    {
        var succeeded = false;
        NetSimulatorHelper simulator = null;

        simulator = new NetSimulatorHelper();

        try
        {
            ToolHelper.RunTestTarget("Hx_NetBuild", simulator.SolutionDir, null);

            Act(() =>
            {
                var properties = new Dictionary<string, string>
                {
                    { "Hx_NetTest_MinLineCoverage", "40" },
                    { "Hx_NetTest_MinBranchCoverage", "50" },
                    { "Hx_NetTest_MinMethodCoverage", "50" },
                };

                succeeded = ToolHelper.RunTestTarget("Hx_NetTest", simulator.SolutionDir, properties);
            });

            When("Hx_NetTest target is executed", () =>
            {
                Should("succeed", () =>
                {
                    var artifactsDir = simulator.GetArtifactsDir("Hx_NetTest");

                    Assert.That(succeeded, Is.True);
                    Assert.That(File.Exists(Path.Combine(artifactsDir, "NUnitTestResults.xml")), Is.True);

                    var coverageFile = new DirectoryInfo(artifactsDir)
                        .GetFiles("coverage.cobertura.xml", SearchOption.AllDirectories).Single().FullName;

                    Assert.That(
                        File.ReadAllText(coverageFile),
                        Contains.Substring("https://raw.githubusercontent.com/Heleonix/" +
                        Path.GetFileName(simulator.SolutionDir) +
                        "/master/src/"));
                });
            });
        }
        finally
        {
            simulator.Clear();
        }
    }
}
