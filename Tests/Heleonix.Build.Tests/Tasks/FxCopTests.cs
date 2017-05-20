﻿/*
The MIT License (MIT)

Copyright (c) 2015-present Heleonix - Hennadii Lutsyshyn

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

using System.IO;
using Heleonix.Build.Tasks;
using Heleonix.Build.Tests.Common;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace Heleonix.Build.Tests.Tasks
{
    /// <summary>
    /// Tests the <see cref="FxCop"/>.
    /// </summary>
    public static class FxCopTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="BaseTask.Execute"/>.
        /// </summary>
        [TestCase(true, FxCop.IssueTypes.Any)]
        [TestCase(false, FxCop.IssueTypes.Any)]
        [TestCase(false, FxCop.IssueTypes.None)]
        public static void Execute(bool useXsl, FxCop.IssueTypes failOn)
        {
            MSBuildHelper.ExecuteMSBuild(LibSimulatorPath.SolutionFile, "Build", null);

            var analysisResults = Path.Combine(LibSimulatorPath.GetArtifactsDir("Hxb-FxCop"), Path.GetRandomFileName());

            var task = new FxCop
            {
                BuildEngine = new FakeBuildEngine(),
                FxCopCmdFile = new TaskItem(SystemPath.FxCopExe),
                AnalysisResultFile = new TaskItem(analysisResults),
                TargetsFilesDirs = new ITaskItem[] { new TaskItem(LibSimulatorPath.OutFile) },
                AnalysisResultsXslFile = useXsl
                    ? new TaskItem(Path.Combine(Path.GetDirectoryName(SystemPath.FxCopExe) ?? string.Empty, "Xml",
                        "CodeAnalysisReport.xsl"))
                    : null,
                FailOn = failOn.ToString()
            };

            var succeeded = task.Execute();

            var analysisResultsExists = File.Exists(analysisResults);
            var analysisResultsHtml = Path.ChangeExtension(analysisResults, ".html");
            var analysisResultsHtmlExists = File.Exists(analysisResultsHtml);

            try
            {
                if (failOn.HasFlag(FxCop.IssueTypes.None))
                {
                    Assert.That(succeeded, Is.True);
                }
                else
                {
                    Assert.That(succeeded, Is.False);
                }

                Assert.That(analysisResultsExists, Is.True);
            }
            finally
            {
                if (analysisResultsExists)
                {
                    File.Delete(analysisResults);
                }

                if (analysisResultsHtmlExists)
                {
                    File.Delete(analysisResultsHtml);
                }
            }
        }

        #endregion
    }
}