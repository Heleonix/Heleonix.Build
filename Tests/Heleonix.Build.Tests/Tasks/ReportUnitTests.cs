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
    public static class ReportUnitTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="BaseTask.Execute"/>.
        /// </summary>
        [Test]
        public static void Execute()
        {
            MSBuildHelper.ExecuteMSBuild(LibSimulatorPath.SolutionFile, "Build", null);

            var artifactsDir = LibSimulatorPath.GetArtifactsDir("Hxb-ReportUnit");

            var nunitResult = Path.Combine(artifactsDir, Path.GetRandomFileName() + ".xml");
            var report = Path.Combine(artifactsDir, Path.GetRandomFileName() + ".html");

            var nunitTask = new Build.Tasks.NUnit
            {
                BuildEngine = new FakeBuildEngine(),
                NUnitConsoleExeFile = new TaskItem(SystemPath.NUnitConsoleExe),
                NUnitProjectFileOrTestsFiles = new ITaskItem[]
                {
                    new TaskItem(LibSimulatorPath.TestsOutFile)
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
                    ReportUnitExeFile = new TaskItem(SystemPath.ReportUnitExe),
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
    }
}