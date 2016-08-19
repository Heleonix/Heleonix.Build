/*
The MIT License (MIT)

Copyright (c) 2015-2016 Heleonix - Hennadii Lutsyshyn

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;

namespace Heleonix.Build.Tasks
{
    /// <summary>
    /// Deletes contents of a directory, but not the directory itself.
    /// </summary>
    public class DirectoryClean : BaseTask
    {
        #region Properties

        /// <summary>
        /// Directories to clean.
        /// </summary>
        [Required]
        public ITaskItem[] DirectoriesPath { get; set; }

        /// <summary>
        /// Gets or sets the cleaned directories paths.
        /// </summary>
        [Output]
        public ITaskItem[] CleanedDirectoriesPath { get; set; }

        /// <summary>
        /// Gets or sets the failed to clean directories paths.
        /// </summary>
        [Output]
        public ITaskItem[] FailedDirectoriesPath { get; set; }

        #endregion

        #region BaseTask Members

        /// <summary>
        /// Deletes contents of a directory, but not the directory itself.
        /// </summary>
        protected override void ExecuteInternal()
        {
            var cleanedDirectoriesItems = new List<ITaskItem>();
            var failedDirectoriesItems = new List<ITaskItem>();

            foreach (var directoryItem in DirectoriesPath)
            {
                try
                {
                    if (Directory.Exists(directoryItem.ItemSpec))
                    {
                        foreach (var filePath in Directory.GetFiles(directoryItem.ItemSpec))
                        {
                            File.Delete(filePath);
                        }

                        foreach (var directoryPath in Directory.GetDirectories(directoryItem.ItemSpec))
                        {
                            Directory.Delete(directoryPath, true);
                        }

                        cleanedDirectoriesItems.Add(directoryItem);
                    }
                    else
                    {
                        Log.LogMessage($"The directory '{directoryItem.ItemSpec}' does not exist. Skipping.");
                    }
                }
                catch (Exception ex)
                {
                    Log.LogErrorFromException(ex);

                    failedDirectoriesItems.Add(directoryItem);
                }

                CleanedDirectoriesPath = cleanedDirectoriesItems.ToArray();
                FailedDirectoriesPath = failedDirectoriesItems.ToArray();
            }
        }

        #endregion
    }
}