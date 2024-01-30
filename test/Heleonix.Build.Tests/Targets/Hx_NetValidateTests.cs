// <copyright file="Hx_NetValidateTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Targets;

/// <summary>
/// Tests the GitHubReleaseNet target.
/// </summary>
[ComponentTest(Type = typeof(Hx_NetValidateTests))]
public static class Hx_NetValidateTests
{
    /// <summary>
    /// Tests the <see cref="Hx_NetValidateTests"/>.
    /// </summary>
    [MemberTest(Name = nameof(Hx_NetValidateTests))]
    public static void Execute()
    {
        var succeeded = false;
        NetSimulatorHelper simulator = null;

        Arrange(() =>
        {
            simulator = new NetSimulatorHelper();
        });

        Act(() =>
        {
            succeeded = ToolHelper.RunTestTarget("Hx_NetValidate", simulator.SolutionDir, null);
        });

        Teardown(() =>
        {
            simulator.Clear();
        });

        When("Hx_NetValidate target is executed", () =>
        {
            Should("succeed", () =>
            {
                Assert.That(succeeded, Is.True);
            });
        });
    }
}
