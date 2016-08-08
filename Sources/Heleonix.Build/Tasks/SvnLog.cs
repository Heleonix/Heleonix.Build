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

using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Heleonix.Build.Tasks
{
    /// <summary>
    /// Retrieves the Svn log.
    /// </summary>
    public class SvnLog : BaseTask
    {
        #region Properties

        /// <summary>
        /// The Svn executable path.
        /// </summary>
        [Required]
        public ITaskItem SvnExePath { get; set; }

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
        public ITaskItem[] Commits { get; set; }

        #endregion

        #region BaseTask Members

        /// <summary>
        /// Gets the Svn log.
        /// </summary>
        protected override void ExecuteInternal()
        {
            var args = ArgsBuilder.By(' ', ' ')
                .Add("log")
                .Add("--limit", MaxCount)
                .Add("--revision", $"{{{SinceDate}}}:{{{UntilDate}}}", false,
                    !string.IsNullOrEmpty(SinceDate) && !string.IsNullOrEmpty(UntilDate))
                .Add("--verbose")
                .Add("--xml")
                .Add("--with-all-revprops")
                .Add(RepositoryPath.ItemSpec, true);

            var workingDirectoryPath = File.Exists(RepositoryPath.ItemSpec)
                ? Path.GetDirectoryName(RepositoryPath.ItemSpec)
                : RepositoryPath.ItemSpec;

            string output;

            var exitCode = ExeHelper.Execute(SvnExePath.ItemSpec, args, out output, workingDirectoryPath);

            if (exitCode != 0)
            {
                Log.LogError($"{nameof(SvnLog)} failed. Exit code: {exitCode}.");

                return;
            }

            var commits = new List<ITaskItem>();

            foreach (var logEntryNode in XDocument.Parse(output).Descendants("logentry"))
            {
                var commit = new TaskItem { ItemSpec = logEntryNode.Attribute("revision").Value };

                commit.SetMetadata("Revision", commit.ItemSpec);
                commit.SetMetadata("AuthorName", logEntryNode.Element("author")?.Value);
                commit.SetMetadata("AuthorDate", logEntryNode.Element("date")?.Value);
                commit.SetMetadata("Message", logEntryNode.Element("msg")?.Value);

                commits.Add(commit);
            }

            Commits = commits.ToArray();
        }

        #endregion
    }
}