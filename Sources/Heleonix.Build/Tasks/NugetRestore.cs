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
    /// Runs the Nuget restore command.
    /// </summary>
    public class NugetRestore : BaseTask
    {
        #region Properties

        /// <summary>
        /// The Nuget executable path.
        /// </summary>
        [Required]
        public ITaskItem NugetExeFile { get; set; }

        /// <summary>
        /// The solution file to restore packages for.
        /// </summary>
        [Required]
        public ITaskItem SolutionFile { get; set; }

        /// <summary>
        /// The packages directory where restore packages to.
        /// </summary>
        /// <remarks>
        /// Default is the "packages" directory inside the solution's directory.
        /// </remarks>
        public ITaskItem PackagesDir { get; set; }

        /// <summary>
        /// The sources paths to load packages from.
        /// </summary>
        public ITaskItem[] SourcesPaths { get; set; }

        /// <summary>
        /// Specifies the directory of MSBuild to use with the command, taking precedence over MSBuildVersion.
        /// </summary>
        public ITaskItem MSBuildDir { get; set; }

        /// <summary>
        /// Indicates whether disable using machine cache as the first package source.
        /// </summary>
        public bool NoCache { get; set; }

        /// <summary>
        /// The verbosity of the command.
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

        #endregion

        #region BaseTask Members

        /// <summary>
        /// Executes the Nuget "push" command.
        /// </summary>
        protected override void ExecuteInternal()
        {
            var args = ArgsBuilder.By("-", " ")
                .AddValue("restore")
                .AddPath(SolutionFile.ItemSpec)
                .AddPath("MSBuildPath", MSBuildDir.ItemSpec)
                .AddArgument("NoCache", NoCache, NoCache)
                .AddPath("PackagesDirectory",
                    PackagesDir?.ItemSpec ?? Path.Combine(Path.GetDirectoryName(SolutionFile.ItemSpec)
                                                          ?? "." + Path.DirectorySeparatorChar, "packages"))
                .AddPaths("Source", SourcesPaths?.Select(sp => sp.ItemSpec), false)
                .AddArgument("Verbosity", Verbosity)
                .AddKey("NonInteractive");

            Log.LogMessage(Resources.NugetRestore_Started, SolutionFile.ItemSpec);

            var result = ExeHelper.Execute(NugetExeFile.ItemSpec, args, true);

            Log.LogMessage(result.Output);

            if (!string.IsNullOrEmpty(result.Error))
            {
                Log.LogError(result.Error);
            }

            if (result.ExitCode != 0)
            {
                Log.LogError(Resources.NugetRestore_Failed, SolutionFile.ItemSpec, result.ExitCode);
            }
        }

        #endregion
    }
}