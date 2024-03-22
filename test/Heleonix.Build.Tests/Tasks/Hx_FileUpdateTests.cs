// <copyright file="Hx_FileUpdateTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

[ComponentTest(Type = typeof(Hx_FileUpdate))]
public static class Hx_FileUpdateTests
{
    [MemberTest(Name = nameof(Hx_FileUpdate.Execute))]
    public static void Execute()
    {
        Hx_FileUpdate task = null;
        var succeeded = false;
        string file = null;
        ITaskItem[] regExps = new TaskItem[1];

        Arrange(() =>
        {
            file = PathHelper.GetRandomFileNameInCurrentDir();

            task = new Hx_FileUpdate
            {
                BuildEngine = new TestBuildEngine(),
                File = file,
                RegExps = regExps,
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
                using var f = File.CreateText(file);

                f.Write("text text REPLACE_THIS text text");
            });

            Teardown(() =>
            {
                File.Delete(file);
            });

            And("regex is specified", () =>
            {
                regExps[0] = new TaskItem("REPLACE_THIS");

                And("replacement is specified", () =>
                {
                    regExps[0].SetMetadata("Replacement", "REPLACEMENT");

                    Should("replace the text", () =>
                    {
                        Assert.That(succeeded, Is.True);

                        Assert.That(File.ReadAllText(file), Is.EqualTo("text text REPLACEMENT text text"));
                    });
                });

                And("replacement is not specified", () =>
                {
                    regExps[0].RemoveMetadata("Replacement");

                    Should("replace the text with empty string", () =>
                    {
                        Assert.That(succeeded, Is.True);

                        Assert.That(File.ReadAllText(file), Is.EqualTo("text text  text text"));
                    });
                });
            });
        });
    }
}
