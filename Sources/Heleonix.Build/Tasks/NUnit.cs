﻿/*
The MIT License (MIT)

Copyright (c) 2015-present Heleonix - Hennadii Lutsyshyn

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Heleonix.Build.Properties;
using Microsoft.Build.Framework;

namespace Heleonix.Build.Tasks
{
    /// <summary>
    /// Runs the NUnit.
    /// </summary>
    public class NUnit : BaseTask
    {
        #region Methods

        /// <summary>
        /// Builds the arguments.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The arguments.</returns>
        internal static string BuildArgs(ITaskItem item)
        {
            return ArgsBuilder.By("--", "=")
                .AddPath(item.GetMetadata(nameof(NUnitProjectFileOrTestsFiles)).Replace(";", "\" \""))
                .AddPath("result", item.GetMetadata(nameof(TestsResultFile)))
                .AddKey("noresult", item.GetMetadata(nameof(TestsResultFile)) == null)
                .AddPath("testlist", item.GetMetadata(nameof(TestsListFile)))
                .AddPath("where", item.GetMetadata(nameof(TestsFilter)))
                .AddPath("params", item.GetMetadata(nameof(TestsParameters)))
                .AddArgument("agents", item.GetMetadata(nameof(AgentsNumber)))
                .AddKey("stoponerror", item.GetMetadata(nameof(StopOnErrorOrFailedTest)) == "true")
                .AddKey("teamcity", item.GetMetadata(nameof(UseTeamCityServiceMessages)) == "true")
                .AddArgument("trace", item.GetMetadata(nameof(TraceLevel)))
                .AddPath("output", item.GetMetadata(nameof(TestsOutputFile)))
                .AddPath("err", item.GetMetadata(nameof(ErrorsOutputFile)))
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
            Prepare(target.GetMetadata(nameof(TestsResultFile)), target.GetMetadata(nameof(TestsOutputFile)),
                target.GetMetadata(nameof(ErrorsOutputFile)));
        }

        /// <summary>
        /// Prepares execution of the task.
        /// </summary>
        /// <param name="testsResultFile">The NUnit tests result file path.</param>
        /// <param name="testsOutputFile">The tests output file path.</param>
        /// <param name="errorsOutputFile">The errors output file path.</param>
        internal static void Prepare(string testsResultFile, string testsOutputFile, string errorsOutputFile)
        {
            // NUnit does not create a directory for tests result file.
            if (!string.IsNullOrEmpty(testsResultFile) &&
                !Directory.Exists(Path.GetDirectoryName(testsResultFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(testsResultFile));
            }

            // NUnit does not create a directory for tests output file.
            if (!string.IsNullOrEmpty(testsOutputFile) &&
                !Directory.Exists(Path.GetDirectoryName(testsOutputFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(testsOutputFile));
            }

            // NUnit does not create a directory for errors output file.
            if (!string.IsNullOrEmpty(errorsOutputFile) &&
                !Directory.Exists(Path.GetDirectoryName(errorsOutputFile)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(errorsOutputFile));
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The NUnit console executable path.
        /// </summary>
        [Required]
        public ITaskItem NUnitConsoleExeFile { get; set; }

        /// <summary>
        /// The NUnit project or tests files paths.
        /// </summary>
        [Required]
        public ITaskItem[] NUnitProjectFileOrTestsFiles { get; set; }

        /// <summary>
        /// The NUnit tests result file path.
        /// </summary>
        public ITaskItem TestsResultFile { get; set; }

        /// <summary>
        /// The tests list file path.
        /// </summary>
        /// <remarks>
        /// File is containing a list of tests to run or explore, one per line.
        /// </remarks>
        public ITaskItem TestsListFile { get; set; }

        /// <summary>
        /// The tests parameters, specified in the form name=value.
        /// </summary>
        /// <remarks>
        /// Multiple parameters may be specified, separated by semicolons.
        /// </remarks>
        public string TestsParameters { get; set; }

        /// <summary>
        /// A tests filter to filter tests to run.
        /// </summary>
        /// <remarks>
        /// For more details, see NUnit Test Selection Language.
        /// </remarks>
        public string TestsFilter { get; set; }

        /// <summary>
        /// The tests output file path.
        /// </summary>
        public ITaskItem TestsOutputFile { get; set; }

        /// <summary>
        /// The errors output file path.
        /// </summary>
        public ITaskItem ErrorsOutputFile { get; set; }

        /// <summary>
        /// The agents number.
        /// </summary>
        public int AgentsNumber { get; set; }

        /// <summary>
        /// Indicates whether to stop on first error or first failed test.
        /// </summary>
        public bool StopOnErrorOrFailedTest { get; set; }

        /// <summary>
        /// Indicates whether to use TeamCity service messages.
        /// </summary>
        public bool UseTeamCityServiceMessages { get; set; }

        /// <summary>
        /// The framework to use for tests. Examples: "mono", "net-4.5", "v4.0", "2.0", "mono-4.0".
        /// </summary>
        public string Framework { get; set; }

        /// <summary>
        /// The trace level.
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
        /// The project configuration to load, i.e. "Debug", "Release".
        /// </summary>
        public string Configuration { get; set; }

        /// <summary>
        /// Process isolation for test assemblies.
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
        /// Domain isolation for test assemblies.
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
        /// Tells .NET to copy loaded assemblies to the shadow copy directory.
        /// </summary>
        public bool ShadowCopy { get; set; }

        /// <summary>
        /// Indicates whether to fail the task on failed tests.
        /// </summary>
        public bool FailOnFailedTests { get; set; }

        /// <summary>
        /// [Output] The count of test cases.
        /// </summary>
        [Output]
        public int TestCases { get; set; }

        /// <summary>
        /// [Output] The total count of tests.
        /// </summary>
        [Output]
        public int Total { get; set; }

        /// <summary>
        /// [Output] The count of passed tests.
        /// </summary>
        [Output]
        public int Passed { get; set; }

        /// <summary>
        /// [Output] The count of failed tests.
        /// </summary>
        [Output]
        public int Failed { get; set; }

        /// <summary>
        /// [Output] The count of inconclusive tests.
        /// </summary>
        [Output]
        public int Inconclusive { get; set; }

        /// <summary>
        /// [Output] The count of skipped tests.
        /// </summary>
        [Output]
        public int Skipped { get; set; }

        /// <summary>
        /// [Output] The count of asserts.
        /// </summary>
        [Output]
        public int Asserts { get; set; }

        /// <summary>
        /// [Output] The start time.
        /// </summary>
        [Output]
        public string StartTime { get; set; }

        /// <summary>
        /// [Output] The end time.
        /// </summary>
        [Output]
        public string EndTime { get; set; }

        /// <summary>
        /// [Output] The duration of tests running.
        /// </summary>
        [Output]
        public float Duration { get; set; }

        #endregion

        #region BaseTask Members

        /// <summary>
        /// Executes the NUnit.
        /// </summary>
        protected override void ExecuteInternal()
        {
            Prepare(TestsResultFile?.ItemSpec, TestsOutputFile?.ItemSpec, ErrorsOutputFile?.ItemSpec);

            var args = ArgsBuilder.By("--", "=")
                .AddPaths(NUnitProjectFileOrTestsFiles.Select(i => i.ItemSpec))
                .AddPath("result", TestsResultFile?.ItemSpec)
                .AddKey("noresult", TestsResultFile == null)
                .AddPath("testlist", TestsListFile?.ItemSpec)
                .AddPath("where", TestsFilter)
                .AddArgument("params", TestsParameters)
                .AddArgument("agents", AgentsNumber, AgentsNumber > 0)
                .AddKey("stoponerror", StopOnErrorOrFailedTest)
                .AddKey("teamcity", UseTeamCityServiceMessages)
                .AddArgument("trace", TraceLevel)
                .AddPath("output", TestsOutputFile?.ItemSpec)
                .AddPath("err", ErrorsOutputFile?.ItemSpec)
                .AddArgument("framework", Framework)
                .AddArgument("config", Configuration)
                .AddArgument("process", ProcessIsolation)
                .AddArgument("domain", DomainIsolation)
                .AddKey("shadowcopy", ShadowCopy);

            var result = ExeHelper.Execute(NUnitConsoleExeFile.ItemSpec, args, true);

            Log.LogMessage(result.Output);

            if (!string.IsNullOrEmpty(result.Error))
            {
                Log.LogError(result.Error);
            }

            if (result.ExitCode < 0)
            {
                Log.LogError(Resources.TaskFailedWithExitCode, nameof(NUnit), result.ExitCode);

                return;
            }

            if (result.ExitCode > 0)
            {
                if (FailOnFailedTests)
                {
                    Log.LogError(Resources.NUnit_FinishedWithFailedTests, nameof(NUnit), result.ExitCode);
                }
                else
                {
                    Log.LogWarning(Resources.NUnit_FinishedWithFailedTests, nameof(NUnit), result.ExitCode);
                }
            }

            // If tests result file was not specified, then there is nothing to parse.
            if (TestsResultFile == null)
            {
                return;
            }

            var testRun = XDocument.Load(TestsResultFile.ItemSpec).Element("test-run");

            TestCases = Convert.ToInt32(testRun.Attribute("testcasecount").Value, NumberFormatInfo.InvariantInfo);
            Total = Convert.ToInt32(testRun.Attribute("total").Value, NumberFormatInfo.InvariantInfo);
            Passed = Convert.ToInt32(testRun.Attribute("passed").Value, NumberFormatInfo.InvariantInfo);
            Failed = Convert.ToInt32(testRun.Attribute("failed").Value, NumberFormatInfo.InvariantInfo);
            Inconclusive = Convert.ToInt32(testRun.Attribute("inconclusive").Value, NumberFormatInfo.InvariantInfo);
            Skipped = Convert.ToInt32(testRun.Attribute("skipped").Value, NumberFormatInfo.InvariantInfo);
            Asserts = Convert.ToInt32(testRun.Attribute("asserts").Value, NumberFormatInfo.InvariantInfo);
            StartTime = testRun.Attribute("start-time").Value;
            EndTime = testRun.Attribute("end-time").Value;
            Duration = Convert.ToSingle(testRun.Attribute("duration").Value, CultureInfo.InvariantCulture);

            Log.LogMessage(Resources.NUnit_TestCases, TestCases);
            Log.LogMessage(Resources.NUnit_Total, Total);
            Log.LogMessage(Resources.NUnit_Passed, Passed);
            Log.LogMessage(Resources.NUnit_Failed, Failed);
            Log.LogMessage(Resources.NUnit_Inconclusive, Inconclusive);
            Log.LogMessage(Resources.NUnit_Skipped, Skipped);
            Log.LogMessage(Resources.NUnit_Asserts, Asserts);
            Log.LogMessage(Resources.NUnit_StartTime, StartTime);
            Log.LogMessage(Resources.NUnit_EndTime, EndTime);
            Log.LogMessage(Resources.NUnit_Duration, Duration);
        }

        #endregion
    }
}