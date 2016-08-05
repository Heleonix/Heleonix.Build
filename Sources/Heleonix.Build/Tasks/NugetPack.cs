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
    /// Runs the Nuget "pack" command.
    /// </summary>
    public class NugetPack : BaseTask
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Nuget executable path.
        /// </summary>
        [Required]
        public ITaskItem NugetExePath { get; set; }

        /// <summary>
        /// Gets or sets the nuspec file path.
        /// </summary>
        [Required]
        public ITaskItem NuspecFilePath { get; set; }

        /// <summary>
        /// Gets or sets the project file path.
        /// </summary>
        [Required]
        public ITaskItem ProjectFilePath { get; set; }

        /// <summary>
        /// Gets or sets the output package directory path.
        /// </summary>
        public ITaskItem PackageDirectoryPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include referenced projects.
        /// </summary>
        public bool IncludeReferencedProjects { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to exclude empty directories.
        /// </summary>
        public bool ExcludeEmptyDirectories { get; set; }

        /// <summary>
        /// Gets or sets the verbosity.
        /// </summary>
        /// <remarks>
        /// Possible values:
        /// <list type="bullet">
        /// <item><description>normal</description></item>
        /// <item><description>quiet</description></item>
        /// <item><description>detailed</description></item>
        /// </list>
        /// </remarks>
        public string Verbosity { get; set; }

        /// <summary>
        /// Gets or sets the package file path.
        /// </summary>
        [Output]
        public ITaskItem PackageFilePath { get; set; }

        #endregion

        #region BaseTask Members

        /// <summary>
        /// Executes the Nuget "pack" command.
        /// </summary>
        protected override void ExecuteInternal()
        {
            var tempOutputDirectoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            Directory.CreateDirectory(tempOutputDirectoryPath);

            var args = ArgsBuilder.By(' ', ' ')
                .Add("pack", ProjectFilePath.ItemSpec, true)
                .Add("-OutputDirectory", tempOutputDirectoryPath, true)
                .Add("-NonInteractive")
                .Add("-IncludeReferencedProjects", false, IncludeReferencedProjects)
                .Add("-ExcludeEmptyDirectories", false, ExcludeEmptyDirectories)
                .Add("-Verbosity", Verbosity);

            Log.LogMessage($"Packing '{ProjectFilePath.ItemSpec}' via '{NuspecFilePath.ItemSpec}'.");

            var projectDirectoryPath = Path.GetDirectoryName(ProjectFilePath.ItemSpec);

            var destNuspecFilePath = Path.Combine(projectDirectoryPath,
                Path.ChangeExtension(Path.GetFileName(ProjectFilePath.ItemSpec), ".nuspec"));

            File.Copy(NuspecFilePath.ItemSpec, destNuspecFilePath, true);

            var exitCode = ExeHelper.Execute(NugetExePath.ItemSpec, args);

            if (exitCode != 0)
            {
                Log.LogError($"Failed packing '{ProjectFilePath.ItemSpec}'. Exit code: {exitCode}.");

                return;
            }

            var srcPackageFilePath = Directory.GetFiles(tempOutputDirectoryPath).First();

            var destPackageFilePath = Path.Combine(PackageDirectoryPath?.ItemSpec ?? projectDirectoryPath,
                Path.GetFileName(srcPackageFilePath));

            File.Copy(srcPackageFilePath, destPackageFilePath);

            try
            {
                Directory.Delete(tempOutputDirectoryPath, true);
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

            PackageFilePath = new TaskItem(destPackageFilePath);
        }

        #endregion
    }
}