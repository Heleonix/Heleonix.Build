// <copyright file="OpenCover.cs" company="Heleonix - Hennadii Lutsyshyn">
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
    /// Runs the OpenCover.
    /// </summary>
    /// <seealso cref="BaseTask" />
    public class OpenCover : BaseTask
    {
        /// <summary>
        /// Gets or sets the OpenCover executable path.
        /// </summary>
        [Required]
        public ITaskItem OpenCoverExe { get; set; }

        /// <summary>
        /// Gets or sets the target executable path with command line arguments and its type in metadata.
        /// </summary>
        [Required]
        public ITaskItem Target { get; set; }

        /// <summary>
        /// Gets or sets the coverage result output file path.
        /// </summary>
        [Required]
        public ITaskItem CoverageResultFile { get; set; }

#pragma warning disable CA1819 // Properties should not return arrays
        /// <summary>
        /// Gets or sets the PDB search directories path.
        /// </summary>
        [Required]
        public ITaskItem[] PdbSearchDirs { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Gets or sets the minimum class coverage, in range: 0% - 100%.
        /// </summary>
        public int MinClassCoverage { get; set; }

        /// <summary>
        /// Gets or sets the minimum method coverage, in range: 0% - 100%.
        /// </summary>
        public int MinMethodCoverage { get; set; }

        /// <summary>
        /// Gets or sets the minimum branch coverage, in range: 0% - 100%.
        /// </summary>
        public int MinBranchCoverage { get; set; }

        /// <summary>
        /// Gets or sets the minimum line coverage, in range: 0% - 100%.
        /// </summary>
        public int MinLineCoverage { get; set; }

        /// <summary>
        /// Gets or sets the filters to exclude code from coverage by attribute in format: Name*;*Attribute.
        /// </summary>
        public string ExcludeByAttributeFilters { get; set; }

        /// <summary>
        /// Gets or sets the filters of binaries to cover in format: +[ModuleName*]*ClassName -[ModuleName*]*ClassName.
        /// </summary>
        public string Filters { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether show unvisited methods and classes after coverage finishes and results are presented.
        /// </summary>
        public bool ShowUnvisited { get; set; }

        /// <summary>
        /// Gets or sets the maximum visits count. Limiting can improve performance.
        /// </summary>
        public int MaxVisitCount { get; set; }

        /// <summary>
        /// Gets or sets the type of registration of the OpenCover profiler.
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
        /// Gets or sets the total lines count [Output].
        /// </summary>
        [Output]
        public long TotalLines { get; set; }

        /// <summary>
        /// Gets or sets the visited lines count [Output].
        /// </summary>
        [Output]
        public long VisitedLines { get; set; }

        /// <summary>
        /// Gets or sets the total branches count [Output].
        /// </summary>
        [Output]
        public long TotalBranches { get; set; }

        /// <summary>
        /// Gets or sets the visited branches count [Output].
        /// </summary>
        [Output]
        public long VisitedBranches { get; set; }

        /// <summary>
        /// Gets or sets the total classes count [Output].
        /// </summary>
        [Output]
        public long TotalClasses { get; set; }

        /// <summary>
        /// Gets or sets the visited classes count [Output].
        /// </summary>
        [Output]
        public long VisitedClasses { get; set; }

        /// <summary>
        /// Gets or sets the total methods count [Output].
        /// </summary>
        [Output]
        public long TotalMethods { get; set; }

        /// <summary>
        /// Gets or sets the visited methods count [Output].
        /// </summary>
        [Output]
        public long VisitedMethods { get; set; }

        /// <summary>
        /// Gets or sets the minimum cyclomatic complexity [Output].
        /// </summary>
        [Output]
        public int MinCyclomaticComplexity { get; set; }

        /// <summary>
        /// Gets or sets the maximum cyclomatic complexity [Output].
        /// </summary>
        [Output]
        public int MaxCyclomaticComplexity { get; set; }

        /// <summary>
        /// Gets or sets the class coverage, in range: 0% - 100% [Output].
        /// </summary>
        [Output]
        public int ClassCoverage { get; set; }

        /// <summary>
        /// Gets or sets the method coverage, in range: 0% - 100% [Output].
        /// </summary>
        [Output]
        public int MethodCoverage { get; set; }

        /// <summary>
        /// Gets or sets the line coverage, in range: 0% - 100% [Output].
        /// </summary>
        [Output]
        public int LineCoverage { get; set; }

        /// <summary>
        /// Gets or sets the branch coverage, in range: 0% - 100% [Output].
        /// </summary>
        [Output]
        public int BranchCoverage { get; set; }

        /// <summary>
        /// Executes the OpenCover.
        /// </summary>
        protected override void ExecuteInternal()
        {
            string targetArgs;

            if (this.Target.GetMetadata("Type") == nameof(NUnit))
            {
                NUnit.Prepare(this.Target);

                targetArgs = NUnit.BuildArgs(this.Target);
            }
            else
            {
                this.Log.LogError(Resources.OpenCover_TargetTypeIsNotRecognized, this.Target.GetMetadata("Type"));

                return;
            }

            var args = ArgsBuilder.By("-", ":")
                .AddPath("target", this.Target.ItemSpec)
                .AddPath("targetargs", targetArgs.Replace("\"", "\\\""))
                .AddPath("excludebyattribute", this.ExcludeByAttributeFilters)
                .AddPath("filter", this.Filters)
                .AddKey("mergebyhash")
                .AddPath("output", this.CoverageResultFile.ItemSpec)
                .AddKey("skipautoprops")
                .AddPaths(
                    "searchdirs",
                    this.PdbSearchDirs.Select(i => i.ItemSpec),
                    false,
                    this.PdbSearchDirs.Length > 0)
                .AddKey("showunvisited", this.ShowUnvisited)
                .AddKey("returntargetcode")
                .AddArgument("threshold", this.MaxVisitCount, this.MaxVisitCount > 0)
                .AddKey("register", this.Register == "auto")
                .AddArgument("register", this.Register, this.Register != "auto");

            var coverageResultDir = Path.GetDirectoryName(this.CoverageResultFile.ItemSpec);

            // OpenCover does not create a directory for coverage result file.
            Directory.CreateDirectory(coverageResultDir);

            var result = ExeHelper.Execute(this.OpenCoverExe.ItemSpec, args, true);

            this.Log.LogMessage(result.Output);

            if (!File.Exists(this.CoverageResultFile.ItemSpec))
            {
                this.Log.LogError(Resources.TaskFailedWithExitCode, nameof(OpenCover), result.ExitCode);

                return;
            }

            if (result.ExitCode != 0)
            {
                this.Log.LogWarning(Resources.OpenCover_TargetFailed, result.ExitCode);
            }

            var summary = XDocument.Load(this.CoverageResultFile.ItemSpec).Element("CoverageSession").Element("Summary");

            this.TotalLines = Convert.ToInt64(
                summary.Attribute("numSequencePoints").Value,
                NumberFormatInfo.InvariantInfo);
            this.VisitedLines = Convert.ToInt64(
                summary.Attribute("visitedSequencePoints").Value,
                NumberFormatInfo.InvariantInfo);
            this.TotalBranches = Convert.ToInt64(
                summary.Attribute("numBranchPoints").Value,
                NumberFormatInfo.InvariantInfo);
            this.VisitedBranches = Convert.ToInt64(
                summary.Attribute("visitedBranchPoints").Value,
                NumberFormatInfo.InvariantInfo);
            this.TotalClasses = Convert.ToInt64(
                summary.Attribute("numClasses").Value,
                NumberFormatInfo.InvariantInfo);
            this.VisitedClasses = Convert.ToInt64(
                summary.Attribute("visitedClasses").Value,
                NumberFormatInfo.InvariantInfo);
            this.TotalMethods = Convert.ToInt64(
                summary.Attribute("numMethods").Value,
                NumberFormatInfo.InvariantInfo);
            this.VisitedMethods = Convert.ToInt64(
                summary.Attribute("visitedMethods").Value,
                NumberFormatInfo.InvariantInfo);
            this.MinCyclomaticComplexity = Convert.ToInt32(
                summary.Attribute("minCyclomaticComplexity").Value,
                NumberFormatInfo.InvariantInfo);
            this.MaxCyclomaticComplexity = Convert.ToInt32(
                summary.Attribute("maxCyclomaticComplexity").Value,
                NumberFormatInfo.InvariantInfo);
            this.ClassCoverage = (int)((float)this.VisitedClasses / this.TotalClasses * 100);
            this.MethodCoverage = (int)((float)this.VisitedMethods / this.TotalMethods * 100);
            this.LineCoverage = (int)Convert.ToSingle(
                summary.Attribute("sequenceCoverage").Value,
                NumberFormatInfo.InvariantInfo);
            this.BranchCoverage = (int)Convert.ToSingle(
                summary.Attribute("branchCoverage").Value,
                NumberFormatInfo.InvariantInfo);

            this.Log.LogMessage(Resources.OpenCover_TotalLines, this.TotalLines);
            this.Log.LogMessage(Resources.OpenCover_VisitedLines, this.VisitedLines);
            this.Log.LogMessage(Resources.OpenCover_TotalBranches, this.TotalBranches);
            this.Log.LogMessage(Resources.OpenCover_VisitedBranches, this.VisitedBranches);
            this.Log.LogMessage(Resources.OpenCover_TotalClasses, this.TotalClasses);
            this.Log.LogMessage(Resources.OpenCover_VisitedClasses, this.VisitedClasses);
            this.Log.LogMessage(Resources.OpenCover_TotalMethods, this.TotalMethods);
            this.Log.LogMessage(Resources.OpenCover_VisitedMethods, this.VisitedMethods);
            this.Log.LogMessage(Resources.OpenCover_MinCyclomaticComplexity, this.MinCyclomaticComplexity);
            this.Log.LogMessage(Resources.OpenCover_MaxCyclomaticComplexity, this.MaxCyclomaticComplexity);
            this.Log.LogMessage(Resources.OpenCover_ClassCoverage, this.ClassCoverage);
            this.Log.LogMessage(Resources.OpenCover_MethodCoverage, this.MethodCoverage);
            this.Log.LogMessage(Resources.OpenCover_LineCoverage, this.LineCoverage);
            this.Log.LogMessage(Resources.OpenCover_BranchCoverage, this.BranchCoverage);

            if (this.ClassCoverage < this.MinClassCoverage)
            {
                this.Log.LogError(Resources.OpenCover_MinClassCoverageFailed);
            }

            if (this.MethodCoverage < this.MinMethodCoverage)
            {
                this.Log.LogError(Resources.OpenCover_MinMethodCoverageFailed);
            }

            if (this.LineCoverage < this.MinLineCoverage)
            {
                this.Log.LogError(Resources.OpenCover_MinLineCoverageFailed);
            }

            if (this.BranchCoverage < this.MinBranchCoverage)
            {
                this.Log.LogError(Resources.OpenCover_MinBranchCoverageFailed);
            }
        }
    }
}
