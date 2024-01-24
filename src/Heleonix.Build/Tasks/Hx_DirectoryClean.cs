// <copyright file="Hx_DirectoryClean.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

/// <summary>
/// Deletes contents of a directory, but not the directory itself.
/// </summary>
public class Hx_DirectoryClean : BaseTask
{
    /// <summary>
    /// Directories to clean.
    /// </summary>
    [Required]
    public string[] Dirs { get; set; }

    /// <summary>
    /// The cleaned directories paths [Output].
    /// </summary>
    [Output]
    public string[] CleanedDirs { get; set; }

    /// <summary>
    /// Deletes contents of a directory, but not the directory itself.
    /// </summary>
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
