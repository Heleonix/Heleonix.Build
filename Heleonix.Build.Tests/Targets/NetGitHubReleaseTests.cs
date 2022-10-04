// <copyright file="NetGitHubReleaseTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Targets
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Heleonix.Build.Tests.Common;
    using Heleonix.Testing.NUnit.Aaa;
    using NUnit.Framework;
    using static Heleonix.Testing.NUnit.Aaa.AaaSpec;

    /// <summary>
    /// Tests the GitHubReleaseNet target.
    /// </summary>
    [ComponentTest(Type = typeof(NetGitHubReleaseTests))]
    public static class NetGitHubReleaseTests
    {
        /// <summary>
        /// Tests the <see cref="NetGitHubReleaseTests"/>.
        /// </summary>
        [MemberTest(Name = nameof(NetGitHubReleaseTests))]
        public static void Execute()
        {
            Task server = null;
            var succeeded = false;
            IDictionary<string, string> properties = null;
            NetStandardSimulatorHelper simulatorHelper = null;

            Arrange(() =>
            {
                simulatorHelper = new NetStandardSimulatorHelper();

                MSBuildHelper.RunTestTarget("Hx_NetBuild", simulatorHelper.SolutionDir);

                properties = new Dictionary<string, string>
                {
                    { "Hx_NetGithubRelease_Token", "111111111" },
                };

                server = CommunicationHelper.LaunchHttpServer(
                    "http://localhost:33333/repos/Heleonix/NetStandardSimulator/releases/",
                    request =>
                    {
                        using (var reader = new StreamReader(request.InputStream))
                        {
                            return reader.ReadToEnd().Contains("\"v1.0.0\"", StringComparison.Ordinal);
                        }
                    },
                    ("application/json", "{ \"name\": \"v1.0.0\" }", HttpStatusCode.Created),
                    ("application/json", "{ \"name\": \"v1.0.0\" }", HttpStatusCode.BadRequest));
            });

            Act(() =>
            {
                succeeded = MSBuildHelper.RunTestTarget(
                    "Hx_NetGitHubRelease",
                    simulatorHelper.SolutionDir,
                    properties);
            });

            Teardown(() =>
            {
                server.Wait();
                server.Dispose();
                simulatorHelper.Clear();
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
}
