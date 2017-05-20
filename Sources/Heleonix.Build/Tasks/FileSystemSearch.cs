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
using System.Text.RegularExpressions;
using Heleonix.Build.Properties;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Heleonix.Build.Tasks
{
    /// <summary>
    /// Searches items in the file system.
    /// </summary>
    public class FileSystemSearch : BaseTask
    {
        #region Methods

        /// <summary>
        /// Searches items in the specified directory.
        /// </summary>
        /// <param name="currentDir">The current directory path.</param>
        /// <param name="pathRegExp">The .NET regular expression to search items by path.</param>
        /// <param name="contentRegExp">The .NET regular expression to search items by content.</param>
        /// <param name="foundFiles">The found files.</param>
        /// <param name="foundDirs">The found directories.</param>
        /// <param name="foundItems">All the found items.</param>
        private void Search(string currentDir, Regex pathRegExp, Regex contentRegExp,
            ICollection<ITaskItem> foundFiles, ICollection<ITaskItem> foundDirs, ICollection<ITaskItem> foundItems)
        {
            if (string.IsNullOrEmpty(currentDir))
            {
                return;
            }

            if (string.IsNullOrEmpty(Types) || Types == "Directories" || Types == "All")
            {
                if (Direction == "Up")
                {
                    var dirs = Directory.GetDirectories(currentDir)
                        .Where(d =>
                            pathRegExp?.IsMatch(d) ?? true)
                        .Select(d => new TaskItem(d));

                    foreach (var dir in dirs)
                    {
                        foundDirs.Add(dir);
                        foundItems.Add(dir);
                    }
                }

                if (string.IsNullOrEmpty(Direction) || Direction == "Down")
                {
                    if (pathRegExp?.IsMatch(currentDir) ?? true)
                    {
                        var dirItem = new TaskItem(currentDir);

                        foundDirs.Add(dirItem);
                        foundItems.Add(dirItem);
                    }
                }
            }

            if (string.IsNullOrEmpty(Types) || Types == "Files" || Types == "All")
            {
                var files = Directory.GetFiles(currentDir)
                    .Where(f =>
                        (pathRegExp?.IsMatch(f) ?? true) && (contentRegExp?.IsMatch(File.ReadAllText(f)) ?? true))
                    .Select(f => new TaskItem(f));

                foreach (var file in files)
                {
                    foundFiles.Add(file);
                    foundItems.Add(file);
                }
            }

            if (string.IsNullOrEmpty(Direction) || Direction == "Down")
            {
                foreach (var subDir in Directory.GetDirectories(currentDir))
                {
                    Search(subDir, pathRegExp, contentRegExp, foundFiles, foundDirs, foundItems);
                }
            }

            if (Direction == "Up")
            {
                Search(Path.GetDirectoryName(currentDir), pathRegExp, contentRegExp, foundFiles, foundDirs, foundItems);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// The directory to start from, including that directory.
        /// </summary>
        [Required]
        public ITaskItem StartDir { get; set; }

        /// <summary>
        /// The search direction.
        /// </summary>
        /// <remarks>
        /// Possible values:
        /// <list type="bullet">
        /// <item><term>Up</term></item>
        /// <item><term>Down</term></item>
        /// </list>
        /// Default is "Down".
        /// </remarks>
        public string Direction { get; set; }

        /// <summary>
        /// Types of items to search.
        /// </summary>
        /// <remarks>
        /// Possible values:
        /// <list type="bullet">
        /// <item><term>Files</term></item>
        /// <item><term>Directories</term></item>
        /// <item><term>All</term></item>
        /// </list>
        /// Default is "All".
        /// </remarks>
        public string Types { get; set; }

        /// <summary>
        /// The .NET regular expression to search by path.
        /// </summary>
        public string PathRegExp { get; set; }

        /// <summary>
        /// The .NET regular expression options to search by path. Default is "IgnoreCase".
        /// </summary>
        public string PathRegExpOptions { get; set; }

        /// <summary>
        /// The .NET regular expression to search by content.
        /// </summary>
        public string ContentRegExp { get; set; }

        /// <summary>
        /// The .NET regular expression options to search by content.
        /// </summary>
        public string ContentRegExpOptions { get; set; }

        /// <summary>
        /// [Output] The found files.
        /// </summary>
        [Output]
        public ITaskItem[] FoundFiles { get; set; }

        /// <summary>
        /// [Output] The found directories.
        /// </summary>
        [Output]
        public ITaskItem[] FoundDirs { get; set; }

        /// <summary>
        /// [Output] All the found items.
        /// </summary>
        [Output]
        public ITaskItem[] FoundItems { get; set; }

        #endregion

        #region BaseTask Members

        /// <summary>
        /// Searches items in the file system.
        /// </summary>
        protected override void ExecuteInternal()
        {
            if (!Directory.Exists(StartDir.ItemSpec))
            {
                Log.LogMessage(Resources.FileSystemSearch_StartingDirectoryNotFound, StartDir.ItemSpec);

                return;
            }

            var pathRegExp = string.IsNullOrEmpty(PathRegExp)
                ? null
                : new Regex(PathRegExp,
                    string.IsNullOrEmpty(PathRegExpOptions)
                        ? RegexOptions.IgnoreCase
                        : (RegexOptions) Enum.Parse(typeof(RegexOptions), PathRegExpOptions));

            var contentRegExp = string.IsNullOrEmpty(ContentRegExp)
                ? null
                : new Regex(ContentRegExp,
                    string.IsNullOrEmpty(ContentRegExpOptions)
                        ? RegexOptions.IgnoreCase
                        : (RegexOptions) Enum.Parse(typeof(RegexOptions), ContentRegExpOptions));

            var foundFiles = new List<ITaskItem>();
            var foundDirs = new List<ITaskItem>();
            var foundItems = new List<ITaskItem>();

            Log.LogMessage(Resources.FileSystemSearch_StartSearching, StartDir.ItemSpec,
                Types, Direction, PathRegExp, ContentRegExp);

            Search(StartDir.ItemSpec.TrimEnd(Path.DirectorySeparatorChar), pathRegExp, contentRegExp,
                foundFiles, foundDirs, foundItems);

            FoundFiles = foundFiles.ToArray();
            FoundDirs = foundDirs.ToArray();
            FoundItems = foundItems.ToArray();
        }

        #endregion
    }
}