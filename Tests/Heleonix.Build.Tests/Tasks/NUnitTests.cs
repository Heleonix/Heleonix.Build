/*
The MIT License (MIT)

Copyright (c) 2015-present Heleonix - Hennadii Lutsyshyn

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Heleonix.Build.Tasks;
using Heleonix.Build.Tests.Common;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace Heleonix.Build.Tests.Tasks
{
    /// <summary>
    /// Tests the <see cref="NUnit"/>.
    /// </summary>
    public static class NUnitTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="NUnit.Execute"/>.
        /// </summary>
        [Test]
        public static void Execute()
        {
            MSBuildHelper.ExecuteMSBuild(LibSimulatorPath.SolutionFile, "Build", null, LibSimulatorPath.SolutionDir);

            var artifactsDir = LibSimulatorPath.GetArtifactsDir("Hxb-NUnit");

            var errorsOutput = Path.Combine(artifactsDir, Path.GetRandomFileName());
            var testsOutput = Path.Combine(artifactsDir, Path.GetRandomFileName());
            var result = Path.Combine(artifactsDir, Path.GetRandomFileName());

            var task = new Build.Tasks.NUnit
            {
                BuildEngine = new FakeBuildEngine(),
                NUnitConsoleExeFile = new TaskItem(SystemPath.NUnitConsoleExe),
                NUnitProjectFileOrTestsFiles = new ITaskItem[]
                {
                    new TaskItem(LibSimulatorPath.TestsOutFile)
                },
                AgentsNumber = 3,
                ErrorsOutputFile = new TaskItem(errorsOutput),
                TestsOutputFile = new TaskItem(testsOutput),
                TestsResultFile = new TaskItem(result)
            };

            var succeeded = task.Execute();

            var errorsExists = File.Exists(errorsOutput);
            var testsOutputExists = File.Exists(testsOutput);
            var resultExists = File.Exists(result);

            try
            {
                Assert.That(succeeded, Is.True);
                Assert.That(errorsExists, Is.True);
                Assert.That(testsOutputExists, Is.True);
                Assert.That(resultExists, Is.True);

                var testRun = XDocument.Load(result).Element("test-run");

                Assert.That(task.TestCases, Is.EqualTo(Convert.ToInt32(testRun.Attribute("testcasecount").Value,
                    NumberFormatInfo.InvariantInfo)));
                Assert.That(task.Total, Is.EqualTo(Convert.ToInt32(testRun.Attribute("total").Value,
                    NumberFormatInfo.InvariantInfo)));
                Assert.That(task.Passed, Is.EqualTo(Convert.ToInt32(testRun.Attribute("passed").Value,
                    NumberFormatInfo.InvariantInfo)));
                Assert.That(task.Failed, Is.EqualTo(Convert.ToInt32(testRun.Attribute("failed").Value,
                    NumberFormatInfo.InvariantInfo)));
                Assert.That(task.Inconclusive, Is.EqualTo(Convert.ToInt32(testRun.Attribute("inconclusive").Value,
                    NumberFormatInfo.InvariantInfo)));
                Assert.That(task.Skipped, Is.EqualTo(Convert.ToInt32(testRun.Attribute("skipped").Value,
                    NumberFormatInfo.InvariantInfo)));
                Assert.That(task.Asserts, Is.EqualTo(Convert.ToInt32(testRun.Attribute("asserts").Value,
                    NumberFormatInfo.InvariantInfo)));
                Assert.That(task.StartTime, Is.EqualTo(testRun.Attribute("start-time").Value));
                Assert.That(task.EndTime, Is.EqualTo(testRun.Attribute("end-time").Value));
                Assert.That(task.Duration, Is.EqualTo(Convert.ToSingle(
                    testRun.Attribute("duration").Value, NumberFormatInfo.InvariantInfo)));
            }
            finally
            {
                if (errorsExists)
                {
                    File.Delete(errorsOutput);
                }

                if (testsOutputExists)
                {
                    File.Delete(testsOutput);
                }

                if (resultExists)
                {
                    File.Delete(result);
                }
            }
        }

        #endregion
    }
}