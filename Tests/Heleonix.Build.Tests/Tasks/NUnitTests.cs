/*
The MIT License (MIT)

Copyright (c) 2015-2016 Heleonix - Hennadii Lutsyshyn

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
    public class NUnitTests : TaskTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="NUnit.Execute"/>.
        /// </summary>
        [Test]
        public void Execute()
        {
            var errorsFilePath = Path.Combine(LibSimulatorHelper.ReportsDirectoryPath, Path.GetRandomFileName());
            var testsOutputFilePath = Path.Combine(LibSimulatorHelper.ReportsDirectoryPath, Path.GetRandomFileName());
            var resultFilePath = Path.Combine(LibSimulatorHelper.ReportsDirectoryPath, Path.GetRandomFileName());

            var task = new Build.Tasks.NUnit
            {
                BuildEngine = new FakeBuildEngine(),
                NUnitConsoleExePath = new TaskItem(PathHelper.NUnitConsoleExePath),
                NUnitProjectOrTestsFilesPath = new ITaskItem[]
                {
                    new TaskItem(LibSimulatorHelper.TestsOutFilePath)
                },
                AgentsNumber = 3,
                ErrorsFilePath = new TaskItem(errorsFilePath),
                TestsOutputFilePath = new TaskItem(testsOutputFilePath),
                TestsResultsFilePath = new TaskItem(resultFilePath)
            };

            var succeeded = task.Execute();

            var errorsFileExists = File.Exists(errorsFilePath);
            var testsOutputFileExists = File.Exists(testsOutputFilePath);
            var resultFileExists = File.Exists(resultFilePath);

            try
            {
                Assert.That(succeeded, Is.False);
                Assert.That(errorsFileExists, Is.True);
                Assert.That(testsOutputFileExists, Is.True);
                Assert.That(resultFileExists, Is.True);

                var testRun = XDocument.Load(resultFilePath).Element("test-run");

                Assert.That(task.TestCases, Is.EqualTo(Convert.ToInt32(testRun.Attribute("testcasecount").Value)));
                Assert.That(task.Total, Is.EqualTo(Convert.ToInt32(testRun.Attribute("total").Value)));
                Assert.That(task.Passed, Is.EqualTo(Convert.ToInt32(testRun.Attribute("passed").Value)));
                Assert.That(task.Failed, Is.EqualTo(Convert.ToInt32(testRun.Attribute("failed").Value)));
                Assert.That(task.Inconclusive, Is.EqualTo(Convert.ToInt32(testRun.Attribute("inconclusive").Value)));
                Assert.That(task.Skipped, Is.EqualTo(Convert.ToInt32(testRun.Attribute("skipped").Value)));
                Assert.That(task.Asserts, Is.EqualTo(Convert.ToInt32(testRun.Attribute("asserts").Value)));
                Assert.That(task.StartTime, Is.EqualTo(testRun.Attribute("start-time").Value));
                Assert.That(task.EndTime, Is.EqualTo(testRun.Attribute("end-time").Value));
                Assert.That(task.Duration, Is.EqualTo(Convert.ToSingle(
                    testRun.Attribute("duration").Value, CultureInfo.InvariantCulture)));
            }
            finally
            {
                if (errorsFileExists)
                {
                    File.Delete(errorsFilePath);
                }

                if (testsOutputFileExists)
                {
                    File.Delete(testsOutputFilePath);
                }

                if (resultFileExists)
                {
                    File.Delete(resultFilePath);
                }
            }
        }

        #endregion

        #region TaskTests Members

        /// <summary>
        /// Gets the type of the simulator.
        /// </summary>
        protected override SimulatorType SimulatorType => SimulatorType.Library;

        #endregion
    }
}