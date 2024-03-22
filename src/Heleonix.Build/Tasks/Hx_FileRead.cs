// <copyright file="Hx_FileRead.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

public class Hx_FileRead : BaseTask
{
    [Required]
    public string File { get; set; }

    [Required]
    public string RegExp { get; set; }

    public string RegExpOptions { get; set; } = "None";

    [Output]
    public ITaskItem[] Matches { get; set; }

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
