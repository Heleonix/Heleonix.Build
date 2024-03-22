// <copyright file="Hx_DirectoryClean.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

public class Hx_DirectoryClean : BaseTask
{
    [Required]
    public string[] Dirs { get; set; }

    [Output]
    public string[] CleanedDirs { get; set; }

    protected override void ExecuteInternal()
    {
        var cleanedDirectoriesItems = new List<string>();

        foreach (var dir in this.Dirs)
        {
            try
            {
                if (Directory.Exists(dir))
                {
                    this.Log.LogMessage(MessageImportance.High, Resources.DirectoryClean_CleaningDirectoryStarted, dir);

                    foreach (var file in Directory.GetFiles(dir))
                    {
                        File.Delete(file);
                    }

                    foreach (var directory in Directory.GetDirectories(dir))
                    {
                        Directory.Delete(directory, true);
                    }

                    cleanedDirectoriesItems.Add(dir);
                }
                else
                {
                    this.Log.LogMessage(MessageImportance.High, Resources.DirectoryClean_DirectoryNotFound, dir);
                }
            }
            catch (Exception ex)
            {
                this.Log.LogWarningFromException(ex);
            }
        }

        this.CleanedDirs = cleanedDirectoriesItems.ToArray();
    }
}
