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
            string testResultFile = null;
            string reportFile = null;

            Arrange(() =>
            {
                reportFile = Path.ChangeExtension(PathHelper.GetRandomFileInCurrentDir(), "html");

                task = new ReportUnit
                {
                    BuildEngine = new TestBuildEngine(),
                    ReportUnitExe = new TaskItem(PathHelper.ReportUnitExe),
                    TestResultFile = new TaskItem(testResultFile),
                    ReportFile = new TaskItem(reportFile)
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

            When("test result file does not exist", () =>
            {
                testResultFile = "NON_EXISTENT_TEST_RESULT_FILE";

                Should("fail", () =>
                {
                    Assert.That(succeeded, Is.True);
                    Assert.That(File.Exists(reportFile), Is.False);
                });
            });

            When("test result file exists", () =>
            {
                testResultFile = PathHelper.NUnitTestResultFile;

                Should("succeed", () =>
                {
                    Assert.That(succeeded, Is.True);
                    Assert.That(File.Exists(reportFile), Is.True);
                });
            });
        }
    }
}
