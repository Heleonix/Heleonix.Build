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
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace Heleonix.Build.Tests.Tasks
{
    /// <summary>
    /// Tests the <see cref="NugetPack"/>.
    /// </summary>
    public static class NugetPackTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="NugetPack.Execute"/>.
        /// </summary>
        [TestCase(true, true, false)]
        [TestCase(true, false, true)]
        [TestCase(false, false, false)]
        public static void Execute(bool shouldSucceed, bool isNuspecFileInSeparateDir, bool shouldHavePackageDir)
        {
            MSBuildHelper.ExecuteMSBuild(LibSimulatorPath.SolutionFile, "Build", null, LibSimulatorPath.SolutionDir);

            var nuspecFile = Path.Combine(Path.GetDirectoryName(LibSimulatorPath.ProjectFile),
                Path.ChangeExtension(LibSimulatorPath.ProjectFile, ".nuspec"));

            var packageDir = Path.Combine(SystemPath.CurrentDir, Path.GetRandomFileName());
            var separateNuspecDir = Path.Combine(SystemPath.CurrentDir, Path.GetRandomFileName());
            var separateNuspecFile = Path.Combine(separateNuspecDir, Path.GetFileName(nuspecFile));

            if (isNuspecFileInSeparateDir)
            {
                Directory.CreateDirectory(separateNuspecDir);

                File.Copy(nuspecFile, separateNuspecFile);
            }

            try
            {
                var task = new NugetPack
                {
                    BuildEngine = new FakeBuildEngine(),
                    NugetExeFile = new TaskItem(SystemPath.NugetExe),
                    MSBuildDir = new TaskItem(Path.GetDirectoryName(MSBuildHelper.MSBuildExe)),
                    NuspecFile = new TaskItem(isNuspecFileInSeparateDir ? separateNuspecFile : nuspecFile),
                    ProjectFile = new TaskItem(LibSimulatorPath.ProjectFile),
                    PackageDir = shouldHavePackageDir ? new TaskItem(packageDir) : null,
                    Configuration = MSBuildHelper.CurrentConfiguration,
                    Verbosity = shouldSucceed ? "detailed" : "InvalidVerbocity"
                };

                var succeeded = task.Execute();

                if (shouldSucceed)
                {
                    Assert.That(succeeded, Is.True);

                    var packageExists = File.Exists(task.PackageFile.ItemSpec);

                    File.Delete(task.PackageFile.ItemSpec);

                    Assert.That(packageExists, Is.True);
                }
            }
            finally
            {
                if (shouldHavePackageDir)
                {
                    Directory.Delete(packageDir, true);
                }

                if (isNuspecFileInSeparateDir)
                {
                    // Restore original nuspec file because it was deleted by the task.
                    File.Copy(separateNuspecFile, nuspecFile);

                    Directory.Delete(separateNuspecDir, true);
                }
            }
        }

        #endregion
    }
}