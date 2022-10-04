// <copyright file="FileValidate.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Heleonix.Build.Properties;
    using Microsoft.Build.Framework;

    /// <summary>
    /// Validates that contents of specified files corresponds to specified regex patterns.
    /// </summary>
    public class FileValidate : BaseTask
    {
        /// <summary>
        /// Gets or sets file paths with regex patterns in custom metadata to validate.
        /// </summary>
#pragma warning disable CA1819 // Properties should not return arrays
        [Required]
        public ITaskItem[] Files { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Gets or sets the .NET regular expression options.
        /// </summary>
        public string RegExpOptions { get; set; }

        /// <summary>
        /// Reads a file with specified regular expression and content.
        /// </summary>
        protected override void ExecuteInternal()
        {
            var regExpOptions = string.IsNullOrEmpty(this.RegExpOptions)
                ? RegexOptions.None
                : (RegexOptions)Enum.Parse(typeof(RegexOptions), this.RegExpOptions, true);

            foreach (var file in this.Files)
            {
                var patterns = file.CloneCustomMetadata() as IDictionary<string, string>;

                this.Log.LogMessage(
                    Resources.FileValidate_ValidatingFile,
                    file.ItemSpec,
                    string.Join(";", patterns.Select(m => $"{m.Key}={m.Value}")));

                if (!File.Exists(file.ItemSpec))
                {
                    this.Log.LogError(Resources.FileValidate_FileNotFound, file.ItemSpec);

                    continue;
                }

#pragma warning disable SG0018 // Path traversal
                var input = File.ReadAllText(file.ItemSpec);
#pragma warning restore SG0018 // Path traversal

                foreach (var pattern in patterns)
                {
                    var matches = Regex.Matches(input, pattern.Value, regExpOptions);

                    if (matches.Count == 0)
                    {
                        this.Log.LogError(
                            Resources.FileValidate_RuleViolated,
                            file.ItemSpec,
                            pattern.Key,
                            pattern.Value);
                    }
                }
            }
        }
    }
}
