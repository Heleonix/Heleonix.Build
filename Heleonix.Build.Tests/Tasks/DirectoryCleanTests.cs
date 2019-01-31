// <copyright file="DirectoryCleanTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks
{
    using System.IO;
    using System.Linq;
    using Heleonix.Build.Tasks;
    using Heleonix.Build.Tests.Common;
    using Heleonix.Testing.NUnit.Aaa;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NUnit.Framework;
    using static Heleonix.Testing.NUnit.Aaa.AaaSpec;

    /// <summary>
    /// Tests the <see cref="DirectoryClean"/>.
    /// </summary>
    [ComponentTest(Type = typeof(DirectoryClean))]
    public static class DirectoryCleanTests
    {
        /// <summary>
        /// Tests the <see cref="DirectoryClean.ExecuteInternal"/>.
        /// </summary>
        [MemberTest(Name = nameof(DirectoryClean.Execute))]
        public static void Execute()
        {
            ITaskItem[] directories = null;
            DirectoryClean task = null;
            var succeeded = false;

            Arrange(() =>
            {
                task = new DirectoryClean
                {
                    BuildEngine = new TestBuildEngine(),
                    Dirs = directories
                };
            });

            Act(() =>
            {
                succeeded = task.Execute();
            });

            When("directories are specified", () =>
            {
                directories = new[]
                {
                    new TaskItem(PathHelper.GetRandomFileInCurrentDir()),
                    new TaskItem(PathHelper.GetRandomFileInCurrentDir()),
                    new TaskItem(PathHelper.GetRandomFileInCurrentDir())
                };

                And("directories to clean exist", () =>
                {
                    Teardown(() =>
                    {
                        foreach (var dir in directories)
                        {
                            Directory.Delete(dir.ItemSpec, true);
                        }
                    });

                    foreach (var dir in directories)
                    {
                        Directory.CreateDirectory(dir.ItemSpec);
                        File.Create(Path.Combine(dir.ItemSpec, "1.txt")).Close();
                        File.Create(Path.Combine(dir.ItemSpec, "2.txt")).Close();

                        Directory.CreateDirectory(Path.Combine(dir.ItemSpec, "dir1"));
                        File.Create(Path.Combine(dir.ItemSpec, "dir1", "1.txt")).Close();
                        File.Create(Path.Combine(dir.ItemSpec, "dir1", "2.txt")).Close();
                    }

                    Should("successfully clean all the specified directories", () =>
                    {
                        Assert.That(succeeded, Is.True);

                        foreach (var dir in directories)
                        {
                            Assert.That(task.CleanedDirs.Any(item => item.ItemSpec == dir.ItemSpec));
                            Assert.That(Directory.GetFiles(dir.ItemSpec), Has.Length.Zero);
                            Assert.That(Directory.GetDirectories(dir.ItemSpec), Has.Length.Zero);
                        }
                    });
                });

                And("directories to clean do not exist", () =>
                {
                    Should("succeed", () =>
                    {
                        Assert.That(succeeded, Is.True);

                        Assert.That(task.CleanedDirs, Has.Length.Zero);
                    });
                });

                And("an error occurs while cleaning a directory", () =>
                {
                    Directory.CreateDirectory(directories[0].ItemSpec);
                    var stream = File.Create(Path.Combine(directories[0].ItemSpec, "1.txt"));

                    Teardown(() =>
                    {
                        stream.Close();

                        Directory.Delete(directories[0].ItemSpec, true);
                    });

                    Should("succeed and not invalid clean directories", () =>
                    {
                        Assert.That(succeeded, Is.True);

                        Assert.That(task.CleanedDirs, Has.Length.Zero);
                    });
                });
            });
        }
    }
}
