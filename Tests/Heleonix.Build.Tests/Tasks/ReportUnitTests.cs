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
    public class ReportUnitTests : TaskTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="BaseTask.Execute"/>.
        /// </summary>
        [Test]
        public void Execute()
        {
            var nunitResult = Path.Combine(LibSimulatorHelper.ReportsDir, Path.GetRandomFileName() + ".xml");
            var report = Path.Combine(LibSimulatorHelper.ReportsDir, Path.GetRandomFileName() + ".html");

            var nunitTask = new Build.Tasks.NUnit
            {
                BuildEngine = new FakeBuildEngine(),
                NUnitConsoleExeFile = new TaskItem(PathHelper.NUnitConsoleExe),
                NUnitProjectOrTestsFiles = new ITaskItem[]
                {
                    new TaskItem(LibSimulatorHelper.TestsOut)
                },
                TestsResultFile = new TaskItem(nunitResult)
            };

            var succeeded = nunitTask.Execute();

            try
            {
                Assert.That(succeeded, Is.True);

                var task = new ReportUnit
                {
                    BuildEngine = new FakeBuildEngine(),
                    ReportUnitExeFile = new TaskItem(PathHelper.ReportUnitExe),
                    TestsResultFile = new TaskItem(nunitResult),
                    ReportFile = new TaskItem(report)
                };

                succeeded = task.Execute();

                Assert.That(succeeded, Is.True);
                Assert.That(File.Exists(report), Is.True);
            }
            finally
            {
                if (File.Exists(nunitResult))
                {
                    File.Delete(nunitResult);
                }

                if (File.Exists(report))
                {
                    File.Delete(report);
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