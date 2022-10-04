// <copyright file="FileRead.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks
{
    using System;
    using System.Text.RegularExpressions;
    using Heleonix.Build.Properties;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Gets content from file by specified regular expression.
    /// </summary>
    public class FileRead : BaseTask
    {
        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        [Required]
        public ITaskItem File { get; set; }

        /// <summary>
        /// Gets or sets the .NET regular expression to find content.
        /// </summary>
        [Required]
        public string RegExp { get; set; }

        /// <summary>
        /// Gets or sets the .NET regular expression options.
        /// </summary>
        public string RegExpOptions { get; set; }

        /// <summary>
        /// Gets or sets found matches [Output].
        /// </summary>
        [Output]
#pragma warning disable CA1819 // Properties should not return arrays
        public ITaskItem[] Matches { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Reads a file with specified regular expression and content.
        /// </summary>
        protected override void ExecuteInternal()
        {
            if (!System.IO.File.Exists(this.File.ItemSpec))
            {
                this.Log.LogError(Resources.FileRead_FileNotFound, this.File.ItemSpec);

                this.Matches = Array.Empty<ITaskItem>();

                return;
            }

#pragma warning disable SG0018 // Path traversal
            var input = System.IO.File.ReadAllText(this.File.ItemSpec);
#pragma warning restore SG0018 // Path traversal

            var regExpOptions = string.IsNullOrEmpty(this.RegExpOptions)
                ? RegexOptions.None
                : (RegexOptions)Enum.Parse(typeof(RegexOptions), this.RegExpOptions, true);

            var foundMatches = Regex.Matches(input, this.RegExp, regExpOptions);

            this.Matches = new ITaskItem[foundMatches.Count];

            for (var i = 0; i < foundMatches.Count; i++)
            {
                this.Matches[i] = new TaskItem(this.File);
                this.Matches[i].SetMetadata("Match", foundMatches[i].Value);
            }
        }
    }
}
