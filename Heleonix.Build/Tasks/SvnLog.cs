// <copyright file="SvnLog.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;
    using Heleonix.Build.Properties;
    using Heleonix.Execution;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Retrieves the Svn log.
    /// </summary>
    public class SvnLog : BaseTask
    {
        /// <summary>
        /// Gets or sets the Svn executable path.
        /// </summary>
        [Required]
        public ITaskItem SvnExe { get; set; }

        /// <summary>
        /// Gets or sets the file or directory path to retrieve log for.
        /// </summary>
        [Required]
        public ITaskItem RepositoryFileOrDir { get; set; }

        /// <summary>
        /// Gets or sets the maximum count of commits to retrieve from the log.
        /// </summary>
        public long MaxCount { get; set; }

        /// <summary>
        /// Gets or sets the date to start retrieval of commits from.
        /// </summary>
        public string SinceDate { get; set; }

        /// <summary>
        /// Gets or sets the date to stop retrieval of commits on.
        /// </summary>
        public string UntilDate { get; set; }

        /// <summary>
        /// Gets or sets the commits [Output].
        /// </summary>
        /// <remarks>
        /// <see cref="ITaskItem.ItemSpec"/> is a revision number.
        /// Metadata:
        /// <list type="bullet">
        /// <item><term>Revision</term></item>
        /// <item><term>AuthorName</term></item>
        /// <item><term>AuthorDate</term></item>
        /// <item><term>Message</term></item>
        /// </list>
        /// </remarks>
        [Output]
#pragma warning disable CA1819 // Properties should not return arrays
        public ITaskItem[] Commits { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Gets the Svn log.
        /// </summary>
        protected override void ExecuteInternal()
        {
            var commits = new List<ITaskItem>();

            var args = ArgsBuilder.By("--", " ")
                .AddValue("log")
                .AddArgument("limit", this.MaxCount)
                .AddArgument(
                    "revision",
                    $"{{{this.SinceDate}}}:{{{this.UntilDate}}}",
                    !string.IsNullOrEmpty(this.SinceDate) && !string.IsNullOrEmpty(this.UntilDate))
                .AddKey("verbose")
                .AddKey("xml")
                .AddKey("with-all-revprops")
                .AddPath(this.RepositoryFileOrDir.ItemSpec.TrimEnd(Path.DirectorySeparatorChar));

            var workingDir = File.Exists(this.RepositoryFileOrDir.ItemSpec)
                ? Path.GetDirectoryName(this.RepositoryFileOrDir.ItemSpec)
                : this.RepositoryFileOrDir.ItemSpec;

            var result = ExeHelper.Execute(this.SvnExe.ItemSpec, args, true, workingDir, int.MaxValue);

            this.Log.LogMessage(result.Output);

            if (!string.IsNullOrEmpty(result.Error))
            {
                this.Log.LogError(result.Error);
            }

            if (result.ExitCode != 0)
            {
                this.Log.LogError(Resources.TaskFailedWithExitCode, nameof(SvnLog), result.ExitCode);

                this.Commits = commits.ToArray();

                return;
            }

            foreach (var logEntryNode in XDocument.Parse(result.Output).Descendants("logentry"))
            {
                var commit = new TaskItem { ItemSpec = logEntryNode.Attribute("revision").Value };

                commit.SetMetadata("Revision", commit.ItemSpec);
                commit.SetMetadata("AuthorName", logEntryNode.Element("author").Value);
                commit.SetMetadata("AuthorDate", logEntryNode.Element("date").Value);
                commit.SetMetadata("Message", logEntryNode.Element("msg").Value);

                commits.Add(commit);
            }

            if (commits.Count == 0)
            {
                this.Log.LogError(Resources.SvnLog_ThereAreNoCommitsYet);
            }

            this.Commits = commits.ToArray();
        }
    }
}
