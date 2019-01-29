// <copyright file="ReportUnitTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks
{
    using System.IO;
    using Heleonix.Build.Tasks;
    using Heleonix.Build.Tests.Common;
    using Heleonix.Testing.NUnit.Aaa;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NUnit.Framework;
    using static Heleonix.Testing.NUnit.Aaa.AaaSpec;

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
            ITaskItem testResultFile = null;
            ITaskItem reportFile = null;

            Arrange(() =>
            {
                task = new ReportUnit
                {
                    BuildEngine = new TestBuildEngine(),
                    ReportUnitExe = new TaskItem(PathHelper.ReportUnitExe),
                    TestResultFile = testResultFile,
                    ReportFile = new TaskItem(reportFile)
                };
            });

            Act(() =>
            {
                succeeded = task.Execute();
            });

            Teardown(() =>
            {
                if (File.Exists(reportFile.ItemSpec))
                {
                    File.Delete(reportFile.ItemSpec);
                }
            });

            When("report file is not specified", () =>
            {
                reportFile = new TaskItem();

                And("test result file is not specified", () =>
                {
                    testResultFile = new TaskItem();

                    Should("fail and not generate a report file", () =>
                    {
                        Assert.That(succeeded, Is.True);
                        Assert.That(File.Exists(reportFile.ItemSpec), Is.False);
                    });
                });
            });

            When("report file is specified", () =>
            {
                reportFile = new TaskItem(Path.ChangeExtension(PathHelper.GetRandomFileInCurrentDir(), "html"));

                And("test result file is specified", () =>
                {
                    testResultFile = new TaskItem(PathHelper.NUnitTestResultFile);

                    Should("succeed", () =>
                    {
                        Assert.That(succeeded, Is.True);
                        Assert.That(File.Exists(reportFile.ItemSpec), Is.True);
                    });
                });

                And("test result file does not exist", () =>
                {
                    testResultFile = new TaskItem("NON_EXISTENT_TEST_RESULT_FILE");

                    Should("succeed without generating a report file", () =>
                    {
                        Assert.That(succeeded, Is.True);
                        Assert.That(File.Exists(reportFile.ItemSpec), Is.False);
                    });
                });
            });
        }
    }
}
