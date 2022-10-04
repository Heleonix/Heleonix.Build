// <copyright file="DirectoryClean.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Heleonix.Build.Properties;
    using Microsoft.Build.Framework;

    /// <summary>
    /// Deletes contents of a directory, but not the directory itself.
    /// </summary>
    public class DirectoryClean : BaseTask
    {
        /// <summary>
        /// Gets or sets directories to clean.
        /// </summary>
#pragma warning disable CA1819 // Properties should not return arrays
        [Required]
        public ITaskItem[] Dirs { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Gets or sets the cleaned directories paths [Output].
        /// </summary>
        [Output]
#pragma warning disable CA1819 // Properties should not return arrays
        public ITaskItem[] CleanedDirs { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

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
#pragma warning disable SG0018 // Path traversal
                            File.Delete(file);
#pragma warning restore SG0018 // Path traversal
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
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    this.Log.LogWarningFromException(ex);
                }
            }

            this.CleanedDirs = cleanedDirectoriesItems.ToArray();
        }
    }
}
