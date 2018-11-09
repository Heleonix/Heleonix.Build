// <copyright file="FileSystemSearch.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Heleonix.Build.Properties;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Searches items in the file system.
    /// </summary>
    public class FileSystemSearch : BaseTask
    {
        /// <summary>
        /// Gets or sets the directory to start from, including that directory.
        /// </summary>
        [Required]
        public ITaskItem StartDir { get; set; }

        /// <summary>
        /// Gets or sets the search direction.
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
        /// Gets or sets types of items to search.
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
        /// Gets or sets the .NET regular expression to include found paths.
        /// Use ` instead of \ to escape symbols; use : instead of / and \ for path separators.
        /// </summary>
        public string PathRegExp { get; set; }

        /// <summary>
        /// Gets or sets the .NET regular expression options to include found paths. Default is "IgnoreCase".
        /// </summary>
        public string PathRegExpOptions { get; set; }

        /// <summary>
        /// Gets or sets the .NET regular expression to include by content.
        /// </summary>
        public string ContentRegExp { get; set; }

        /// <summary>
        /// Gets or sets the .NET regular expression options to include by content.
        /// </summary>
        public string ContentRegExpOptions { get; set; }

        /// <summary>
        /// Gets or sets the found files [Output].
        /// </summary>
        [Output]
#pragma warning disable CA1819 // Properties should not return arrays
        public ITaskItem[] FoundFiles { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Gets or sets the found directories [Output].
        /// </summary>
        [Output]
#pragma warning disable CA1819 // Properties should not return arrays
        public ITaskItem[] FoundDirs { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Gets or sets All the found items [Output].
        /// </summary>
        [Output]
#pragma warning disable CA1819 // Properties should not return arrays
        public ITaskItem[] FoundItems { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Searches items in the file system.
        /// </summary>
        protected override void ExecuteInternal()
        {
            if (!Directory.Exists(this.StartDir.ItemSpec))
            {
                this.Log.LogMessage(Resources.FileSystemSearch_StartingDirectoryNotFound, this.StartDir.ItemSpec);

                this.FoundFiles = Array.Empty<ITaskItem>();
                this.FoundDirs = Array.Empty<ITaskItem>();
                this.FoundItems = Array.Empty<ITaskItem>();

                return;
            }

            var pathReplacement = Path.DirectorySeparatorChar == '\\'
                ? "\\\\"
                : Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture);

            var options = string.IsNullOrEmpty(this.PathRegExpOptions)
                        ? RegexOptions.IgnoreCase
                        : (RegexOptions)Enum.Parse(typeof(RegexOptions), this.PathRegExpOptions);
            var pathRegExp = string.IsNullOrEmpty(this.PathRegExp)
                ? null
                : new Regex(this.PathRegExp.Replace("\\", pathReplacement).Replace('`', '\\').Replace("/", pathReplacement), options);

            options = string.IsNullOrEmpty(this.ContentRegExpOptions)
                        ? RegexOptions.IgnoreCase
                        : (RegexOptions)Enum.Parse(typeof(RegexOptions), this.ContentRegExpOptions);
            var contentRegExp = string.IsNullOrEmpty(this.ContentRegExp)
                ? null
                : new Regex(this.ContentRegExp, options);

            var foundFiles = new List<ITaskItem>();
            var foundDirs = new List<ITaskItem>();
            var foundItems = new List<ITaskItem>();

            this.Log.LogMessage(
                Resources.FileSystemSearch_StartSearching,
                this.StartDir.ItemSpec,
                this.Types,
                this.Direction,
                this.PathRegExp + $" (Transformed: {pathRegExp})",
                this.ContentRegExp);

            this.Search(
                this.StartDir.ItemSpec.TrimEnd(Path.DirectorySeparatorChar),
                pathRegExp,
                contentRegExp,
                foundFiles,
                foundDirs,
                foundItems);

            this.FoundFiles = foundFiles.ToArray();
            this.FoundDirs = foundDirs.ToArray();
            this.FoundItems = foundItems.ToArray();
        }

        /// <summary>
        /// Searches items in the specified directory.
        /// </summary>
        /// <param name="currentDir">The current directory path.</param>
        /// <param name="pathRegExp">The .NET regular expression to include found paths.</param>
        /// <param name="contentRegExp">The .NET regular expression to include by content.</param>
        /// <param name="foundFiles">The found files.</param>
        /// <param name="foundDirs">The found directories.</param>
        /// <param name="foundItems">All the found items.</param>
        private void Search(
            string currentDir,
            Regex pathRegExp,
            Regex contentRegExp,
            ICollection<ITaskItem> foundFiles,
            ICollection<ITaskItem> foundDirs,
            ICollection<ITaskItem> foundItems)
        {
            if (string.IsNullOrEmpty(currentDir))
            {
                return;
            }

            if (string.IsNullOrEmpty(this.Types) || this.Types == "Directories" || this.Types == "All")
            {
                if (this.Direction == "Up")
                {
                    var dirs = Directory.GetDirectories(currentDir)
                        .Where(d => pathRegExp?.IsMatch(d) ?? true)
                        .Select(d => new TaskItem(d));

                    foreach (var dir in dirs)
                    {
                        foundDirs.Add(dir);
                        foundItems.Add(dir);

                        this.Log.LogMessage(Resources.FileSystemSearch_FoundItem, dir);
                    }
                }

                if ((string.IsNullOrEmpty(this.Direction) || this.Direction == "Down")
                    && ((pathRegExp == null) || pathRegExp.IsMatch(currentDir)))
                {
                    var dirItem = new TaskItem(currentDir);

                    foundDirs.Add(dirItem);
                    foundItems.Add(dirItem);

                    this.Log.LogMessage(Resources.FileSystemSearch_FoundItem, dirItem);
                }
            }

            if (string.IsNullOrEmpty(this.Types) || this.Types == "Files" || this.Types == "All")
            {
                var files = Directory.GetFiles(currentDir)
                    .Where(f =>
                        (pathRegExp?.IsMatch(f) ?? true)
                        && (contentRegExp?.IsMatch(File.ReadAllText(f)) ?? true))
                    .Select(f => new TaskItem(f));

                foreach (var file in files)
                {
                    foundFiles.Add(file);
                    foundItems.Add(file);

                    this.Log.LogMessage(Resources.FileSystemSearch_FoundItem, file);
                }
            }

            if (string.IsNullOrEmpty(this.Direction) || this.Direction == "Down")
            {
                foreach (var subDir in Directory.GetDirectories(currentDir))
                {
                    this.Search(
                        subDir,
                        pathRegExp,
                        contentRegExp,
                        foundFiles,
                        foundDirs,
                        foundItems);
                }
            }

            if (this.Direction == "Up")
            {
                this.Search(
                    Path.GetDirectoryName(currentDir),
                    pathRegExp,
                    contentRegExp,
                    foundFiles,
                    foundDirs,
                    foundItems);
            }
        }
    }
}
