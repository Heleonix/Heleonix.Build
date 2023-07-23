// <copyright file="OpenCoverTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

using System.Xml.Linq;

/// <summary>
/// Tests the <see cref="OpenCover"/>.
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
        string filePathRegExp = null;
        string filePathReplacement = null;
        var minCoverage = 0;
        ITaskItem[] pdbSearchDirs = null;

        var simulator = new NetSimulatorHelper();

        try
        {
            MSBuildHelper.RunTarget(simulator.SolutionFile, "Restore", null, simulator.SolutionDir);
            MSBuildHelper.RunTarget(simulator.SolutionFile, $"Build", null, simulator.SolutionDir);

            Arrange(() =>
            {
                outputDir = PathHelper.GetRandomFileNameInCurrentDir();

                coverageResultFile = Path.Combine(outputDir, "Coverage.xml");

                Directory.CreateDirectory(outputDir);
            });

            Act(() =>
            {
                var targetMetadata = new Dictionary<string, string>
                {
                { "Type", targetType },
                { "TestFilter", testFilter },
                { "NUnitProjectFileOrTestFiles", string.Join(";", simulator.TestBinaries) },
                { "TestOutputFile", Path.Combine(outputDir, "Output.txt") },
                { "TestResultFile", Path.Combine(outputDir, "Results.txt") },
                };

                task = new OpenCover
                {
                    BuildEngine = new TestBuildEngine(),
                    OpenCoverExe = PathHelper.OpenCoverExe,
                    CoverageResultFile = coverageResultFile,
                    Target = new TaskItem(targetExe, targetMetadata),
                    Register = "path64",
                    MinClassCoverage = minCoverage,
                    MinBranchCoverage = minCoverage,
                    MinLineCoverage = minCoverage,
                    MinMethodCoverage = minCoverage,
                    PdbSearchDirs = pdbSearchDirs,
                    ExcludeDirs = new ITaskItem[] { new TaskItem("Z:\\NON_EXISTING_DIR") },
                    FilePathRegExp = filePathRegExp,
                    FilePathReplacement = filePathReplacement,
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

                And("PDB search dirs are specified", () =>
                {
                    pdbSearchDirs = simulator.TestDirs
                        .Select(dir => new TaskItem(dir)).ToArray();

                    And("target exe is invalid", () =>
                    {
                        targetExe = "HX_NO_TARGET";

                        Should("fail", () =>
                        {
                            Assert.That(succeeded, Is.False);
                        });
                    });

                    And("target exe is specified", () =>
                    {
                        targetExe = PathHelper.NUnitConsoleExe;

                        And("replacement of file paths is specified", () =>
                        {
                            filePathRegExp = @".*?\\Heleonix\.Build";
                            filePathReplacement = @"https://some.url.com";

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
                                    Assert.That(task.VisitedClasses, Is.GreaterThan(0));
                                    Assert.That(task.VisitedMethods, Is.GreaterThan(0));
                                    Assert.That(task.VisitedLines, Is.GreaterThan(0));
                                    Assert.That(task.TotalClasses, Is.GreaterThan(0));
                                    Assert.That(task.TotalMethods, Is.GreaterThan(0));
                                    Assert.That(task.TotalBranches, Is.GreaterThan(0));
                                    Assert.That(task.TotalLines, Is.GreaterThan(0));

                                    var paths = XDocument.Load(coverageResultFile)
                                    .Element("CoverageSession")
                                    .Element("Modules")
                                    .Elements(@"Module").Where(e => e.Attribute("skippedDueTo") == null)
                                    .Elements("Files")
                                    .Elements("File")
                                    .Attributes("fullPath");

                                    Assert.That(paths, Has.All.StartsWith(filePathReplacement));
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
                        });

                        And("replacement of file paths is not specified", () =>
                        {
                            filePathRegExp = null;
                            filePathReplacement = null;

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
            });
        }
        finally
        {
            simulator.Clear();
        }
    }
}
