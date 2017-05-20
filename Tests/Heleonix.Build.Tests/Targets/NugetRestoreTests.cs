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
using Heleonix.Build.Tests.Common;
using Heleonix.Build.Tests.Targets.Common;
using NUnit.Framework;

namespace Heleonix.Build.Tests.Targets
{
    /// <summary>
    /// Tests the Hxb-NugetRestore target.
    /// </summary>
    // ReSharper disable once TestFileNameSpaceWarning
    public static class NugetRestoreTests
    {
        #region Tests

        /// <summary>
        /// Tests the Hxb-NugetDeploy target.
        /// </summary>
        [Test]
        public static void HxbNugetRestore()
        {
            var packagesDir = Path.Combine(LibSimulatorPath.SolutionDir, "packages");

            if (Directory.Exists(packagesDir))
            {
                Directory.Delete(packagesDir, true);
            }

            var testCase = new TargetTestCase(true);

            var overridesFilePath = TargetSetup.Overrides("Hxb-NugetRestore", testCase);

            try
            {
                var props = TargetSetup.InputProperties("Hxb-NugetRestore", CIType.Jenkins,
                    SimulatorType.Library, overridesFilePath, testCase);

                var result = MSBuildHelper.ExecuteMSBuild(SystemPath.MainProjectFile, null, props);

                Assert.That(result, Is.Zero);
                Assert.That(Directory.Exists(packagesDir), Is.True);
                Assert.That(Directory.GetDirectories(packagesDir), Is.Not.Empty);
            }
            finally
            {
                TargetTeardown.Overrides(overridesFilePath);

                if (Directory.Exists(packagesDir))
                {
                    Directory.Delete(packagesDir, true);
                }
            }
        }

        #endregion
    }
}