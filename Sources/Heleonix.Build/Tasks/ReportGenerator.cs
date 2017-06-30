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

using System.IO;
using System.Linq;
using Heleonix.Build.Properties;
using Microsoft.Build.Framework;

namespace Heleonix.Build.Tasks
{
    /// <summary>
    /// Generates HTML report using the ReportGenerator tool.
    /// </summary>
    public class ReportGenerator : BaseTask
    {
        #region Properties

        /// <summary>
        /// The ReportGenerator executable path.
        /// </summary>
        [Required]
        public ITaskItem ReportGeneratorExeFile { get; set; }

        /// <summary>
        /// The results files paths.
        /// </summary>
        [Required]
        public ITaskItem[] ResultsFiles { get; set; }

        /// <summary>
        /// The report directory path to save report to.
        /// </summary>
        [Required]
        public ITaskItem ReportDir { get; set; }

        /// <summary>
        /// The report types, separated by semicolon.
        /// </summary>
        /// <remarks>
        /// Possible values:
        /// <list type="bullet">
        /// <item><term>Badges</term></item>
        /// <item><term>Html</term></item>
        /// <item><term>HtmlSummary</term></item>
        /// <item><term>Latex</term></item>
        /// <item><term>LatexSummary</term></item>
        /// <item><term>TextSummary</term></item>
        /// <item><term>Xml</term></item>
        /// <item><term>XmlSummary</term></item>
        /// </list>
        /// </remarks>
        [Required]
        public string ReportTypes { get; set; }

        /// <summary>
        /// The verbosity.
        /// </summary>
        /// <remarks>
        /// Possible values:
        /// <list type="bullet">
        /// <item><term>Error</term></item>
        /// <item><term>Info</term></item>
        /// <item><term>Verbose</term></item>
        /// </list>
        /// </remarks>
        public string Verbosity { get; set; }

        #endregion

        #region BaseTask Members

        /// <summary>
        /// Executes the ReportUnit.
        /// </summary>
        protected override void ExecuteInternal()
        {
            var args = ArgsBuilder.By("-", ":")
                .AddPaths("reports", ResultsFiles.Select(r => r.ItemSpec), false)
                .AddPath("targetdir", ReportDir.ItemSpec)
                .AddArgument("reporttypes", ReportTypes)
                .AddArgument("verbosity", Verbosity);

            if (!Directory.Exists(ReportDir.ItemSpec))
            {
                Directory.CreateDirectory(ReportDir.ItemSpec);
            }

            var result = ExeHelper.Execute(ReportGeneratorExeFile.ItemSpec, args, true);

            Log.LogMessage(result.Output);

            if (result.ExitCode != 0)
            {
                Log.LogError(Resources.ReportGenerator_Failed, nameof(ReportGenerator),
                    string.Join(";", ResultsFiles.Select(r => r.ItemSpec)), result.ExitCode);
            }
        }

        #endregion
    }
}