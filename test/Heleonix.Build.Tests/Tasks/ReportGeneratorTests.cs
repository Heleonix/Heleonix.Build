// <copyright file="ReportGeneratorTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

/// <summary>
/// Tests the <see cref="ReportGenerator"/>.
/// </summary>
[ComponentTest(Type = typeof(ReportGenerator))]
public static class ReportGeneratorTests
{
    /// <summary>
    /// Tests the <see cref="ReportGenerator.ExecuteInternal"/>.
    /// </summary>
    [MemberTest(Name = nameof(ReportGenerator.Execute))]
    public static void Execute()
    {
        ReportGenerator task = null;
        var succeeded = false;
        string reportTypes = null;
        string reportDir = null;

        Arrange(() =>
        {
            reportDir = PathHelper.GetRandomFileNameInCurrentDir();

            task = new ReportGenerator
            {
                BuildEngine = new TestBuildEngine(),
                ReportGeneratorExe = PathHelper.ReportGeneratorExe,
                ReportDir = reportDir,
                ReportTypes = reportTypes,
                ResultFiles = new ITaskItem[] { new TaskItem(PathHelper.OpenCoverResultFile) },
            };
        });

        Act(() =>
        {
            succeeded = task.Execute();
        });

        Teardown(() =>
        {
            Directory.Delete(reportDir, true);
        });

        When("report types are invalid", () =>
        {
            reportTypes = "INVALID_REPORT_TYPE";

            Should("fail", () =>
            {
                Assert.That(succeeded, Is.False);
                Assert.That(File.Exists(Path.Combine(reportDir, "index.html")), Is.False);
            });
        });

        When("report types are valid", () =>
        {
            reportTypes = "Badges;Html;HtmlSummary";

            Should("succeed", () =>
            {
                Assert.That(succeeded, Is.True);
                Assert.That(File.Exists(Path.Combine(reportDir, "index.htm")), Is.True);
            });
        });
    }
}
