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
    /// The system paths.
    /// </summary>
    public static class SystemPath
    {
        #region Properties

        /// <summary>
        /// Gets the current directory path.
        /// </summary>
        public static string CurrentDir => Path.GetDirectoryName(typeof(SystemPath).Assembly.Location);

        /// <summary>
        /// Gets the Nuget executable path.
        /// </summary>
        public static string NugetExe => Path.Combine(CurrentDir, "..", "..", "..", "..",
            "packages", "NuGet.CommandLine.4.1.0", "tools", "NuGet.exe");

        /// <summary>
        /// Gets the ReportUnit executable path.
        /// </summary>
        public static string ReportUnitExe => Path.Combine(CurrentDir, "..", "..", "..", "..",
            "packages", "ReportUnit.1.2.1", "tools", "ReportUnit.exe");

        public static string ReportGeneratorExe => Path.Combine(CurrentDir, "..", "..", "..", "..",
            "packages", "ReportGenerator.2.5.8", "tools", "ReportGenerator.exe");

        /// <summary>
        /// Gets the main project path.
        /// </summary>
        public static string MainProjectFile => Path.Combine(CurrentDir, "Heleonix.Build.Projects.Main.proj");

        /// <summary>
        /// Gets the NUnit console executable path.
        /// </summary>
        public static string NUnitConsoleExe => Path.Combine(CurrentDir, "..", "..", "..", "..",
            "packages", "NUnit.ConsoleRunner.3.6.1", "tools", "nunit3-console.exe");

        /// <summary>
        /// Gets the OpenCover executable path.
        /// </summary>
        public static string OpenCoverExe => Path.Combine(CurrentDir, "..", "..", "..", "..",
            "packages", "OpenCover.4.6.519", "tools", "OpenCover.Console.exe");

        /// <summary>
        /// Gets the FxCop executable path.
        /// </summary>
        public static string FxCopExe
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
                            "Microsoft Visual Studio", "2017", "Community", "Team Tools", "Static Analysis Tools",
                            "FxCop", "FxCopCmd.exe");
                    default:
                        throw new NotSupportedException(
                            Invariant($"Current OS platform {Environment.OSVersion.Platform} is not supported."));
                }
            }
        }

        /// <summary>
        /// Gets the FxCop dictionary file.
        /// </summary>
        public static string FxCopDictionaryFile => Path.Combine(FxCopDir, "CustomDictionary.xml");

        /// <summary>
        /// Gets the FxCop rules directory.
        /// </summary>
        public static string FxCopRulesDir => Path.Combine(FxCopDir, "Rules");

        /// <summary>
        /// Gets the FxCop rule sets file.
        /// </summary>
        public static string FxCopRulesetFile => Path.Combine(FxCopDir, "..", "Rule Sets", "AllRules.ruleset");

        /// <summary>
        /// Gets the Git executable path.
        /// </summary>
        public static string GitExe => "git.exe";

        /// <summary>
        /// Gets the Svn executable path.
        /// </summary>
        public static string SvnExe => "svn.exe";

        /// <summary>
        /// Gets the SvnAdmin executable path.
        /// </summary>
        public static string SvnAdminExe => "svnadmin.exe";

        /// <summary>
        /// Gets the executable mock path.
        /// </summary>
        public static string ExeMock => Path.Combine(CurrentDir, "..", "..", "..", "Heleonix.Build.Tests.ExeMock",
            "bin", MSBuildHelper.CurrentConfiguration, "Heleonix.Build.Tests.ExeMock.exe");

        /// <summary>
        /// Gets the SNK pair file path.
        /// </summary>
        public static string SnkPairFile => Path.Combine(CurrentDir, "Common", "SnkPair.snk");

        /// <summary>
        /// Gets the FxCop report file path.
        /// </summary>
        public static string FxCopReportFile => Path.Combine(CurrentDir, "Common", "FxCopReport.xml");

        /// <summary>
        /// Gets the FxCop project file path.
        /// </summary>
        public static string FxCopProjectFile => Path.Combine(CurrentDir, "Common", "FxCopProject.FxCop");

        /// <summary>
        /// Gets the FxCop directory.
        /// </summary>
        private static string FxCopDir => Path.GetDirectoryName(FxCopExe);

        #endregion
    }
}