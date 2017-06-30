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
using static System.FormattableString;

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
        [TestCase(true, "All", "Down", @".*\.md2$|.*dir1.?$", ExpectedResult = "1,3,4")]
        [TestCase(true, "All", "Up", @".*\.md2$|.*dir1.?$", ExpectedResult = "1,3,4")]
        [TestCase(true, "Directories", "Up", @".*\.md2$|.*dir1.?$", ExpectedResult = "0,3,3")]
        [TestCase(true, "Directories", "Down", @".*\.md2$|.*dir1.?$", ExpectedResult = "0,3,3")]
        [TestCase(true, "Directories", "Down", null, ExpectedResult = "0,7,7")]
        [TestCase(true, "Directories", "", @".*\.md2$|.*dir1.?$", ExpectedResult = "0,3,3")]
        [TestCase(true, "", "Up", @".*\.md2$|.*dir1.?$", ExpectedResult = "1,3,4")]
        [TestCase(true, "Files", "", @".*\.md2$|.*dir1.?$", ExpectedResult = "1,0,1")]
        [TestCase(true, "InvalidValue", "Up", @".*\.md2$|.*dir1.?$", ExpectedResult = "0,0,0")]
        [TestCase(false, null, null, null, ExpectedResult = "0,0,0")]
        public static string Execute(bool startDirectoryExists, string types, string direction, string pathRegExp)
        {
            string startDir = null;
            string startUpDir = null;

            if (startDirectoryExists)
            {
                startDir = Directory.CreateDirectory(
                    Path.Combine(SystemPath.CurrentDir, Path.GetRandomFileName())).FullName;
                File.WriteAllText(Path.Combine(startDir, "file1.txt"), @"Some 1 text to search.");
                File.WriteAllText(Path.Combine(startDir, "file2.md2"), @"Some 2 text to search.");
                Directory.CreateDirectory(Path.Combine(startDir, "dir1"));
                Directory.CreateDirectory(Path.Combine(startDir, "dir2"));
                File.WriteAllText(Path.Combine(startDir, "dir1", "file11.txt"), @"Some 11 text to search.");
                File.WriteAllText(Path.Combine(startDir, "dir1", "file12.md2"), @"Some 12 text to search.");
                File.WriteAllText(Path.Combine(startDir, "dir2", "file21.txt"), @"Some 21 text to search.");
                File.WriteAllText(Path.Combine(startDir, "dir2", "file22.md2"), @"Some 22 text to search.");
                Directory.CreateDirectory(Path.Combine(startDir, "dir1", "dir11"));

                startUpDir = Directory.CreateDirectory(Path.Combine(startDir, "dir1", "dir12")).FullName;
                Directory.CreateDirectory(Path.Combine(startDir, "dir2", "dir21"));
                Directory.CreateDirectory(Path.Combine(startDir, "dir2", "dir22"));
            }

            var task = new FileSystemSearch
            {
                BuildEngine = new FakeBuildEngine(),
                StartDir = startDirectoryExists
                    ? new TaskItem(direction == "Up" ? startUpDir : startDir)
                    : new TaskItem("Non;existent;directory"),
                PathRegExp = pathRegExp,
                ContentRegExp = @".*1.*",
                Direction = direction,
                Types = types
            };

            try
            {
                var succeeded = task.Execute();

                Assert.That(succeeded, Is.True);

                return Invariant($"{task.FoundFiles.Length},{task.FoundDirs.Length},{task.FoundItems.Length}");
            }
            finally
            {
                if (startDirectoryExists)
                {
                    Directory.Delete(startDir, true);
                }
            }
        }

        #endregion
    }
}