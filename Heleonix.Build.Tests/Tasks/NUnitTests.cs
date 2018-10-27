// <copyright file="NUnitTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
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
            var failOnFailedTests = false;
            string traceLevel = null;
            ITaskItem testListFile = null;
            string outputDir = null;

            MSBuildHelper.Publish(
                NetStandardSimulatorPathHelper.TestProjectFile,
                NetStandardSimulatorPathHelper.TestProjectTargetFrameworks,
                NetStandardSimulatorPathHelper.TestProjectDir);

            Arrange(() =>
            {
                outputDir = PathHelper.GetRandomFileInCurrentDir();

                Directory.CreateDirectory(outputDir);
            });

            Act(() =>
            {
                task = new NUnit
                {
                    BuildEngine = new TestBuildEngine(),
                    NUnitConsoleExe = new TaskItem(PathHelper.NUnitConsoleExe),
                    AgentsNumber = 3,
                    NUnitProjectFileOrTestFiles =
                        NetStandardSimulatorPathHelper.TestBinaries.Select(file => new TaskItem(file)).ToArray(),
                    ErrorOutputFile = new TaskItem(Path.Combine(outputDir, "Errors.txt")),
                    TestOutputFile = new TaskItem(Path.Combine(outputDir, "Output.txt")),
                    TestResultFile = new TaskItem(Path.Combine(outputDir, "Results.txt")),
                    FailOnFailedTests = failOnFailedTests,
                    TraceLevel = traceLevel,
                    TestListFile = testListFile
                };

                succeeded = task.Execute();
            });

            Teardown(() =>
            {
                Directory.Delete(outputDir, true);
            });

            When("all parameters are valid", () =>
            {
                And("should not fail on failed tests", () =>
                {
                    failOnFailedTests = false;

                    Should("succeed", () =>
                    {
                        Assert.That(succeeded, Is.True);

                        var testRun = XDocument.Load(Path.Combine(outputDir, "Results.txt")).Element("test-run");

                        Assert.That(
                            task.TestCases,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("testcasecount").Value, NumberFormatInfo.InvariantInfo)));
                        Assert.That(
                            task.Total,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("total").Value, NumberFormatInfo.InvariantInfo)));
                        Assert.That(
                            task.Passed,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("passed").Value, NumberFormatInfo.InvariantInfo)));
                        Assert.That(
                            task.Failed,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("failed").Value, NumberFormatInfo.InvariantInfo)));
                        Assert.That(
                            task.Inconclusive,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("inconclusive").Value, NumberFormatInfo.InvariantInfo)));
                        Assert.That(
                            task.Skipped,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("skipped").Value, NumberFormatInfo.InvariantInfo)));
                        Assert.That(
                            task.Asserts,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("asserts").Value, NumberFormatInfo.InvariantInfo)));
                        Assert.That(
                            task.StartTime,
                            Is.EqualTo(testRun.Attribute("start-time").Value));
                        Assert.That(
                            task.EndTime,
                            Is.EqualTo(testRun.Attribute("end-time").Value));
                        Assert.That(
                            task.Duration,
                            Is.EqualTo(Convert.ToSingle(testRun.Attribute("duration").Value, NumberFormatInfo.InvariantInfo)));
                    });
                });

                And("should fail on failed tests", () =>
                {
                    failOnFailedTests = true;

                    Should("fail", () =>
                    {
                        Assert.That(succeeded, Is.False);

                        var testRun = XDocument.Load(Path.Combine(outputDir, "Results.txt")).Element("test-run");

                        Assert.That(
                            task.TestCases,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("testcasecount").Value, NumberFormatInfo.InvariantInfo)));
                        Assert.That(
                            task.Total,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("total").Value, NumberFormatInfo.InvariantInfo)));
                        Assert.That(
                            task.Passed,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("passed").Value, NumberFormatInfo.InvariantInfo)));
                        Assert.That(
                            task.Failed,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("failed").Value, NumberFormatInfo.InvariantInfo)));
                        Assert.That(
                            task.Inconclusive,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("inconclusive").Value, NumberFormatInfo.InvariantInfo)));
                        Assert.That(
                            task.Skipped,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("skipped").Value, NumberFormatInfo.InvariantInfo)));
                        Assert.That(
                            task.Asserts,
                            Is.EqualTo(Convert.ToInt32(testRun.Attribute("asserts").Value, NumberFormatInfo.InvariantInfo)));
                        Assert.That(
                            task.StartTime,
                            Is.EqualTo(testRun.Attribute("start-time").Value));
                        Assert.That(
                            task.EndTime,
                            Is.EqualTo(testRun.Attribute("end-time").Value));
                        Assert.That(
                            task.Duration,
                            Is.EqualTo(Convert.ToSingle(testRun.Attribute("duration").Value, NumberFormatInfo.InvariantInfo)));
                    });
                });
            });

            When("trace level is invalid to simulate an NUnit error", () =>
            {
                traceLevel = "INVALID_TRACE_LEVEL";

                Should("fail", () =>
                {
                    Assert.That(succeeded, Is.False);
                });
            });
        }
    }
}
