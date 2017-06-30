/*
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
        /// The type of registration of the OpenCover profiler.
        /// </summary>
        /// <remarks>
        /// Possible values:
        /// <list type="bullet">
        /// <item><term>auto</term></item>
        /// <item><term>user</term></item>
        /// <item><term>path32</term></item>
        /// <item><term>path64</term></item>
        /// </list>
        /// </remarks>
        public string Register { get; set; }

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
                Log.LogError(Resources.OpenCover_TargetTypeIsNotRecognized, Target.GetMetadata("Type"));

                return;
            }

            var directorySeparatorReplacer = Path.DirectorySeparatorChar + "\"";

            var args = ArgsBuilder.By("-", ":")
                .AddPath("target", Target.ItemSpec)
                .AddPath("targetargs", targetArgs.Replace("\"", directorySeparatorReplacer))
                .AddPath("excludebyattribute", ExcludeByAttributeFilters)
                .AddPath("filter", Filters)
                .AddKey("mergebyhash")
                .AddPath("output", CoverageResultFile.ItemSpec)
                .AddKey("skipautoprops")
                .AddPath("searchdirs", PdbSearchDirs != null
                        ? directorySeparatorReplacer + string.Join(Path.DirectorySeparatorChar + "\";",
                              PdbSearchDirs.Select(i => i.ItemSpec)) + directorySeparatorReplacer
                        : null,
                    PdbSearchDirs != null)
                .AddKey("showunvisited", ShowUnvisited)
                .AddKey("returntargetcode")
                .AddArgument("threshold", MaxVisitCount, MaxVisitCount > 0)
                .AddKey("register", Register == "auto")
                .AddArgument("register", Register, Register != "auto");

            var coverageResultDir = Path.GetDirectoryName(CoverageResultFile.ItemSpec);

            // OpenCover does not create a directory for coverage result file.
            if (!Directory.Exists(coverageResultDir))
            {
                Directory.CreateDirectory(coverageResultDir);
            }

            var result = ExeHelper.Execute(OpenCoverExeFile.ItemSpec, args, true);

            Log.LogMessage(result.Output);

            if (!File.Exists(CoverageResultFile.ItemSpec))
            {
                Log.LogError(Resources.TaskFailedWithExitCode, nameof(OpenCover), result.ExitCode);

                return;
            }

            if (result.ExitCode != 0)
            {
                Log.LogWarning(Resources.OpenCover_TargetFailed, result.ExitCode);
            }

            var summary = XDocument.Load(CoverageResultFile.ItemSpec).Element("CoverageSession").Element("Summary");

            TotalLines = Convert.ToInt64(summary.Attribute("numSequencePoints").Value, NumberFormatInfo.InvariantInfo);
            VisitedLines = Convert.ToInt64(summary.Attribute("visitedSequencePoints").Value,
                NumberFormatInfo.InvariantInfo);
            TotalBranches = Convert.ToInt64(summary.Attribute("numBranchPoints").Value, NumberFormatInfo.InvariantInfo);
            VisitedBranches = Convert.ToInt64(summary.Attribute("visitedBranchPoints").Value,
                NumberFormatInfo.InvariantInfo);
            TotalClasses = Convert.ToInt64(summary.Attribute("numClasses").Value, NumberFormatInfo.InvariantInfo);
            VisitedClasses = Convert.ToInt64(summary.Attribute("visitedClasses").Value, NumberFormatInfo.InvariantInfo);
            TotalMethods = Convert.ToInt64(summary.Attribute("numMethods").Value, NumberFormatInfo.InvariantInfo);
            VisitedMethods = Convert.ToInt64(summary.Attribute("visitedMethods").Value, NumberFormatInfo.InvariantInfo);
            MinCyclomaticComplexity = Convert.ToInt32(summary.Attribute("minCyclomaticComplexity").Value,
                NumberFormatInfo.InvariantInfo);
            MaxCyclomaticComplexity = Convert.ToInt32(summary.Attribute("maxCyclomaticComplexity").Value,
                NumberFormatInfo.InvariantInfo);
            ClassCoverage = (float) VisitedClasses / TotalClasses * 100;
            MethodCoverage = (float) VisitedMethods / TotalMethods * 100;
            LineCoverage = Convert.ToSingle(summary.Attribute("sequenceCoverage").Value,
                NumberFormatInfo.InvariantInfo);
            BranchCoverage = Convert.ToSingle(summary.Attribute("branchCoverage").Value,
                NumberFormatInfo.InvariantInfo);

            Log.LogMessage(Resources.OpenCover_TotalLines, TotalLines);
            Log.LogMessage(Resources.OpenCover_VisitedLines, VisitedLines);
            Log.LogMessage(Resources.OpenCover_TotalBranches, TotalBranches);
            Log.LogMessage(Resources.OpenCover_VisitedBranches, VisitedBranches);
            Log.LogMessage(Resources.OpenCover_TotalClasses, TotalClasses);
            Log.LogMessage(Resources.OpenCover_VisitedClasses, VisitedClasses);
            Log.LogMessage(Resources.OpenCover_TotalMethods, TotalMethods);
            Log.LogMessage(Resources.OpenCover_VisitedMethods, VisitedMethods);
            Log.LogMessage(Resources.OpenCover_MinCyclomaticComplexity, MinCyclomaticComplexity);
            Log.LogMessage(Resources.OpenCover_MaxCyclomaticComplexity, MaxCyclomaticComplexity);
            Log.LogMessage(Resources.OpenCover_ClassCoverage, ClassCoverage);
            Log.LogMessage(Resources.OpenCover_MethodCoverage, MethodCoverage);
            Log.LogMessage(Resources.OpenCover_LineCoverage, LineCoverage);
            Log.LogMessage(Resources.OpenCover_BranchCoverage, BranchCoverage);

            if (ClassCoverage < MinClassCoverage)
            {
                Log.LogError(Resources.OpenCover_MinClassCoverageFailed);
            }

            if (MethodCoverage < MinMethodCoverage)
            {
                Log.LogError(Resources.OpenCover_MinMethodCoverageFailed);
            }

            if (LineCoverage < MinLineCoverage)
            {
                Log.LogError(Resources.OpenCover_MinLineCoverageFailed);
            }

            if (BranchCoverage < MinBranchCoverage)
            {
                Log.LogError(Resources.OpenCover_MinBranchCoverageFailed);
            }
        }

        #endregion
    }
}