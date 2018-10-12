// <copyright file="GitLog.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Heleonix.Build.Properties;
    using Heleonix.Execution;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    /// <summary>
    /// Retrieves the Git log.
    /// </summary>
    public class GitLog : BaseTask
    {
        /// <summary>
        /// Gets or sets the Git executable path.
        /// </summary>
        [Required]
        public ITaskItem GitExe { get; set; }

        /// <summary>
        /// Gets or sets the file or directory path to retrieve log for.
        /// </summary>
        [Required]
        public ITaskItem RepositoryFileOrDir { get; set; }

        /// <summary>
        /// Gets or sets the maximum count of commits to retrieve from the log.
        /// </summary>
        public int MaxCount { get; set; }

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
        /// <see cref="ITaskItem.ItemSpec"/> is a commit hash.
        /// Metadata:
        /// <list type="bullet">
        /// <item><term>Revision</term></item>
        /// <item><term>AuthorName</term></item>
        /// <item><term>AuthorEmail</term></item>
        /// <item><term>AuthorDate</term></item>
        /// <item><term>CommitterName</term></item>
        /// <item><term>CommitterEmail</term></item>
        /// <item><term>CommitterDate</term></item>
        /// <item><term>Message</term></item>
        /// </list>
        /// </remarks>
        [Output]
#pragma warning disable CA1819 // Properties should not return arrays
        public ITaskItem[] Commits { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

        /// <summary>
        /// Gets the Git log.
        /// </summary>
        protected override void ExecuteInternal()
        {
            var commits = new List<ITaskItem>();

            var args = ArgsBuilder.By("--", "=")
                .AddValue("log")
                .AddPath(
                    "format",
                    "<Heleonix.Build.Tasks.GitLog.Commit>%n%H%n%h%n%an%n%ae%n%aI%n%cn%n%ce%n%cI%n%B%n</Heleonix.Build.Tasks.GitLog.Commit>")
                .AddPath("since", this.SinceDate)
                .AddPath("until", this.UntilDate)
                .AddArgument("max-count", this.MaxCount)
                .AddPath(this.RepositoryFileOrDir.ItemSpec.TrimEnd(Path.DirectorySeparatorChar));

            var workingDir = File.Exists(this.RepositoryFileOrDir.ItemSpec)
                ? Path.GetDirectoryName(this.RepositoryFileOrDir.ItemSpec)
                : this.RepositoryFileOrDir.ItemSpec;

            var result = ExeHelper.Execute(this.GitExe.ItemSpec, args, true, workingDir, int.MaxValue);

            this.Log.LogMessage(result.Output);

            if (!string.IsNullOrEmpty(result.Error))
            {
                this.Log.LogError(result.Error);
            }

            if (result.ExitCode != 0)
            {
                this.Log.LogError(Resources.TaskFailedWithExitCode, nameof(GitLog), result.ExitCode);

                this.Commits = commits.ToArray();

                return;
            }

            using (var outputReader = new StringReader(result.Output))
            {
                string line;

                do
                {
                    line = outputReader.ReadLine();

                    if (line != "<Heleonix.Build.Tasks.GitLog.Commit>")
                    {
                        continue;
                    }

                    var commit = new TaskItem { ItemSpec = outputReader.ReadLine() };

                    commit.SetMetadata("Revision", outputReader.ReadLine());
                    commit.SetMetadata("AuthorName", outputReader.ReadLine());
                    commit.SetMetadata("AuthorEmail", outputReader.ReadLine());
                    commit.SetMetadata("AuthorDate", outputReader.ReadLine());
                    commit.SetMetadata("CommitterName", outputReader.ReadLine());
                    commit.SetMetadata("CommitterEmail", outputReader.ReadLine());
                    commit.SetMetadata("CommitterDate", outputReader.ReadLine());

                    var textBuilder = new StringBuilder();

                    line = outputReader.ReadLine();

                    while (line != "</Heleonix.Build.Tasks.GitLog.Commit>")
                    {
                        textBuilder.AppendLine(line);

                        line = outputReader.ReadLine();
                    }

                    textBuilder.Replace(
                        Environment.NewLine,
                        string.Empty,
                        textBuilder.Length - Environment.NewLine.Length,
                        Environment.NewLine.Length);

                    commit.SetMetadata("Message", textBuilder.ToString());

                    commits.Add(commit);
                }
                while (line != null);
            }

            this.Commits = commits.ToArray();
        }
    }
}
