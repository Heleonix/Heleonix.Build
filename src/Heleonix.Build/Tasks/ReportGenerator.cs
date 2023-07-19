// <copyright file="ReportGenerator.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

/// <summary>
/// Generates HTML report using the ReportGenerator tool.
/// </summary>
public class ReportGenerator : BaseTask
{
    /// <summary>
    /// Gets or sets the ReportGenerator executable path.
    /// </summary>
    [Required]
    public string ReportGeneratorExe { get; set; }

    /// <summary>
    /// Gets or sets the results files paths.
    /// </summary>
    [Required]
    public ITaskItem[] ResultFiles { get; set; }

    /// <summary>
    /// Gets or sets the report directory path to save report to.
    /// </summary>
    [Required]
    public string ReportDir { get; set; }

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
    /// Gets or sets the working directory to run the executable in.
    /// </summary>
    public string WorkingDir { get; set; } = string.Empty;

    /// <summary>
    /// Executes the ReportUnit.
    /// </summary>
    protected override void ExecuteInternal()
    {
        var args = ArgsBuilder.By("-", ":")
            .AddPaths("reports", this.ResultFiles.Select(r => r.ItemSpec), false)
            .AddPath("targetdir", this.ReportDir)
            .AddArgument("reporttypes", this.ReportTypes)
            .AddArgument("verbosity", this.Verbosity);

        Directory.CreateDirectory(this.ReportDir);

        var result = ExeHelper.Execute(this.ReportGeneratorExe, args, true, this.WorkingDir);

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
