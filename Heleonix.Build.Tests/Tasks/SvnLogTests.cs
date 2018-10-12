// <copyright file="SvnLogTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks
{
    using System.IO;
    using Heleonix.Build.Tasks;
    using Heleonix.Build.Tests.Common;
    using Heleonix.Execution;
    using Heleonix.Testing.NUnit.Aaa;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NUnit.Framework;
    using static Heleonix.Testing.NUnit.Aaa.AaaSpec;

    /// <summary>
    /// Tests the <see cref="SvnLog"/>.
    /// </summary>
    [ComponentTest(Type = typeof(SvnLog))]
    public static class SvnLogTests
    {
        /// <summary>
        /// Tests the <see cref="SvnLog.ExecuteInternal"/>.
        /// </summary>
        [MemberTest(Name = nameof(SvnLog.Execute))]
        public static void Execute()
        {
            SvnLog task = null;
            var succeeded = false;
            string repositoryDir = null;
            string workingCopyDir = null;
            ITaskItem repositoryFileOrDir = null;

            Arrange(() =>
            {
                repositoryDir = PathHelper.GetRandomFileInCurrentDir();
                workingCopyDir = PathHelper.GetRandomFileInCurrentDir();

                ExeHelper.Execute(
                    PathHelper.SvnAdminExe,
                    ArgsBuilder.By(string.Empty, " ").AddPath("create", repositoryDir));

                ExeHelper.Execute(
                    PathHelper.SvnExe,
                    ArgsBuilder.By(string.Empty, string.Empty)
                        .AddValue("checkout")
                        .AddPath("file:///" + repositoryDir)
                        .AddPath(workingCopyDir));
            });

            Act(() =>
            {
                task = new SvnLog
                {
                    BuildEngine = new TestBuildEngine(),
                    RepositoryFileOrDir = repositoryFileOrDir,
                    MaxCount = 1,
                    SvnExe = new TaskItem(PathHelper.SvnExe)
                };

                succeeded = task.Execute();
            });

            Teardown(() =>
            {
                ExeHelper.Execute("cmd", $"/C rmdir /s /q {repositoryDir}");
                ExeHelper.Execute("cmd", $"/C rmdir /s /q {workingCopyDir}");
            });

            When("there are commits", () =>
            {
                Arrange(() =>
                {
                    repositoryFileOrDir = new TaskItem(Path.Combine(workingCopyDir, Path.GetRandomFileName()));

                    File.Create(repositoryFileOrDir.ItemSpec).Close();

                    ExeHelper.Execute(
                        PathHelper.SvnExe,
                        ArgsBuilder.By(string.Empty, " ").AddValue("add").AddPath(repositoryFileOrDir.ItemSpec),
                        false,
                        workingCopyDir,
                        int.MaxValue);

                    ExeHelper.Execute(
                        PathHelper.SvnExe,
                        ArgsBuilder.By(string.Empty, " ").AddValue("commit").AddPath("-m", "Commit 1."),
                        false,
                        workingCopyDir,
                        int.MaxValue);
                });

                Should("succeed and provide information about commits", () =>
                {
                    Assert.That(succeeded, Is.True);
                    Assert.That(task.Commits, Has.Length.EqualTo(task.MaxCount));
                    Assert.That(task.Commits[0].ItemSpec, Is.Not.Empty);
                    Assert.That(task.Commits[0].ItemSpec, Is.EqualTo(task.Commits[0].GetMetadata("Revision")));
                    Assert.That(task.Commits[0].GetMetadata("Revision"), Is.Not.Empty);
                    Assert.That(task.Commits[0].GetMetadata("AuthorName"), Is.Not.Empty);
                    Assert.That(task.Commits[0].GetMetadata("AuthorDate"), Is.Not.Empty);
                    Assert.That(task.Commits[0].GetMetadata("Message"), Is.EqualTo("Commit 1."));
                });
            });

            When("there are no commits", () =>
            {
                Arrange(() =>
                {
                    repositoryFileOrDir = new TaskItem(workingCopyDir);
                });

                Should("succeed", () =>
                {
                    Assert.That(succeeded, Is.False);
                    Assert.That(task.Commits, Has.Length.Zero);
                });
            });
        }
    }
}
