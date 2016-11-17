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
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;
using Microsoft.Build.Framework;

namespace Heleonix.Build.Tasks
{
    /// <summary>
    /// Runs the FxCop.
    /// </summary>
    public class FxCop : BaseTask
    {
        #region Properties

        /// <summary>
        /// The FxCop command file path.
        /// </summary>
        [Required]
        public ITaskItem FxCopCmdFile { get; set; }

        /// <summary>
        /// The analysis result file path.
        /// </summary>
        [Required]
        public ITaskItem AnalysisResultFile { get; set; }

        /// <summary>
        /// A value indicating whether the FxCop is verbose.
        /// </summary>
        public bool IsVerbose { get; set; }

        /// <summary>
        /// The comma-separated types to analyze.
        /// </summary>
        /// <remarks>
        /// This option disables analysis of assemblies, namespaces, and resources;
        /// only the specified types and their members are included in the analysis.
        /// Can use the wildcard character '*' at the end of the name to select multiple types.
        /// </remarks>
        public string TargetsTypes { get; set; }

        /// <summary>
        /// Locations of rule libraries to load.
        /// </summary>
        /// <remarks>
        /// If you specify a directory, FxCop tries to load all files that have the .dll extension.
        /// </remarks>
        public ITaskItem[] RulesFilesDirs { get; set; }

        /// <summary>
        /// The file path of an FxCop project file.
        /// </summary>
        public ITaskItem ProjectFile { get; set; }

        /// <summary>
        /// The files to analyze.
        /// </summary>
        /// <remarks>
        /// If you specify a directory, FxCop tries to analyze all files that have the .exe or .dll extension.
        /// </remarks>
        public ITaskItem[] TargetsFilesDirs { get; set; }

        /// <summary>
        /// The analysis results XSL file that is referenced in the processing instruction.
        /// </summary>
        public ITaskItem AnalysisResultsXslFile { get; set; }

        /// <summary>
        /// The dependencies directories paths to search for target assembly dependencies.
        /// </summary>
        public ITaskItem[] DependenciesDirs { get; set; }

        /// <summary>
        /// The ruleset file path to be used for the analysis.
        /// </summary>
        /// <remarks>
        /// It can be a file path to the rule set file or the file name of a built-in rule set.
        /// </remarks>
        public ITaskItem RulesetFile { get; set; }

        /// <summary>
        /// The dictionary file path to be used by spelling rules.
        /// </summary>
        public ITaskItem DictionaryFile { get; set; }

        /// <summary>
        /// Types of issues to fail the task.
        /// </summary>
        /// <remarks>
        /// Possible values:
        /// <list type="bullet">
        /// <item><term>None</term></item>
        /// <item><term>Any</term></item>
        /// <item><term>CriticalErrors</term></item>
        /// <item><term>Errors</term></item>
        /// <item><term>CriticalWarnings</term></item>
        /// <item><term>Warnings</term></item>
        /// <item><term>Informational</term></item>
        /// </list>
        /// Default is "Any".
        /// </remarks>
        public string FailOn { get; set; }

        /// <summary>
        /// [Output] The critical errors count.
        /// </summary>
        [Output]
        public int CriticalErrors { get; set; }

        /// <summary>
        /// [Output] The errors count.
        /// </summary>
        [Output]
        public int Errors { get; set; }

        /// <summary>
        /// [Output] The critical warnings count.
        /// </summary>
        [Output]
        public int CriticalWarnings { get; set; }

        /// <summary>
        /// [Output] The warnings count.
        /// </summary>
        [Output]
        public int Warnings { get; set; }

        /// <summary>
        /// [Output] The informational messages count.
        /// </summary>
        [Output]
        public int Informational { get; set; }

        #endregion

        #region Enums

        /// <summary>
        /// Defines types of issues.
        /// </summary>
        [Flags]
        public enum IssueTypes
        {
            /// <summary>
            /// No issues.
            /// </summary>
            None = 1,

            /// <summary>
            /// The critical errors.
            /// </summary>
            CriticalErrors = 2,

            /// <summary>
            /// The errors.
            /// </summary>
            Errors = 4,

            /// <summary>
            /// The critical warnings.
            /// </summary>
            CriticalWarnings = 8,

            /// <summary>
            /// The warnings.
            /// </summary>
            Warnings = 16,

            /// <summary>
            /// The informational.
            /// </summary>
            Informational = 32,

            /// <summary>
            /// Any type of the issue.
            /// </summary>
            Any = CriticalErrors | Errors | CriticalWarnings | Warnings | Informational
        }

        #endregion

        #region BaseTask Members

        /// <summary>
        /// Executes the FxCop.
        /// </summary>
        protected override void ExecuteInternal()
        {
            var tempAnalysisResults = Path.Combine(
                Path.GetDirectoryName(AnalysisResultFile.ItemSpec) ?? string.Empty, Path.GetRandomFileName());

            var args = ArgsBuilder.By(' ', ':')
                .Add("/verbose", false, IsVerbose)
                .Add("/types", TargetsTypes)
                .Add("/project", ProjectFile?.ItemSpec, true)
                .Add("/rule", RulesFilesDirs?.Select(i => i.ItemSpec), true, ProjectFile == null)
                .Add("/file", TargetsFilesDirs?.Select(i => i.ItemSpec), true, ProjectFile == null)
                .Add("/out", tempAnalysisResults, true)
                .Add("/directory", DependenciesDirs?.Select(i => i.ItemSpec), true)
                .Add("/ignoregeneratedcode")
                .Add("/ruleSet", RulesetFile?.ItemSpec, true)
                .Add("/searchgac")
                .Add("/dictionary", DictionaryFile?.ItemSpec, true)
                .Add("/summary");

            // FxCopCmd does not create a directory for analysis result file.
            if (!Directory.Exists(Path.GetDirectoryName(AnalysisResultFile.ItemSpec) ?? string.Empty))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(AnalysisResultFile.ItemSpec) ?? string.Empty);
            }

            string output;
            string error;

            var exitCode = ExeHelper.Execute(FxCopCmdFile.ItemSpec, args, out output, out error);

            Log.LogMessage(output);

            if (!string.IsNullOrEmpty(error))
            {
                Log.LogError(error);
            }

            XDocument results = null;

            try
            {
                if (exitCode != 0)
                {
                    Log.LogError($"{nameof(FxCop)} failed. Exit code: {exitCode}.");

                    return;
                }

                //Treat absence of results as success.
                if (!File.Exists(tempAnalysisResults))
                {
                    return;
                }

                results = XDocument.Load(tempAnalysisResults);

                var issues = results.Descendants("Issue").ToArray();

                CriticalErrors = issues.Count(i => string.Equals(i.Attribute("Level").Value, "CriticalError",
                    StringComparison.InvariantCultureIgnoreCase));
                Errors = issues.Count(i => string.Equals(i.Attribute("Level").Value, "Error",
                    StringComparison.InvariantCultureIgnoreCase));
                CriticalWarnings = issues.Count(i => string.Equals(i.Attribute("Level").Value, "CriticalWarning",
                    StringComparison.InvariantCultureIgnoreCase));
                Warnings = issues.Count(i => string.Equals(i.Attribute("Level").Value, "Warning",
                    StringComparison.InvariantCultureIgnoreCase));
                Informational = issues.Count(i => string.Equals(i.Attribute("Level").Value, "Informational",
                    StringComparison.InvariantCultureIgnoreCase));

                Log.LogMessage($"Critical errors: {CriticalErrors}.");
                Log.LogMessage($"Errors: {Errors}.");
                Log.LogMessage($"Critical warnings: {CriticalWarnings}.");
                Log.LogMessage($"Warnings: {Warnings}.");
                Log.LogMessage($"Informational: {Informational}.");

                var failOn = string.IsNullOrEmpty(FailOn)
                    ? IssueTypes.Any
                    : (IssueTypes) Enum.Parse(typeof(IssueTypes), FailOn);

                if (failOn.HasFlag(IssueTypes.None))
                {
                    return;
                }

                if (failOn.HasFlag(IssueTypes.CriticalErrors) && CriticalErrors > 0)
                {
                    Log.LogError("The task failed due to critical errors.");
                }
                if (failOn.HasFlag(IssueTypes.Errors) && Errors > 0)
                {
                    Log.LogError("The task failed due to errors.");
                }
                if (failOn.HasFlag(IssueTypes.CriticalWarnings) && CriticalWarnings > 0)
                {
                    Log.LogError("The task failed due to critical warnings.");
                }
                if (failOn.HasFlag(IssueTypes.Warnings) && Warnings > 0)
                {
                    Log.LogError("The task failed due to warnings.");
                }
                if (failOn.HasFlag(IssueTypes.Informational) && Informational > 0)
                {
                    Log.LogError("The task failed due to informational.");
                }
            }
            finally
            {
                if (File.Exists(tempAnalysisResults))
                {
                    // Overwrite existing report.
                    if (File.Exists(AnalysisResultFile.ItemSpec))
                    {
                        File.Delete(AnalysisResultFile.ItemSpec);
                    }

                    if (AnalysisResultsXslFile != null && File.Exists(AnalysisResultsXslFile.ItemSpec))
                    {
                        results = results ?? XDocument.Load(tempAnalysisResults);

                        using (var outputStream = File.Create(AnalysisResultFile.ItemSpec))
                        {
                            var transform = new XslCompiledTransform();

                            transform.Load(AnalysisResultsXslFile.ItemSpec);

                            transform.Transform(results.CreateNavigator(), null, outputStream);
                        }

                        File.Delete(tempAnalysisResults);
                    }
                    else
                    {
                        File.Move(tempAnalysisResults, AnalysisResultFile.ItemSpec);
                    }
                }
            }
        }

        #endregion
    }
}