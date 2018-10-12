// <copyright file="FileCopyTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks
{
    using System.Collections.Generic;
    using System.IO;
    using Heleonix.Build.Tasks;
    using Heleonix.Build.Tests.Common;
    using Heleonix.Testing.NUnit.Aaa;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NUnit.Framework;
    using static Heleonix.Testing.NUnit.Aaa.AaaSpec;

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
            ITaskItem[] destinations = null;

            Arrange(() =>
            {
                task = new FileCopy
                {
                    BuildEngine = new TestBuildEngine(),
                    Files = files,
                    Destinations = destinations
                };
            });

            Act(() =>
            {
                succeeded = task.Execute();
            });

            When("files are provided", () =>
            {
                And("sub directories to copy files to are specified", () =>
                {
                    files = new[]
                    {
                        new TaskItem(
                            Path.Combine(PathHelper.CurrentDir, "1", "11", "111", "1111", "file1.txt"),
                            new Dictionary<string, string> { { "WithSubDirsFrom", Path.Combine(PathHelper.CurrentDir, "1", "11") } }),
                        new TaskItem(
                            Path.Combine(PathHelper.CurrentDir, "2", "22", "222", "2222", "file2.txt"),
                            new Dictionary<string, string> { { "WithSubDirsFrom", Path.Combine(PathHelper.CurrentDir, "2", "22") } })
                    };

                    And("files exist", () =>
                    {
                        foreach (var file in files)
                        {
                            if (!Directory.Exists(Path.GetDirectoryName(file.ItemSpec)))
                            {
                                Directory.CreateDirectory(Path.GetDirectoryName(file.ItemSpec));
                            }

                            File.Create(file.ItemSpec).Close();
                        }

                        Teardown(() =>
                        {
                            if (Directory.Exists(Path.Combine(PathHelper.CurrentDir, "1")))
                            {
                                Directory.Delete(Path.Combine(PathHelper.CurrentDir, "1"), true);
                            }

                            if (Directory.Exists(Path.Combine(PathHelper.CurrentDir, "2")))
                            {
                                Directory.Delete(Path.Combine(PathHelper.CurrentDir, "2"), true);
                            }
                        });

                        And("destination is a single directory", () =>
                        {
                            destinations = new[] { new TaskItem(PathHelper.GetRandomFileInCurrentDir()) };

                            Teardown(() =>
                            {
                                if (Directory.Exists(destinations[0].ItemSpec))
                                {
                                    Directory.Delete(destinations[0].ItemSpec, true);
                                }
                            });

                            Should("succeed", () =>
                            {
                                Assert.That(succeeded, Is.True);
                                Assert.That(
                                    task.CopiedFiles[0].ItemSpec,
                                    Is.EqualTo(Path.Combine(destinations[0].ItemSpec, "111", "1111", Path.GetFileName(task.CopiedFiles[0].ItemSpec))));
                                Assert.That(
                                    task.CopiedFiles[1].ItemSpec,
                                    Is.EqualTo(Path.Combine(destinations[0].ItemSpec, "222", "2222", Path.GetFileName(task.CopiedFiles[1].ItemSpec))));
                            });
                        });
                    });
                });
            });
        }
    }
}
