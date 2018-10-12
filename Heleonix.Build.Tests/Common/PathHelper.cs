// <copyright file="PathHelper.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Common
{
    using System;
    using System.IO;

    /// <summary>
    /// The path helper.
    /// </summary>
    public static class PathHelper
    {
        /// <summary>
        /// Gets the current directory path.
        /// </summary>
        public static string CurrentDir => Path.GetDirectoryName(typeof(PathHelper).Assembly.Location);

        /// <summary>
        /// Gets the current configuration: Debug, Release.
        /// </summary>
        public static string Configuration => Path.GetFileName(Path.GetDirectoryName(CurrentDir));

        /// <summary>
        /// Gets the OpenCover coverage results file.
        /// </summary>
        public static string OpenCoverResultFile => Path.Combine(CurrentDir, "OpenCover.xml");

        /// <summary>
        /// Gets the OpenCover coverage results file.
        /// </summary>
        public static string NUnitTestResultFile => Path.Combine(CurrentDir, "NUnitTestResult.xml");

        /// <summary>
        /// Gets the OpenCover coverage results file.
        /// </summary>
        public static string CustomBuildProj => Path.Combine(CurrentDir, "Custom.hxbproj");

        /// <summary>
        /// Gets the Nuget executable path.
        /// </summary>
        public static string NugetExe => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".nuget",
            "packages",
            "NuGet.CommandLine",
            "4.7.1",
            "tools",
            "NuGet.exe");

        /// <summary>
        /// Gets the ReportUnit executable path.
        /// </summary>
        public static string ReportUnitExe => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".nuget",
            "packages",
            "ReportUnit",
            "1.2.1",
            "tools",
            "ReportUnit.exe");

        /// <summary>
        /// Gets the report generator executable.
        /// </summary>
        /// <value>
        /// The report generator executable.
        /// </value>
        public static string ReportGeneratorExe => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".nuget",
            "packages",
            "ReportGenerator",
            "3.1.2",
            "tools",
            "ReportGenerator.exe");

        /// <summary>
        /// Gets the main project path.
        /// </summary>
        public static string BuildProjectFile => Path.Combine(CurrentDir, "Heleonix.Build.hxbproj");

        /// <summary>
        /// Gets the NUnit console executable path.
        /// </summary>
        public static string NUnitConsoleExe => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".nuget",
            "packages",
            "NUnit.ConsoleRunner",
            "3.8.0",
            "tools",
            "nunit3-console.exe");

        /// <summary>
        /// Gets the OpenCover executable path.
        /// </summary>
        public static string OpenCoverExe => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".nuget",
            "packages",
            "OpenCover",
            "4.6.519",
            "tools",
            "OpenCover.Console.exe");

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
        /// Gets the random file in current directory.
        /// </summary>
        /// <returns>The random file in current directory.</returns>
        public static string GetRandomFileInCurrentDir() =>
            Path.Combine(CurrentDir, Path.GetRandomFileName());
    }
}
