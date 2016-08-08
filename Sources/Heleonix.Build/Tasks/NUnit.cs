/*
The MIT License (MIT)

Copyright (c) 2015-2016 Heleonix - Hennadii Lutsyshyn

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
using System.Linq;
using System.Xml.Linq;
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
            return ArgsBuilder.By(' ', '=')
                .Add(item.GetMetadata(nameof(NUnitProjectOrTestsFilesPath)).Replace(";", " "), true)
                .Add("--result", item.GetMetadata(nameof(TestsResultsFilePath)), true)
                .Add("--noresult", false, item.GetMetadata(nameof(TestsResultsFilePath)) == null)
                .Add("--testlist", item.GetMetadata(nameof(TestsListFilePath)), true)
                .Add("--where", item.GetMetadata(nameof(TestsFilter)))
                .Add("--params", item.GetMetadata(nameof(TestsParameters)))
                .Add("--agents", item.GetMetadata(nameof(AgentsNumber)))
                .Add("--stoponerror", false, !string.IsNullOrEmpty(item.GetMetadata(nameof(StopOnErrorOrFailedTest))))
                .Add("--teamcity", false, !string.IsNullOrEmpty(item.GetMetadata(nameof(UseTeamCityServiceMessages))))
                .Add("--trace", item.GetMetadata(nameof(TraceLevel)))
                .Add("--output", item.GetMetadata(nameof(TestsOutputFilePath)), true)
                .Add("--err", item.GetMetadata(nameof(ErrorsFilePath)), true);
        }

        #endregion

        #region Properties

        /// <summary>
        /// The NUnit console executable path.
        /// </summary>
        [Required]
        public ITaskItem NUnitConsoleExePath { get; set; }

        /// <summary>
        /// The NUnit project or tests files path.
        /// </summary>
        [Required]
        public ITaskItem[] NUnitProjectOrTestsFilesPath { get; set; }

        /// <summary>
        /// The NUnit tests results file path.
        /// </summary>
        public ITaskItem TestsResultsFilePath { get; set; }

        /// <summary>
        /// The tests list file path.
        /// </summary>
        /// <remarks>
        /// File is containing a list of tests to run or explore, one per line.
        /// </remarks>
        public ITaskItem TestsListFilePath { get; set; }

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
        public ITaskItem TestsOutputFilePath { get; set; }

        /// <summary>
        /// The errors file path.
        /// </summary>
        public ITaskItem ErrorsFilePath { get; set; }

        /// <summary>
        /// The agents number.
        /// </summary>
        public int AgentsNumber { get; set; }

        /// <summary>
        /// A value indicating whether stop on first first error or first failed test.
        /// </summary>
        public bool StopOnErrorOrFailedTest { get; set; }

        /// <summary>
        /// A value indicating whether to use TeamCity service messages.
        /// </summary>
        public bool UseTeamCityServiceMessages { get; set; }

        /// <summary>
        /// The framework to use for tests. Examples: mono, net-4.5, v4.0, 2.0, mono-4.0.
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
        /// The count of test cases.
        /// </summary>
        [Output]
        public int TestCases { get; set; }

        /// <summary>
        /// The total count of tests.
        /// </summary>
        [Output]
        public int Total { get; set; }

        /// <summary>
        /// The count of passed tests.
        /// </summary>
        [Output]
        public int Passed { get; set; }

        /// <summary>
        /// The count of failed tests.
        /// </summary>
        [Output]
        public int Failed { get; set; }

        /// <summary>
        /// The inconclusive.
        /// </summary>
        [Output]
        public int Inconclusive { get; set; }

        /// <summary>
        /// The skipped.
        /// </summary>
        [Output]
        public int Skipped { get; set; }

        /// <summary>
        /// The asserts.
        /// </summary>
        [Output]
        public int Asserts { get; set; }

        /// <summary>
        /// The start time.
        /// </summary>
        [Output]
        public string StartTime { get; set; }

        /// <summary>
        /// The end time.
        /// </summary>
        [Output]
        public string EndTime { get; set; }

        /// <summary>
        /// The duration.
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
            var args = ArgsBuilder.By(' ', '=')
                .Add(NUnitProjectOrTestsFilesPath.Select(i => i.ItemSpec), true)
                .Add("--result", TestsResultsFilePath?.ItemSpec, true)
                .Add("--noresult", false, TestsResultsFilePath == null)
                .Add("--testlist", TestsListFilePath?.ItemSpec, true)
                .Add("--where", TestsFilter)
                .Add("--params", TestsParameters)
                .Add("--agents", AgentsNumber, false, AgentsNumber > 0)
                .Add("--stoponerror", false, StopOnErrorOrFailedTest)
                .Add("--teamcity", false, UseTeamCityServiceMessages)
                .Add("--trace", TraceLevel)
                .Add("--output", TestsOutputFilePath?.ItemSpec, true)
                .Add("--err", ErrorsFilePath?.ItemSpec, true)
                .Add("--framework", Framework);

            var exitCode = ExeHelper.Execute(NUnitConsoleExePath.ItemSpec, args);

            if (exitCode < 0)
            {
                Log.LogError($"{nameof(NUnit)} falied. Exit code: {exitCode}.");

                return;
            }

            if (exitCode > 0)
            {
                Log.LogError($"{nameof(NUnit)} finished with failed tests. Exit code: {exitCode}.");
            }

            // If test results file was not specified, then there is nothing to parse.
            if (TestsResultsFilePath == null)
            {
                return;
            }

            var testRun = XDocument.Load(TestsResultsFilePath.ItemSpec).Element("test-run");

            if (testRun == null)
            {
                Log.LogMessage("No tests were run.");

                return;
            }

            TestCases = Convert.ToInt32(testRun.Attribute("testcasecount").Value);
            Total = Convert.ToInt32(testRun.Attribute("total").Value);
            Passed = Convert.ToInt32(testRun.Attribute("passed").Value);
            Failed = Convert.ToInt32(testRun.Attribute("failed").Value);
            Inconclusive = Convert.ToInt32(testRun.Attribute("inconclusive").Value);
            Skipped = Convert.ToInt32(testRun.Attribute("skipped").Value);
            Asserts = Convert.ToInt32(testRun.Attribute("asserts").Value);
            StartTime = testRun.Attribute("start-time").Value;
            EndTime = testRun.Attribute("end-time").Value;
            Duration = Convert.ToSingle(testRun.Attribute("duration").Value, CultureInfo.InvariantCulture);

            Log.LogMessage($"Test cases: {TestCases}.");
            Log.LogMessage($"Total: {Total}.");
            Log.LogMessage($"Passed: {Passed}.");
            Log.LogMessage($"Failed: {Failed}.");
            Log.LogMessage($"Inconclusive: {Inconclusive}.");
            Log.LogMessage($"Skipped: {Skipped}.");
            Log.LogMessage($"Asserts: {Asserts}.");
            Log.LogMessage($"Start time: {StartTime}.");
            Log.LogMessage($"End time: {EndTime}.");
            Log.LogMessage($"Duration: {Duration}.");
        }

        #endregion
    }
}