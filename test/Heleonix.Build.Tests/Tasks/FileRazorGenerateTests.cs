// <copyright file="FileRazorGenerateTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

/// <summary>
/// Tests the <see cref="FileRazorGenerate"/>.
/// </summary>
[ComponentTest(Type = typeof(FileRazorGenerate))]
public static class FileRazorGenerateTests
{
    /// <summary>
    /// Tests the <see cref="FileRazorGenerate.ExecuteInternal"/>.
    /// </summary>
    [MemberTest(Name = nameof(FileRazorGenerate.Execute))]
    public static void Execute()
    {
        FileRazorGenerate task = null;
        var succeeded = false;
        string templateFile = null;
        TaskItem[] data = null;
        TestBuildEngine buildEngine = null;

        var generatedFile = PathHelper.GetRandomFileNameInCurrentDir();

        var template = @"@using System
@using Microsoft.Build.Framework
@inherits RazorEngineCore.RazorEngineTemplateBase<ITaskItem[]>
@DateTime.UtcNow.ToShortDateString()
@foreach (var item in Model)
{
    <text>- </text> @item.GetMetadata(""description"")
    @:
}";

        Arrange(() =>
        {
            buildEngine = new TestBuildEngine();

            task = new FileRazorGenerate
            {
                BuildEngine = buildEngine,
                TemplateFile = templateFile,
                GeneratedFile = generatedFile,
                Data = data,
            };
        });

        Act(() =>
        {
            succeeded = task.Execute();
        });

        Teardown(() =>
        {
            if (templateFile != null)
            {
                File.Delete(templateFile);
            }

            File.Delete(generatedFile);
        });

        When("the template file and data are specified", () =>
        {
            templateFile = Path.ChangeExtension(PathHelper.GetRandomFileNameInCurrentDir(), ".cshtml");

            data = new TaskItem[]
            {
                new TaskItem("fix(ID-1): Fix 1.", new Dictionary<string, string>
                {
                    { "type", "fix" }, { "scope", "ID-1" }, { "description", "Fix 1." },
                }),
                new TaskItem("feat(ID-2): Feat 2.", new Dictionary<string, string>
                {
                    { "type", "feat" }, { "scope", "ID-2" }, { "description", "Feat 2." },
                }),
                new TaskItem("feat(ID-3)!: Breaking Feat 3.", new Dictionary<string, string>
                {
                    { "type", "feat" }, { "scope", "ID-3" }, { "description", "Feat 3." }, { "breaking", "!" },
                }),
                new TaskItem("fix(ID-4): Fix 4.", new Dictionary<string, string>
                {
                    { "type", "fix" }, { "scope", "ID-4" }, { "description", "Fix 4." },
                }),
                new TaskItem("feat(ID-5): Feat 5.", new Dictionary<string, string>
                {
                    { "type", "feat" }, { "scope", "ID-5" }, { "description", "Feat 5." },
                }),
                new TaskItem("feat(ID-6): Feat 6.\\r\\n\\r\\nBREAKING-CHANGE: Breaking 6.", new Dictionary<string, string>
                {
                    { "type", "feat" }, { "scope", "ID-6" }, { "description", "Fix 1." }, { "breaking", "BREAKING-CHANGE" }, { "footer", "BREAKING-CHANGE: Breaking 6." },
                }),
            };

            Arrange(() =>
            {
                File.WriteAllText(templateFile, template);
            });

            Should("succeed", () =>
            {
                Assert.That(succeeded, Is.True);

                var generatedContent = File.ReadAllText(generatedFile);

                foreach (var d in data)
                {
                    Assert.That(generatedContent, Contains.Substring(d.GetMetadata("description")));
                }
            });
        });

        When("data is not specified", () =>
        {
            data = null;

            Arrange(() =>
            {
                File.WriteAllText(templateFile, template);
            });

            Should("succeed", () =>
            {
                Assert.That(succeeded, Is.True);
            });
        });

        When("the template file is invalid", () =>
        {
            Arrange(() =>
            {
                template = @"@asdasdasd";

                File.WriteAllText(templateFile, template);
            });

            Should("fail", () =>
            {
                Assert.That(succeeded, Is.False);
                Assert.That(buildEngine.ErrorMessages.Count, Is.EqualTo(2));
            });
        });

        When("the template file is not specified", () =>
        {
            templateFile = null;

            Should("fail", () =>
            {
                Assert.That(succeeded, Is.False);
            });
        });
    }
}
