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
        /// Gets or sets the OpenCover executable path.
        /// </summary>
        [Required]
        public ITaskItem OpenCoverExePath { get; set; }

        /// <summary>
        /// Gets or sets the type of the target. Currently only "NUnit" is supported.
        /// </summary>
        [Required]
        public string TargetType { get; set; }

        /// <summary>
        /// Gets or sets the target executable path with command line arguments in metadata.
        /// </summary>
        [Required]
        public ITaskItem TargetExe { get; set; }

        /// <summary>
        /// Gets or sets the output file path.
        /// </summary>
        [Required]
        public ITaskItem CoverageResultsFilePath { get; set; }

        /// <summary>
        /// Gets or sets the minimum class coverage, in range: 0% - 100%.
        /// </summary>
        public float MinClassCoverage { get; set; }

        /// <summary>
        /// Gets or sets the minimum method coverage, in range: 0% - 100%.
        /// </summary>
        public float MinMethodCoverage { get; set; }

        /// <summary>
        /// Gets or sets the minimum branch coverage, in range: 0% - 100%.
        /// </summary>
        public float MinBranchCoverage { get; set; }

        /// <summary>
        /// Gets or sets the minimum line coverage, in range: 0% - 100%.
        /// </summary>
        public float MinLineCoverage { get; set; }

        /// <summary>
        /// Gets or sets the filters to exclude code from coverage by attribute.
        /// </summary>
        /// <remarks>Format: Name*;*Attribute</remarks>
        public string ExcludeByAttributeFilters { get; set; }

        /// <summary>
        /// Gets or sets the filters of binaries to cover.
        /// </summary>
        /// <remarks>Format: +[ModuleName*]*ClassName -[ModuleName*]*ClassName</remarks>
        public string Filters { get; set; }

        /// <summary>
        /// Gets or sets the PDB search directories path.
        /// </summary>
        public ITaskItem[] PdbSearchDirectoriesPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show unvisited methods and classes
        /// after the coverage run is finished and the results are presented.
        /// </summary>
        public bool ShowUnvisited { get; set; }

        /// <summary>
        /// Gets or sets the maximum visits count.
        /// </summary>
        public int MaxVisitCount { get; set; }

        /// <summary>
        /// Gets or sets the total lines count.
        /// </summary>
        [Output]
        public long TotalLines { get; set; }

        /// <summary>
        /// Gets or sets the visited lines count.
        /// </summary>
        [Output]
        public long VisitedLines { get; set; }

        /// <summary>
        /// Gets or sets the total branches count.
        /// </summary>
        [Output]
        public long TotalBranches { get; set; }

        /// <summary>
        /// Gets or sets the visited branches count.
        /// </summary>
        [Output]
        public long VisitedBranches { get; set; }

        /// <summary>
        /// Gets or sets the total classes count.
        /// </summary>
        [Output]
        public long TotalClasses { get; set; }

        /// <summary>
        /// Gets or sets the visited classes count.
        /// </summary>
        [Output]
        public long VisitedClasses { get; set; }

        /// <summary>
        /// Gets or sets the total methods count.
        /// </summary>
        [Output]
        public long TotalMethods { get; set; }

        /// <summary>
        /// Gets or sets the visited methods count.
        /// </summary>
        [Output]
        public long VisitedMethods { get; set; }

        /// <summary>
        /// Gets or sets the minimum cyclomatic complexity.
        /// </summary>
        [Output]
        public int MinCyclomaticComplexity { get; set; }

        /// <summary>
        /// Gets or sets the maximum cyclomatic complexity.
        /// </summary>
        [Output]
        public int MaxCyclomaticComplexity { get; set; }

        /// <summary>
        /// Gets or sets the class coverage, in range: 0% - 100%..
        /// </summary>
        [Output]
        public float ClassCoverage { get; set; }

        /// <summary>
        /// Gets or sets the method coverage, in range: 0% - 100%..
        /// </summary>
        [Output]
        public float MethodCoverage { get; set; }

        /// <summary>
        /// Gets or sets the line coverage, in range: 0% - 100%..
        /// </summary>
        [Output]
        public float LineCoverage { get; set; }

        /// <summary>
        /// Gets or sets the branch coverage, in range: 0% - 100%..
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
            string targetArgs = null;

            if (string.Compare(TargetType, "NUnit", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                targetArgs = NUnit.BuildArgs(TargetExe);
            }
            else
            {
                Log.LogError($"The target type is not supported: '{TargetType}'.");

                return;
            }

            var args = ArgsBuilder.By(' ', ':')
                .Add("-target", TargetExe.ItemSpec, true)
                .Add("-targetargs", targetArgs.Replace("\"", "\\\""), true)
                .Add("-excludebyattribute", ExcludeByAttributeFilters, true)
                .Add("-filter", Filters, true)
                .Add("-mergebyhash")
                .Add("-output", CoverageResultsFilePath, true)
                .Add("-register", "user")
                .Add("-skipautoprops")
                .Add("-searchdirs", "\\\"" + string.Join("\\\";", PdbSearchDirectoriesPath?.Select(i => i.ItemSpec)
                                                                  ?? Enumerable.Empty<string>()) + "\\\"", true,
                    PdbSearchDirectoriesPath != null)
                .Add("-showunvisited", false, ShowUnvisited)
                .Add("-returntargetcode")
                .Add("-threshold", MaxVisitCount, false, MaxVisitCount > 0);

            var exitCode = ExeHelper.Execute(OpenCoverExePath.ItemSpec, args);

            if (!File.Exists(CoverageResultsFilePath.ItemSpec))
            {
                Log.LogError($"{nameof(OpenCover)} failed. Exit code: {exitCode}.");

                return;
            }

            if (exitCode != 0)
            {
                Log.LogWarning($"Target failed. Target's exit code: {exitCode}.");
            }

            var summary = XDocument.Load(CoverageResultsFilePath.ItemSpec).Element("CoverageSession").Element("Summary");

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