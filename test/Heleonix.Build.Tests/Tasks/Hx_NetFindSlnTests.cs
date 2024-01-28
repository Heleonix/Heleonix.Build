// <copyright file="Hx_NetFindSlnTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

/// <summary>
/// Tests the <see cref="Hx_NetFindSln"/>.
/// </summary>
[ComponentTest(Type = typeof(Hx_NetFindSln))]

public static class Hx_NetFindSlnTests
{
    /// <summary>
    /// Tests the <see cref="Hx_NetFindSln.ExecuteInternal"/>.
    /// </summary>
    [MemberTest(Name = nameof(Hx_NetFindSln.Execute))]
    public static void Execute()
    {
        Hx_NetFindSln task = null;
        string workspaceDir = null;
        var succeeded = false;

        Act(() =>
        {
            task = new Hx_NetFindSln
            {
                BuildEngine = new TestBuildEngine(),
                StartDir = workspaceDir,
            };

            succeeded = task.Execute();
        });

        Teardown(() =>
        {
            Directory.Delete(workspaceDir, true);
        });

        When("the task is executed", () =>
        {
            And("the solution file exists", () =>
            {
                workspaceDir = PathHelper.GetRandomFileNameInCurrentDir();

                Directory.CreateDirectory(workspaceDir);

                File.WriteAllText(Path.Combine(workspaceDir, "My.sln"), "Solution's content");

                Should("succeed with found solution file", () =>
                {
                    Assert.That(succeeded);
                    Assert.That(task.SlnFile, Is.EqualTo(Path.Combine(workspaceDir, "My.sln")));
                });
            });

            And("the solution file does not exist", () =>
            {
                workspaceDir = PathHelper.GetRandomFileNameInCurrentDir();

                Directory.CreateDirectory(workspaceDir);

                Should("succeed without found solution file", () =>
                {
                    Assert.That(succeeded);
                    Assert.That(task.SlnFile, Is.Null);
                });
            });
        });
    }
}
