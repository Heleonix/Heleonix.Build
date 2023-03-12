// <copyright file="FileCopyTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

/// <summary>
/// Tests the <see cref="FileCopy"/>.
/// </summary>
[ComponentTest(Type = typeof(FileCopy))]
public static class FileCopyTests
{
    /// <summary>
    /// Tests the <see cref="FileCopy.ExecuteInternal"/>.
    /// </summary>
    [MemberTest(Name = nameof(FileCopy.Execute))]
    public static void Execute()
    {
        FileCopy task = null;
        var succeeded = false;
        ITaskItem[] files = null;
        ITaskItem[] destinationDirs = null;

        Act(() =>
        {
            task = new FileCopy
            {
                BuildEngine = new TestBuildEngine(),
                Files = files,
                DestinationDirs = destinationDirs,
            };

            succeeded = task.Execute();
        });

        When("files are specified", () =>
        {
            var rootDir = PathHelper.GetRandomFileNameInCurrentDir();

            files = new[]
            {
                new TaskItem(Path.Combine(rootDir,  "1", "11", "111", "1111", "file1.txt")),
                new TaskItem(Path.Combine(rootDir,  "2", "22", "222", "2222", "file2.txt")),
                new TaskItem(Path.Combine(rootDir,  "2", "22", "222", "2222", "NOT_EXIST.txt")),
            };

            And("some files exist", () =>
            {
                Arrange(() =>
                {
                    foreach (var file in files.Take(2))
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(file.ItemSpec)))
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(file.ItemSpec));
                        }

                        File.Create(file.ItemSpec).Close();
                    }
                });

                Teardown(() =>
                {
                    if (Directory.Exists(rootDir))
                    {
                        Directory.Delete(rootDir, true);
                    }
                });

                And("destination is a single directory", () =>
                {
                    Arrange(() =>
                    {
                        destinationDirs = new[]
                        {
                            new TaskItem(Path.Combine(rootDir, Path.GetRandomFileName(), "dir")),
                        };
                    });

                    Teardown(() =>
                    {
                        if (Directory.Exists(destinationDirs[0].ItemSpec))
                        {
                            Directory.Delete(destinationDirs[0].ItemSpec, true);
                        }
                    });

                    And("sub directories to copy files to are specified", () =>
                    {
                        files[0].SetMetadata("WithSubDirsFrom", Path.Combine(rootDir, "1", "11"));
                        files[1].SetMetadata("WithSubDirsFrom", Path.Combine(rootDir, "2", "22"));

                        Should("succeed", () =>
                        {
                            Assert.That(succeeded, Is.True);
                            Assert.That(
                                task.CopiedFiles[0].ItemSpec,
                                Is.EqualTo(Path.Combine(destinationDirs[0].ItemSpec, "111", "1111", Path.GetFileName(task.CopiedFiles[0].ItemSpec))));
                            Assert.That(
                                task.CopiedFiles[1].ItemSpec,
                                Is.EqualTo(Path.Combine(destinationDirs[0].ItemSpec, "222", "2222", Path.GetFileName(task.CopiedFiles[1].ItemSpec))));
                        });
                    });

                    And("sub directories to copy files to are not whithin file path", () =>
                    {
                        files[0].SetMetadata("WithSubDirsFrom", Path.Combine("Q:\\", Path.GetRandomFileName()));
                        files[1].SetMetadata("WithSubDirsFrom", Path.Combine("Q:\\", Path.GetRandomFileName()));

                        Should("not copy files with invalid sub directories", () =>
                        {
                            Assert.That(succeeded, Is.True);
                            Assert.That(task.CopiedFiles, Has.Length.Zero);
                        });
                    });

                    And("sub directories to copy files to are not specified", () =>
                    {
                        files[0].RemoveMetadata("WithSubDirsFrom");
                        files[1].RemoveMetadata("WithSubDirsFrom");

                        Should("succeed", () =>
                        {
                            Assert.That(succeeded, Is.True);
                            Assert.That(task.CopiedFiles.Length, Is.EqualTo(2));
                            Assert.That(
                                task.CopiedFiles[0].ItemSpec,
                                Is.EqualTo(Path.Combine(
                                    destinationDirs[0].ItemSpec,
                                    Path.GetFileName(task.CopiedFiles[0].ItemSpec))));
                            Assert.That(
                                task.CopiedFiles[1].ItemSpec,
                                Is.EqualTo(Path.Combine(
                                    destinationDirs[0].ItemSpec,
                                    Path.GetFileName(task.CopiedFiles[1].ItemSpec))));
                        });
                    });

                    And("sub directories to copy files to are invalid and raise an exception", () =>
                    {
                        files[0].SetMetadata("WithSubDirsFrom", Path.Combine("Q", Path.GetRandomFileName()));
                        files[1].SetMetadata("WithSubDirsFrom", Path.Combine("Q", Path.GetRandomFileName()));

                        Teardown(() =>
                        {
                            files[0].RemoveMetadata("WithSubDirsFrom");
                            files[1].RemoveMetadata("WithSubDirsFrom");
                        });

                        Should("not copy files with invalid sub directories", () =>
                        {
                            Assert.That(succeeded, Is.True);
                            Assert.That(task.CopiedFiles, Has.Length.Zero);
                        });
                    });
                });

                And("destination is multiple directories", () =>
                {
                    Arrange(() =>
                    {
                        destinationDirs = new[]
                        {
                            new TaskItem(Path.Combine(rootDir, Path.GetRandomFileName(), "dir1")),
                            new TaskItem(Path.Combine(rootDir, Path.GetRandomFileName(), "dir2")),
                        };
                    });

                    Teardown(() =>
                    {
                        foreach (var dir in destinationDirs)
                        {
                            if (Directory.Exists(dir.ItemSpec))
                            {
                                Directory.Delete(dir.ItemSpec, true);
                            }
                        }
                    });

                    Should("copy each file into the separate directory", () =>
                    {
                        Assert.That(succeeded, Is.True);
                        Assert.That(
                                task.CopiedFiles[0].ItemSpec,
                                Is.EqualTo(Path.Combine(
                                    destinationDirs[0].ItemSpec,
                                    Path.GetFileName(task.CopiedFiles[0].ItemSpec))));
                        Assert.That(
                            task.CopiedFiles[1].ItemSpec,
                            Is.EqualTo(Path.Combine(
                                destinationDirs[1].ItemSpec,
                                Path.GetFileName(task.CopiedFiles[1].ItemSpec))));
                    });
                });
            });
        });
    }
}