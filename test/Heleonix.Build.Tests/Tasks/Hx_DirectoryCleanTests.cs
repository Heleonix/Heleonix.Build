// <copyright file="Hx_DirectoryCleanTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

/// <summary>
/// Tests the <see cref="Hx_DirectoryClean"/>.
/// </summary>
[ComponentTest(Type = typeof(Hx_DirectoryClean))]
public static class Hx_DirectoryCleanTests
{
    /// <summary>
    /// Tests the <see cref="Hx_DirectoryClean.ExecuteInternal"/>.
    /// </summary>
    [MemberTest(Name = nameof(Hx_DirectoryClean.Execute))]
    public static void Execute()
    {
        string[] directories = null;
        Hx_DirectoryClean task = null;
        var succeeded = false;

        Arrange(() =>
        {
            task = new Hx_DirectoryClean
            {
                BuildEngine = new TestBuildEngine(),
                Dirs = directories,
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
                PathHelper.GetRandomFileNameInCurrentDir(),
                PathHelper.GetRandomFileNameInCurrentDir(),
                PathHelper.GetRandomFileNameInCurrentDir(),
            };

            And("directories to clean exist", () =>
            {
                Teardown(() =>
                {
                    foreach (var dir in directories)
                    {
                        Directory.Delete(dir, true);
                    }
                });

                foreach (var dir in directories)
                {
                    Directory.CreateDirectory(dir);
                    File.Create(Path.Combine(dir, "1.txt")).Close();
                    File.Create(Path.Combine(dir, "2.txt")).Close();

                    Directory.CreateDirectory(Path.Combine(dir, "dir1"));
                    File.Create(Path.Combine(dir, "dir1", "1.txt")).Close();
                    File.Create(Path.Combine(dir, "dir1", "2.txt")).Close();
                }

                Should("successfully clean all the specified directories", () =>
                {
                    Assert.That(succeeded, Is.True);

                    foreach (var dir in directories)
                    {
                        Assert.That(task.CleanedDirs.Any(item => item == dir));
                        Assert.That(Directory.GetFiles(dir), Has.Length.Zero);
                        Assert.That(Directory.GetDirectories(dir), Has.Length.Zero);
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
                Directory.CreateDirectory(directories[0]);
                var stream = File.Create(Path.Combine(directories[0], "1.txt"));

                Teardown(() =>
                {
                    stream.Close();

                    Directory.Delete(directories[0], true);
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
