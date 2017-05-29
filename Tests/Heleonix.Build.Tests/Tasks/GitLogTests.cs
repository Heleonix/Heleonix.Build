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

using System;
using System.IO;
using Heleonix.Build.Tasks;
using Heleonix.Build.Tests.Common;
using Microsoft.Build.Utilities;
using NUnit.Framework;
using static System.FormattableString;

namespace Heleonix.Build.Tests.Tasks
{
    /// <summary>
    /// Tests the <see cref="GitLog"/>.
    /// </summary>
    public static class GitLogTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="BaseTask.Execute"/>.
        /// </summary>
        [Test]
        public static void Execute()
        {
            var repositoryDir = Path.Combine(SystemPath.CurrentDir, Path.GetRandomFileName());

            Directory.CreateDirectory(repositoryDir);

            File.Create(Path.Combine(repositoryDir, "File1.txt")).Close();

            var result = ExeHelper.Execute(SystemPath.GitExe, ArgsBuilder.By("-", " ").AddValue("init"), false,
                repositoryDir, int.MaxValue).ExitCode;

            Assert.That(result, Is.Zero);

            result = ExeHelper.Execute(SystemPath.GitExe, ArgsBuilder.By("-", " ").AddValue("add ."), false,
                repositoryDir, int.MaxValue).ExitCode;

            Assert.That(result, Is.Zero);

            result = ExeHelper.Execute(SystemPath.GitExe,
                ArgsBuilder.By("-", " ").AddValue("commit").AddPath("m", "Commit 1."), false, repositoryDir,
                int.MaxValue).ExitCode;

            Assert.That(result, Is.Zero);

            try
            {
                var task = new GitLog
                {
                    BuildEngine = new FakeBuildEngine(),
                    GitExeFile = new TaskItem(SystemPath.GitExe),
                    RepositoryFileDir = new TaskItem(repositoryDir),
                    MaxCount = 1
                };

                var succeeded = task.Execute();

                Assert.That(succeeded, Is.True);
                Assert.That(task.Commits, Has.Length.EqualTo(task.MaxCount));
                Assert.That(task.Commits[0].ItemSpec, Is.Not.Empty);
                Assert.That(task.Commits[0].ItemSpec.StartsWith(task.Commits[0].GetMetadata("Revision"),
                    StringComparison.Ordinal), Is.True);
                Assert.That(task.Commits[0].GetMetadata("Revision"), Is.Not.Empty);
                Assert.That(task.Commits[0].GetMetadata("AuthorName"), Is.Not.Empty);
                Assert.That(task.Commits[0].GetMetadata("AuthorEmail"), Is.Not.Empty);
                Assert.That(task.Commits[0].GetMetadata("AuthorDate"), Is.Not.Empty);
                Assert.That(task.Commits[0].GetMetadata("CommitterName"), Is.Not.Empty);
                Assert.That(task.Commits[0].GetMetadata("CommitterEmail"), Is.Not.Empty);
                Assert.That(task.Commits[0].GetMetadata("CommitterDate"), Is.Not.Empty);
                Assert.That(task.Commits[0].GetMetadata("Message"), Is.EqualTo("Commit 1.\r\n"));
            }
            finally
            {
                ExeHelper.Execute("cmd", Invariant($"/C rmdir /s /q {repositoryDir}"));
            }
        }

        #endregion
    }
}