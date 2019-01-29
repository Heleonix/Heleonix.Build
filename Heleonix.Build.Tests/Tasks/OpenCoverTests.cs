// <copyright file="OpenCoverTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Heleonix.Build.Tasks;
    using Heleonix.Build.Tests.Common;
    using Heleonix.Testing.NUnit.Aaa;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NUnit.Framework;
    using static Heleonix.Testing.NUnit.Aaa.AaaSpec;

    /// <summary>
    /// Tests the <see cref="NUnit"/>.
    /// </summary>
    [ComponentTest(Type = typeof(OpenCover))]
    public static class OpenCoverTests
    {
        /// <summary>
        /// Tests the <see cref="OpenCover.ExecuteInternal"/>.
        /// </summary>
        [MemberTest(Name = nameof(OpenCover.Execute))]
        public static void Execute()
        {
            OpenCover task = null;
            var succeeded = false;
            string coverageResultFile = null;
            string targetType = null;
            string targetExe = null;
            string outputDir = null;
            string testFilter = null;
            var minCoverage = 0;
            ITaskItem[] pdbSearchDirs = null;
            var simulatorHelper = new NetStandardSimulatorHelper();

            MSBuildHelper.RunTarget(simulatorHelper.SolutionFile, "Restore", null, simulatorHelper.SolutionDir);
            MSBuildHelper.RunTarget(simulatorHelper.SolutionFile, $"Build", null, simulatorHelper.SolutionDir);
            MSBuildHelper.Publish(
                simulatorHelper.TestProjectFile,
                simulatorHelper.TestProjectTargetFrameworks,
                simulatorHelper.TestProjectDir);

            Arrange(() =>
            {
                outputDir = PathHelper.GetRandomFileInCurrentDir();

                coverageResultFile = Path.Combine(outputDir, "Coverage.xml");

                Directory.CreateDirectory(outputDir);
            });

            Act(() =>
            {
                var targetMetadata = new Dictionary<string, string>
                {
                    { "Type", targetType },
                    { "TestFilter", testFilter },
                    { "NUnitProjectFileOrTestFiles", string.Join(";", simulatorHelper.TestBinaries) },
                    { "ErrorOutputFile", Path.Combine(outputDir, "Errors.txt") },
                    { "TestOutputFile", Path.Combine(outputDir, "Output.txt") },
                    { "TestResultFile", Path.Combine(outputDir, "Results.txt") }
                };

                task = new OpenCover
                {
                    BuildEngine = new TestBuildEngine(),
                    OpenCoverExe = new TaskItem(PathHelper.OpenCoverExe),
                    CoverageResultFile = new TaskItem(coverageResultFile),
                    Target = new TaskItem(targetExe, targetMetadata),
                    Register = "path64",
                    MinClassCoverage = minCoverage,
                    MinBranchCoverage = minCoverage,
                    MinLineCoverage = minCoverage,
                    MinMethodCoverage = minCoverage,
                    PdbSearchDirs = pdbSearchDirs
                };

                succeeded = task.Execute();
            });

            Teardown(() =>
            {
                Directory.Delete(outputDir, true);
            });

            When("target type is invalid", () =>
            {
                targetExe = "HX_NO_TARGET";

                Should("fail", () =>
                {
                    Assert.That(succeeded, Is.False);
                });
            });

            When("target type is 'NUnit'", () =>
            {
                targetType = nameof(NUnit);

                And("target exe is not specified", () =>
                {
                    targetExe = "HX_NO_TARGET";

                    Should("fail", () =>
                    {
                        Assert.That(succeeded, Is.False);
                    });

                    And("target exe is specified", () =>
                    {
                        targetExe = PathHelper.NUnitConsoleExe;

                        And("source code should pass coverage threshold", () =>
                        {
                            minCoverage = 0;

                            Should("succeed and provide coverage greater than or equal to minimal coverage", () =>
                            {
                                Assert.That(succeeded, Is.True);
                                Assert.That(task.ClassCoverage, Is.GreaterThanOrEqualTo(minCoverage));
                                Assert.That(task.MethodCoverage, Is.GreaterThanOrEqualTo(minCoverage));
                                Assert.That(task.BranchCoverage, Is.GreaterThanOrEqualTo(minCoverage));
                                Assert.That(task.LineCoverage, Is.GreaterThanOrEqualTo(minCoverage));
                            });

                            And("PDB search dirs are specified", () =>
                            {
                                pdbSearchDirs = simulatorHelper.TestPublishDirs
                                .Select(dir => new TaskItem(dir)).ToArray();

                                Should("succeed", () =>
                                {
                                    Assert.That(succeeded, Is.True);
                                });
                            });

                            And("PDB search dirs are not specified", () =>
                            {
                                pdbSearchDirs = null;

                                Should("succeed", () =>
                                {
                                    Assert.That(succeeded, Is.True);
                                });
                            });

                            And("target exit code is 0", () =>
                            {
                                testFilter = "method == PlusOne";

                                Should("succeed and provide coverage greater than or equal to minimal coverage", () =>
                                {
                                    Assert.That(succeeded, Is.True);
                                    Assert.That(task.ClassCoverage, Is.GreaterThanOrEqualTo(minCoverage));
                                    Assert.That(task.MethodCoverage, Is.GreaterThanOrEqualTo(minCoverage));
                                    Assert.That(task.BranchCoverage, Is.GreaterThanOrEqualTo(minCoverage));
                                    Assert.That(task.LineCoverage, Is.GreaterThanOrEqualTo(minCoverage));
                                });
                            });
                        });

                        And("source code should not pass coverage threshold", () =>
                        {
                            minCoverage = 100;

                            Should("fail", () =>
                            {
                                Assert.That(succeeded, Is.False);
                                Assert.That(task.ClassCoverage, Is.LessThan(minCoverage));
                                Assert.That(task.MethodCoverage, Is.LessThan(minCoverage));
                                Assert.That(task.BranchCoverage, Is.LessThan(minCoverage));
                                Assert.That(task.LineCoverage, Is.LessThan(minCoverage));
                            });
                        });
                    });
                });
            });

            simulatorHelper.Clear();
        }
    }
}
