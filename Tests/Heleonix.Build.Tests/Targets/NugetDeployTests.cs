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
using Heleonix.Utilities.Execution;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;
using static System.FormattableString;

namespace Heleonix.Build.Tests.Targets
{
    /// <summary>
    /// Tests the Hxb-NugetDeploy target.
    /// </summary>
    // ReSharper disable once TestFileNameWarning
    public static class NugetDeployTests
    {
        #region Tests

        /// <summary>
        /// Tests the Hxb-NugetDeploy target.
        /// </summary>
        [Test]
        public static void HxbNugetDeploy()
        {
            var tempSource = Path.Combine(SystemPath.CurrentDir, Path.GetRandomFileName());

            try
            {
                Directory.CreateDirectory(tempSource);

                var result = ExeHelper.Execute(SystemPath.NugetExe, Invariant($"init \"{tempSource}\""));

                Assert.That(result, Is.Zero);

                var testCase = new TargetTestCase(
                    null,
                    new Dictionary<string, ITaskItem[]>
                    {
                        { "Hxb-NugetDeploy-In-Source", new ITaskItem[] { new TaskItem(tempSource) } }
                    },
                    "Hxb-NugetRestore;Hxb-Rebuild",
                    true);

                var overridesFilePath = TargetSetup.Overrides("Hxb-NugetDeploy", testCase);

                try
                {
                    var props = TargetSetup.InputProperties("Hxb-NugetDeploy", CIType.Jenkins, SimulatorType.Library,
                        overridesFilePath, testCase);

                    result = MSBuildHelper.ExecuteMSBuild(SystemPath.MainProjectFile, null, props,
                        LibSimulatorPath.SolutionDir);

                    Assert.That(result, Is.Zero);

                    Assert.That(Directory.Exists(LibSimulatorPath.GetArtifactsDir("Hxb-NugetDeploy")), Is.True);
                    Assert.That(Directory.GetFiles(LibSimulatorPath.GetArtifactsDir("Hxb-NugetDeploy")),
                        Has.Length.EqualTo(1));
                    Assert.That(Directory.Exists(tempSource));
                    Assert.That(Directory.GetFiles(tempSource), Has.Length.EqualTo(1));
                }
                finally
                {
                    TargetTeardown.Overrides(overridesFilePath);

                    Directory.Delete(LibSimulatorPath.GetArtifactsDir("Hxb-NugetDeploy"), true);
                }
            }
            finally
            {
                Directory.Delete(tempSource, true);
            }
        }

        #endregion
    }
}