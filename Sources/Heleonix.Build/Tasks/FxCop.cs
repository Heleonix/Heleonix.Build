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
        /// Gets or sets the FxCop executable path.
        /// </summary>
        [Required]
        public ITaskItem FxCopExePath { get; set; }

        /// <summary>
        /// Gets or sets the analysis results file path.
        /// </summary>
        [Required]
        public ITaskItem AnalysisResultsFilePath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the FxCop is verbose.
        /// </summary>
        public bool IsVerbose { get; set; }

        /// <summary>
        /// Gets or sets the comma-separated types to analyze.
        /// </summary>
        /// <remarks>
        /// This option disables analysis of assemblies, namespaces, and resources;
        /// only the specified types and their members are included in the analysis.
        /// Can use the wildcard character '*' at the end of the name to select multiple types.
        /// </remarks>
        public string TargetsTypes { get; set; }

        /// <summary>
        /// Gets or sets the location of rule libraries to load. If you specify a directory,
        /// FxCop tries to load all files that have the .dll extension.
        /// </summary>
        public ITaskItem[] RulesPath { get; set; }

        /// <summary>
        /// Gets or sets the file path of an FxCop project file.
        /// </summary>
        public ITaskItem ProjectFilePath { get; set; }

        /// <summary>
        /// Gets or sets the files to analyze. If you specify a directory, FxCop tries to analyze all files
        /// that have the .exe or .dll extension.
        /// </summary>
        public ITaskItem[] TargetsPath { get; set; }

        /// <summary>
        /// Gets or sets the analysis results XSL file that is referenced in the processing instruction.
        /// </summary>
        public ITaskItem AnalysisResultsXslFilePath { get; set; }

        /// <summary>
        /// Gets or sets the dependencies directories paths to search for target assembly dependencies.
        /// </summary>
        public ITaskItem[] DependenciesDirectoriesPath { get; set; }

        /// <summary>
        /// Gets or sets the ruleset file path to be used for the analysis.
        /// It can be a file path to the rule set file or the file name of a built-in rule set.
        /// </summary>
        public ITaskItem RulesetFilePath { get; set; }

        /// <summary>
        /// Gets or sets the dictionary file path to be used by spelling rules.
        /// </summary>
        public ITaskItem DictionaryFilePath { get; set; }

        /// <summary>
        /// Gets or sets the critical errors count.
        /// </summary>
        [Output]
        public int CriticalErrors { get; set; }

        /// <summary>
        /// Gets or sets the errors count.
        /// </summary>
        [Output]
        public int Errors { get; set; }

        /// <summary>
        /// Gets or sets the critical warnings count.
        /// </summary>
        [Output]
        public int CriticalWarnings { get; set; }

        /// <summary>
        /// Gets or sets the warnings count.
        /// </summary>
        [Output]
        public int Warnings { get; set; }

        /// <summary>
        /// Gets or sets the informational messages count.
        /// </summary>
        [Output]
        public int Informational { get; set; }

        #endregion

        #region BaseTask Members

        /// <summary>
        /// Executes the FxCop.
        /// </summary>
        protected override void ExecuteInternal()
        {
            var tempAnalysisResultsFilePath = Path.Combine(Path.GetDirectoryName(AnalysisResultsFilePath.ItemSpec),
                Path.GetRandomFileName());

            var args = ArgsBuilder.By(' ', ':')
                .Add("/verbose", false, IsVerbose)
                .Add("/types", TargetsTypes)
                .Add("/project", ProjectFilePath?.ItemSpec, true)
                .Add("/rule", RulesPath?.Select(i => i.ItemSpec), true, ProjectFilePath == null)
                .Add("/file", TargetsPath?.Select(i => i.ItemSpec), true, ProjectFilePath == null)
                .Add("/out", tempAnalysisResultsFilePath, true)
                .Add("/directory", DependenciesDirectoriesPath?.Select(i => i.ItemSpec), true)
                .Add("/ignoregeneratedcode")
                .Add("/ruleSet", RulesetFilePath?.ItemSpec, true)
                .Add("/searchgac")
                .Add("/dictionary", DictionaryFilePath?.ItemSpec, true)
                .Add("/summary");

            // FxCopCmd does not create a directory for analysis results file.
            if (!Directory.Exists(Path.GetDirectoryName(AnalysisResultsFilePath.ItemSpec)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(AnalysisResultsFilePath.ItemSpec));
            }

            var exitCode = ExeHelper.Execute(FxCopExePath.ItemSpec, args);

            XDocument results = null;

            try
            {
                if (exitCode != 0)
                {
                    Log.LogError($"{nameof(FxCop)} failed. Exit code: {exitCode}.");

                    return;
                }

                //Treat absence of results as success.
                if (!File.Exists(tempAnalysisResultsFilePath))
                {
                    return;
                }

                results = XDocument.Load(tempAnalysisResultsFilePath);

                var issues = results.Descendants("Issue");

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

                if (CriticalErrors > 0 || Errors > 0)
                {
                    Log.LogError($"Critical errors: {CriticalErrors}; Errors:{Errors}.");
                }

                if (CriticalWarnings > 0 || Warnings > 0)
                {
                    Log.LogWarning($"Critical warnings: {CriticalWarnings}; Warnings:{Warnings}.");
                }

                if (Informational > 0)
                {
                    Log.LogMessage($"Informational: {Informational}.");
                }
            }
            finally
            {
                if (File.Exists(tempAnalysisResultsFilePath))
                {
                    //Overwrite existing report.
                    if (File.Exists(AnalysisResultsFilePath.ItemSpec))
                    {
                        File.Delete(AnalysisResultsFilePath.ItemSpec);
                    }

                    if (AnalysisResultsXslFilePath != null && File.Exists(AnalysisResultsXslFilePath.ItemSpec))
                    {
                        results = results ?? XDocument.Load(tempAnalysisResultsFilePath);

                        using (var outputStream = File.Create(AnalysisResultsFilePath.ItemSpec))
                        {
                            var transform = new XslCompiledTransform();

                            transform.Load(AnalysisResultsXslFilePath.ItemSpec);

                            transform.Transform(results.CreateNavigator(), null, outputStream);
                        }

                        File.Delete(tempAnalysisResultsFilePath);
                    }
                    else
                    {
                        File.Move(tempAnalysisResultsFilePath, AnalysisResultsFilePath.ItemSpec);
                    }
                }
            }
        }

        #endregion
    }
}