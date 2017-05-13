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
    /// Tests the <see cref="FileSystemSearch"/>.
    /// </summary>
    public static class FileSystemSearchTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="BaseTask.Execute"/>.
        /// </summary>
        [TestCase("Down")]
        [TestCase("Up")]
        public static void Execute(string direction)
        {
            var startDir = Directory.CreateDirectory(Path.Combine(
                    SystemPath.CurrentDir, Path.GetRandomFileName()))
                .FullName;
            File.WriteAllText(Path.Combine(startDir, "file1.txt"), @"Some 1 text to search.");
            File.WriteAllText(Path.Combine(startDir, "file2.md2"), @"Some 2 text to search.");
            Directory.CreateDirectory(Path.Combine(startDir, "dir1"));
            Directory.CreateDirectory(Path.Combine(startDir, "dir2"));
            File.WriteAllText(Path.Combine(startDir, "dir1", "file11.txt"), @"Some 11 text to search.");
            File.WriteAllText(Path.Combine(startDir, "dir1", "file12.md2"), @"Some 12 text to search.");
            File.WriteAllText(Path.Combine(startDir, "dir2", "file21.txt"), @"Some 21 text to search.");
            File.WriteAllText(Path.Combine(startDir, "dir2", "file22.md2"), @"Some 22 text to search.");
            Directory.CreateDirectory(Path.Combine(startDir, "dir1", "dir11"));
            var startUpDir = Directory.CreateDirectory(Path.Combine(startDir, "dir1", "dir12")).FullName;
            Directory.CreateDirectory(Path.Combine(startDir, "dir2", "dir21"));
            Directory.CreateDirectory(Path.Combine(startDir, "dir2", "dir22"));

            var task = new FileSystemSearch
            {
                BuildEngine = new FakeBuildEngine(),
                StartDir = new TaskItem(direction == "Up" ? startUpDir : startDir),
                PathRegExp = @".*\.md2$|.*dir1.?$",
                ContentRegExp = @".*1.*",
                Direction = direction,
                Types = "All"
            };

            try
            {
                var succeeded = task.Execute();

                Assert.That(succeeded, Is.True);
                Assert.That(task.FoundFiles, Has.Length.EqualTo(1));
                Assert.That(task.FoundDirs, Has.Length.EqualTo(3));
                Assert.That(task.FoundItems, Has.Length.EqualTo(4));
            }
            finally
            {
                Directory.Delete(startDir, true);
            }
        }

        #endregion
    }
}