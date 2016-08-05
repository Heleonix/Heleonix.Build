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

using Microsoft.Build.Framework;

namespace Heleonix.Build.Tasks
{
    /// <summary>
    /// Generates HTML report using the ReportUnit tool.
    /// </summary>
    public class ReportUnit : BaseTask
    {
        #region Properties

        /// <summary>
        /// Gets or sets the ReportUnit executable path.
        /// </summary>
        [Required]
        public ITaskItem ReportUnitExePath { get; set; }

        /// <summary>
        /// Gets or sets the report file path.
        /// </summary>
        [Required]
        public ITaskItem ReportFilePath { get; set; }

        /// <summary>
        /// Gets or sets the tests results file path.
        /// </summary>
        [Required]
        public ITaskItem TestsResultsFilePath { get; set; }

        #endregion

        #region BaseTask Members

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        protected override void ExecuteInternal()
        {
            var args = ArgsBuilder.By(' ', ' ')
                .Add(TestsResultsFilePath.ItemSpec, true)
                .Add(ReportFilePath.ItemSpec, true);

            var exitCode = ExeHelper.Execute(ReportUnitExePath.ItemSpec, args);

            if (exitCode != 0)
            {
                Log.LogError(
                    $"{nameof(ReportUnit)} failed for '{TestsResultsFilePath.ItemSpec}'. Exit code: {exitCode}.");
            }
        }

        #endregion
    }
}