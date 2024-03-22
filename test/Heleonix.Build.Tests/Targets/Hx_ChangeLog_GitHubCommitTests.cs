// <copyright file="Hx_ChangeLog_GitHubCommitTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Targets;

using System.Net;

[ComponentTest(Type = typeof(Hx_ChangeLog_GitHubCommitTests))]
[NonParallelizable]
public static class Hx_ChangeLog_GitHubCommitTests
{
    [MemberTest(Name = nameof(Hx_ChangeLog_GitHubCommitTests))]
    public static void Execute()
    {
        var succeeded = false;
        NetSimulatorHelper simulator = null;
        HttpListener listener = null;
        IDictionary<string, string> properties = null;
        string commits = null;

        Arrange(() =>
        {
            simulator = new NetSimulatorHelper();

            commits =
                @"[{""commit"":{""message"":""fix(ID-1): Fix 1.""}}," +
                @"{""commit"":{""message"":""feat(ID-2): Feat 2.""}}," +
                @"{""commit"":{""message"":""feat(ID-3)!: Breaking Feat 3.""}}," +
                @"{""commit"":{""message"":""fix(ID-4): Fix 4.""}}," +
                @"{""commit"":{""message"":""feat(ID-5): Feat 5.""}}," +
                @"{""commit"":{""message"":""feat(ID-6)!: Feat 6.\r\n\r\nBREAKING-CHANGE: Breaking 6.""}}," +
                @"{""commit"":{""message"":""perf(ID-4): Perf 7.""}}," +
                @"{""commit"":{""message"":""Free commit to be ignored.""}}]";

            var responses = new (string, Func<HttpListenerRequest, (string, HttpStatusCode)>)[]
            {
                (
                    "http://localhost:12345/repos/Heleonix/NetSimulator/releases/latest/",
                    request => (@"{""tag_name"":""v1.2.3"",""target_commitish"":""master"",""created_at"":""2023-02-27T19:35:32Z""}", HttpStatusCode.OK)),
                (
                    "http://localhost:12345/repos/Heleonix/NetSimulator/commits/",
                    request => (commits, HttpStatusCode.OK)),
            };

            listener = HttpHelper.LaunchHttpServer(responses);

            properties = new Dictionary<string, string>
            {
                { "Hx_ChangeLog_GitHubCommit_Token", "111111111" },
            };
        });

        Act(() =>
        {
            succeeded = ToolHelper.RunTestTarget("Hx_ChangeLog_GitHubCommit", simulator.SolutionDir, properties);
        });

        Teardown(() =>
        {
            listener.Abort();

            simulator.Clear();
        });

        When("Hx_ChangeLog_GitHubCommit target is executed", () =>
        {
            Should("succeed", () =>
            {
                Assert.That(succeeded, Is.True);

                var artifactsDir = simulator.GetArtifactsDir("Hx_ChangeLog");

                Assert.That(File.ReadAllLines(Path.Combine(artifactsDir, "semver.txt"))[0], Is.EqualTo("2.0.0"));

                var releaseNotes = File.ReadAllText(Path.Combine(artifactsDir, "ReleaseNotes.md"));

                Assert.That(releaseNotes, Contains.Substring("Fix 1."));
                Assert.That(releaseNotes, Contains.Substring("Feat 2."));
                Assert.That(releaseNotes, Contains.Substring("Breaking Feat 3."));
                Assert.That(releaseNotes, Contains.Substring("Fix 4."));
                Assert.That(releaseNotes, Contains.Substring("Feat 5."));
                Assert.That(releaseNotes, Contains.Substring("Feat 6."));
                Assert.That(releaseNotes, Contains.Substring("Perf 7."));

                releaseNotes = File.ReadAllText(Path.Combine(artifactsDir, "ReleaseNotes.txt"));

                Assert.That(releaseNotes, Contains.Substring("Fix 1."));
                Assert.That(releaseNotes, Contains.Substring("Feat 2."));
                Assert.That(releaseNotes, Contains.Substring("Breaking Feat 3."));
                Assert.That(releaseNotes, Contains.Substring("Fix 4."));
                Assert.That(releaseNotes, Contains.Substring("Feat 5."));
                Assert.That(releaseNotes, Contains.Substring("Feat 6."));
                Assert.That(releaseNotes, Contains.Substring("Perf 7."));
            });
        });
    }
}
