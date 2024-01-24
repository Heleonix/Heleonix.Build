// <copyright file="Hx_FileRead.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

/// <summary>
/// Gets content from file by specified regular expression.
/// </summary>
public class Hx_FileRead : BaseTask
{
    /// <summary>
    /// The file path.
    /// </summary>
    [Required]
    public string File { get; set; }

    /// <summary>
    /// The .NET regular expression to find content.
    /// </summary>
    [Required]
    public string RegExp { get; set; }

    /// <summary>
    /// The .NET regular expression options. Default is 'None'.
    /// </summary>
    public string RegExpOptions { get; set; } = "None";

    /// <summary>
    /// The found matches. The Itemspec contains the File, the 'Match' metadata contains the matched value [Output].
    /// </summary>
    [Output]
    public ITaskItem[] Matches { get; set; }

    /// <summary>
    /// Reads a file with specified regular expression and content.
    /// </summary>
    protected override void ExecuteInternal()
    {
        if (!System.IO.File.Exists(this.File))
        {
            this.Log.LogError(Resources.FileRead_FileNotFound, this.File);

            this.Matches = Array.Empty<ITaskItem>();

            return;
        }

        var input = System.IO.File.ReadAllText(this.File);
        var regExpOptions = (RegexOptions)Enum.Parse(typeof(RegexOptions), this.RegExpOptions, true);

        var foundMatches = Regex.Matches(input, this.RegExp, regExpOptions);

        this.Matches = new ITaskItem[foundMatches.Count];

        for (var i = 0; i < foundMatches.Count; i++)
        {
            this.Matches[i] = new TaskItem(this.File);
            this.Matches[i].SetMetadata("Match", foundMatches[i].Value);
        }
    }
}
