// <copyright file="Hx_FileCopy.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

public class Hx_FileCopy : BaseTask
{
    private const string WithSubDirsFromKey = "WithSubDirsFrom";

    [Required]
    public ITaskItem[] Files { get; set; }

    [Required]
    public string[] DestinationDirs { get; set; }

    public bool Overwrite { get; set; }

    [Output]
    public string[] CopiedFiles { get; set; }

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
