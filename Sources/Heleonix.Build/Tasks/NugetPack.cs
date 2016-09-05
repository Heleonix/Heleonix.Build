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

using System;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Heleonix.Build.Tasks
{
    /// <summary>
    /// Runs the Nuget pack command.
    /// </summary>
    public class NugetPack : BaseTask
    {
        #region Properties

        /// <summary>
        /// The Nuget executable path.
        /// </summary>
        [Required]
        public ITaskItem NugetExeFile { get; set; }

        /// <summary>
        /// The nuspec file path.
        /// </summary>
        [Required]
        public ITaskItem NuspecFile { get; set; }

        /// <summary>
        /// The project file path.
        /// </summary>
        [Required]
        public ITaskItem ProjectFile { get; set; }

        /// <summary>
        /// The output package directory path.
        /// </summary>
        public ITaskItem PackageDir { get; set; }

        /// <summary>
        /// Indicates whether to include referenced projects.
        /// </summary>
        public bool IncludeReferencedProjects { get; set; }

        /// <summary>
        /// Indicates whether to exclude empty directories.
        /// </summary>
        public bool ExcludeEmptyDirectories { get; set; }

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

        /// <summary>
        /// [Output] The output package file path.
        /// </summary>
        [Output]
        public ITaskItem PackageFile { get; set; }

        #endregion

        #region BaseTask Members

        /// <summary>
        /// Executes the Nuget "pack" command.
        /// </summary>
        protected override void ExecuteInternal()
        {
            var projectDir = Path.GetDirectoryName(ProjectFile.ItemSpec) ?? string.Empty;

            var tempOutputDir = Path.Combine(projectDir, Path.GetRandomFileName());

            Directory.CreateDirectory(tempOutputDir);

            var args = ArgsBuilder.By(' ', ' ')
                .Add("pack", ProjectFile.ItemSpec, true)
                .Add("-OutputDirectory", tempOutputDir, true)
                .Add("-NonInteractive")
                .Add("-IncludeReferencedProjects", false, IncludeReferencedProjects)
                .Add("-ExcludeEmptyDirectories", false, ExcludeEmptyDirectories)
                .Add("-Verbosity", Verbosity);

            Log.LogMessage($"Packing '{ProjectFile.ItemSpec}' using '{NuspecFile.ItemSpec}'.");

            var destNuspecFilePath = Path.Combine(projectDir, Path.GetFileName(NuspecFile.ItemSpec) ?? string.Empty);

            File.Copy(NuspecFile.ItemSpec, destNuspecFilePath, true);

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
                Log.LogError($"Failed packing '{ProjectFile.ItemSpec}'. Exit code: {exitCode}.");

                return;
            }

            var srcPackage = Directory.GetFiles(tempOutputDir).First();

            var destPackageDir = PackageDir?.ItemSpec ?? projectDir;

            var destPackage = Path.Combine(destPackageDir, Path.GetFileName(srcPackage) ?? string.Empty);

            if (!Directory.Exists(destPackageDir))
            {
                Directory.CreateDirectory(destPackageDir);
            }

            File.Copy(srcPackage, destPackage);

            try
            {
                Directory.Delete(tempOutputDir, true);
            }
            catch (Exception ex)
            {
                Log.LogWarningFromException(ex);
            }

            try
            {
                File.Delete(destNuspecFilePath);
            }
            catch (Exception ex)
            {
                Log.LogWarningFromException(ex);
            }

            PackageFile = new TaskItem(destPackage);
        }

        #endregion
    }
}