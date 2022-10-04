// <copyright file="NugetDeployTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Targets
{
    using System.Collections.Generic;
    using System.IO;
    using Heleonix.Build.Tests.Common;
    using Heleonix.Execution;
    using Heleonix.Testing.NUnit.Aaa;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NUnit.Framework;
    using static Heleonix.Testing.NUnit.Aaa.AaaSpec;

    /// <summary>
    /// Tests the NugetDeploy target.
    /// </summary>
    [ComponentTest(Type = typeof(NugetDeployTests))]
    public static class NugetDeployTests
    {
        /// <summary>
        /// Tests the <see cref="NugetDeployTests"/>.
        /// </summary>
        [MemberTest(Name = nameof(NugetDeployTests))]
        public static void Execute()
        {
            string sourceDir = null;
            var succeeded = false;
            IDictionary<string, ITaskItem[]> items = null;
            NetStandardSimulatorHelper simulatorHelper = null;

            Arrange(() =>
            {
                simulatorHelper = new NetStandardSimulatorHelper();

                MSBuildHelper.RunTestTarget("Hx_NetBuild", simulatorHelper.SolutionDir);

                sourceDir = PathHelper.GenerateRandomFileInCurrentDir();

                Directory.CreateDirectory(sourceDir);

                ExeHelper.Execute(PathHelper.NugetExe, $"init \"{sourceDir}\"");

                items = new Dictionary<string, ITaskItem[]>
                {
                    { "Hx_NugetDeploy_SourceURL", new ITaskItem[] { new TaskItem(sourceDir) } },
                };
            });

            Act(() =>
            {
                succeeded = MSBuildHelper.RunTestTarget("Hx_NugetDeploy", simulatorHelper.SolutionDir, null, items);
            });

            Teardown(() =>
            {
                Directory.Delete(sourceDir, true);
                simulatorHelper.Clear();
            });

            When("target is executed", () =>
            {
                Should("succeed", () =>
                {
                    var artifactDir = simulatorHelper.GetArtifactDir("Hx_NugetDeploy");

                    Assert.That(succeeded, Is.True);
                    Assert.That(File.Exists(Path.Combine(artifactDir, "NetStandardSimulator.1.0.0.nupkg")), Is.True);
                    Assert.That(File.Exists(Path.Combine(artifactDir, "NetStandardSimulator.1.0.0.snupkg")), Is.True);
                    Assert.That(File.Exists(Path.Combine(sourceDir, "NetStandardSimulator.1.0.0.nupkg")), Is.True);
                });
            });
        }
    }
}
