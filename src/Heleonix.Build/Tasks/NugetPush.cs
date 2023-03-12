// <copyright file="NugetPush.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

/// <summary>
/// Runs the Nuget push command.
/// </summary>
public class NugetPush : BaseTask
{
    /// <summary>
    /// Gets or sets the Nuget executable path.
    /// </summary>
    [Required]
    public string NugetExe { get; set; }

    /// <summary>
    /// Gets or sets the package file path.
    /// </summary>
    [Required]
    public string PackageFile { get; set; }

    /// <summary>
    /// Gets or sets the API key.
    /// </summary>
    public string APIKey { get; set; }

    /// <summary>
    /// Gets or sets the source path.
    /// </summary>
    public string SourceURL { get; set; }

    /// <summary>
    /// Gets or sets the configuration file path.
    /// </summary>
    public string ConfigFile { get; set; }

    /// <summary>
    /// Gets or sets the verbosity of the command.
    /// </summary>
    /// <remarks>
    /// Possible values:
    /// <list type="bullet">
    /// <item><term>normal</term></item>
    /// <item><term>quiet</term></item>
    /// <item><term>detailed</term></item>
    /// </list>
    /// </remarks>
    public string Verbosity { get; set; }

    /// <summary>
    /// Executes the Nuget "push" command.
    /// </summary>
    protected override void ExecuteInternal()
    {
        var args = ArgsBuilder.By("-", " ")
            .AddValue("push")
            .AddPath(this.PackageFile)
            .AddValue(this.APIKey)
            .AddPath("source", this.SourceURL)
            .AddPath($"ConfigFile", this.ConfigFile)
            .AddArgument($"Verbosity", this.Verbosity)
            .AddKey("NonInteractive");

        this.Log.LogMessage(Resources.NugetPush_Started, this.PackageFile);

        var result = ExeHelper.Execute(this.NugetExe, args, true);

        this.Log.LogMessage(result.Output);

        if (!string.IsNullOrEmpty(result.Error))
        {
            this.Log.LogError(result.Error);
        }

        if (result.ExitCode != 0)
        {
            this.Log.LogError(Resources.NugetPush_Failed, this.PackageFile, result.ExitCode);
        }
    }
}
