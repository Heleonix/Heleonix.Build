// <copyright file="GitHubCommitChangeLogTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

using System.Net;
using System.Text.Json.Nodes;

/// <summary>
/// Tests the <see cref="GitHubCommitChangeLog"/>.
/// </summary>
[ComponentTest(Type = typeof(GitHubCommitChangeLog))]
public static class GitHubCommitChangeLogTests
{
    /// <summary>
    /// Tests the <see cref="GitHubCommitChangeLog.ExecuteInternal"/>.
    /// </summary>
    [MemberTest(Name = nameof(GitHubCommitChangeLog.Execute))]
    public static void Execute()
    {
        GitHubCommitChangeLog task = null;
        var succeeded = false;
        HttpListener listener = null;
        (string, Func<HttpListenerRequest, (string, HttpStatusCode)>)[] responses = null;

        Arrange(() =>
        {
            listener = HttpHelper.LaunchHttpServer(responses);

            task = new GitHubCommitChangeLog
            {
                BuildEngine = new TestBuildEngine(),
                GitHubRepositoryApiUrl = "http://localhost:12345/repos/heleonix/heleonix.build",
                Token = "111111111",
                UserAgent = "heleonix/heleonix.build",
                VersionTagRegExp = @"(?<=\D*|^)(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)(?=\D*)",
                MajorChangeRegExp = @"^(\w+)(\([\w-.]+\))?!: (.+)|^(\w+)(\([\w-.]+\))?: (.+)([\r\n])*(?<=\r\r|\n\n|\r\n\r\n)[\s\S]*?(BREAKING CHANGE|BREAKING-CHANGE)(: | #)",
                MinorChangeRegExp = @"^(feat)(\([\w-.]+\))?: (.+)",
                PatchChangeRegExp = @"^(fix)(\([\w-.]+\))?: (.+)",
                ChangeLogRegExp = @"^(?'type'[a-z]+)(?:\((?'scope'[\w-.]+)\))?(?'breaking'!)?: (?'description'.+)(?:[\r\n])*(?'body'(?<=\r\r|\n\n|\r\n\r\n)[\s\S]+?(?!([\w-.]+|BREAKING CHANGE)(: | #)[\s\S]+))??(?:[\r\n])*(?'footer'(?<=\r\r|\n\n|\r\n\r\n)(?:[\w-.]+|(?'breaking'BREAKING-CHANGE|BREAKING CHANGE))(?:: | #)[\s\S]+?)?(?:[\r\n])*$",
            };
        });

        Act(() =>
        {
            succeeded = task.Execute();
        });

        Teardown(() =>
        {
            listener.Abort();
        });

        When("there is a release and commits after it", () =>
        {
            var requestsCount = 0;

            responses = new (string, Func<HttpListenerRequest, (string, HttpStatusCode)>)[]
            {
                (
                    "http://localhost:12345/repos/heleonix/heleonix.build/releases/latest/",
                    request => (@"{""tag_name"":""v1.2.3"",""target_commitish"":""master"",""created_at"":""2023-02-27T19:35:32Z""}", HttpStatusCode.OK)),
                (
                    "http://localhost:12345/repos/heleonix/heleonix.build/commits/",
                    (HttpListenerRequest request) =>
                    {
                        if (requestsCount == 0)
                        {
                            requestsCount += 1;

                            return (
                            @"[{""commit"":{""message"":""fix(ID-1): Fix 1.""}}," +
                            @"{""commit"":{""message"":""feat(ID-2): Feat 2.""}}," +
                            @"{""commit"":{""message"":""feat(ID-3)!: Breaking Feat 3.""}}," +
                            @"{""commit"":{""message"":""fix(ID-4): Fix 4.""}}," +
                            @"{""commit"":{""message"":""feat(ID-5): Feat 5.""}}," +
                            @"{""commit"":{""message"":""feat(ID-6)!: Feat 6.\r\n\r\nBREAKING-CHANGE: Breaking 6.""}}," +
                            @"{""commit"":{""message"":""perf(ID-4): Fix 4.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}]", HttpStatusCode.OK);
                        }
                        else
                        {
                            requestsCount += 1;

                            return (
                            @"[{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""message"":""Free commit to be ignored.""}}]", HttpStatusCode.OK);
                        }
                    }),
            };

            Should("succeed", () =>
            {
                Assert.That(succeeded, Is.True);
                Assert.That(task.Version, Is.EqualTo("2.0.0"));
                Assert.That(task.Changes.Length, Is.EqualTo(9));

                const string commits = @"[{""commit"":{""message"":""fix(ID-1): Fix 1.""}}," +
                            @"{""commit"":{""message"":""feat(ID-2): Feat 2.""}}," +
                            @"{""commit"":{""message"":""feat(ID-3)!: Breaking Feat 3.""}}," +
                            @"{""commit"":{""message"":""fix(ID-4): Fix 4.""}}," +
                            @"{""commit"":{""message"":""feat(ID-5): Feat 5.""}}," +
                            @"{""commit"":{""message"":""feat(ID-6)!: Feat 6.\r\n\r\nBREAKING-CHANGE: Breaking 6.""}}," +
                            @"{""commit"":{""message"":""perf(ID-4): Fix 4.""}}]";

                var commitsJson = JsonArray.Parse(commits);

                for (var i = 0; i < 7; i++)
                {
                    Assert.That(
                        commitsJson[i]["commit"]["message"].ToString(),
                        Is.EqualTo(task.Changes[i].ItemSpec));
                }

                Assert.That(task.Changes[7].ItemSpec, Is.EqualTo("2.0.0"));
                Assert.That(task.Changes[7].GetMetadata("Version"), Is.EqualTo("2.0.0"));
                Assert.That(task.Changes[8].ItemSpec, Is.EqualTo("1.2.3"));
                Assert.That(task.Changes[8].GetMetadata("PreviousVersion"), Is.EqualTo("1.2.3"));
            });
        });

        When("there is no releases", () =>
        {
            responses = new (string, Func<HttpListenerRequest, (string, HttpStatusCode)>)[]
            {
                (
                    "http://localhost:12345/repos/heleonix/heleonix.build/releases/latest/",
                    request => (@"{}", HttpStatusCode.OK)),
                (
                    "http://localhost:12345/repos/heleonix/heleonix.build/commits/",
                    request => (@"[]", HttpStatusCode.OK)),
            };

            Should("succeed", () =>
            {
                Assert.That(succeeded, Is.True);
                Assert.That(task.Version, Is.EqualTo("1.0.0"));
                Assert.That(task.Changes.Length, Is.EqualTo(2));

                Assert.That(task.Changes[0].ItemSpec, Is.EqualTo("1.0.0"));
                Assert.That(task.Changes[0].GetMetadata("Version"), Is.EqualTo("1.0.0"));
                Assert.That(task.Changes[1].ItemSpec, Is.EqualTo("0.0.0"));
                Assert.That(task.Changes[1].GetMetadata("PreviousVersion"), Is.EqualTo("0.0.0"));
            });
        });
    }
}