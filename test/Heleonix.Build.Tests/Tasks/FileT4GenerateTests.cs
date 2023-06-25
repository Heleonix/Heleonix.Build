// <copyright file="FileT4GenerateTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

/// <summary>
/// Tests the <see cref="FileT4Generate"/>.
/// </summary>
[ComponentTest(Type = typeof(FileT4Generate))]
public static class FileT4GenerateTests
{
    /// <summary>
    /// Tests the <see cref="FileT4Generate.ExecuteInternal"/>.
    /// </summary>
    [MemberTest(Name = nameof(FileT4Generate.Execute))]
    public static void Execute()
    {
        FileT4Generate task = null;
        var succeeded = false;
        string templateFile = null;
        string generatedFile = null;
        TaskItem[] data = null;
        TestBuildEngine buildEngine = null;

        Arrange(() =>
        {
            templateFile = PathHelper.GetRandomFileNameInCurrentDir();
            generatedFile = PathHelper.GetRandomFileNameInCurrentDir();

            const string template = @"<#@ template hostspecific=""true"" #>" +
                @"<# foreach (var item in Host.Data) #>" +
                @"<#{#>" +
                @"-<#= item.GetMetadata(""description"")#>" +
                @"<#}#>";

            File.WriteAllText(templateFile, template);

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

            buildEngine = new TestBuildEngine();

            task = new FileT4Generate
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
            File.Delete(templateFile);
            File.Delete(generatedFile);
        });

        When("the template file, generated file and data are specified", () =>
        {
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
    }
}
