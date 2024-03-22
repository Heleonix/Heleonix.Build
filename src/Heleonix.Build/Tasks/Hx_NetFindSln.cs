// <copyright file="Hx_NetFindSln.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

public class Hx_NetFindSln : BaseTask
{
    [Required]
    public string StartDir { get; set; }

    [Output]
    public string SlnFile { get; set; }

    protected override void ExecuteInternal()
    {
        this.SlnFile = Directory.GetFiles(this.StartDir, "*.sln").SingleOrDefault();
    }
}
