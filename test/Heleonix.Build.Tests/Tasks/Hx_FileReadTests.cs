// <copyright file="Hx_FileReadTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

[ComponentTest(Type = typeof(Hx_FileRead))]
public static class Hx_FileReadTests
{
    [MemberTest(Name = nameof(Hx_FileRead.Execute))]
    public static void Execute()
    {
        Hx_FileRead task = null;
        var succeeded = false;
        string file = null;
        string regExp = null;

        Arrange(() =>
        {
            task = new Hx_FileRead
            {
                BuildEngine = new TestBuildEngine(),
                File = file,
                RegExp = regExp,
            };
        });

        Act(() =>
        {
            succeeded = task.Execute();
        });

        When("file to read is specified", () =>
        {
            file = PathHelper.GetRandomFileNameInCurrentDir();

            Should("fail", () =>
            {
                Assert.That(succeeded, Is.False);

                Assert.That(task.Matches, Is.Empty);
            });

            And("the specified file exists", () =>
            {
                Arrange(() =>
                {
                    using (var f = File.CreateText(file))
                    {
                        f.WriteLine("text text READ_THIS text text");
                        f.WriteLine("text text READ_THIS text text");
                        f.WriteLine("text text READ_THIS text text");
                    }
                });

                Teardown(() =>
                {
                    File.Delete(file);
                });

                And("regex is specified", () =>
                {
                    regExp = "READ_THIS";

                    Should("read the text", () =>
                    {
                        Assert.That(succeeded, Is.True);

                        Assert.That(task.Matches, Has.Length.EqualTo(3));
                        Assert.That(task.Matches[0].ItemSpec, Is.EqualTo(file));
                        Assert.That(task.Matches[0].GetMetadata("Match"), Is.EqualTo("READ_THIS"));
                        Assert.That(task.Matches[1].ItemSpec, Is.EqualTo(file));
                        Assert.That(task.Matches[1].GetMetadata("Match"), Is.EqualTo("READ_THIS"));
                        Assert.That(task.Matches[2].ItemSpec, Is.EqualTo(file));
                        Assert.That(task.Matches[2].GetMetadata("Match"), Is.EqualTo("READ_THIS"));
                    });
                });
            });
        });
    }
}
