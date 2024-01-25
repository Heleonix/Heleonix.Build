// <copyright file="Hx_FileCopy.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

/// <summary>
/// Copies files from sources into destinations.
/// </summary>
public class Hx_FileCopy : BaseTask
{
    private const string WithSubDirsFromKey = "WithSubDirsFrom";

    /// <summary>
    /// The files to copy.
    /// </summary>
    /// <remarks>
    /// Metadata: 'WithSubDirsFrom' - if defined, copies a file into sub folders
    /// starting from the end of the defined root path. It is used to keep folders hierarchy.
    /// </remarks>
    /// <example>
    /// File path: C:\Files\SubDir1\SubDir2\SubDir3\file.txt <br/>
    /// WithSubDirsFrom: C:\Files\SubDir1 <br/>
    /// Destination: D:\Destination <br/>
    /// Result: file is copied into D:\Destination\SubDir2\SubDir3\file.txt.
    /// </example>
    [Required]
    public ITaskItem[] Files { get; set; }

    /// <summary>
    /// The destinations to copy files to.
    /// </summary>
    /// <remarks>
    /// If number of destinations equals to number of files, then files are copied into those directories.
    /// If destination is a single directory, then files are copied into that directory.
    /// </remarks>
    [Required]
    public string[] DestinationDirs { get; set; }

    /// <summary>
    /// A value indicating whether to overwrite destination file or ignore.
    /// </summary>
    public bool Overwrite { get; set; }

    /// <summary>
    /// A list of successfully copied files [Output].
    /// </summary>
    [Output]
    public string[] CopiedFiles { get; set; }

    /// <summary>
    /// Updates a file with specified regular expression and content.
    /// </summary>
    protected override void ExecuteInternal()
    {
        var copiedFiles = new List<string>();

        for (var i = 0; i < this.Files.Length; i++)
        {
            if (!File.Exists(this.Files[i].ItemSpec))
            {
                this.Log.LogMessage(MessageImportance.High, Resources.FileCopy_FileNotFound, this.Files[i].ItemSpec);

                continue;
            }

            try
            {
                var destinationPath = this.DestinationDirs.Length == 1
                    ? this.DestinationDirs[0]
                    : this.DestinationDirs[i];

                var filePath = new Uri(this.Files[i].ItemSpec).LocalPath.TrimEnd(Path.DirectorySeparatorChar);

                var subDirsFrom = this.Files[i].GetMetadata(WithSubDirsFromKey);

                if (!string.IsNullOrEmpty(subDirsFrom))
                {
                    subDirsFrom = new Uri(subDirsFrom).LocalPath.TrimEnd(Path.DirectorySeparatorChar);

                    if (filePath.StartsWith(subDirsFrom, StringComparison.OrdinalIgnoreCase))
                    {
                        destinationPath = Path.Combine(
                            destinationPath,
                            Path.GetDirectoryName(filePath).Replace(subDirsFrom, string.Empty).TrimStart(Path.DirectorySeparatorChar));
                    }
                    else
                    {
                        this.Log.LogWarning(Resources.FileCopy_WithSubDirsFromIsInvalid, subDirsFrom, filePath);

                        continue;
                    }
                }

                if (!Directory.Exists(destinationPath))
                {
                    Directory.CreateDirectory(destinationPath);
                }

                destinationPath = Path.Combine(destinationPath, Path.GetFileName(filePath));

                this.Log.LogMessage(MessageImportance.High, Resources.FileCopy_CopyingFile, filePath, destinationPath);
                File.Copy(filePath, destinationPath, this.Overwrite);
                copiedFiles.Add(destinationPath);
            }
            catch (Exception e)
            {
                this.Log.LogWarningFromException(e);
            }
        }

        this.CopiedFiles = copiedFiles.ToArray();
    }
}
