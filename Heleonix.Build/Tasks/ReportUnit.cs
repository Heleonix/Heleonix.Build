// <copyright file="ReportUnit.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks
{
    using Heleonix.Execution;
    using Microsoft.Build.Framework;

    /// <summary>
    /// Generates HTML report using the ReportUnit tool.
    /// </summary>
    public class ReportUnit : BaseTask
    {
        /// <summary>
        /// Gets or sets the ReportUnit executable path.
        /// </summary>
        [Required]
        public ITaskItem ReportUnitExe { get; set; }

        /// <summary>
        /// Gets or sets the tests result file path.
        /// </summary>
        [Required]
        public ITaskItem TestResultFile { get; set; }

        /// <summary>
        /// Gets or sets the report file path.
        /// </summary>
        [Required]
        public ITaskItem ReportFile { get; set; }

        /// <summary>
        /// Executes the ReportUnit.
        /// </summary>
        protected override void ExecuteInternal()
        {
            var args = ArgsBuilder.By(string.Empty, string.Empty)
                .AddPath(this.TestResultFile.ItemSpec)
                .AddPath(this.ReportFile.ItemSpec);

            var result = ExeHelper.Execute(this.ReportUnitExe.ItemSpec, args, true);

            this.Log.LogMessage(result.Output);
        }
    }
}
