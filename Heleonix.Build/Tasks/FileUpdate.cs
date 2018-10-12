// <copyright file="FileUpdate.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks
{
    using System;
    using System.Text.RegularExpressions;
    using Heleonix.Build.Properties;
    using Microsoft.Build.Framework;

    /// <summary>
    /// Updates a file with specified regular expression and content.
    /// </summary>
    public class FileUpdate : BaseTask
    {
        /// <summary>
        /// Gets or sets the file path.
        /// </summary>
        public ITaskItem File { get; set; }

        /// <summary>
        /// Gets or sets the .NET regular expression to find content to replace.
        /// </summary>
        [Required]
        public string RegExp { get; set; }

        /// <summary>
        /// Gets or sets the .NET regular expression options.
        /// </summary>
        public string RegExpOptions { get; set; }

        /// <summary>
        /// Gets or sets content to replace with.
        /// </summary>
        public string Replacement { get; set; }

        /// <summary>
        /// Updates a file with specified regular expression and content.
        /// </summary>
        protected override void ExecuteInternal()
        {
            if (this.File == null || !System.IO.File.Exists(this.File.ItemSpec))
            {
                this.Log.LogMessage(Resources.FileUpdate_FileNotFound, this.File?.ItemSpec);

                return;
            }

#pragma warning disable SG0018 // Path traversal
            var input = System.IO.File.ReadAllText(this.File.ItemSpec);
#pragma warning restore SG0018 // Path traversal

            var regExpOptions = string.IsNullOrEmpty(this.RegExpOptions)
                ? RegexOptions.None
                : (RegexOptions)Enum.Parse(typeof(RegexOptions), this.RegExpOptions, true);

            var output = Regex.Replace(input, this.RegExp, this.Replacement ?? string.Empty, regExpOptions);

            this.Log.LogMessage(Resources.FileUpdate_UpdatingFile, this.File.ItemSpec);

#pragma warning disable SG0018 // Path traversal
            System.IO.File.WriteAllText(this.File.ItemSpec, output);
#pragma warning restore SG0018 // Path traversal
        }
    }
}
