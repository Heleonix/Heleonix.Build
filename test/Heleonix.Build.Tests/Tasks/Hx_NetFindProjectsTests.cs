// <copyright file="Hx_NetFindProjectsTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

/// <summary>
/// Tests the <see cref="Hx_NetFindProjects"/>.
/// </summary>
[ComponentTest(Type = typeof(Hx_NetFindProjects))]

public static class Hx_NetFindProjectsTests
{
    /// <summary>
    /// Tests the <see cref="Hx_NetFindProjects.ExecuteInternal"/>.
    /// </summary>
    [MemberTest(Name = nameof(Hx_NetFindProjects.Execute))]
    public static void Execute()
    {
        Hx_NetFindProjects task = null;
        string slnFile = null;
        NetSimulatorHelper simulator = null;
        var succeeded = false;

        Act(() =>
        {
            task = new Hx_NetFindProjects
            {
                BuildEngine = new TestBuildEngine(),
                SlnFile = slnFile,
            };

            succeeded = task.Execute();
        });

        When("the task is executed", () =>
        {
            And("the sln file is povided", () =>
            {
                Arrange(() =>
                {
                    simulator = new NetSimulatorHelper();
                    slnFile = simulator.SolutionFile;
                });

                Teardown(() =>
                {
                    simulator.Clear();
                });

                Should("succeed with found full paths to project files", () =>
                {
                    Assert.That(succeeded);
                    Assert.That(task.ProjectFiles[0], Is.EqualTo(simulator.SourceProjectFile));
                    Assert.That(task.ProjectFiles[1], Is.EqualTo(simulator.TestProjectFile));
                });
            });
        });
    }
}
