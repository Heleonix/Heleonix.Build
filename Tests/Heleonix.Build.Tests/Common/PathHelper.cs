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

namespace Heleonix.Build.Tests.Common
{
    /// <summary>
    /// The path helper.
    /// </summary>
    public static class PathHelper
    {
        #region Properties

        /// <summary>
        /// Gets the Nuget executable path.
        /// </summary>
        public static string NugetExePath => Path.Combine(CurrentDirectoryPath, "..", "..", "..", "..",
            "packages", "NuGet.CommandLine.3.4.3", "tools", "NuGet.exe");

        /// <summary>
        /// Gets the ReportUnit executable path.
        /// </summary>
        public static string ReportUnitExePath => Path.Combine(CurrentDirectoryPath, "..", "..", "..", "..",
            "packages", "ReportUnit.1.2.1", "tools", "ReportUnit.exe");

        /// <summary>
        /// Gets the current directory path.
        /// </summary>
        public static string CurrentDirectoryPath => AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// Gets the NUnit console executable path.
        /// </summary>
        public static string NUnitConsoleExePath => Path.Combine(CurrentDirectoryPath, "..", "..", "..", "..",
            "packages", "NUnit.ConsoleRunner.3.4.0", "tools", "nunit3-console.exe");

        /// <summary>
        /// Gets the OpenCover executable path.
        /// </summary>
        public static string OpenCoverExePath => Path.Combine(CurrentDirectoryPath, "..", "..", "..", "..",
            "packages", "OpenCover.4.6.519", "tools", "OpenCover.Console.exe");

        /// <summary>
        /// Gets the FxCop executable path.
        /// </summary>
        public static string FxCopExePath
        {
            get
            {
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32NT:
                    case PlatformID.Win32S:
                    case PlatformID.Win32Windows:
                    case PlatformID.WinCE:
                        return Path.Combine("C:\\",
                            Environment.Is64BitOperatingSystem ? "Program Files (x86)" : "Program Files",
                            "Microsoft Visual Studio 14.0", "Team Tools", "Static Analysis Tools", "FxCop",
                            "FxCopCmd.exe");
                    default:
                        throw new NotSupportedException(
                            $"Current OS platform {Environment.OSVersion.Platform} is not supported.");
                }
            }
        }

        /// <summary>
        /// Gets the Git executable path.
        /// </summary>
        public static string GitExePath => "git.exe";

        #endregion
    }
}