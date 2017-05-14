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

using System.Collections.Generic;
using System.IO;
using Heleonix.Build.Tasks;
using Heleonix.Build.Tests.Common;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace Heleonix.Build.Tests.Tasks
{
    /// <summary>
    /// Tests the <see cref="ReportGenerator"/>.
    /// </summary>
    public static class ReportGeneratorTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="BaseTask.Execute"/>.
        /// </summary>
        [Test]
        public static void Execute()
        {
            MSBuildHelper.ExecuteMSBuild(LibSimulatorPath.SolutionFile, "Build", null);

            var coverageResults = Path.Combine(LibSimulatorPath.ReportsDir, Path.GetRandomFileName());
            var errorsOutput = Path.Combine(LibSimulatorPath.ReportsDir, "Errors.txt");
            var testsOutput = Path.Combine(LibSimulatorPath.ReportsDir, "Output.txt");
            var testsResult = Path.Combine(LibSimulatorPath.ReportsDir, "NUnit.xml");

            var openCoverTask = new OpenCover
            {
                BuildEngine = new FakeBuildEngine(),
                OpenCoverExeFile = new TaskItem(SystemPath.OpenCoverExe),
                Target = new TaskItem(SystemPath.NUnitConsoleExe, new Dictionary<string, string>
                {
                    { nameof(Build.Tasks.NUnit.NUnitProjectFileOrTestsFiles), LibSimulatorPath.TestsOutFile },
                    { "Type", nameof(Build.Tasks.NUnit) },
                    { nameof(Build.Tasks.NUnit.ErrorsOutputFile), errorsOutput },
                    { nameof(Build.Tasks.NUnit.TestsOutputFile), testsOutput },
                    { nameof(Build.Tasks.NUnit.TestsResultFile), testsResult }
                }),
                CoverageResultFile = new TaskItem(coverageResults),
                MinClassCoverage = 0,
                Register = "path64"
            };

            var succeeded = openCoverTask.Execute();

            var reportDir = Path.Combine(LibSimulatorPath.ReportsDir, Path.GetRandomFileName());

            try
            {
                Assert.That(succeeded, Is.True);

                var task = new ReportGenerator
                {
                    BuildEngine = new FakeBuildEngine(),
                    ReportGeneratorExeFile = new TaskItem(SystemPath.ReportGeneratorExe),
                    Verbosity = "Error",
                    ResultsFiles = new ITaskItem[] { new TaskItem(coverageResults) },
                    ReportDir = new TaskItem(reportDir),
                    ReportTypes = "Badges;Html;HtmlSummary"
                };

                succeeded = task.Execute();

                Assert.That(succeeded, Is.True);
                Assert.That(File.Exists(Path.Combine(reportDir, "index.htm")), Is.True);
                Assert.That(File.Exists(Path.Combine(reportDir, "summary.htm")), Is.True);
                Assert.That(Directory.GetFiles(reportDir, "*.png"), Has.Length.GreaterThan(0));
            }
            finally
            {
                if (File.Exists(coverageResults))
                {
                    File.Delete(coverageResults);
                }

                if (File.Exists(errorsOutput))
                {
                    File.Delete(errorsOutput);
                }

                if (File.Exists(testsOutput))
                {
                    File.Delete(testsOutput);
                }

                if (File.Exists(testsResult))
                {
                    File.Delete(testsResult);
                }

                if (Directory.Exists(reportDir))
                {
                    Directory.Delete(reportDir, true);
                }
            }
        }

        #endregion
    }
}