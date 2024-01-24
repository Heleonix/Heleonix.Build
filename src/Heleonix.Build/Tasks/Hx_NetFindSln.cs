// <copyright file="Hx_NetFindSln.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

/// <summary>
/// Finds a solution file in the specified directory.
/// </summary>
public class Hx_NetFindSln : BaseTask
{
    /// <summary>
    /// A directory to start search in.
    /// </summary>
    [Required]
    public string StartDir { get; set; }

    /// <summary>
    /// The path to the fould solution file [Output].
    /// </summary>
    [Output]
    public string SlnFile { get; set; }

    /// <summary>
    /// Executes the implementation of the task.
    /// </summary>
    protected override void ExecuteInternal()
    {
        this.SlnFile = Directory.GetFiles(this.StartDir, "*.sln").Single();
    }
}
