// <copyright file="Hx_MetadataToCmdArgsTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

[ComponentTest(Type = typeof(Hx_MetadataToCmdArgs))]

public static class Hx_MetadataToCmdArgsTests
{
    [MemberTest(Name = nameof(Hx_MetadataToCmdArgs.Execute))]
    public static void Execute()
    {
        Hx_MetadataToCmdArgs task = null;
        ITaskItem item = null;
        var dottedKeys = false;
        var succeeded = false;

        Act(() =>
        {
            task = new Hx_MetadataToCmdArgs
            {
                BuildEngine = new TestBuildEngine(),
                DottedKeys = dottedKeys,
                Item = item,
            };

            succeeded = task.Execute();
        });

        When("the task is executed", () =>
        {
            And("the item with metadata is not povided", () =>
            {
                item = null;

                Should("succeed without stringified metadata", () =>
                {
                    Assert.That(succeeded);
                    Assert.That(task.Result, Is.Null);
                });
            });

            And("the item with metadata to stringify is povided", () =>
            {
                item = new TaskItem(
                    "not-used",
                    new Dictionary<string, string> { { "__Arg_One", "One" }, { "Arg_Two", string.Empty } });

                Should("succeed with stringified metadata", () =>
                {
                    Assert.That(succeeded);
                    Assert.That(task.Result, Is.EqualTo("Arg_Two --Arg_One=One "));
                });

                And("the keys should be dotted", () =>
                {
                    dottedKeys = true;

                    Should("succeed with stringified dotted metadata", () =>
                    {
                        Assert.That(succeeded);
                        Assert.That(task.Result, Is.EqualTo("Arg.Two --Arg.One=One "));
                    });
                });
            });
        });
    }
}
