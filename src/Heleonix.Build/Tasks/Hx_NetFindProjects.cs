// <copyright file="Hx_NetFindProjects.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

public class Hx_NetFindProjects : BaseTask
{
    [Required]
    public string SlnFile { get; set; }

    [Output]
    public string[] ProjectFiles { get; set; }

    protected override void ExecuteInternal()
    {
        var slnContent = File.ReadAllText(this.SlnFile);
        var slnDir = Path.GetDirectoryName(this.SlnFile);
        var matches = Regex.Matches(
            slnContent,
            "(?<=Project.+=\\s*\".+\"\\s*,\\s*\")(.+proj)(?=\"\\s*,\\s*\".+\"\\s*EndProject)");

        this.ProjectFiles = matches.Select(m => Path.Combine(slnDir, m.Value)).ToArray();
    }
}
