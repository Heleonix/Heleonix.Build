// <copyright file="Hx_GitHubCommitChangeLogTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

using System.Net;
using System.Text.Json.Nodes;

[ComponentTest(Type = typeof(Hx_GitHubCommitChangeLog))]
[NonParallelizable]
public static class Hx_GitHubCommitChangeLogTests
{
    [MemberTest(Name = nameof(Hx_GitHubCommitChangeLog.Execute))]
    public static void Execute()
    {
        Hx_GitHubCommitChangeLog task = null;
        var succeeded = false;
        HttpListener listener = null;
        (string, Func<HttpListenerRequest, (string, HttpStatusCode)>)[] responses = null;

        Arrange(() =>
        {
            listener = HttpHelper.LaunchHttpServer(responses);

            task = new Hx_GitHubCommitChangeLog
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
                    request => (@"{""tag_name"":""v1.2.3"",""target_commitish"":""master"",""created_at"":""2023-01-01T11:00:00Z""}", HttpStatusCode.OK)),
                (
                    "http://localhost:12345/repos/heleonix/heleonix.build/commits/",
                    (HttpListenerRequest request) =>
                    {
                        if (requestsCount == 0)
                        {
                            requestsCount += 1;

                            return (
                            @"[{""commit"":{""committer"":{""date"":""2022-11-01T11:00:00Z""},""message"":""fix(ID-1): Fix 1.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""feat(ID-2): Feat 2.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""feat(ID-3)!: Breaking Feat 3.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""fix(ID-4): Fix 4.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""feat(ID-5): Feat 5.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""feat(ID-6)!: Feat 6.\r\n\r\nBREAKING-CHANGE: Breaking 6.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""perf(ID-4): Fix 4.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}]", HttpStatusCode.OK);
                        }
                        else
                        {
                            requestsCount += 1;

                            return (
                            @"[{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}," +
                            @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""Free commit to be ignored.""}}]", HttpStatusCode.OK);
                        }
                    }),
            };

            Should("succeed", () =>
            {
                Assert.That(succeeded, Is.True);
                Assert.That(task.Version, Is.EqualTo("2.0.0"));
                Assert.That(task.Changes.Length, Is.EqualTo(8));

                const string commits =
                @"[{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""feat(ID-2): Feat 2.""}}," +
                @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""feat(ID-3)!: Breaking Feat 3.""}}," +
                @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""fix(ID-4): Fix 4.""}}," +
                @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""feat(ID-5): Feat 5.""}}," +
                @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""feat(ID-6)!: Feat 6.\r\n\r\nBREAKING-CHANGE: Breaking 6.""}}," +
                @"{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""perf(ID-4): Fix 4.""}}]";

                var commitsJson = JsonArray.Parse(commits);

                for (var i = 0; i < 6; i++)
                {
                    Assert.That(
                        commitsJson[i]["commit"]["message"].ToString(),
                        Is.EqualTo(task.Changes[i].ItemSpec));
                }

                Assert.That(task.Changes[6].ItemSpec, Is.EqualTo("2.0.0"));
                Assert.That(task.Changes[6].GetMetadata("Version"), Is.EqualTo("2.0.0"));
                Assert.That(task.Changes[7].ItemSpec, Is.EqualTo("1.2.3"));
                Assert.That(task.Changes[7].GetMetadata("PreviousVersion"), Is.EqualTo("1.2.3"));
            });
        });

        When("there is no releases", () =>
        {
            responses = new (string, Func<HttpListenerRequest, (string, HttpStatusCode)>)[]
            {
                (
                    "http://localhost:12345/repos/heleonix/heleonix.build/releases/latest/",
                    request => (@"{}", HttpStatusCode.NotFound)),
                (
                    "http://localhost:12345/repos/heleonix/heleonix.build/commits/",
                    request => (@"[{""commit"":{""committer"":{""date"":""2023-02-01T11:00:00Z""},""message"":""fix(ID-1): Fix 1.""}}]", HttpStatusCode.OK)),
            };

            Should("succeed", () =>
            {
                Assert.That(succeeded, Is.True);
                Assert.That(task.Version, Is.EqualTo("1.0.0"));
                Assert.That(task.Changes.Length, Is.EqualTo(3));

                Assert.That(task.Changes[0].ItemSpec, Is.EqualTo("fix(ID-1): Fix 1."));
                Assert.That(task.Changes[1].ItemSpec, Is.EqualTo("1.0.0"));
                Assert.That(task.Changes[1].GetMetadata("Version"), Is.EqualTo("1.0.0"));
                Assert.That(task.Changes[2].ItemSpec, Is.EqualTo("0.0.0"));
                Assert.That(task.Changes[2].GetMetadata("PreviousVersion"), Is.EqualTo("0.0.0"));
            });
        });
    }
}