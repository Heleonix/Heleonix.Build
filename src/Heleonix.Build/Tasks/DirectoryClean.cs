// <copyright file="DirectoryClean.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

/// <summary>
/// Deletes contents of a directory, but not the directory itself.
/// </summary>
public class DirectoryClean : BaseTask
{
    /// <summary>
    /// Gets or sets directories to clean.
    /// </summary>
    [Required]
    public ITaskItem[] Dirs { get; set; }

    /// <summary>
    /// Gets or sets the cleaned directories paths [Output].
    /// </summary>
    [Output]
    public ITaskItem[] CleanedDirs { get; set; }

    /// <summary>
    /// Deletes contents of a directory, but not the directory itself.
    /// </summary>
    protected override void ExecuteInternal()
    {
        var cleanedDirectoriesItems = new List<ITaskItem>();

        foreach (var dir in this.Dirs)
        {
            try
            {
                if (Directory.Exists(dir.ItemSpec))
                {
                    this.Log.LogMessage(Resources.DirectoryClean_CleaningDirectoryStarted, dir.ItemSpec);

                    foreach (var file in Directory.GetFiles(dir.ItemSpec))
                    {
                        File.Delete(file);
                    }

                    foreach (var directory in Directory.GetDirectories(dir.ItemSpec))
                    {
                        Directory.Delete(directory, true);
                    }

                    cleanedDirectoriesItems.Add(dir);
                }
                else
                {
                    this.Log.LogMessage(Resources.DirectoryClean_DirectoryNotFound, dir.ItemSpec);
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
