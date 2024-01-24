// <copyright file="Hx_FileUpdate.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

using System.Text.RegularExpressions;

/// <summary>
/// Updates a file with specified regular expressions and replacements.
/// </summary>
public class Hx_FileUpdate : BaseTask
{
    /// <summary>
    /// The file path.
    /// </summary>
    [Required]
    public string File { get; set; }

    /// <summary>
    /// The .NET regular expressions to find content to replace with values taken from the 'Replacement' metadata.
    /// </summary>
    [Required]
    public ITaskItem[] RegExps { get; set; }

    /// <summary>
    /// The .NET regular expression options. Default is "None".
    /// </summary>
    public string RegExpOptions { get; set; } = "None";

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

        var options = (RegexOptions)Enum.Parse(typeof(RegexOptions), this.RegExpOptions, true);

        this.Log.LogMessage(MessageImportance.High, Resources.FileUpdate_UpdatingFile, this.File);

        var input = System.IO.File.ReadAllText(this.File);

        var output = input;

        foreach (var regExp in this.RegExps)
        {
            output = Regex.Replace(input, regExp.ItemSpec, regExp.GetMetadata("Replacement"), options);
        }

        System.IO.File.WriteAllText(this.File, output);
    }
}
