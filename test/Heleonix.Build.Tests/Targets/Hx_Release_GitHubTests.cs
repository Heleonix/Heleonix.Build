// <copyright file="Hx_Release_GitHubTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Targets;

using System.Net;

/// <summary>
/// Tests the GitHubReleaseNet target.
/// </summary>
[ComponentTest(Type = typeof(Hx_Release_GitHubTests))]
[NonParallelizable]
public static class Hx_Release_GitHubTests
{
    /// <summary>
    /// Tests the <see cref="Hx_Release_GitHubTests"/>.
    /// </summary>
    [MemberTest(Name = nameof(Hx_Release_GitHubTests))]
    public static void Execute()
    {
        HttpListener listener = null;
        var succeeded = false;
        IDictionary<string, string> properties = null;
        NetSimulatorHelper simulator = null;

        Arrange(() =>
        {
            simulator = new NetSimulatorHelper();
            const string content = @"Release
-release note 1;
-release note 2";

            Directory.CreateDirectory(simulator.GetArtifactsDir("Hx_ChangeLog"));
            File.WriteAllText(Path.Combine(simulator.GetArtifactsDir("Hx_ChangeLog"), "semver.txt"), "1.2.3\r\n");
            File.WriteAllText(Path.Combine(simulator.GetArtifactsDir("Hx_ChangeLog"), "ReleaseNotes.md"), content);

            properties = new Dictionary<string, string>
            {
                { "Hx_Release_GitHub_Token", "111111111" },
            };

            listener = HttpHelper.LaunchHttpServer((
                "http://localhost:12345/repos/Heleonix/NetSimulator/releases/",
                request =>
                {
                    using (var reader = new StreamReader(request.InputStream))
                    {
                        var req = reader.ReadToEnd();

                        return req.Contains("v1.2.3") && req.Contains("-release note 1;") ?
                        ("{ \"name\": \"v1.2.3\" }", HttpStatusCode.Created) :
                        ("{ \"name\": \"v1.2.3\" }", HttpStatusCode.BadRequest);
                    }
                }));
        });

        Act(() =>
        {
            succeeded = ToolHelper.RunTestTarget("Hx_Release_GitHub", simulator.SolutionDir, properties);
        });

        Teardown(() =>
        {
            listener.Abort();
            simulator.Clear();
        });

        When("Hx_Release_GitHub target is executed", () =>
        {
            Should("succeed", () =>
            {
                Assert.That(succeeded, Is.True);
            });
        });
    }
}
