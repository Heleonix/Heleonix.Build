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
    /// Tests the <see cref="OpenCover"/>.
    /// </summary>
    public static class OpenCoverTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="OpenCover.Execute"/>.
        /// </summary>
        [TestCase(nameof(Build.Tasks.NUnit), null, 28, true, false, false, "Coverage.xml")]
        [TestCase(nameof(Build.Tasks.NUnit), null, 0, true, true, false, "Coverage.xml")]
        [TestCase(nameof(Build.Tasks.NUnit), null, 90, false, false, false, "Coverage.xml")]
        [TestCase("InvalidType", null, 0, true, false, null, "Coverage.xml")]
        [TestCase(nameof(Build.Tasks.NUnit), null, 28, true, false, true, "Coverage.xml")]
        [TestCase(nameof(Build.Tasks.NUnit), "name != 'Add(3,2)'", 0, true, false, false, "Coverage.xml")]
        [TestCase(nameof(Build.Tasks.NUnit), null, 0, true, false, false, null)]
        public static void Execute(string type, string testsFilter, int minCoverage, bool shouldPassCoverage,
            bool shouldExeFail, bool usePdbSearchDirs, string coverageResultsFile)
        {
            MSBuildHelper.ExecuteMSBuild(LibSimulatorPath.SolutionFile, "Build", null, LibSimulatorPath.SolutionDir);

            var artifactsDir = LibSimulatorPath.GetArtifactsDir("Hxb-OpenCover");
            var targetArtifactsDir = LibSimulatorPath.GetArtifactsDir("Hxb-NUnit");
            var errorsOutput = Path.Combine(targetArtifactsDir, "Errors.txt");
            var testsOutput = Path.Combine(targetArtifactsDir, "Output.txt");
            var testsResult = Path.Combine(targetArtifactsDir, "NUnit.xml");

            var task = new OpenCover
            {
                BuildEngine = new FakeBuildEngine(),
                OpenCoverExeFile = new TaskItem(SystemPath.OpenCoverExe),
                Target = new TaskItem(shouldExeFail ? "``" : SystemPath.NUnitConsoleExe, new Dictionary<string, string>
                {
                    { nameof(Build.Tasks.NUnit.NUnitProjectFileOrTestsFiles), LibSimulatorPath.TestsOutFile },
                    { "Type", type },
                    { nameof(Build.Tasks.NUnit.ErrorsOutputFile), errorsOutput },
                    { nameof(Build.Tasks.NUnit.TestsOutputFile), testsOutput },
                    { nameof(Build.Tasks.NUnit.TestsResultFile), testsResult },
                    { nameof(Build.Tasks.NUnit.TestsFilter), testsFilter }
                }),
                CoverageResultFile =
                    coverageResultsFile != null ? new TaskItem(Path.Combine(artifactsDir, coverageResultsFile)) : null,
                MinClassCoverage = minCoverage,
                MinBranchCoverage = minCoverage,
                MinLineCoverage = minCoverage,
                MinMethodCoverage = minCoverage,
                Register = "path64",
                PdbSearchDirs = usePdbSearchDirs ? new ITaskItem[] { new TaskItem(LibSimulatorPath.OutDir) } : null
            };

            task.Execute();

            try
            {
                if (!shouldExeFail && type != "InvalidType" && coverageResultsFile != null)
                {
                    Assert.That(File.Exists(task.CoverageResultFile.ItemSpec), Is.True);
                }

                if (shouldPassCoverage)
                {
                    Assert.That(task.ClassCoverage, Is.GreaterThanOrEqualTo(minCoverage));
                    Assert.That(task.MethodCoverage, Is.GreaterThanOrEqualTo(minCoverage));
                    Assert.That(task.BranchCoverage, Is.GreaterThanOrEqualTo(minCoverage));
                    Assert.That(task.LineCoverage, Is.GreaterThanOrEqualTo(minCoverage));
                }
            }
            finally
            {
                if (type != "InvalidType" && coverageResultsFile != null)
                {
                    Directory.Delete(artifactsDir, true);
                    Directory.Delete(targetArtifactsDir, true);
                }
            }
        }

        #endregion
    }
}