// <copyright file="FileValidateTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

/// <summary>
/// Tests the <see cref="FileValidate"/>.
/// </summary>
[ComponentTest(Type = typeof(FileValidate))]
public static class FileValidateTests
{
    /// <summary>
    /// Tests the <see cref="FileValidate.ExecuteInternal"/>.
    /// </summary>
    [MemberTest(Name = nameof(FileValidate.Execute))]
    public static void Execute()
    {
        FileValidate task = null;
        var succeeded = false;
        ITaskItem[] files = null;

        Arrange(() =>
        {
            task = new FileValidate
            {
                BuildEngine = new TestBuildEngine(),
                Files = files,
            };
        });

        Act(() =>
        {
            succeeded = task.Execute();
        });

        When("files are specified", () =>
        {
            files = new ITaskItem[3]
            {
                new TaskItem(PathHelper.GetRandomFileNameInCurrentDir()),
                new TaskItem(PathHelper.GetRandomFileNameInCurrentDir()),
                new TaskItem(PathHelper.GetRandomFileNameInCurrentDir()),
            };

            foreach (var file in files)
            {
                file.SetMetadata("VALUE1", "VALUE 1");
                file.SetMetadata("VALUE2", "VALUE 2");
            }

            And("specified files exist", () =>
            {
                Teardown(() =>
                {
                    foreach (var file in files)
                    {
                        File.Delete(file.ItemSpec);
                    }
                });

                And("content is valid", () =>
                {
                    Arrange(() =>
                    {
                        foreach (var file in files)
                        {
                            using (var fs = File.CreateText(file.ItemSpec))
                            {
                                fs.WriteLine("text text VALUE 1 text text");
                                fs.WriteLine("text text VALUE 2 text text");
                                fs.WriteLine("text text SOME OTHER VALUE text text");
                            }
                        }
                    });

                    Should("succeed", () =>
                    {
                        Assert.That(succeeded, Is.True);
                    });
                });

                And("content is invalid", () =>
                {
                    Arrange(() =>
                    {
                        foreach (var file in files)
                        {
                            using (var fs = File.CreateText(file.ItemSpec))
                            {
                                fs.WriteLine("text text NO VALUE text text");
                                fs.WriteLine("text text NO VALUE text text");
                                fs.WriteLine("text text SOME OTHER VALUE text text");
                            }
                        }
                    });

                    Should("fail", () =>
                    {
                        Assert.That(succeeded, Is.False);
                    });
                });
            });

            And("specified files do not exist", () =>
            {
                Should("fail", () =>
                {
                    Assert.That(succeeded, Is.False);
                });
            });
        });
    }
}
