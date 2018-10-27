// <copyright file="NugetPush.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks
{
    using Heleonix.Build.Properties;
    using Heleonix.Execution;
    using Microsoft.Build.Framework;

    /// <summary>
    /// Runs the Nuget push command.
    /// </summary>
    public class NugetPush : BaseTask
    {
        /// <summary>
        /// Gets or sets the Nuget executable path.
        /// </summary>
        [Required]
        public ITaskItem NugetExe { get; set; }

        /// <summary>
        /// Gets or sets the package file path.
        /// </summary>
        [Required]
        public ITaskItem PackageFile { get; set; }

        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        public string APIKey { get; set; }

        /// <summary>
        /// Gets or sets the source path.
        /// </summary>
        public ITaskItem SourceURL { get; set; }

        /// <summary>
        /// Gets or sets the configuration file path.
        /// </summary>
        public ITaskItem ConfigFile { get; set; }

        /// <summary>
        /// Gets or sets the verbosity of the command.
        /// </summary>
        /// <remarks>
        /// Possible values:
        /// <list type="bullet">
        /// <item><term>normal</term></item>
        /// <item><term>quiet</term></item>
        /// <item><term>detailed</term></item>
        /// </list>
        /// </remarks>
        public string Verbosity { get; set; }

        /// <summary>
        /// Executes the Nuget "push" command.
        /// </summary>
        protected override void ExecuteInternal()
        {
            var args = ArgsBuilder.By("-", " ")
                .AddValue("push")
                .AddPath(this.PackageFile.ItemSpec)
                .AddValue(this.APIKey)
                .AddPath("source", this.SourceURL?.ItemSpec)
                .AddPath($"ConfigFile", this.ConfigFile?.ItemSpec)
                .AddArgument($"Verbosity", this.Verbosity)
                .AddKey("NonInteractive");

            this.Log.LogMessage(Resources.NugetPush_Started, this.PackageFile.ItemSpec);

            var result = ExeHelper.Execute(this.NugetExe.ItemSpec, args, true);

            this.Log.LogMessage(result.Output);

            if (!string.IsNullOrEmpty(result.Error))
            {
                this.Log.LogError(result.Error);
            }

            if (result.ExitCode != 0)
            {
                this.Log.LogError(Resources.NugetPush_Failed, this.PackageFile.ItemSpec, result.ExitCode);
            }
        }
    }
}
