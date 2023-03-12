// <copyright file="NetValidateTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Targets;

/// <summary>
/// Tests the GitHubReleaseNet target.
/// </summary>
[ComponentTest(Type = typeof(NetValidateTests))]
public static class NetValidateTests
{
    /// <summary>
    /// Tests the <see cref="NetValidateTests"/>.
    /// </summary>
    [MemberTest(Name = nameof(NetValidateTests))]
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
            succeeded = MSBuildHelper.RunTestTarget("Hx_NetValidate", simulator.SolutionDir);
        });

        Teardown(() =>
        {
            simulator.Clear();
        });

        When("target is executed", () =>
        {
            Should("succeed", () =>
            {
                Assert.That(succeeded, Is.True);
            });
        });
    }
}
