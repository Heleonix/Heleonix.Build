// <copyright file="Hx_GitParseRepoUrl.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

/// <summary>
/// Extracts owner and repository name from the repository url.
/// </summary>
public class Hx_GitParseRepoUrl : BaseTask
{
    /// <summary>
    /// A reporsitory url to parse,
    /// like https://github.com/Heleonix/Heleonix.Build.git or git@github.com:Heleonix/Heleonix.Build.git.
    /// </summary>
    [Required]
    public string RepositoryUrl { get; set; }

    /// <summary>
    /// The name of the owner extracted from the url [Output].
    /// </summary>
    [Output]
    public string OwnerName { get; set; }

    /// <summary>
    /// The name of the repository extracted from the url [Output].
    /// </summary>
    [Output]
    public string RepositoryName { get; set; }

    /// <summary>
    /// Executes the implementation of the task.
    /// </summary>
    protected override void ExecuteInternal()
    {
        this.OwnerName = Regex.Match(this.RepositoryUrl, "[^/:]+(?=/[^/]+\\.git$)").Value;

        this.RepositoryName = Regex.Match(this.RepositoryUrl, "[^/]+(?=\\.git$)").Value;
    }
}
