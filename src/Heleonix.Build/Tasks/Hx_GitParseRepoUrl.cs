// <copyright file="Hx_GitParseRepoUrl.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

public class Hx_GitParseRepoUrl : BaseTask
{
    [Required]
    public string RepositoryUrl { get; set; }

    [Output]
    public string OwnerName { get; set; }

    [Output]
    public string RepositoryName { get; set; }

    protected override void ExecuteInternal()
    {
        this.OwnerName = Regex.Match(this.RepositoryUrl, "[^/:]+(?=/[^/]+\\.git$)").Value;

        this.RepositoryName = Regex.Match(this.RepositoryUrl, "[^/]+(?=\\.git$)").Value;
    }
}
