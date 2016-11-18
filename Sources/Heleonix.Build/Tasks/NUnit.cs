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
using System.IO;
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
                .Add(item.GetMetadata(nameof(NUnitProjectOrTestsFiles)).Replace(";", "\" \""), true)
                .Add("--result", item.GetMetadata(nameof(TestsResultFile)), true)
                .Add("--noresult", false, item.GetMetadata(nameof(TestsResultFile)) == null)
                .Add("--testlist", item.GetMetadata(nameof(TestsListFile)), true)
                .Add("--where", item.GetMetadata(nameof(TestsFilter)))
                .Add("--params", item.GetMetadata(nameof(TestsParameters)))
                .Add("--agents", item.GetMetadata(nameof(AgentsNumber)))
                .Add("--stoponerror", false, item.GetMetadata(nameof(StopOnErrorOrFailedTest)) == "true")
                .Add("--teamcity", false, item.GetMetadata(nameof(UseTeamCityServiceMessages)) == "true")
                .Add("--trace", item.GetMetadata(nameof(TraceLevel)))
                .Add("--output", item.GetMetadata(nameof(TestsOutputFile)), true)
                .Add("--err", item.GetMetadata(nameof(ErrorsOutputFile)), true)
                .Add("--framework", item.GetMetadata(nameof(Framework)))
                .Add("--config", item.GetMetadata(nameof(Configuration)))
                .Add("--process", item.GetMetadata(nameof(ProcessIsolation)))
                .Add("--domain", item.GetMetadata(nameof(DomainIsolation)))
                .Add("--shadowcopy", false, item.GetMetadata(nameof(ShadowCopy)) == "true");
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
                !Directory.Exists(Path.GetDirectoryName(testsResultFile) ?? string.Empty))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(testsResultFile) ?? string.Empty);
            }

            // NUnit does not create a directory for tests output file.
            if (!string.IsNullOrEmpty(testsOutputFile) &&
                !Directory.Exists(Path.GetDirectoryName(testsOutputFile) ?? string.Empty))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(testsOutputFile) ?? string.Empty);
            }

            // NUnit does not create a directory for errors output file.
            if (!string.IsNullOrEmpty(errorsOutputFile) &&
                !Directory.Exists(Path.GetDirectoryName(errorsOutputFile) ?? string.Empty))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(errorsOutputFile) ?? string.Empty);
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
        public ITaskItem[] NUnitProjectOrTestsFiles { get; set; }

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
            var args = ArgsBuilder.By(' ', '=')
                .Add(NUnitProjectOrTestsFiles.Select(i => i.ItemSpec), true)
                .Add("--result", TestsResultFile?.ItemSpec, true)
                .Add("--noresult", false, TestsResultFile == null)
                .Add("--testlist", TestsListFile?.ItemSpec, true)
                .Add("--where", TestsFilter)
                .Add("--params", TestsParameters)
                .Add("--agents", AgentsNumber, false, AgentsNumber > 0)
                .Add("--stoponerror", false, StopOnErrorOrFailedTest)
                .Add("--teamcity", false, UseTeamCityServiceMessages)
                .Add("--trace", TraceLevel)
                .Add("--output", TestsOutputFile?.ItemSpec, true)
                .Add("--err", ErrorsOutputFile?.ItemSpec, true)
                .Add("--framework", Framework)
                .Add("--config", Configuration)
                .Add("--process", ProcessIsolation)
                .Add("--domain", DomainIsolation)
                .Add("--shadowcopy", false, ShadowCopy);

            Prepare(TestsResultFile?.ItemSpec, TestsOutputFile?.ItemSpec, ErrorsOutputFile?.ItemSpec);

            string output;
            string error;

            var exitCode = ExeHelper.Execute(NUnitConsoleExeFile.ItemSpec, args, out output, out error);

            Log.LogMessage(output);

            if (!string.IsNullOrEmpty(error))
            {
                Log.LogError(error);
            }

            if (exitCode < 0)
            {
                Log.LogError($"{nameof(NUnit)} falied. Exit code: {exitCode}.");

                return;
            }

            if (exitCode > 0)
            {
                var message = $"{nameof(NUnit)} finished with failed tests. Exit code: {exitCode}.";

                if (FailOnFailedTests)
                {
                    Log.LogError(message);
                }
                else
                {
                    Log.LogWarning(message);
                }
            }

            // If tests result file was not specified, then there is nothing to parse.
            if (TestsResultFile == null)
            {
                return;
            }

            var testRun = XDocument.Load(TestsResultFile.ItemSpec).Element("test-run");

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