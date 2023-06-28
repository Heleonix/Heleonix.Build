// <copyright file="FileSystemSearchTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

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
        string startDir = null;
        string direction = null;
        string types = null;
        string pathRegExp = null;
        string contentRegExp = null;
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
                ContentRegExp = contentRegExp,
            };

            succeeded = task.Execute();
        });

        When("directories exist", () =>
        {
            Arrange(() =>
            {
                rootDir = PathHelper.GetRandomFileNameInCurrentDir();

                Directory.CreateDirectory(Path.Combine(rootDir, "1", "11", "111"));

                using (var f = File.CreateText(Path.Combine(rootDir, "0.txt")))
                {
                    f.Write("0a");
                }

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

            Teardown(() =>
            {
                Directory.Delete(rootDir, true);
            });

            And("start directory exist", () =>
            {
                Arrange(() =>
                {
                    startDir = rootDir;
                });

                And("type of items to search is not specified", () =>
                {
                    And("search direction is not specified", () =>
                    {
                        Should("find all directories and files", () =>
                        {
                            Assert.That(succeeded, Is.True);
                            Assert.That(task.FoundFiles, Has.Length.EqualTo(9));
                            Assert.That(task.FoundDirs, Has.Length.EqualTo(7));
                            Assert.That(task.FoundItems, Has.Length.EqualTo(16));
                        });

                        And("search options are specified", () =>
                        {
                            Arrange(() =>
                            {
                                pathRegExp = @".*aaa\.txt$";
                                contentRegExp = ".*111aaa.*";
                            });

                            Teardown(() =>
                            {
                                pathRegExp = null;
                                contentRegExp = null;
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
                            direction = "Down";
                        });

                        Teardown(() =>
                        {
                            direction = null;
                        });

                        Should("find all directories and files", () =>
                        {
                            Assert.That(succeeded, Is.True);
                            Assert.That(task.FoundFiles, Has.Length.EqualTo(9));
                            Assert.That(task.FoundDirs, Has.Length.EqualTo(7));
                            Assert.That(task.FoundItems, Has.Length.EqualTo(16));
                        });
                    });

                    And("search direction is Up", () =>
                    {
                        Arrange(() =>
                        {
                            direction = "Up";
                            startDir = Path.Combine(rootDir, "1", "11", "111");
                        });

                        Teardown(() =>
                        {
                            direction = null;
                            startDir = null;
                        });

                        And("search options are specified", () =>
                        {
                            Arrange(() =>
                            {
                                pathRegExp = @"(.*/111$)|(.*/111[a-z]+\.txt$)";
                            });

                            Teardown(() =>
                            {
                                pathRegExp = null;
                            });

                            Should("find all directories and files", () =>
                            {
                                Assert.That(succeeded, Is.True);
                                Assert.That(task.FoundFiles, Has.Length.EqualTo(2));
                                Assert.That(task.FoundDirs, Has.Length.EqualTo(1));
                                Assert.That(task.FoundItems, Has.Length.EqualTo(3));
                            });
                        });

                        And("search options are not specified", () =>
                        {
                            Arrange(() =>
                            {
                                pathRegExp = null;
                            });

                            Should("find all directories and files", () =>
                            {
                                Assert.That(succeeded, Is.True);
                                Assert.That(
                                    task.FoundFiles[0].ItemSpec,
                                    Is.EqualTo(Path.Combine(rootDir, "1", "11", "111", "111aaa.txt")));
                                Assert.That(
                                    task.FoundFiles[1].ItemSpec,
                                    Is.EqualTo(Path.Combine(rootDir, "1", "11", "111", "111bbb.txt")));
                                Assert.That(
                                    task.FoundFiles[2].ItemSpec,
                                    Is.EqualTo(Path.Combine(rootDir, "1", "1a.txt")));
                                Assert.That(
                                    task.FoundFiles[3].ItemSpec,
                                    Is.EqualTo(Path.Combine(rootDir, "1", "1b.txt")));
                                Assert.That(
                                    task.FoundDirs[0].ItemSpec,
                                    Is.EqualTo(Path.Combine(rootDir, "1", "11", "111")));
                                Assert.That(
                                    task.FoundDirs[1].ItemSpec,
                                    Is.EqualTo(Path.Combine(rootDir, "1", "11")));
                                Assert.That(
                                    task.FoundDirs[2].ItemSpec,
                                    Is.EqualTo(Path.Combine(rootDir, "1")));
                                Assert.That(
                                    task.FoundDirs[3].ItemSpec,
                                    Is.EqualTo(Path.Combine(rootDir, "2")));
                                Assert.That(task.FoundItems, Has.Length.GreaterThan(8));
                            });
                        });
                    });
                });

                And("type of items to search is Files", () =>
                {
                    Arrange(() =>
                    {
                        types = "Files";
                    });

                    Should("find all files", () =>
                    {
                        Assert.That(succeeded, Is.True);
                        Assert.That(task.FoundFiles, Has.Length.EqualTo(9));
                        Assert.That(task.FoundDirs, Has.Length.EqualTo(0));
                        Assert.That(task.FoundItems, Has.Length.EqualTo(9));
                    });
                });

                And("type of items to search is Directories", () =>
                {
                    Arrange(() =>
                    {
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
                        types = "All";
                    });

                    Should("find all directories and files", () =>
                    {
                        Assert.That(succeeded, Is.True);
                        Assert.That(task.FoundFiles, Has.Length.EqualTo(9));
                        Assert.That(task.FoundDirs, Has.Length.EqualTo(7));
                        Assert.That(task.FoundItems, Has.Length.EqualTo(16));
                    });
                });
            });

            And("start directory does not exist", () =>
            {
                Arrange(() =>
                {
                    startDir = Path.Combine(rootDir, Path.GetRandomFileName());
                });

                Should("succeed fithout any found items", () =>
                {
                    Assert.That(succeeded, Is.True);
                    Assert.That(task.FoundFiles, Has.Length.Zero);
                    Assert.That(task.FoundDirs, Has.Length.Zero);
                    Assert.That(task.FoundItems, Has.Length.Zero);
                });
            });
        });
    }
}
