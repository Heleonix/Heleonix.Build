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

using System;
using System.IO;
using System.Linq;
using Heleonix.Build.Properties;
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
        /// The configuration to create package from: Debug, Release etc.
        /// </summary>
        [Required]
        public string Configuration { get; set; }

        /// <summary>
        /// The output package directory path.
        /// </summary>
        public ITaskItem PackageDir { get; set; }

        /// <summary>
        /// Additional properties to pass to the .nuspec file, in format: token1=value1;token2="value2".
        /// </summary>
        public string Properties { get; set; }

        /// <summary>
        /// Indicates whether to include referenced projects.
        /// </summary>
        public bool IncludeReferencedProjects { get; set; }

        /// <summary>
        /// Indicates whether to exclude empty directories.
        /// </summary>
        public bool ExcludeEmptyDirectories { get; set; }

        /// <summary>
        /// Specifies the directory of MSBuild to use with the command, taking precedence over MSBuildVersion".
        /// </summary>
        public ITaskItem MSBuildDir { get; set; }

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

            var props = ArgsBuilder.By(string.Empty, "=", string.Empty, string.Empty, ";")
                .AddArgument("Configuration", Configuration);

            var args = ArgsBuilder.By("-", " ")
                .AddValue("pack")
                .AddPath(ProjectFile.ItemSpec)
                .AddPath("OutputDirectory", tempOutputDir)
                .AddPath("MSBuildPath", MSBuildDir.ItemSpec)
                .AddKey("IncludeReferencedProjects", IncludeReferencedProjects)
                .AddKey("ExcludeEmptyDirectories", ExcludeEmptyDirectories)
                .AddArgument("Verbosity", Verbosity)
                .AddKey("NonInteractive")
                .AddArgument("Properties", string.Join(";", props, Properties).Trim(';'));

            var nuspecFilePath = new Uri(NuspecFile.ItemSpec).LocalPath;

            Log.LogMessage(Resources.NugetPack_Started, ProjectFile.ItemSpec, nuspecFilePath);

            var destNuspecFilePath = new Uri(Path.Combine(projectDir, Path.GetFileName(nuspecFilePath))).LocalPath;

            if (!string.Equals(nuspecFilePath, destNuspecFilePath, StringComparison.OrdinalIgnoreCase))
            {
                File.Copy(nuspecFilePath, destNuspecFilePath, true);
            }

            var result = ExeHelper.Execute(NugetExeFile.ItemSpec, args, true);

            Log.LogMessage(result.Output);

            if (!string.IsNullOrEmpty(result.Error))
            {
                Log.LogError(result.Error);
            }

            if (result.ExitCode != 0)
            {
                Log.LogError(Resources.NugetPack_Failed, ProjectFile.ItemSpec, result.ExitCode);

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
                if (!string.Equals(nuspecFilePath, destNuspecFilePath, StringComparison.OrdinalIgnoreCase))
                {
                    File.Delete(destNuspecFilePath);
                }
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