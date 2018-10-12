// <copyright file="FileSystemSearchTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks
{
    using System.IO;
    using System.Text.RegularExpressions;
    using Heleonix.Build.Tasks;
    using Heleonix.Build.Tests.Common;
    using Heleonix.Testing.NUnit.Aaa;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NUnit.Framework;
    using static Heleonix.Testing.NUnit.Aaa.AaaSpec;

    /// <summary>
    /// Tests the <see cref="FileSystemSearch"/>.
    /// </summary>
    [ComponentTest(Type = typeof(FileSystemSearch))]
    public static class FileSystemSearchTests
    {
        /// <summary>
        /// Tests the <see cref="FileSystemSearch.ExecuteInternal"/>.
        /// </summary>
        [MemberTest(Name = nameof(FileSystemSearch.Execute))]
        public static void Execute()
        {
            FileSystemSearch task = null;
            var succeeded = false;
            ITaskItem startDir = null;
            string direction = null;
            string types = null;
            string pathRegExp = null;
            string pathRegExpOptions = null;
            string contentRegExp = null;
            string contentRegExpOptions = null;
            string rootDir = null;

            Act(() =>
            {
                task = new FileSystemSearch
                {
                    BuildEngine = new TestBuildEngine(),
                    StartDir = startDir,
                    Direction = direction,
                    Types = types,
                    PathRegExp = pathRegExp,
                    PathRegExpOptions = pathRegExpOptions,
                    ContentRegExp = contentRegExp,
                    ContentRegExpOptions = contentRegExpOptions
                };

                succeeded = task.Execute();
            });

            Teardown(() =>
            {
                Directory.Delete(rootDir, true);
            });

            When("directories exist", () =>
            {
                Arrange(() =>
                {
                    rootDir = PathHelper.GetRandomFileInCurrentDir();

                    Directory.CreateDirectory(Path.Combine(rootDir, "1", "11", "111"));
                    using (var f = File.CreateText(Path.Combine(rootDir, "1", "1a.txt")))
                    {
                        f.Write("1a");
                    }
                    using (var f = File.CreateText(Path.Combine(rootDir, "1", "1b.txt")))
                    {
                        f.Write("1b");
                    }
                    using (var f = File.CreateText(Path.Combine(rootDir, "1", "11", "111", "111aaa.txt")))
                    {
                        f.Write("111aaa");
                    }
                    using (var f = File.CreateText(Path.Combine(rootDir, "1", "11", "111", "111bbb.txt")))
                    {
                        f.Write("111bbb");
                    }

                    Directory.CreateDirectory(Path.Combine(rootDir, "2", "22", "222"));
                    using (var f = File.CreateText(Path.Combine(rootDir, "2", "2a.txt")))
                    {
                        f.Write("2a");
                    }
                    using (var f = File.CreateText(Path.Combine(rootDir, "2", "2b.txt")))
                    {
                        f.Write("2b");
                    }
                    using (var f = File.CreateText(Path.Combine(rootDir, "2", "22", "222", "222aaa.txt")))
                    {
                        f.Write("222aaa");
                    }
                    using (var f = File.CreateText(Path.Combine(rootDir, "2", "22", "222", "222bbb.txt")))
                    {
                        f.Write("222bbb");
                    }
                });

                And("type of items to search is not specified", () =>
                {
                    And("search direction is not specified", () =>
                    {
                        Arrange(() =>
                        {
                            startDir = new TaskItem(rootDir);
                        });

                        And("search options are not specified", () =>
                        {
                            Should("find all directories and files", () =>
                            {
                                Assert.That(succeeded, Is.True);
                                Assert.That(task.FoundFiles, Has.Length.EqualTo(8));
                                Assert.That(task.FoundDirs, Has.Length.EqualTo(7));
                                Assert.That(task.FoundItems, Has.Length.EqualTo(15));
                            });
                        });

                        And("search options are specified", () =>
                        {
                            Arrange(() =>
                            {
                                pathRegExp = @"^.*aaa\.txt$";
                                pathRegExpOptions = RegexOptions.IgnoreCase.ToString();
                                contentRegExp = ".*111aaa.*";
                                contentRegExpOptions = RegexOptions.IgnoreCase.ToString();
                            });

                            Teardown(() =>
                            {
                                pathRegExp = null;
                                pathRegExpOptions = null;
                                contentRegExp = null;
                                contentRegExpOptions = null;
                            });

                            Should("find the only 111aaa.txt file", () =>
                            {
                                Assert.That(succeeded, Is.True);
                                Assert.That(task.FoundFiles, Has.Length.EqualTo(1));
                                Assert.That(task.FoundFiles[0].ItemSpec, Is.EqualTo(Path.Combine(rootDir, "1", "11", "111", "111aaa.txt")));
                                Assert.That(task.FoundDirs, Has.Length.EqualTo(0));
                                Assert.That(task.FoundItems, Has.Length.EqualTo(1));
                            });
                        });
                    });

                    And("search direction is Down", () =>
                    {
                        Arrange(() =>
                        {
                            startDir = new TaskItem(rootDir);
                            direction = "Down";
                        });

                        Teardown(() =>
                        {
                            direction = null;
                        });

                        Should("find all directories and files", () =>
                        {
                            Assert.That(succeeded, Is.True);
                            Assert.That(task.FoundFiles, Has.Length.EqualTo(8));
                            Assert.That(task.FoundDirs, Has.Length.EqualTo(7));
                            Assert.That(task.FoundItems, Has.Length.EqualTo(15));
                        });
                    });

                    And("search direction is Up", () =>
                    {
                        Arrange(() =>
                        {
                            startDir = new TaskItem(Path.Combine(rootDir, "1", "11", "111"));
                            pathRegExp = @"^.*a\.txt$";
                            direction = "Up";
                        });

                        Teardown(() =>
                        {
                            pathRegExp = null;
                            direction = null;
                        });

                        Should("find all directories and files", () =>
                        {
                            Assert.That(succeeded, Is.True);
                            Assert.That(task.FoundFiles, Has.Length.EqualTo(2));
                            Assert.That(task.FoundDirs, Has.Length.EqualTo(0));
                            Assert.That(task.FoundItems, Has.Length.EqualTo(2));
                        });
                    });
                });

                And("type of items to search is Files", () =>
                {
                    Arrange(() =>
                    {
                        startDir = new TaskItem(rootDir);
                        types = "Files";
                    });

                    Should("find all files", () =>
                    {
                        Assert.That(succeeded, Is.True);
                        Assert.That(task.FoundFiles, Has.Length.EqualTo(8));
                        Assert.That(task.FoundDirs, Has.Length.EqualTo(0));
                        Assert.That(task.FoundItems, Has.Length.EqualTo(8));
                    });
                });

                And("type of items to search is Directories", () =>
                {
                    Arrange(() =>
                    {
                        startDir = new TaskItem(rootDir);
                        types = "Directories";
                    });

                    Should("find all directories", () =>
                    {
                        Assert.That(succeeded, Is.True);
                        Assert.That(task.FoundFiles, Has.Length.EqualTo(0));
                        Assert.That(task.FoundDirs, Has.Length.EqualTo(7));
                        Assert.That(task.FoundItems, Has.Length.EqualTo(7));
                    });
                });

                And("type of items to search is All", () =>
                {
                    Arrange(() =>
                    {
                        startDir = new TaskItem(rootDir);
                        types = "All";
                    });

                    Should("find all directories and files", () =>
                    {
                        Assert.That(succeeded, Is.True);
                        Assert.That(task.FoundFiles, Has.Length.EqualTo(8));
                        Assert.That(task.FoundDirs, Has.Length.EqualTo(7));
                        Assert.That(task.FoundItems, Has.Length.EqualTo(15));
                    });
                });
            });
        }
    }
}
