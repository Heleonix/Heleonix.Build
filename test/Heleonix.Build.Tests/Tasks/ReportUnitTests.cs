// <copyright file="ReportUnitTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

/// <summary>
/// Tests the <see cref="ReportUnit"/>.
/// </summary>
[ComponentTest(Type = typeof(ReportUnit))]
public static class ReportUnitTests
{
    /// <summary>
    /// Tests the <see cref="ReportUnit.ExecuteInternal"/>.
    /// </summary>
    [MemberTest(Name = nameof(ReportUnit.Execute))]
    public static void Execute()
    {
        ReportUnit task = null;
        var succeeded = false;
        string testResultFile = null;
        string reportFile = null;

        Arrange(() =>
        {
            task = new ReportUnit
            {
                BuildEngine = new TestBuildEngine(),
                ReportUnitExe = PathHelper.ReportUnitExe,
                TestResultFile = testResultFile,
                ReportFile = reportFile,
            };
        });

        Act(() =>
        {
            succeeded = task.Execute();
        });

        Teardown(() =>
        {
            if (File.Exists(reportFile))
            {
                File.Delete(reportFile);
            }
        });

        When("report file is not specified", () =>
        {
            reportFile = string.Empty;

            And("test result file is not specified", () =>
            {
                testResultFile = string.Empty;

                Should("fail and not generate a report file", () =>
                {
                    Assert.That(succeeded, Is.True);
                    Assert.That(File.Exists(reportFile), Is.False);
                });
            });
        });

        When("report file is specified", () =>
        {
            reportFile = Path.ChangeExtension(PathHelper.GetRandomFileNameInCurrentDir(), "html");

            And("test result file is specified", () =>
            {
                testResultFile = PathHelper.NUnitTestResultFile;

                Should("succeed", () =>
                {
                    Assert.That(succeeded, Is.True);
                    Assert.That(File.Exists(reportFile), Is.True);
                });
            });

            And("test result file does not exist", () =>
            {
                testResultFile = "NON_EXISTENT_TEST_RESULT_FILE";

                Should("succeed without generating a report file", () =>
                {
                    Assert.That(succeeded, Is.True);
                    Assert.That(File.Exists(reportFile), Is.False);
                });
            });
        });
    }
}
