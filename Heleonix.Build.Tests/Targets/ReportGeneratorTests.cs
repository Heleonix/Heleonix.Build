// <copyright file="ReportGeneratorTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
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
    /// Tests the ReportGenerator target.
    /// </summary>
    [ComponentTest(Type = typeof(ReportGeneratorTests))]
    public static class ReportGeneratorTests
    {
        /// <summary>
        /// Tests the <see cref="ReportGeneratorTests"/>.
        /// </summary>
        [MemberTest(Name = nameof(ReportGeneratorTests))]
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
                        "Hx_ReportGenerator_CoverageResultFiles",
                        new ITaskItem[] { new TaskItem(PathHelper.OpenCoverResultFile) }
                    },
                };
            });

            Act(() =>
            {
                succeeded = MSBuildHelper.RunTestTarget("Hx_ReportGenerator", simulatorHelper.SolutionDir, null, items);
            });

            Teardown(() =>
            {
                simulatorHelper.Clear();
            });

            When("target is executed", () =>
            {
                Should("succeed", () =>
                {
                    var artifactDir = simulatorHelper.GetArtifactDir("Hx_ReportGenerator");

                    Assert.That(succeeded, Is.True);
                    Assert.That(File.Exists(Path.Combine(artifactDir, "index.htm")), Is.True);
                    Assert.That(File.Exists(Path.Combine(artifactDir, "report.css")), Is.True);
                });
            });
        }
    }
}
