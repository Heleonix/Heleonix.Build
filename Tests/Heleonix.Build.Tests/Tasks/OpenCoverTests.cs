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
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace Heleonix.Build.Tests.Tasks
{
    /// <summary>
    /// Tests the <see cref="OpenCover"/>.
    /// </summary>
    public class OpenCoverTests : TaskTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="OpenCover.Execute"/>.
        /// </summary>
        [TestCase(40, ExpectedResult = true)]
        [TestCase(90, ExpectedResult = false)]
        public bool Execute(int minClassCoverage)
        {
            var coverageResults = Path.Combine(LibSimulatorHelper.ReportsDir, Path.GetRandomFileName());

            var task = new OpenCover
            {
                BuildEngine = new FakeBuildEngine(),
                OpenCoverExeFile = new TaskItem(PathHelper.OpenCoverExe),
                Target = new TaskItem(PathHelper.NUnitConsoleExe, new Dictionary<string, string>
                {
                    { nameof(Build.Tasks.NUnit.NUnitProjectOrTestsFiles), LibSimulatorHelper.TestsOut },
                    { "Type", nameof(Build.Tasks.NUnit) }
                }),
                CoverageResultFile = new TaskItem(coverageResults),
                MinClassCoverage = minClassCoverage
            };

            task.Execute();

            var coverageResultsExists = File.Exists(coverageResults);

            try
            {
                Assert.That(coverageResultsExists, Is.True);
            }
            finally
            {
                if (coverageResultsExists)
                {
                    File.Delete(coverageResults);
                }
            }

            return task.ClassCoverage >= minClassCoverage;
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