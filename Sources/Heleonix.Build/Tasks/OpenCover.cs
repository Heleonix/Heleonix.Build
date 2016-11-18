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
    /// Runs the OpenCover.
    /// </summary>
    /// <seealso cref="BaseTask" />
    public class OpenCover : BaseTask
    {
        #region Properties

        /// <summary>
        /// The OpenCover executable path.
        /// </summary>
        [Required]
        public ITaskItem OpenCoverExeFile { get; set; }

        /// <summary>
        /// The target executable path with command line arguments and its type in metadata.
        /// </summary>
        [Required]
        public ITaskItem Target { get; set; }

        /// <summary>
        /// The coverage result output file path.
        /// </summary>
        [Required]
        public ITaskItem CoverageResultFile { get; set; }

        /// <summary>
        /// The minimum class coverage, in range: 0% - 100%.
        /// </summary>
        public float MinClassCoverage { get; set; }

        /// <summary>
        /// The minimum method coverage, in range: 0% - 100%.
        /// </summary>
        public float MinMethodCoverage { get; set; }

        /// <summary>
        /// The minimum branch coverage, in range: 0% - 100%.
        /// </summary>
        public float MinBranchCoverage { get; set; }

        /// <summary>
        /// The minimum line coverage, in range: 0% - 100%.
        /// </summary>
        public float MinLineCoverage { get; set; }

        /// <summary>
        /// The filters to exclude code from coverage by attribute in format: Name*;*Attribute.
        /// </summary>
        public string ExcludeByAttributeFilters { get; set; }

        /// <summary>
        /// The filters of binaries to cover in format: +[ModuleName*]*ClassName -[ModuleName*]*ClassName.
        /// </summary>
        public string Filters { get; set; }

        /// <summary>
        /// The PDB search directories path.
        /// </summary>
        public ITaskItem[] PdbSearchDirs { get; set; }

        /// <summary>
        /// Show unvisited methods and classes after coverage finishes and results are presented.
        /// </summary>
        public bool ShowUnvisited { get; set; }

        /// <summary>
        /// The maximum visits count. Limiting can improve performance.
        /// </summary>
        public int MaxVisitCount { get; set; }

        /// <summary>
        /// [Output] The total lines count.
        /// </summary>
        [Output]
        public long TotalLines { get; set; }

        /// <summary>
        /// [Output] The visited lines count.
        /// </summary>
        [Output]
        public long VisitedLines { get; set; }

        /// <summary>
        /// [Output] The total branches count.
        /// </summary>
        [Output]
        public long TotalBranches { get; set; }

        /// <summary>
        /// [Output] The visited branches count.
        /// </summary>
        [Output]
        public long VisitedBranches { get; set; }

        /// <summary>
        /// [Output] The total classes count.
        /// </summary>
        [Output]
        public long TotalClasses { get; set; }

        /// <summary>
        /// [Output] The visited classes count.
        /// </summary>
        [Output]
        public long VisitedClasses { get; set; }

        /// <summary>
        /// [Output] The total methods count.
        /// </summary>
        [Output]
        public long TotalMethods { get; set; }

        /// <summary>
        /// [Output] The visited methods count.
        /// </summary>
        [Output]
        public long VisitedMethods { get; set; }

        /// <summary>
        /// [Output] The minimum cyclomatic complexity.
        /// </summary>
        [Output]
        public int MinCyclomaticComplexity { get; set; }

        /// <summary>
        /// [Output] The maximum cyclomatic complexity.
        /// </summary>
        [Output]
        public int MaxCyclomaticComplexity { get; set; }

        /// <summary>
        /// [Output] The class coverage, in range: 0% - 100%.
        /// </summary>
        [Output]
        public float ClassCoverage { get; set; }

        /// <summary>
        /// [Output] The method coverage, in range: 0% - 100%.
        /// </summary>
        [Output]
        public float MethodCoverage { get; set; }

        /// <summary>
        /// [Output] The line coverage, in range: 0% - 100%.
        /// </summary>
        [Output]
        public float LineCoverage { get; set; }

        /// <summary>
        /// [Output] The branch coverage, in range: 0% - 100%.
        /// </summary>
        [Output]
        public float BranchCoverage { get; set; }

        #endregion

        #region BaseTask Members

        /// <summary>
        /// Executes the OpenCover.
        /// </summary>
        protected override void ExecuteInternal()
        {
            string targetArgs;

            if (Target.GetMetadata("Type") == "NUnit")
            {
                targetArgs = NUnit.BuildArgs(Target);

                NUnit.Prepare(Target);
            }
            else
            {
                Log.LogError($"The target type is not supported: '{Target.GetMetadata("Type")}'.");

                return;
            }

            var args = ArgsBuilder.By(' ', ':')
                .Add("-target", Target.ItemSpec, true)
                .Add("-targetargs", targetArgs.Replace("\"", "\\\""), true)
                .Add("-excludebyattribute", ExcludeByAttributeFilters, true)
                .Add("-filter", Filters, true)
                .Add("-mergebyhash")
                .Add("-output", CoverageResultFile.ItemSpec, true)
                .Add("-register", "path64")
                .Add("-skipautoprops")
                .Add("-searchdirs", "\\\"" + string.Join("\\\";", PdbSearchDirs?.Select(i => i.ItemSpec)
                                                                  ?? Enumerable.Empty<string>()) + "\\\"", true,
                    PdbSearchDirs != null)
                .Add("-showunvisited", false, ShowUnvisited)
                .Add("-returntargetcode")
                .Add("-threshold", MaxVisitCount, false, MaxVisitCount > 0);

            // OpenCover does not create a directory for coverage result file.
            if (!Directory.Exists(Path.GetDirectoryName(CoverageResultFile.ItemSpec) ?? string.Empty))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(CoverageResultFile.ItemSpec) ?? string.Empty);
            }

            string output;
            string error;

            var exitCode = ExeHelper.Execute(OpenCoverExeFile.ItemSpec, args, out output, out error);

            Log.LogMessage(output);

            if (!string.IsNullOrEmpty(error))
            {
                Log.LogError(error);
            }

            if (!File.Exists(CoverageResultFile.ItemSpec))
            {
                Log.LogError($"{nameof(OpenCover)} failed. Exit code: {exitCode}.");

                return;
            }

            if (exitCode != 0)
            {
                Log.LogWarning($"Target failed. Target's exit code: {exitCode}.");
            }

            var summary = XDocument.Load(CoverageResultFile.ItemSpec)
                .Element("CoverageSession")?
                .Element("Summary");

            if (summary == null)
            {
                Log.LogMessage("Summary was not found.");

                return;
            }

            TotalLines = Convert.ToInt64(summary.Attribute("numSequencePoints").Value);
            VisitedLines = Convert.ToInt64(summary.Attribute("visitedSequencePoints").Value);
            TotalBranches = Convert.ToInt64(summary.Attribute("numBranchPoints").Value);
            VisitedBranches = Convert.ToInt64(summary.Attribute("visitedBranchPoints").Value);
            TotalClasses = Convert.ToInt64(summary.Attribute("numClasses").Value);
            VisitedClasses = Convert.ToInt64(summary.Attribute("visitedClasses").Value);
            TotalMethods = Convert.ToInt64(summary.Attribute("numMethods").Value);
            VisitedMethods = Convert.ToInt64(summary.Attribute("visitedMethods").Value);
            MinCyclomaticComplexity = Convert.ToInt32(summary.Attribute("minCyclomaticComplexity").Value);
            MaxCyclomaticComplexity = Convert.ToInt32(summary.Attribute("maxCyclomaticComplexity").Value);
            ClassCoverage = (float) VisitedClasses/TotalClasses*100;
            MethodCoverage = (float) VisitedMethods/TotalMethods*100;
            LineCoverage = Convert.ToSingle(summary.Attribute("sequenceCoverage").Value, CultureInfo.InvariantCulture);
            BranchCoverage = Convert.ToSingle(summary.Attribute("branchCoverage").Value, CultureInfo.InvariantCulture);

            Log.LogMessage($"Total lines: {TotalLines}.");
            Log.LogMessage($"Visited lines: {VisitedLines}.");
            Log.LogMessage($"Total branches: {TotalBranches}.");
            Log.LogMessage($"Visited branches: {VisitedBranches}.");
            Log.LogMessage($"Total classes: {TotalClasses}.");
            Log.LogMessage($"Visited classes: {VisitedClasses}.");
            Log.LogMessage($"Total methods: {TotalMethods}.");
            Log.LogMessage($"Visited methods: {VisitedMethods}.");
            Log.LogMessage($"Minimum cyclomatic complexity: {MinCyclomaticComplexity}.");
            Log.LogMessage($"Maximum cyclomatic complexity: {MaxCyclomaticComplexity}.");
            Log.LogMessage($"Class coverage: {ClassCoverage}.");
            Log.LogMessage($"Method coverage: {MethodCoverage}.");
            Log.LogMessage($"Line coverage: {LineCoverage}.");
            Log.LogMessage($"Branch coverage: {BranchCoverage}.");

            if (ClassCoverage < MinClassCoverage)
            {
                Log.LogError("Minimum class coverage failed.");
            }

            if (MethodCoverage < MinMethodCoverage)
            {
                Log.LogError("Minimum method coverage failed.");
            }

            if (LineCoverage < MinLineCoverage)
            {
                Log.LogError("Minimum line coverage failed.");
            }

            if (BranchCoverage < MinBranchCoverage)
            {
                Log.LogError("Minimum branch coverage failed.");
            }
        }

        #endregion
    }
}