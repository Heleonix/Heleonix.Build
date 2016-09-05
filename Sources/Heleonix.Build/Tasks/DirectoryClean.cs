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
        public ITaskItem[] Dirs { get; set; }

        /// <summary>
        /// [Output] The cleaned directories paths.
        /// </summary>
        [Output]
        public ITaskItem[] CleanedDirs { get; set; }

        /// <summary>
        /// [Output] The failed to clean directories paths.
        /// </summary>
        [Output]
        public ITaskItem[] FailedDirs { get; set; }

        #endregion

        #region BaseTask Members

        /// <summary>
        /// Deletes contents of a directory, but not the directory itself.
        /// </summary>
        protected override void ExecuteInternal()
        {
            var cleanedDirectoriesItems = new List<ITaskItem>();
            var failedDirectoriesItems = new List<ITaskItem>();

            foreach (var dir in Dirs)
            {
                try
                {
                    if (Directory.Exists(dir.ItemSpec))
                    {
                        Log.LogMessage($"Cleaning directory '{dir.ItemSpec}' started.");

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
                        Log.LogMessage($"The directory '{dir.ItemSpec}' is not found. Skipping.");
                    }
                }
                catch (Exception ex)
                {
                    Log.LogErrorFromException(ex);

                    failedDirectoriesItems.Add(dir);
                }

                CleanedDirs = cleanedDirectoriesItems.ToArray();
                FailedDirs = failedDirectoriesItems.ToArray();
            }
        }

        #endregion
    }
}