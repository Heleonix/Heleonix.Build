// <copyright file="Hx_GitParseRepoUrlTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

/// <summary>
/// Tests the <see cref="Hx_GitParseRepoUrl"/>.
/// </summary>
[ComponentTest(Type = typeof(Hx_GitParseRepoUrl))]

public static class Hx_GitParseRepoUrlTests
{
    /// <summary>
    /// Tests the <see cref="Hx_GitParseRepoUrl.ExecuteInternal"/>.
    /// </summary>
    [MemberTest(Name = nameof(Hx_GitParseRepoUrl.Execute))]
    public static void Execute()
    {
        Hx_GitParseRepoUrl task = null;
        string url = null;
        var succeeded = false;

        Act(() =>
        {
            task = new Hx_GitParseRepoUrl
            {
                BuildEngine = new TestBuildEngine(),
                RepositoryUrl = url,
            };

            succeeded = task.Execute();
        });

        When("the task is executed", () =>
        {
            And("the url in SSH format is provided", () =>
            {
                url = "git@github.com:SomeOwner/Some.Repo.git";

                Should("succeed with parsed owner and repository names", () =>
                {
                    Assert.That(succeeded);
                    Assert.That(task.OwnerName, Is.EqualTo("SomeOwner"));
                    Assert.That(task.RepositoryName, Is.EqualTo("Some.Repo"));
                });
            });

            And("the url in HTTPS format is provided", () =>
            {
                url = "https://github.com/SomeOwner/Some.Repo.git";

                Should("succeed with parsed owner and repository names", () =>
                {
                    Assert.That(succeeded);
                    Assert.That(task.OwnerName, Is.EqualTo("SomeOwner"));
                    Assert.That(task.RepositoryName, Is.EqualTo("Some.Repo"));
                });
            });

            And("the url in an unsupported format is provided", () =>
            {
                url = "https://heleonix.github.io/docs";

                Should("succeed without found owner and repository names", () =>
                {
                    Assert.That(succeeded);
                    Assert.That(task.OwnerName, Is.Empty);
                    Assert.That(task.RepositoryName, Is.Empty);
                });
            });
        });
    }
}
