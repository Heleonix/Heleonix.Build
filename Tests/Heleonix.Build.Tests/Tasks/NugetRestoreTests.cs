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
    /// Tests the <see cref="NugetRestore"/>.
    /// </summary>
    public static class NugetRestoreTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="NugetRestore.Execute"/>.
        /// </summary>
        [TestCase(true, true, true)]
        [TestCase(false, false, false)]
        public static void Execute(bool shouldExeSucceed, bool specifyPackagesDir, bool specifySourcePath)
        {
            var packagesDir = Path.Combine(LibSimulatorPath.SolutionDir, "packages");

            if (Directory.Exists(packagesDir))
            {
                Directory.Delete(packagesDir, true);
            }

            try
            {
                var task = new NugetRestore
                {
                    BuildEngine = new FakeBuildEngine(),
                    NugetExeFile = new TaskItem(SystemPath.NugetExe),
                    MSBuildDir = new TaskItem(Path.GetDirectoryName(MSBuildHelper.MSBuildExe)),
                    SolutionFile = new TaskItem(LibSimulatorPath.SolutionFile),
                    Verbosity = shouldExeSucceed ? "detailed" : "InvalidVerbosity",
                    PackagesDir = specifyPackagesDir ? new TaskItem(packagesDir) : null,
                    SourcesPaths = specifySourcePath
                        ? new ITaskItem[] { new TaskItem("https://nuget.org/api/v2/") }
                        : null
                };

                var succeeded = task.Execute();

                if (shouldExeSucceed)
                {
                    Assert.That(succeeded, Is.True);

                    Assert.That(Directory.Exists(packagesDir), Is.True);
                    Assert.That(Directory.GetDirectories(packagesDir), Is.Not.Empty);
                }
            }
            finally
            {
                if (Directory.Exists(packagesDir))
                {
                    Directory.Delete(packagesDir, true);
                }
            }
        }

        #endregion
    }
}