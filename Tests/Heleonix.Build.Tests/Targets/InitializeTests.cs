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

using Heleonix.Build.Tests.Common;
using Heleonix.Build.Tests.Targets.Common;
using NUnit.Framework;

namespace Heleonix.Build.Tests.Targets
{
    /// <summary>
    /// Tests the Hxb-Initialize target.
    /// </summary>
    // ReSharper disable once TestFileNameWarning
    public static class InitializeTests
    {
        #region Tests

        /// <summary>
        /// Tests the Hxb-Initialize target.
        /// </summary>
        /// <param name="ciType">The continuous integration system type.</param>
        [Test]
        public static void HxbInitialize([Values(CIType.Jenkins, CIType.TeamCity)] CIType ciType)
        {
            var testCase = new TargetTestCase(true);

            var overridesFilePath = TargetSetup.Overrides("Hxb-Initialize", testCase);

            try
            {
                var props = TargetSetup.InputProperties("Hxb-Initialize", ciType,
                    SimulatorType.Library, overridesFilePath, testCase);

                var result = MSBuildHelper.ExecuteMSBuild(SystemPath.MainProjectFile, null, props,
                    LibSimulatorPath.SolutionDir);

                Assert.That(result, Is.Zero);
            }
            finally
            {
                TargetTeardown.Overrides(overridesFilePath);
            }
        }

        #endregion
    }
}