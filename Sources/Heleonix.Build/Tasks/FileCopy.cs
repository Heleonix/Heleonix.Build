/*
The MIT License (MIT)

Copyright (c) 2015-present Heleonix - Hennadii Lutsyshyn

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
using System.Linq;
using Heleonix.Build.Properties;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Heleonix.Build.Tasks
{
    /// <summary>
    /// Copies files from sources into destinations.
    /// </summary>
    public class FileCopy : BaseTask
    {
        #region Constants

        private const string WithSubDirsFromMetadataKey = "WithSubDirsFrom";

        #endregion

        #region Properties

        /// <summary>
        /// Files to copy.
        /// </summary>
        /// <remarks>
        /// Metadata: 'WithSubDirsFrom' - if defined, copies a file into sub folders
        /// starting from the end of the defined root path.
        /// </remarks>
        /// <example>
        /// File path: C:\Files\SubDir1\SubDir2\SubDir3\file.txt
        /// WithSubDirsFrom: C:\Files\SubDir1
        /// Destination: D:\Destination
        /// Result: file is copied into D:\Destination\SubDir2\SubDir3\file.txt
        /// </example>
        public ITaskItem[] Files { get; set; }

        /// <summary>
        /// Destinations to copy files to.
        /// </summary>
        /// <remarks>
        /// If number of destinations equals to number of files, then copying proceeds as follows:
        /// if destinations are files, then sources are copied as destination files;
        /// if destinations are directories, then files are copied into those directories.
        /// If destination is a single directory, then files are copied into that directory.
        /// Otherwise tast is failed.
        /// </remarks>
        public ITaskItem[] Destinations { get; set; }

        /// <summary>
        /// Determines whether to overwrite destination file or ignore.
        /// </summary>
        public bool Overwrite { get; set; }

        /// <summary>
        /// A list of successfully copied files.
        /// </summary>
        [Output]
        public ITaskItem[] CopiedFiles { get; set; }

        /// <summary>
        /// A list of failed files, which were not copied.
        /// </summary>
        [Output]
        public ITaskItem[] FailedFiles { get; set; }

        #endregion

        #region BaseTask Members

        /// <summary>
        /// Updates a file with specified regular expression and content.
        /// </summary>
        protected override void ExecuteInternal()
        {
            if (Files == null || Files.Length == 0)
            {
                Log.LogMessage(Resources.FileCopy_NoFilesToCopy);

                return;
            }

            if (Destinations == null || Destinations.Length == 0)
            {
                Log.LogMessage(Resources.FileCopy_NoDestination);

                FailedFiles = Files.ToArray();

                return;
            }

            if (Destinations.Length == 1 && File.Exists(Destinations[0].ItemSpec))
            {
                Log.LogError(Resources.FileCopy_SingleDestinationIsFile);

                FailedFiles = Files.ToArray();

                return;
            }

            if (!Directory.Exists(Destinations[0].ItemSpec))
            {
                Directory.CreateDirectory(Destinations[0].ItemSpec);
            }

            var failedFiles = new List<ITaskItem>();
            var copiedFiles = new List<ITaskItem>();

            for (var i = 0; i < Files.Length; i++)
            {
                if (string.IsNullOrEmpty(Files[i]?.ItemSpec) || !File.Exists(Files[i].ItemSpec))
                {
                    Log.LogMessage(Resources.FileCopy_FileNotFound, Files[i]?.ItemSpec ?? string.Empty);

                    continue;
                }

                try
                {
                    Log.LogMessage(Resources.FileCopy_CopyingFile, Files[i].ItemSpec);

                    var destinationPath = Destinations.Length == 1
                        ? Destinations[0].ItemSpec
                        : Destinations[i].ItemSpec;

                    var filePath = new Uri(Files[i].ItemSpec).LocalPath.TrimEnd(Path.DirectorySeparatorChar);

                    var subDirsFrom = Files[i].GetMetadata(WithSubDirsFromMetadataKey);

                    if (!string.IsNullOrEmpty(subDirsFrom))
                    {
                        Log.LogMessage(Resources.FileCopy_WithSubDirsFrom, subDirsFrom);

                        subDirsFrom = new Uri(subDirsFrom).LocalPath.TrimEnd(Path.DirectorySeparatorChar);

                        if (filePath.StartsWith(subDirsFrom, StringComparison.OrdinalIgnoreCase))
                        {
                            destinationPath = Path.Combine(destinationPath,
                                filePath.Replace(subDirsFrom, string.Empty).TrimStart(Path.DirectorySeparatorChar));
                        }
                        else
                        {
                            Log.LogError(Resources.FileCopy_WithSubDirsFromIsInvalid, subDirsFrom, filePath);

                            failedFiles.Add(new TaskItem(Files[i]));

                            continue;
                        }
                    }

                    if (!Path.HasExtension(destinationPath))
                    {
                        destinationPath = Path.Combine(destinationPath, Path.GetFileName(filePath));
                    }

                    Log.LogMessage(Resources.FileCopy_ToDestination, destinationPath);

                    if (!Directory.Exists(Path.GetDirectoryName(destinationPath)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
                    }

                    File.Copy(filePath, destinationPath, Overwrite);

                    copiedFiles.Add(new TaskItem(Files[i]));
                }
                catch (Exception e)
                {
                    Log.LogWarningFromException(e);

                    failedFiles.Add(new TaskItem(Files[i]));
                }
            }

            FailedFiles = failedFiles.ToArray();

            CopiedFiles = copiedFiles.ToArray();
        }

        #endregion
    }
}