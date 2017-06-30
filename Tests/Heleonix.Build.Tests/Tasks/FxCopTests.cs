/*
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

using System.Globalization;
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
        [TestCase(null, true, true, true, false, false, false, false, false, false, FxCop.IssueTypes.Any)]
        [TestCase(null, true, true, true, false, false, true, true, true, true, FxCop.IssueTypes.Any)]
        [TestCase(null, false, true, true, false, true, false, false, false, false, FxCop.IssueTypes.Any)]
        [TestCase(null, true, false, false, false, false, false, false, false, false, FxCop.IssueTypes.Any)]
        [TestCase(null, true, false, false, false, false, false, false, false, false, FxCop.IssueTypes.None)]
        [TestCase("```", true, false, false, false, false, false, false, false, false, FxCop.IssueTypes.None)]
        [TestCase(null, true, false, false, true, false, false, false, false, false, FxCop.IssueTypes.Any)]
        [TestCase(null, true, false, false, true, false, false, false, false, false, FxCop.IssueTypes.None)]
        [TestCase(null, true, false, false, true, false, false, false, false, false, FxCop.IssueTypes.CriticalErrors)]
        [TestCase(null, true, false, false, true, false, false, false, false, false, FxCop.IssueTypes.Errors)]
        [TestCase(null, true, false, false, true, false, false, false, false, false, FxCop.IssueTypes.CriticalWarnings)]
        [TestCase(null, true, false, false, true, false, false, false, false, false, FxCop.IssueTypes.Warnings)]
        [TestCase(null, true, false, false, true, false, false, false, false, false, FxCop.IssueTypes.Informational)]
        [TestCase("FullyCoveredType", true, false, false, false, false, false, false, false, false,
            FxCop.IssueTypes.Any)]
        public static void Execute(string targetTypes, bool useTargetsFiles, bool shouldReportAlreadyExist, bool useXsl,
            bool simulateAllViolations, bool useProjectFile, bool useDictionaryFile, bool useRulesDir,
            bool useRulesetFile, bool useDependenciesDir, FxCop.IssueTypes failOn)
        {
            MSBuildHelper.ExecuteMSBuild(LibSimulatorPath.SolutionFile, "Build", null, LibSimulatorPath.SolutionDir);

            var artifactsDir = LibSimulatorPath.GetArtifactsDir("Hxb-FxCop");

            var analysisResults = Path.Combine(artifactsDir, Path.GetRandomFileName());
            var exeMock = Path.Combine(artifactsDir, Path.GetFileName(SystemPath.ExeMock));
            var projectFile = Path.Combine(artifactsDir, Path.GetFileName(SystemPath.FxCopProjectFile));

            if (shouldReportAlreadyExist)
            {
                if (!Directory.Exists(artifactsDir))
                {
                    Directory.CreateDirectory(artifactsDir);
                }

                File.Copy(SystemPath.FxCopReportFile, analysisResults, true);
            }

            if (simulateAllViolations)
            {
                if (!Directory.Exists(artifactsDir))
                {
                    Directory.CreateDirectory(artifactsDir);
                }

                File.Copy(SystemPath.FxCopReportFile, Path.ChangeExtension(analysisResults, ".tmp"), true);

                File.Copy(SystemPath.ExeMock, exeMock, true);

                using (var cfg = File.CreateText(Path.ChangeExtension(exeMock, ".mock")))
                {
                    cfg.WriteLine(0);
                }
            }

            if (useProjectFile)
            {
                var projectContent = string.Format(CultureInfo.InvariantCulture,
                    File.ReadAllText(SystemPath.FxCopProjectFile), LibSimulatorPath.OutDir,
                    LibSimulatorPath.OutFile, SystemPath.FxCopRulesDir);

                File.WriteAllText(projectFile, projectContent);
            }

            var task = new FxCop
            {
                BuildEngine = new FakeBuildEngine(),
                FxCopCmdFile = new TaskItem(simulateAllViolations ? exeMock : SystemPath.FxCopExe),
                AnalysisResultFile = new TaskItem(analysisResults),
                TargetsFilesDirs = useTargetsFiles ? new ITaskItem[] { new TaskItem(LibSimulatorPath.OutFile) } : null,
                AnalysisResultsXslFile = useXsl
                    ? new TaskItem(Path.Combine(Path.GetDirectoryName(SystemPath.FxCopExe) ?? string.Empty, "Xml",
                        "CodeAnalysisReport.xsl"))
                    : null,
                FailOn = failOn.ToString(),
                TargetsTypes = targetTypes,
                DictionaryFile = useDictionaryFile ? new TaskItem(SystemPath.FxCopDictionaryFile) : null,
                RulesFilesDirs = useRulesDir ? new ITaskItem[] { new TaskItem(SystemPath.FxCopRulesDir) } : null,
                RulesetFile = useRulesetFile ? new TaskItem(SystemPath.FxCopRulesetFile) : null,
                DependenciesDirs =
                    useDependenciesDir ? new ITaskItem[] { new TaskItem(LibSimulatorPath.OutDir) } : null,
                ProjectFile = useProjectFile ? new TaskItem(projectFile) : null
            };

            var succeeded = task.Execute();

            var analysisResultsExists = File.Exists(analysisResults);

            try
            {
                if (targetTypes == "```")
                {
                    Assert.That(succeeded, Is.False);
                }
                else if (targetTypes == "FullyCoveredType" || failOn.HasFlag(FxCop.IssueTypes.None))
                {
                    Assert.That(succeeded, Is.True);
                }
                else
                {
                    Assert.That(succeeded, Is.False);
                }

                Assert.That(analysisResultsExists, Is.EqualTo(targetTypes == null || simulateAllViolations));
            }
            finally
            {
                if (Directory.Exists(artifactsDir))
                {
                    Directory.Delete(artifactsDir, true);
                }
            }
        }

        #endregion
    }
}