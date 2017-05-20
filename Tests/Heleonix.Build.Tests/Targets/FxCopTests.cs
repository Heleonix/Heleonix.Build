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
using Heleonix.Build.Tests.Common;
using Heleonix.Build.Tests.Targets.Common;
using NUnit.Framework;

namespace Heleonix.Build.Tests.Targets
{
    /// <summary>
    /// Tests the Hxb-FxCop target.
    /// </summary>
    // ReSharper disable once TestFileNameSpaceWarning
    public static class FxCopTests
    {
        #region Tests

        /// <summary>
        /// The test case source.
        /// </summary>
        /// <returns>Test cases.</returns>
        public static IEnumerable<TargetTestCase> HxbFxCopTestCaseValueSource()
        {
            yield return new TargetTestCase(new Dictionary<string, string> { { "Hxb-FxCop-In-FailOn", "Any" } }, null,
                "Hxb-NugetRestore;Hxb-Rebuild", false);
            yield return new TargetTestCase(new Dictionary<string, string> { { "Hxb-FxCop-In-FailOn", "None" } }, null,
                "Hxb-NugetRestore;Hxb-Rebuild", true);
        }

        /// <summary>
        /// Tests the Hxb-FxCop target.
        /// </summary>
        /// <param name="testCase">The test case.</param>
        [Test]
        public static void HxbFxCop([ValueSource(nameof(HxbFxCopTestCaseValueSource))] TargetTestCase testCase)
        {
            var overridesFilePath = TargetSetup.Overrides("Hxb-FxCop", testCase);

            try
            {
                var props = TargetSetup.InputProperties("Hxb-FxCop", CIType.Jenkins,
                    SimulatorType.Library, overridesFilePath, testCase);

                var result = MSBuildHelper.ExecuteMSBuild(SystemPath.MainProjectFile, null, props);

                Assert.That(result == 0, Is.EqualTo(testCase.Success));
            }
            finally
            {
                TargetTeardown.Overrides(overridesFilePath);

                var path = LibSimulatorPath.GetArtifactsDir("Hxb-FxCop");

                Directory.Delete(path, true);
            }
        }

        #endregion
    }
}