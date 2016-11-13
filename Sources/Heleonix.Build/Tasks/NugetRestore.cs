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

using System.IO;
using System.Linq;
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
        /// The MSBuild version. If not specified, that one in your path is used, otherwise the highest installed.
        /// </summary>
        public int MsBuildVersion { get; set; }

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
            var args = ArgsBuilder.By(' ', ' ')
                .Add("restore", SolutionFile.ItemSpec, true)
                .Add("-MSBuildVersion", MsBuildVersion, false, MsBuildVersion > 0)
                .Add("-NoCache", NoCache, false, NoCache)
                .Add("-PackagesDirectory", PackagesDir?.ItemSpec
                                           ?? Path.Combine(Path.GetDirectoryName(SolutionFile.ItemSpec)
                                                           ?? "." + Path.DirectorySeparatorChar, "packages"), true,
                    PackagesDir != null)
                .Add("-Source", string.Join(";", SourcesPaths?.Select(sp => sp.ItemSpec) ?? new string[0]), true,
                    SourcesPaths != null && SourcesPaths.Length > 0)
                .Add("-Verbosity", Verbosity, false, !string.IsNullOrEmpty(Verbosity))
                .Add("-NonInteractive");

            Log.LogMessage($"Restoring '{SolutionFile.ItemSpec}'.");

            string output;
            string error;

            var exitCode = ExeHelper.Execute(NugetExeFile.ItemSpec, args, out output, out error);

            Log.LogMessage(output);

            if (!string.IsNullOrEmpty(error))
            {
                Log.LogError(error);
            }

            if (exitCode != 0)
            {
                Log.LogError($"Failed restoring '{SolutionFile.ItemSpec}'. Exit code: {exitCode}.");
            }
        }

        #endregion
    }
}