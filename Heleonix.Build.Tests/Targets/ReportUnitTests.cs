// <copyright file="ReportUnitTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Targets
{
    using System.Collections.Generic;
    using System.IO;
    using Heleonix.Build.Tests.Common;
    using Heleonix.Testing.NUnit.Aaa;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NUnit.Framework;
    using static Heleonix.Testing.NUnit.Aaa.AaaSpec;

    /// <summary>
    /// Tests the ReportUnit target.
    /// </summary>
    [ComponentTest(Type = typeof(ReportUnitTests))]
    public static class ReportUnitTests
    {
        /// <summary>
        /// Tests the <see cref="ReportUnitTests"/>.
        /// </summary>
        [MemberTest(Name = nameof(ReportUnitTests))]
        public static void Execute()
        {
            var succeeded = false;
            IDictionary<string, ITaskItem[]> items = null;
            NetStandardSimulatorHelper simulatorHelper = null;

            Arrange(() =>
            {
                simulatorHelper = new NetStandardSimulatorHelper();

                items = new Dictionary<string, ITaskItem[]>
                {
                    {
                        "Hx_ReportUnit_TestResultFile",
                        new ITaskItem[] { new TaskItem(PathHelper.NUnitTestResultFile) }
                    },
                };
            });

            Act(() =>
            {
                succeeded = MSBuildHelper.RunTestTarget("Hx_ReportUnit", simulatorHelper.SolutionDir, null, items);
            });

            Teardown(() =>
            {
                simulatorHelper.Clear();
            });

            When("target is executed", () =>
            {
                Should("succeed", () =>
                {
                    var artifactDir = simulatorHelper.GetArtifactDir("Hx_ReportUnit");

                    Assert.That(succeeded, Is.True);
                    Assert.That(File.Exists(Path.Combine(artifactDir, "ReportUnit.html")), Is.True);
                });
            });
        }
    }
}
