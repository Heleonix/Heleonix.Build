﻿/*
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
    /// Tests the <see cref="DirectoryClean"/>.
    /// </summary>
    public class DirectoryCleanTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="BaseTask.Execute"/>.
        /// </summary>
        [Test]
        public void Execute()
        {
            var directoriesToClean = new[]
            {
                Path.Combine(PathHelper.CurrentDir, Path.GetRandomFileName()),
                Path.Combine(PathHelper.CurrentDir, Path.GetRandomFileName())
            };

            foreach (var directoryToClean in directoriesToClean)
            {
                Directory.CreateDirectory(directoryToClean);
                Directory.CreateDirectory(Path.Combine(directoryToClean, Path.GetRandomFileName()));
                File.Create(Path.Combine(directoryToClean, Path.GetRandomFileName())).Close();
                File.Create(Path.Combine(Directory.CreateDirectory(Path.Combine(directoryToClean,
                    Path.GetRandomFileName())).FullName, Path.GetRandomFileName())).Close();
                File.Create(Path.Combine(Directory.CreateDirectory(Path.Combine(directoryToClean,
                    Path.GetRandomFileName())).FullName, Path.GetRandomFileName())).Close();
            }

            try
            {
                var task = new DirectoryClean
                {
                    BuildEngine = new FakeBuildEngine(),
                    Dirs = directoriesToClean.Select(d => new TaskItem(d) as ITaskItem).ToArray()
                };

                var succeeded = task.Execute();

                Assert.That(succeeded, Is.True);
                Assert.That(task.CleanedDirs, Has.Length.EqualTo(directoriesToClean.Length));
                Assert.That(task.FailedDirs, Has.Length.Zero);

                foreach (var directoryToClean in directoriesToClean)
                {
                    Assert.That(Directory.GetFiles(directoryToClean), Has.Length.Zero);
                    Assert.That(Directory.GetDirectories(directoryToClean), Has.Length.Zero);
                }
            }
            finally
            {
                foreach (var dir in directoriesToClean)
                {
                    Directory.Delete(dir, true);
                }
            }
        }

        #endregion
    }
}