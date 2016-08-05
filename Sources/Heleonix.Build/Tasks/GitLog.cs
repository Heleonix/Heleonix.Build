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
    /// Gets the Git log.
    /// </summary>
    public class GitLog : BaseTask
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Git executable path.
        /// </summary>
        [Required]
        public ITaskItem GitExePath { get; set; }

        /// <summary>
        /// Gets or sets the maximum count of commits to retrieve from the log.
        /// </summary>
        public long MaxCount { get; set; }

        /// <summary>
        /// Gets or sets the count of commits to skip before starting retrieval from the log.
        /// </summary>
        public long SkipCount { get; set; }

        /// <summary>
        /// Gets or sets the date to start retrieval of commits from.
        /// </summary>
        public string SinceDate { get; set; }

        /// <summary>
        /// Gets or sets the date to stop retrieval of commits on.
        /// </summary>
        public string UntilDate { get; set; }

        /// <summary>
        /// Gets or sets the commits.
        /// </summary>
        [Output]
        public ITaskItem[] Commits { get; set; }

        #endregion

        #region BaseTask Members

        /// <summary>
        /// Gets the Git log.
        /// </summary>
        protected override void ExecuteInternal()
        {
            var outputFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            // Commit output format:
            // <Heleonix.Build.Tasks.GitLog.Commit>
            // sha1
            // hash
            // author name
            // author email
            // author date (strict ISO 8601 format)
            // committer name
            // committer email
            // committer date (strict ISO 8601 format)
            // raw body (unwrapped subject and body)
            // <Heleonix.Build.Tasks.GitLog.Commit>
            var args = ArgsBuilder.By(' ', '=')
                .Add("git")
                .Add("log")
                .Add("--format",
                    "<Heleonix.Build.Tasks.GitLog.Commit>%n%H%n%h%n%an%n%ae%n%aI%n%cn%n%ce%n%cI%n%B%n</Heleonix.Build.Tasks.GitLog.Commit>",
                    true)
                .Add("--since", SinceDate, true)
                .Add("--until", UntilDate, true)
                .Add("--skip", SkipCount, false, SkipCount > 0)
                .Add("--max-count", MaxCount == 0 ? 1 : MaxCount)
                .Add(">")
                .Add(outputFilePath, true);

            try
            {
                var exitCode = ExeHelper.Execute(GitExePath.ItemSpec, args);

                if (exitCode != 0)
                {
                    Log.LogError($"{nameof(GitLog)} failed. Exit code: {exitCode}.");

                    return;
                }

                var commits = new List<ITaskItem>();

                using (var outputStream = new StreamReader(File.OpenRead(outputFilePath)))
                {
                    while (!outputStream.EndOfStream)
                    {
                        var line = outputStream.ReadLine();

                        if (line != "<Heleonix.Build.Tasks.GitLog.Commit>") continue;

                        var commit = new TaskItem { ItemSpec = outputStream.ReadLine() };

                        commit.SetMetadata("Hash", outputStream.ReadLine());
                        commit.SetMetadata("AuthorName", outputStream.ReadLine());
                        commit.SetMetadata("AuthorEmail", outputStream.ReadLine());
                        commit.SetMetadata("AuthorDate", outputStream.ReadLine());
                        commit.SetMetadata("CommitterName", outputStream.ReadLine());
                        commit.SetMetadata("CommitterEmail", outputStream.ReadLine());
                        commit.SetMetadata("CommitterDate", outputStream.ReadLine());

                        var textBuilder = new StringBuilder();

                        line = outputStream.ReadLine();

                        while (line != "</Heleonix.Build.Tasks.GitLog.Commit>")
                        {
                            textBuilder.AppendLine(line);

                            line = outputStream.ReadLine();
                        }

                        if (textBuilder.ToString().EndsWith(Environment.NewLine))
                        {
                            textBuilder.Remove(textBuilder.Length - Environment.NewLine.Length,
                                Environment.NewLine.Length);
                        }

                        commit.SetMetadata("Text", textBuilder.ToString());

                        commits.Add(commit);
                    }
                }
            }
            finally
            {
                if (File.Exists(outputFilePath))
                {
                    File.Delete(outputFilePath);
                }
            }
        }

        #endregion
    }
}