// <copyright file="Hx_NetFindProjects.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

/// <summary>
/// Finds .NET projects files in a given solution files.
/// </summary>
public class Hx_NetFindProjects : BaseTask
{
    /// <summary>
    /// The path to the solution file to search projects files in.
    /// </summary>
    [Required]
    public string SlnFile { get; set; }

    /// <summary>
    /// Projects files found in the specified solution file [Output].
    /// </summary>
    [Output]
    public ITaskItem[] ProjectFiles { get; set; }

    /// <summary>
    /// Executes the implementation of the task.
    /// </summary>
    protected override void ExecuteInternal()
    {
        var slnContent = File.ReadAllText(this.SlnFile);
        var slnDir = Path.GetDirectoryName(this.SlnFile);
        var matches = Regex.Matches(
            slnContent,
            "(?<=Project.+=\\s*\".+\"\\s*,\\s*\")(.+proj)(?=\"\\s*,\\s*\".+\"\\s*EndProject)");

        this.ProjectFiles = matches.Select(m => new TaskItem(Path.Combine(slnDir, m.Value))).ToArray();
    }
}
