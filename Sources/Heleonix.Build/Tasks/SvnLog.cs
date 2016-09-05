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
        public ITaskItem SvnExeFile { get; set; }

        /// <summary>
        /// The file or directory path to retrieve log for.
        /// </summary>
        [Required]
        public ITaskItem RepositoryFileDir { get; set; }

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
        /// [Output] The commits.
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
                .Add(RepositoryFileDir.ItemSpec, true);

            var workingDir = File.Exists(RepositoryFileDir.ItemSpec)
                ? Path.GetDirectoryName(RepositoryFileDir.ItemSpec)
                : RepositoryFileDir.ItemSpec;

            string output;
            string error;

            var exitCode = ExeHelper.Execute(SvnExeFile.ItemSpec, args, out output, out error, workingDir);

            Log.LogMessage(output);

            if (!string.IsNullOrEmpty(error))
            {
                Log.LogError(error);
            }

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