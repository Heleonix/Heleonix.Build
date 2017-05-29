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
    /// Tests the <see cref="SvnLog"/>.
    /// </summary>
    public static class SvnLogTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="BaseTask.Execute"/>.
        /// </summary>
        [Test]
        public static void Execute()
        {
            var repositoryDir = Path.Combine(SystemPath.CurrentDir, Path.GetRandomFileName());
            var workingCopyDir = Path.Combine(SystemPath.CurrentDir, Path.GetRandomFileName());

            try
            {
                ExeHelper.Execute(SystemPath.SvnAdminExe, ArgsBuilder.By(string.Empty, " ")
                    .AddPath("create", repositoryDir));

                ExeHelper.Execute(SystemPath.SvnExe, ArgsBuilder.By(string.Empty, "")
                    .AddValue("checkout")
                    .AddPath("file:///" + repositoryDir)
                    .AddPath(workingCopyDir));

                var workingCopyFilePath = Path.Combine(workingCopyDir, Path.GetRandomFileName());

                File.Create(workingCopyFilePath).Close();

                ExeHelper.Execute(SystemPath.SvnExe, ArgsBuilder.By(string.Empty, " ")
                    .AddValue("add")
                    .AddPath(workingCopyFilePath), false, workingCopyDir, int.MaxValue);

                ExeHelper.Execute(SystemPath.SvnExe, ArgsBuilder.By(string.Empty, " ")
                    .AddValue("commit")
                    .AddPath("-m", "Commit 1."), false, workingCopyDir, int.MaxValue);

                var task = new SvnLog
                {
                    BuildEngine = new FakeBuildEngine(),
                    SvnExeFile = new TaskItem(SystemPath.SvnExe),
                    RepositoryFileDir = new TaskItem(workingCopyFilePath),
                    MaxCount = 1
                };

                var succeeded = task.Execute();

                Assert.That(succeeded, Is.True);
                Assert.That(task.Commits, Has.Length.EqualTo(task.MaxCount));
                Assert.That(task.Commits[0].ItemSpec, Is.Not.Empty);
                Assert.That(task.Commits[0].ItemSpec, Is.EqualTo(task.Commits[0].GetMetadata("Revision")));
                Assert.That(task.Commits[0].GetMetadata("Revision"), Is.Not.Empty);
                Assert.That(task.Commits[0].GetMetadata("AuthorName"), Is.Not.Empty);
                Assert.That(task.Commits[0].GetMetadata("AuthorDate"), Is.Not.Empty);
                Assert.That(task.Commits[0].GetMetadata("Message"), Is.EqualTo("Commit 1."));
            }
            finally
            {
                ExeHelper.Execute("cmd", Invariant($"/C rmdir /s /q {repositoryDir}"));

                ExeHelper.Execute("cmd", Invariant($"/C rmdir /s /q {workingCopyDir}"));
            }
        }

        #endregion
    }
}