// <copyright file="FileUpdate.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

using System.Text.RegularExpressions;

/// <summary>
/// Updates a file with specified regular expression and content.
/// </summary>
public class FileUpdate : BaseTask
{
    /// <summary>
    /// Gets or sets the file path.
    /// </summary>
    [Required]
    public string File { get; set; }

    /// <summary>
    /// Gets or sets the .NET regular expression to find content to replace.
    /// </summary>
    [Required]
    public string RegExp { get; set; }

    /// <summary>
    /// Gets or sets the .NET regular expression options.
    /// </summary>
    public string RegExpOptions { get; set; } = "None";

    /// <summary>
    /// Gets or sets content to replace with.
    /// </summary>
    public string Replacement { get; set; }

    /// <summary>
    /// Updates a file with specified regular expression and content.
    /// </summary>
    protected override void ExecuteInternal()
    {
        if (!System.IO.File.Exists(this.File))
        {
            this.Log.LogError(Resources.FileUpdate_FileNotFound, this.File);

            return;
        }

        var input = System.IO.File.ReadAllText(this.File);
        var regExpOptions = (RegexOptions)Enum.Parse(typeof(RegexOptions), this.RegExpOptions, true);

        var output = Regex.Replace(input, this.RegExp, this.Replacement ?? string.Empty, regExpOptions);

        this.Log.LogMessage(MessageImportance.High, Resources.FileUpdate_UpdatingFile, this.File);
        System.IO.File.WriteAllText(this.File, output);
    }
}
