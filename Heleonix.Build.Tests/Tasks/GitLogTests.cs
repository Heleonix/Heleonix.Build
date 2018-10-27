// <copyright file="GitLogTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks
{
    using System;
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
    /// Tests the <see cref="GitLog"/>.
    /// </summary>
    [ComponentTest(Type = typeof(GitLog))]
    public static class GitLogTests
    {
        /// <summary>
        /// Tests the <see cref="GitLog.ExecuteInternal"/>.
        /// </summary>
        [MemberTest(Name = nameof(GitLog.Execute))]
        public static void Execute()
        {
            GitLog task = null;
            var succeeded = false;
            string repositoryDir = null;
            ITaskItem repositoryFileOrDir = null;

            Arrange(() =>
            {
                repositoryDir = PathHelper.GetRandomFileInCurrentDir();

                Directory.CreateDirectory(repositoryDir);

                repositoryFileOrDir = new TaskItem(Path.Combine(repositoryDir, "File.txt"));

                File.Create(repositoryFileOrDir.ItemSpec).Close();

                ExeHelper.Execute(
                    PathHelper.GitExe,
                    ArgsBuilder.By("-", " ").AddValue("init"),
                    false,
                    repositoryDir);

                task = new GitLog
                {
                    BuildEngine = new TestBuildEngine(),
                    RepositoryFileOrDir = repositoryFileOrDir,
                    MaxCount = 1,
                    GitExe = new TaskItem(PathHelper.GitExe)
                };
            });

            Act(() =>
            {
                succeeded = task.Execute();
            });

            Teardown(() =>
            {
                ExeHelper.Execute("cmd", $"/C rmdir /s /q {repositoryDir}");
            });

            When("there are commits", () =>
            {
                Arrange(() =>
                {
                    ExeHelper.Execute(
                        PathHelper.GitExe,
                        ArgsBuilder.By("-", " ").AddValue("config user.name").AddPath("Heleonix - Hennadii Lutsyshyn"),
                        false,
                        repositoryDir);

                    ExeHelper.Execute(
                        PathHelper.GitExe,
                        ArgsBuilder.By("-", " ").AddValue("config user.email").AddValue("Heleonix.sln@gmail.com"),
                        false,
                        repositoryDir);

                    ExeHelper.Execute(
                        PathHelper.GitExe,
                        ArgsBuilder.By("-", " ").AddValue("add ."),
                        false,
                        repositoryDir);

                    ExeHelper.Execute(
                        PathHelper.GitExe,
                        ArgsBuilder.By("-", " ").AddValue("commit").AddPath("m", "Commit 1."),
                        false,
                        repositoryDir);
                });

                Should("succeed and provide information about commits", () =>
                {
                    Assert.That(succeeded, Is.True);
                    Assert.That(task.Commits, Has.Length.EqualTo(1));
                    Assert.That(task.Commits[0].ItemSpec, Is.Not.Empty);
                    Assert.That(
                        task.Commits[0].ItemSpec.StartsWith(
                            task.Commits[0].GetMetadata("Revision"),
                            StringComparison.Ordinal),
                        Is.True);
                    Assert.That(task.Commits[0].GetMetadata("AuthorName"), Is.EqualTo("Heleonix - Hennadii Lutsyshyn"));
                    Assert.That(task.Commits[0].GetMetadata("AuthorEmail"), Is.EqualTo("Heleonix.sln@gmail.com"));
                    Assert.That(task.Commits[0].GetMetadata("AuthorDate"), Is.Not.Empty);
                    Assert.That(task.Commits[0].GetMetadata("CommitterName"), Is.Not.Empty);
                    Assert.That(task.Commits[0].GetMetadata("CommitterEmail"), Is.Not.Empty);
                    Assert.That(task.Commits[0].GetMetadata("CommitterDate"), Is.Not.Empty);
                    Assert.That(task.Commits[0].GetMetadata("Message"), Is.EqualTo("Commit 1.\r\n"));
                });
            });

            When("there are no commits", () =>
            {
                Should("fail", () =>
                {
                    Assert.That(succeeded, Is.False);
                    Assert.That(task.Commits, Has.Length.Zero);
                });
            });
        }
    }
}
