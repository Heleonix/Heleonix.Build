// <copyright file="GitHubReleaseTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

using System.Net;

/// <summary>
/// Tests the <see cref="GitHubRelease"/>.
/// </summary>
[ComponentTest(Type = typeof(GitHubRelease))]
public static class GitHubReleaseTests
{
    /// <summary>
    /// Tests the <see cref="GitHubRelease.ExecuteInternal"/>.
    /// </summary>
    [MemberTest(Name = nameof(GitHubRelease.Execute))]
    public static void Execute()
    {
        GitHubRelease task = null;
        var succeeded = false;
        var isDraft = false;
        var isPrerelease = false;
        HttpListener listener = null;
        string tagName = null;

        Arrange(() =>
        {
            listener = HttpHelper.LaunchHttpServer((
                "http://localhost:33333/repos/heleonix/heleonix.build/releases/",
                request =>
                {
                    using (var reader = new StreamReader(request.InputStream))
                    {
                        var req = reader.ReadToEnd();

                        return req.Contains("\"v1.0.0\"") && req.Contains("-release note 1; -release note 2") ?
                        ("{ \"name\": \"v1.0.0\" }", HttpStatusCode.Created) :
                        ("{ \"name\": \"v1.0.0\" }", HttpStatusCode.BadRequest);
                    }
                }));

            task = new GitHubRelease
            {
                BuildEngine = new TestBuildEngine(),
                GitHubRepositoryApiUrl = "http://localhost:33333/repos/heleonix/heleonix.build",
                Name = tagName,
                Body = "-release note 1; -release note 2",
                TagName = tagName,
                TagSource = "master",
                Token = "111111111",
                UserAgent = "heleonix/heleonix.build",
                IsDraft = isDraft,
                IsPrerelease = isPrerelease,
            };
        });

        Act(() =>
        {
            succeeded = task.Execute();
        });

        Teardown(() =>
        {
            listener.Stop();
        });

        When("creation should succeed", () =>
        {
            tagName = "v1.0.0";

            And("it is draft release", () =>
            {
                isDraft = true;

                Should("succeed", () =>
                {
                    Assert.That(succeeded, Is.True);
                });
            });

            And("it is not draft release", () =>
            {
                isDraft = false;

                Should("succeed", () =>
                {
                    Assert.That(succeeded, Is.True);
                });
            });

            And("it is pre-release", () =>
            {
                isPrerelease = true;

                Should("succeed", () =>
                {
                    Assert.That(succeeded, Is.True);
                });
            });

            And("it is not pre-release", () =>
            {
                isPrerelease = false;

                Should("succeed", () =>
                {
                    Assert.That(succeeded, Is.True);
                });
            });
        });

        When("creation should fail", () =>
        {
            tagName = "v2.0.0";

            Should("fail", () =>
            {
                Assert.That(succeeded, Is.False);
            });
        });
    }
}
