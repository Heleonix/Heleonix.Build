// <copyright file="NUnit.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Heleonix.Build.Properties;
    using Heleonix.Execution;
    using Microsoft.Build.Framework;

    /// <summary>
    /// Runs the NUnit.
    /// </summary>
    public class NUnit : BaseTask
    {
        /// <summary>
        /// Gets or sets the NUnit console executable path.
        /// </summary>
        [Required]
        public ITaskItem NUnitConsoleExe { get; set; }

        /// <summary>
        /// Gets or sets the NUnit project or tests files paths.
        /// </summary>
        [Required]
#pragma warning disable CA1819 // Properties should not return arrays
        public ITaskItem[] NUnitProjectFileOrTestFiles { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Gets or sets the NUnit tests result file path.
        /// </summary>
        [Required]
        public ITaskItem TestResultFile { get; set; }

        /// <summary>
        /// Gets or sets the tests output file path.
        /// </summary>
        [Required]
        public ITaskItem TestOutputFile { get; set; }

        /// <summary>
        /// Gets or sets the errors output file path.
        /// </summary>
        [Required]
        public ITaskItem ErrorOutputFile { get; set; }

        /// <summary>
        /// Gets or sets the tests list file path.
        /// </summary>
        /// <remarks>
        /// File is containing a list of tests to run or explore, one per line.
        /// </remarks>
        public ITaskItem TestListFile { get; set; }

        /// <summary>
        /// Gets or sets the tests parameters, specified in the form name=value.
        /// </summary>
        /// <remarks>
        /// Multiple parameters may be specified, separated by semicolons.
        /// </remarks>
        public string TestParameters { get; set; }

        /// <summary>
        /// Gets or sets a tests filter to filter tests to run.
        /// </summary>
        /// <remarks>
        /// For more details, see NUnit Test Selection Language.
        /// </remarks>
        public string TestFilter { get; set; }

        /// <summary>
        /// Gets or sets the agents number.
        /// </summary>
        public int AgentsNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether indicates whether to stop on first error or first failed test.
        /// </summary>
        public bool StopOnErrorOrFailedTest { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether indicates whether to use TeamCity service messages.
        /// </summary>
        public bool UseTeamCityServiceMessages { get; set; }

        /// <summary>
        /// Gets or sets the framework to use for tests. Examples: "mono", "net-4.5", "v4.0", "2.0", "mono-4.0".
        /// </summary>
        public string Framework { get; set; }

        /// <summary>
        /// Gets or sets the trace level.
        /// </summary>
        /// <remarks>
        /// Possible values:
        /// <list type="bullet">
        /// <item><term>Off</term></item>
        /// <item><term>Error</term></item>
        /// <item><term>Warning</term></item>
        /// <item><term>Info</term></item>
        /// <item><term>Verbose</term></item>
        /// </list>
        /// </remarks>
        public string TraceLevel { get; set; }

        /// <summary>
        /// Gets or sets the project configuration to load, i.e. "Debug", "Release".
        /// </summary>
        public string Configuration { get; set; }

        /// <summary>
        /// Gets or sets process isolation for test assemblies.
        /// </summary>
        /// <remarks>
        /// If not specified, defaults to "Separate" for a single assembly or "Multiple" for more than one.
        /// By default, processes are run in parallel.
        /// Possible values:
        /// <list type="bullet">
        /// <item><term>Single</term></item>
        /// <item><term>Separate</term></item>
        /// <item><term>Multiple</term></item>
        /// </list>
        /// </remarks>
        public string ProcessIsolation { get; set; }

        /// <summary>
        /// Gets or sets domain isolation for test assemblies.
        /// </summary>
        /// <remarks>
        /// If not specified, defaults to "Separate" for a single assembly or "Multiple" for more than one.
        /// Possible values:
        /// <list type="bullet">
        /// <item><term>None</term></item>
        /// <item><term>Single</term></item>
        /// <item><term>Multiple</term></item>
        /// </list>
        /// </remarks>
        public string DomainIsolation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether tells .NET to copy loaded assemblies to the shadow copy directory.
        /// </summary>
        public bool ShadowCopy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether indicates whether to fail the task on failed tests.
        /// </summary>
        public bool FailOnFailedTests { get; set; }

        /// <summary>
        /// Gets or sets the count of test cases [Output].
        /// </summary>
        [Output]
        public int TestCases { get; set; }

        /// <summary>
        /// Gets or sets the total count of tests [Output].
        /// </summary>
        [Output]
        public int Total { get; set; }

        /// <summary>
        /// Gets or sets the count of passed tests [Output].
        /// </summary>
        [Output]
        public int Passed { get; set; }

        /// <summary>
        /// Gets or sets the count of failed tests [Output].
        /// </summary>
        [Output]
        public int Failed { get; set; }

        /// <summary>
        /// Gets or sets the count of inconclusive tests [Output].
        /// </summary>
        [Output]
        public int Inconclusive { get; set; }

        /// <summary>
        /// Gets or sets the count of skipped tests [Output].
        /// </summary>
        [Output]
        public int Skipped { get; set; }

        /// <summary>
        /// Gets or sets the count of asserts [Output].
        /// </summary>
        [Output]
        public int Asserts { get; set; }

        /// <summary>
        /// Gets or sets the start time [Output].
        /// </summary>
        [Output]
        public string StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time [Output].
        /// </summary>
        [Output]
        public string EndTime { get; set; }

        /// <summary>
        /// Gets or sets the duration of tests running [Output].
        /// </summary>
        [Output]
        public float Duration { get; set; }

        /// <summary>
        /// Builds the arguments.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The arguments.</returns>
        internal static string BuildArgs(ITaskItem item)
        {
            return ArgsBuilder.By("--", "=")
                .AddPath(item.GetMetadata(nameof(NUnitProjectFileOrTestFiles)).Replace(";", "\" \""))
                .AddPath("result", item.GetMetadata(nameof(TestResultFile)))
                .AddPath("testlist", item.GetMetadata(nameof(TestListFile)))
                .AddPath("where", item.GetMetadata(nameof(TestFilter)))
                .AddPath("params", item.GetMetadata(nameof(TestParameters)))
                .AddArgument("agents", item.GetMetadata(nameof(AgentsNumber)))
                .AddKey("stoponerror", item.GetMetadata(nameof(StopOnErrorOrFailedTest)) == "true")
                .AddKey("teamcity", item.GetMetadata(nameof(UseTeamCityServiceMessages)) == "true")
                .AddArgument("trace", item.GetMetadata(nameof(TraceLevel)))
                .AddPath("output", item.GetMetadata(nameof(TestOutputFile)))
                .AddPath("err", item.GetMetadata(nameof(ErrorOutputFile)))
                .AddArgument("framework", item.GetMetadata(nameof(Framework)))
                .AddArgument("config", item.GetMetadata(nameof(Configuration)))
                .AddArgument("process", item.GetMetadata(nameof(ProcessIsolation)))
                .AddArgument("domain", item.GetMetadata(nameof(DomainIsolation)))
                .AddKey("shadowcopy", item.GetMetadata(nameof(ShadowCopy)) == "true");
        }

        /// <summary>
        /// Prepares execution of the task.
        /// </summary>
        /// <param name="target">The target.</param>
        internal static void Prepare(ITaskItem target)
        {
            Prepare(
                target.GetMetadata(nameof(TestResultFile)),
                target.GetMetadata(nameof(TestOutputFile)),
                target.GetMetadata(nameof(ErrorOutputFile)));
        }

        /// <summary>
        /// Prepares execution of the task.
        /// </summary>
        /// <param name="testResultFile">The NUnit tests result file path.</param>
        /// <param name="testOutputFile">The tests output file path.</param>
        /// <param name="errorOutputFile">The errors output file path.</param>
        internal static void Prepare(string testResultFile, string testOutputFile, string errorOutputFile)
        {
            // NUnit does not create directories for these files.
            Directory.CreateDirectory(Path.GetDirectoryName(testResultFile));
            Directory.CreateDirectory(Path.GetDirectoryName(testOutputFile));
            Directory.CreateDirectory(Path.GetDirectoryName(errorOutputFile));
        }

        /// <summary>
        /// Executes the NUnit.
        /// </summary>
        protected override void ExecuteInternal()
        {
            Prepare(this.TestResultFile.ItemSpec, this.TestOutputFile.ItemSpec, this.ErrorOutputFile.ItemSpec);

            var args = ArgsBuilder.By("--", "=")
                .AddPaths(this.NUnitProjectFileOrTestFiles.Select(i => i.ItemSpec))
                .AddPath("result", this.TestResultFile.ItemSpec)
                .AddPath("testlist", this.TestListFile?.ItemSpec)
                .AddPath("where", this.TestFilter)
                .AddArgument("params", this.TestParameters)
                .AddArgument("agents", this.AgentsNumber, this.AgentsNumber > 0)
                .AddKey("stoponerror", this.StopOnErrorOrFailedTest)
                .AddKey("teamcity", this.UseTeamCityServiceMessages)
                .AddArgument("trace", this.TraceLevel)
                .AddPath("output", this.TestOutputFile.ItemSpec)
                .AddPath("err", this.ErrorOutputFile.ItemSpec)
                .AddArgument("framework", this.Framework)
                .AddArgument("config", this.Configuration)
                .AddArgument("process", this.ProcessIsolation)
                .AddArgument("domain", this.DomainIsolation)
                .AddKey("shadowcopy", this.ShadowCopy);

            var result = ExeHelper.Execute(this.NUnitConsoleExe.ItemSpec, args, true);

            this.Log.LogMessage(result.Output);

            if (!string.IsNullOrEmpty(result.Error))
            {
                this.Log.LogError(result.Error);
            }

            if (result.ExitCode < 0)
            {
                this.Log.LogError(Resources.TaskFailedWithExitCode, nameof(NUnit), result.ExitCode);

                return;
            }

            if (result.ExitCode > 0)
            {
                if (this.FailOnFailedTests)
                {
                    this.Log.LogError(Resources.NUnit_FinishedWithFailedTests, nameof(NUnit), result.ExitCode);
                }
                else
                {
                    this.Log.LogWarning(Resources.NUnit_FinishedWithFailedTests, nameof(NUnit), result.ExitCode);
                }
            }

            var testRun = XDocument.Load(this.TestResultFile.ItemSpec).Element("test-run");

            this.TestCases = Convert.ToInt32(testRun.Attribute("testcasecount").Value, NumberFormatInfo.InvariantInfo);
            this.Total = Convert.ToInt32(testRun.Attribute("total").Value, NumberFormatInfo.InvariantInfo);
            this.Passed = Convert.ToInt32(testRun.Attribute("passed").Value, NumberFormatInfo.InvariantInfo);
            this.Failed = Convert.ToInt32(testRun.Attribute("failed").Value, NumberFormatInfo.InvariantInfo);
            this.Inconclusive = Convert.ToInt32(testRun.Attribute("inconclusive").Value, NumberFormatInfo.InvariantInfo);
            this.Skipped = Convert.ToInt32(testRun.Attribute("skipped").Value, NumberFormatInfo.InvariantInfo);
            this.Asserts = Convert.ToInt32(testRun.Attribute("asserts").Value, NumberFormatInfo.InvariantInfo);
            this.StartTime = testRun.Attribute("start-time").Value;
            this.EndTime = testRun.Attribute("end-time").Value;
            this.Duration = Convert.ToSingle(testRun.Attribute("duration").Value, CultureInfo.InvariantCulture);

            this.Log.LogMessage(Resources.NUnit_TestCases, this.TestCases);
            this.Log.LogMessage(Resources.NUnit_Total, this.Total);
            this.Log.LogMessage(Resources.NUnit_Passed, this.Passed);
            this.Log.LogMessage(Resources.NUnit_Failed, this.Failed);
            this.Log.LogMessage(Resources.NUnit_Inconclusive, this.Inconclusive);
            this.Log.LogMessage(Resources.NUnit_Skipped, this.Skipped);
            this.Log.LogMessage(Resources.NUnit_Asserts, this.Asserts);
            this.Log.LogMessage(Resources.NUnit_StartTime, this.StartTime);
            this.Log.LogMessage(Resources.NUnit_EndTime, this.EndTime);
            this.Log.LogMessage(Resources.NUnit_Duration, this.Duration);
        }
    }
}
