// <copyright file="FileUpdateTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

/// <summary>
/// Tests the <see cref="FileUpdate"/>.
/// </summary>
[ComponentTest(Type = typeof(FileUpdate))]
public static class FileUpdateTests
{
    /// <summary>
    /// Tests the <see cref="FileUpdate.ExecuteInternal"/>.
    /// </summary>
    [MemberTest(Name = nameof(FileUpdate.Execute))]
    public static void Execute()
    {
        FileUpdate task = null;
        var succeeded = false;
        string file = null;
        string regExp = null;
        string regExpOptions = null;
        string replacement = null;

        Arrange(() =>
        {
            file = PathHelper.GetRandomFileNameInCurrentDir();

            task = new FileUpdate
            {
                BuildEngine = new TestBuildEngine(),
                File = file,
                RegExp = regExp,
                RegExpOptions = regExpOptions,
                Replacement = replacement,
            };
        });

        Act(() =>
        {
            succeeded = task.Execute();
        });

        When("file to update does not exist", () =>
        {
            Should("fail", () =>
            {
                Assert.That(succeeded, Is.False);
            });
        });

        When("file to update exists", () =>
        {
            Arrange(() =>
            {
                using (var f = File.CreateText(file))
                {
                    f.Write("text text REPLACE_THIS text text");
                }
            });

            Teardown(() =>
            {
                File.Delete(file);
            });

            And("regex is specified", () =>
            {
                regExp = "replace_this";

                And("regex options are specified", () =>
                {
                    regExpOptions = RegexOptions.IgnoreCase.ToString();

                    And("replacement is specified", () =>
                    {
                        replacement = "REPLACEMENT";

                        Should("replace the text", () =>
                        {
                            Assert.That(succeeded, Is.True);

                            Assert.That(File.ReadAllText(file), Is.EqualTo("text text REPLACEMENT text text"));
                        });
                    });

                    And("replacement is not specified", () =>
                    {
                        replacement = null;

                        Should("replace the text with empty string", () =>
                        {
                            Assert.That(succeeded, Is.True);

                            Assert.That(File.ReadAllText(file), Is.EqualTo("text text  text text"));
                        });
                    });
                });

                And("regex options are not specified", () =>
                {
                    regExpOptions = null;

                    And("replacement is specified", () =>
                    {
                        replacement = "REPLACEMENT";

                        Should("not replace the text", () =>
                        {
                            Assert.That(succeeded, Is.True);

                            Assert.That(File.ReadAllText(file), Is.EqualTo("text text REPLACE_THIS text text"));
                        });
                    });
                });
            });
        });
    }
}
