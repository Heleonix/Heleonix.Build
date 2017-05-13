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
using static System.FormattableString;

namespace Heleonix.Build.Tests.Common
{
    /// <summary>
    /// The MSBuild helper.
    /// </summary>
    public static class MSBuildHelper
    {
        #region Methods

        /// <summary>
        /// Executes the MSBuild.
        /// </summary>
        /// <param name="projectPath">The project path.</param>
        /// <param name="target">The build target.</param>
        /// <param name="properties">The build properties.</param>
        /// <returns>The exit code.</returns>
        /// <exception cref="NotSupportedException">Current OS platform is not supported.</exception>
        public static int ExecuteMSBuild(string projectPath, string target, string properties)
        {
            var props = ArgsBuilder.By(string.Empty, "=", string.Empty, string.Empty, ";")
                .AddValue(properties)
                .AddArgument("Configuration", CurrentConfiguration);

            var args = ArgsBuilder.By("/", ":")
                .AddPath(projectPath)
                .AddArgument("t", target)
                .AddArgument("p", props);

            return ExeHelper.Execute(MSBuildExe, args);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current configuration.
        /// </summary>
        public static string CurrentConfiguration => Path.GetFileName(SystemPath.CurrentDir);

        /// <summary>
        /// Gets the MSBuild version.
        /// </summary>
        public static int MSBuildVersion => 14;

        /// <summary>
        /// Gets the MSBuild executable path.
        /// </summary>
        /// <exception cref="NotSupportedException">Current OS platform is not supported.</exception>
        public static string MSBuildExe
        {
            get
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32NT:
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                    case PlatformID.WinCE:
                        return Path.Combine(Environment.Is64BitOperatingSystem
                                ? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
                                : Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                            "Microsoft Visual Studio", "2017", "Community", "MSBuild", "15.0", "Bin", "MSBuild.exe");
                    default:
                        throw new NotSupportedException(
                            Invariant($"Current OS platform {Environment.OSVersion.Platform} is not supported."));
                }
            }
        }

        #endregion
    }
}