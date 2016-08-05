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

using System.IO;
using Heleonix.Build.Tasks;
using Heleonix.Build.Tests.Common;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace Heleonix.Build.Tests.Tasks
{
    /// <summary>
    /// Tests the <see cref="ReportUnit"/>.
    /// </summary>
    public class ReportUnitTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="BaseTask.Execute"/>.
        /// </summary>
        [Test]
        public void Execute()
        {
            var nunitResultFilePath = Path.Combine(LibSimulatorHelper.ReportsDirectoryPath,
                Path.GetRandomFileName() + ".xml");
            var reportFilePath = Path.Combine(LibSimulatorHelper.ReportsDirectoryPath,
                Path.GetRandomFileName() + ".html");

            var nunitTask = new Build.Tasks.NUnit
            {
                BuildEngine = new FakeBuildEngine(),
                NUnitConsoleExePath = new TaskItem(PathHelper.NUnitConsoleExePath),
                NUnitProjectOrTestsFilesPath = new ITaskItem[]
                {
                    new TaskItem(LibSimulatorHelper.TestsOutFilePath)
                },
                TestsResultsFilePath = new TaskItem(nunitResultFilePath)
            };

            var succeeded = nunitTask.Execute();

            try
            {
                Assert.That(succeeded, Is.False);

                var task = new ReportUnit
                {
                    BuildEngine = new FakeBuildEngine(),
                    ReportUnitExePath = new TaskItem(PathHelper.ReportUnitExePath),
                    TestsResultsFilePath = new TaskItem(nunitResultFilePath),
                    ReportFilePath = new TaskItem(reportFilePath)
                };

                succeeded = task.Execute();

                Assert.That(succeeded, Is.True);
                Assert.That(File.Exists(reportFilePath), Is.True);
            }
            finally
            {
                if (File.Exists(nunitResultFilePath))
                {
                    File.Delete(nunitResultFilePath);
                }

                if (File.Exists(reportFilePath))
                {
                    File.Delete(reportFilePath);
                }
            }
        }

        #endregion
    }
}