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
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Heleonix.Build.Tasks
{
    /// <summary>
    /// Retrieves the Git log.
    /// </summary>
    public class GitLog : BaseTask
    {
        #region Properties

        /// <summary>
        /// The Git executable path.
        /// </summary>
        [Required]
        public ITaskItem GitExePath { get; set; }

        /// <summary>
        /// The file or directory path to retrieve log for.
        /// </summary>
        [Required]
        public ITaskItem RepositoryPath { get; set; }

        /// <summary>
        /// The maximum count of commits to retrieve from the log.
        /// </summary>
        public long MaxCount { get; set; }

        /// <summary>
        /// The date to start retrieval of commits from.
        /// </summary>
        public string SinceDate { get; set; }

        /// <summary>
        /// The date to stop retrieval of commits on.
        /// </summary>
        public string UntilDate { get; set; }

        /// <summary>
        /// The commits.
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
        public ITaskItem[] Commits { get; set; }

        #endregion

        #region BaseTask Members

        /// <summary>
        /// Gets the Git log.
        /// </summary>
        protected override void ExecuteInternal()
        {
            var args = ArgsBuilder.By(' ', '=')
                .Add("log")
                .Add("--format",
                    "<Heleonix.Build.Tasks.GitLog.Commit>%n%H%n%h%n%an%n%ae%n%aI%n%cn%n%ce%n%cI%n%B%n</Heleonix.Build.Tasks.GitLog.Commit>",
                    true)
                .Add("--since", SinceDate, true)
                .Add("--until", UntilDate, true)
                .Add("--max-count", MaxCount == 0 ? 1 : MaxCount)
                .Add("--")
                .Add(RepositoryPath.ItemSpec, true);

            var workingDirectoryPath = File.Exists(RepositoryPath.ItemSpec)
                ? Path.GetDirectoryName(RepositoryPath.ItemSpec)
                : RepositoryPath.ItemSpec;

            string output;
            string error;

            var exitCode = ExeHelper.Execute(GitExePath.ItemSpec, args, out output, out error, workingDirectoryPath);

            if (exitCode != 0)
            {
                Log.LogError($"{nameof(GitLog)} failed. Exit code: {exitCode}.");

                return;
            }

            var commits = new List<ITaskItem>();

            using (var outputReader = new StringReader(output))
            {
                var line = string.Empty;

                while (line != null)
                {
                    line = outputReader.ReadLine();

                    if (line != "<Heleonix.Build.Tasks.GitLog.Commit>") continue;

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

                    if (textBuilder.ToString().EndsWith(Environment.NewLine))
                    {
                        textBuilder.Remove(textBuilder.Length - Environment.NewLine.Length,
                            Environment.NewLine.Length);
                    }

                    commit.SetMetadata("Message", textBuilder.ToString());

                    commits.Add(commit);
                }
            }

            Commits = commits.ToArray();
        }

        #endregion
    }
}