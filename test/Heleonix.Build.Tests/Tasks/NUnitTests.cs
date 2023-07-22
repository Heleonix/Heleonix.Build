// <copyright file="NUnitTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

using System.Xml.Linq;
using Heleonix.Build.Tasks;
using NUnit.Framework.Internal;
using static System.Globalization.NumberFormatInfo;

/// <summary>
/// Tests the <see cref="NUnit"/>.
/// </summary>
[ComponentTest(Type = typeof(NUnit))]
public static class NUnitTests
{
    /// <summary>
    /// Tests the <see cref="NUnit.ExecuteInternal"/>.
    /// </summary>
    [MemberTest(Name = nameof(NUnit.Execute))]
    public static void Execute()
    {
        NUnit task = null;
        var succeeded = false;
        string traceLevel = null;
        string testFilter = null;
        string testListFile = null;
        string outputDir = null;

        var simulator = new NetSimulatorHelper();

        TestExecutionContext.CurrentContext.OutWriter.WriteLine("Tasks.NUnitTests: copied NetSimulator");

        try
        {
            MSBuildHelper.RunTarget(simulator.SolutionFile, "Restore", null, simulator.SolutionDir);

            TestExecutionContext.CurrentContext.OutWriter.WriteLine("Tasks.NUnitTests: restored");

            MSBuildHelper.RunTarget(simulator.SolutionFile, $"Build", null, simulator.SolutionDir);

            TestExecutionContext.CurrentContext.OutWriter.WriteLine("Tasks.NUnitTests: Built");

            Arrange(() =>
            {
                testFilter = null;

                testListFile = null;

                outputDir = PathHelper.GetRandomFileNameInCurrentDir();

                Directory.CreateDirectory(outputDir);
            });

            Act(() =>
            {
                TestExecutionContext.CurrentContext.OutWriter.WriteLine("Tasks.NUnitTests: Starting task");

                task = new NUnit
                {
                    BuildEngine = new TestBuildEngine(),
                    NUnitConsoleExe = PathHelper.NUnitConsoleExe,
                    AgentsNumber = 3,
                    NUnitProjectFileOrTestFiles =
                        simulator.TestBinaries.Select(file => new TaskItem(file)).ToArray(),
                    TestOutputFile = Path.Combine(outputDir, "Output.txt"),
                    TestResultFile = Path.Combine(outputDir, "Results.txt"),
                    TestListFile = testListFile,
                    TraceLevel = traceLevel,
                    TestFilter = testFilter,
                };

                succeeded = task.Execute();

                TestExecutionContext.CurrentContext.OutWriter.WriteLine("Tasks.NUnitTests: Task finished");
            });

            Teardown(() =>
            {
                Directory.Delete(outputDir, true);
            });

            When("all parameters are valid", () =>
            {
                And("all tests are included", () =>
                {
                    Arrange(() =>
                    {
                        testFilter = null;
                    });

                    Should("should fail on failed tests", () =>
                    {
                        Assert.That(succeeded, Is.False);

                        var testRun = XDocument.Load(Path.Combine(outputDir, "Results.txt")).Element("test-run");

                        Assert.That(
                            task.TestCases,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("testcasecount").Value, InvariantInfo)));
                        Assert.That(
                            task.Total,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("total").Value, InvariantInfo)));
                        Assert.That(
                            task.Passed,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("passed").Value, InvariantInfo)));
                        Assert.That(
                            task.Failed,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("failed").Value, InvariantInfo)));
                        Assert.That(
                            task.Inconclusive,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("inconclusive").Value, InvariantInfo)));
                        Assert.That(
                            task.Skipped,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("skipped").Value, InvariantInfo)));
                        Assert.That(
                            task.Asserts,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("asserts").Value, InvariantInfo)));
                        Assert.That(
                            task.StartTime,
                            Is.EqualTo(testRun.Attribute("start-time").Value));
                        Assert.That(
                            task.EndTime,
                            Is.EqualTo(testRun.Attribute("end-time").Value));
                        Assert.That(
                            task.Duration,
                            Is.EqualTo(Convert.ToSingle(testRun.Attribute("duration").Value, InvariantInfo)));
                    });
                });

                And("empty test list file is specified", () =>
                {
                    Arrange(() =>
                    {
                        testListFile = PathHelper.GetRandomFileNameInCurrentDir();

                        File.Create(testListFile).Close();
                    });

                    Teardown(() =>
                    {
                        File.Delete(testListFile);
                    });

                    Should("fail with failed tests", () =>
                    {
                        Assert.That(succeeded, Is.False);

                        var testRun = XDocument.Load(Path.Combine(outputDir, "Results.txt")).Element("test-run");

                        Assert.That(
                            task.TestCases,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("testcasecount").Value, InvariantInfo)));
                        Assert.That(
                            task.Total,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("total").Value, InvariantInfo)));
                        Assert.That(
                            task.Passed,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("passed").Value, InvariantInfo)));
                        Assert.That(
                            task.Failed,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("failed").Value, InvariantInfo)));
                        Assert.That(
                            task.Inconclusive,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("inconclusive").Value, InvariantInfo)));
                        Assert.That(
                            task.Skipped,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("skipped").Value, InvariantInfo)));
                        Assert.That(
                            task.Asserts,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("asserts").Value, InvariantInfo)));
                        Assert.That(
                            task.StartTime,
                            Is.EqualTo(testRun.Attribute("start-time").Value));
                        Assert.That(
                            task.EndTime,
                            Is.EqualTo(testRun.Attribute("end-time").Value));
                        Assert.That(
                            task.Duration,
                            Is.EqualTo(Convert.ToSingle(testRun.Attribute("duration").Value, InvariantInfo)));
                    });
                });

                And("only successful tests are included", () =>
                {
                    Arrange(() =>
                    {
                        testFilter = "name == PlusOne";
                    });

                    Should("succeed", () =>
                    {
                        // One test per TFM assembly.
                        var numOfTFMs = simulator.TestProjectTargetFrameworks.Count();

                        Assert.That(succeeded, Is.True);
                        Assert.That(task.Total, Is.EqualTo(numOfTFMs));
                        Assert.That(task.Passed, Is.EqualTo(numOfTFMs));
                        Assert.That(task.Failed, Is.EqualTo(0));
                    });
                });
            });

            When("trace level is invalid to simulate an NUnit error", () =>
            {
                Arrange(() =>
                {
                    traceLevel = "INVALID_TRACE_LEVEL";
                });

                Should("fail", () =>
                {
                    Assert.That(succeeded, Is.False);
                });
            });
        }
        finally
        {
            simulator.Clear();
        }
    }
}
