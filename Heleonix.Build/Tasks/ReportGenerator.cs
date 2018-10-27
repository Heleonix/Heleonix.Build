// <copyright file="ReportGenerator.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks
{
    using System.IO;
    using System.Linq;
    using Heleonix.Build.Properties;
    using Heleonix.Execution;
    using Microsoft.Build.Framework;

    /// <summary>
    /// Generates HTML report using the ReportGenerator tool.
    /// </summary>
    public class ReportGenerator : BaseTask
    {
        /// <summary>
        /// Gets or sets the ReportGenerator executable path.
        /// </summary>
        [Required]
        public ITaskItem ReportGeneratorExe { get; set; }

        /// <summary>
        /// Gets or sets the results files paths.
        /// </summary>
        [Required]
#pragma warning disable CA1819 // Properties should not return arrays
        public ITaskItem[] ResultFiles { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Gets or sets the report directory path to save report to.
        /// </summary>
        [Required]
        public ITaskItem ReportDir { get; set; }

        /// <summary>
        /// Gets or sets the report types, separated by semicolon.
        /// </summary>
        /// <remarks>
        /// Possible values:
        /// <list type="bullet">
        /// <item><term>Badges</term></item>
        /// <item><term>Html</term></item>
        /// <item><term>HtmlSummary</term></item>
        /// <item><term>Latex</term></item>
        /// <item><term>LatexSummary</term></item>
        /// <item><term>TextSummary</term></item>
        /// <item><term>Xml</term></item>
        /// <item><term>XmlSummary</term></item>
        /// </list>
        /// </remarks>
        [Required]
        public string ReportTypes { get; set; }

        /// <summary>
        /// Gets or sets the verbosity.
        /// </summary>
        /// <remarks>
        /// Possible values:
        /// <list type="bullet">
        /// <item><term>Error</term></item>
        /// <item><term>Info</term></item>
        /// <item><term>Verbose</term></item>
        /// </list>
        /// </remarks>
        public string Verbosity { get; set; }

        /// <summary>
        /// Executes the ReportUnit.
        /// </summary>
        protected override void ExecuteInternal()
        {
            var args = ArgsBuilder.By("-", ":")
                .AddPaths("reports", this.ResultFiles.Select(r => r.ItemSpec), false)
                .AddPath("targetdir", this.ReportDir.ItemSpec)
                .AddArgument("reporttypes", this.ReportTypes)
                .AddArgument("verbosity", this.Verbosity);

            Directory.CreateDirectory(this.ReportDir.ItemSpec);

            var result = ExeHelper.Execute(this.ReportGeneratorExe.ItemSpec, args, true);

            this.Log.LogMessage(result.Output);

            if (result.ExitCode != 0)
            {
                this.Log.LogError(
                    Resources.ReportGenerator_Failed,
                    nameof(ReportGenerator),
                    string.Join(";", this.ResultFiles.Select(r => r.ItemSpec)),
                    result.ExitCode);
            }
        }
    }
}
