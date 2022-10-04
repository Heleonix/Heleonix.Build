// <copyright file="GitHubReleaseTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Heleonix.Build.Tasks;
    using Heleonix.Build.Tests.Common;
    using Heleonix.Testing.NUnit.Aaa;
    using NUnit.Framework;
    using static Heleonix.Testing.NUnit.Aaa.AaaSpec;

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
            Task server = null;
            string tagName = null;

            Arrange(() =>
            {
                server = CommunicationHelper.LaunchHttpServer(
                    "http://localhost:33333/repos/heleonix/heleonix.build/releases/",
                    request =>
                    {
                        using (var reader = new StreamReader(request.InputStream))
                        {
                            return reader.ReadToEnd().Contains("\"v1.0.0\"", StringComparison.Ordinal);
                        }
                    },
                    ("application/json", "{ \"name\": \"v1.0.0\" }", HttpStatusCode.Created),
                    ("application/json", "{ \"name\": \"v1.0.0\" }", HttpStatusCode.BadRequest));

                task = new GitHubRelease
                {
                    BuildEngine = new TestBuildEngine(),
                    ReleasesApiUrl = "http://localhost:33333/repos/heleonix/heleonix.build/releases",
                    Name = tagName,
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
                server.Wait();

                server.Dispose();
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

                And("it is no draft release", () =>
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

                And("it is no pre-release", () =>
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
}
