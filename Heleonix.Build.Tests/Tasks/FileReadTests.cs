// <copyright file="FileReadTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
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
    /// Tests the <see cref="FileRead"/>.
    /// </summary>
    [ComponentTest(Type = typeof(FileRead))]
    public static class FileReadTests
    {
        /// <summary>
        /// Tests the <see cref="FileRead.ExecuteInternal"/>.
        /// </summary>
        [MemberTest(Name = nameof(FileRead.Execute))]
        public static void Execute()
        {
            FileRead task = null;
            var succeeded = false;
            ITaskItem file = null;
            string regExp = null;
            string regExpOptions = null;

            Arrange(() =>
            {
                task = new FileRead
                {
                    BuildEngine = new TestBuildEngine(),
                    File = file,
                    RegExp = regExp,
                    RegExpOptions = regExpOptions,
                };
            });

            Act(() =>
            {
                succeeded = task.Execute();
            });

            When("file to read is specified", () =>
            {
                file = new TaskItem(PathHelper.GenerateRandomFileInCurrentDir());

                Should("fail", () =>
                {
                    Assert.That(succeeded, Is.False);

                    Assert.That(task.Matches, Is.Empty);
                });

                And("the specified file exists", () =>
                {
                    Arrange(() =>
                    {
                        using (var f = File.CreateText(file.ItemSpec))
                        {
                            f.WriteLine("text text READ_THIS text text");
                            f.WriteLine("text text READ_THIS text text");
                            f.WriteLine("text text READ_THIS text text");
                        }
                    });

                    Teardown(() =>
                    {
                        File.Delete(file.ItemSpec);
                    });

                    And("regex is specified", () =>
                    {
                        regExp = "read_this";

                        And("regex options are specified", () =>
                        {
                            regExpOptions = RegexOptions.IgnoreCase.ToString();

                            Should("read the text", () =>
                            {
                                Assert.That(succeeded, Is.True);

                                Assert.That(task.Matches, Has.Length.EqualTo(3));
                                Assert.That(task.Matches[0].ItemSpec, Is.EqualTo(file.ItemSpec));
                                Assert.That(task.Matches[0].GetMetadata("Match"), Is.EqualTo("READ_THIS"));
                                Assert.That(task.Matches[1].ItemSpec, Is.EqualTo(file.ItemSpec));
                                Assert.That(task.Matches[1].GetMetadata("Match"), Is.EqualTo("READ_THIS"));
                                Assert.That(task.Matches[2].ItemSpec, Is.EqualTo(file.ItemSpec));
                                Assert.That(task.Matches[2].GetMetadata("Match"), Is.EqualTo("READ_THIS"));
                            });
                        });

                        And("regex options are not specified", () =>
                        {
                            regExpOptions = null;

                            Should("not read the text", () =>
                            {
                                Assert.That(succeeded, Is.True);

                                Assert.That(task.Matches, Is.Empty);
                            });
                        });
                    });
                });
            });
        }
    }
}
