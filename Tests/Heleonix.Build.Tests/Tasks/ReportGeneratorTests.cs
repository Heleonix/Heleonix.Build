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
    public class ReportGeneratorTests : TaskTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="BaseTask.Execute"/>.
        /// </summary>
        [Test]
        public void Execute()
        {
            var coverageResults = Path.Combine(LibSimulatorHelper.ReportsDir, Path.GetRandomFileName());
            var errorsOutput = Path.Combine(LibSimulatorHelper.ReportsDir, "Errors.txt");
            var testsOutput = Path.Combine(LibSimulatorHelper.ReportsDir, "Output.txt");
            var testsResult = Path.Combine(LibSimulatorHelper.ReportsDir, "NUnit.xml");

            var openCoverTask = new OpenCover
            {
                BuildEngine = new FakeBuildEngine(),
                OpenCoverExeFile = new TaskItem(PathHelper.OpenCoverExe),
                Target = new TaskItem(PathHelper.NUnitConsoleExe, new Dictionary<string, string>
                {
                    { nameof(Build.Tasks.NUnit.NUnitProjectOrTestsFiles), LibSimulatorHelper.TestsOut },
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

            var reportDir = Path.Combine(LibSimulatorHelper.ReportsDir, Path.GetRandomFileName());

            try
            {
                Assert.That(succeeded, Is.True);

                var task = new ReportGenerator
                {
                    BuildEngine = new FakeBuildEngine(),
                    ReportGeneratorExeFile = new TaskItem(PathHelper.ReportGeneratorExe),
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

        #region TaskTests Members

        /// <summary>
        /// Gets the type of the simulator.
        /// </summary>
        protected override SimulatorType SimulatorType => SimulatorType.Library;

        #endregion
    }
}