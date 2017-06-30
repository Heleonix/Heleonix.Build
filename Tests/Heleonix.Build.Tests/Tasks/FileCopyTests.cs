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
using System.Linq;
using Heleonix.Build.Tasks;
using Heleonix.Build.Tests.Common;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace Heleonix.Build.Tests.Tasks
{
    /// <summary>
    /// Tests the <see cref="FileCopy"/>.
    /// </summary>
    public static class FileCopyTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="BaseTask.Execute"/>.
        /// </summary>
        [TestCase(null, false, null, false, null, false, ExpectedResult = true)]
        [TestCase("", false, null, false, null, false, ExpectedResult = true)]
        [TestCase("src", true, null, false, null, false, ExpectedResult = true)]
        [TestCase("src", true, new string[0], false, null, false, ExpectedResult = true)]
        [TestCase("src", true, new[] { "" }, false, null, false, ExpectedResult = true)]
        [TestCase("src", true, new[] { "dest" }, false, null, true, ExpectedResult = false)]
        [TestCase("src", true, new[] { "dest1", "dest2" }, false, null, false, ExpectedResult = true)]
        [TestCase("src", true, new[] { "dest" }, true, null, false, ExpectedResult = true)]
        [TestCase("dir\\subdir\\src", true, new[] { "dest" }, false, "dir\\subdir", false, ExpectedResult = true)]
        [TestCase("dir\\subdir\\src.ext", true, new[] { "dest" }, false, "dir\\subdir", false, ExpectedResult = true)]
        [TestCase("src", false, new[] { "dest" }, false, "dir\\subdir", false, ExpectedResult = true)]
        [TestCase("dir\\subdir\\src", true, new[] { "dest" }, false, "wrong_dir\\wrong_subdir", false, ExpectedResult =
            false)]
        public static bool Execute(string source, bool shouldSourceExist, string[] destination,
            bool shouldDestinationExist, string subDirFrom, bool isDestinationAsFile)
        {
            var workingDir = Path.Combine(SystemPath.CurrentDir, Path.GetRandomFileName());

            Directory.CreateDirectory(workingDir);

            var srcFile = !string.IsNullOrEmpty(source) ? Path.Combine(workingDir, source) : source;
            var destDirs = destination?.Select(d => !string.IsNullOrEmpty(d) ? Path.Combine(workingDir, d) : d);
            var metadata = new Dictionary<string, string>();

            if (shouldSourceExist)
            {
                if (!Directory.Exists(Path.GetDirectoryName(srcFile)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(srcFile));
                }

                File.WriteAllText(srcFile, @"text");
            }

            if (destDirs != null)
            {
                foreach (var destDir in destDirs)
                {
                    if (!string.IsNullOrEmpty(destDir))
                    {
                        if (isDestinationAsFile)
                        {
                            File.WriteAllText(destDir, @"text");
                        }
                        else if (shouldDestinationExist)
                        {
                            Directory.CreateDirectory(destDir);
                        }
                    }
                }
            }

            if (subDirFrom != null)
            {
                metadata.Add("WithSubDirsFrom", Path.Combine(workingDir, subDirFrom));
            }

            try
            {
                var task = new FileCopy
                {
                    BuildEngine = new FakeBuildEngine(),
                    Files = !string.IsNullOrEmpty(srcFile)
                        ? new ITaskItem[] { new TaskItem(srcFile, metadata) }
                        : srcFile != null
                            ? new ITaskItem[0]
                            : null,

                    Destinations = destDirs?.Select(d => d == null
                        ? null
                        : new TaskItem(d)).ToArray()
                };

                var succeeded = task.Execute();

                if (isDestinationAsFile)
                {
                    Assert.That(task.FailedFiles, Has.Length.EqualTo(1));
                }

                if (succeeded && !string.IsNullOrEmpty(source) && shouldSourceExist
                    && destination != null && destination.Length > 0 && !destination.Any(string.IsNullOrEmpty))
                {
                    Assert.That(task.CopiedFiles, Has.Length.EqualTo(1));
                    //Assert.That(task.CopiedFiles[0].ItemSpec, Is.EqualTo());
                }

                return succeeded;
            }
            finally
            {
                Directory.Delete(workingDir, true);
            }
        }

        #endregion
    }
}