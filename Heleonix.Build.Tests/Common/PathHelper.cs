// <copyright file="PathHelper.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Common
{
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

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
        public static string Configuration => typeof(PathHelper).Assembly.GetCustomAttributes(false)
                    .OfType<DebuggableAttribute>().Any(da => da.IsJITTrackingEnabled) ? "Debug" : "Release";

        /// <summary>
        /// Gets the OpenCover coverage results file.
        /// </summary>
        public static string OpenCoverResultFile => Path.Combine(CurrentDir, "OpenCover.xml");

        /// <summary>
        /// Gets the OpenCover coverage results file.
        /// </summary>
        public static string NUnitTestResultFile => Path.Combine(CurrentDir, "NUnitTestResult.xml");

        /// <summary>
        /// Gets the Nuget package file.
        /// </summary>
        public static string NugetPackageFile => Path.Combine(CurrentDir, "NetStandardSimulator.1.0.0.nupkg");

        /// <summary>
        /// Gets the OpenCover coverage results file.
        /// </summary>
        public static string CustomBuildProj => Path.Combine(CurrentDir, "Custom.hxbproj");

        /// <summary>
        /// Gets the Nuget executable path.
        /// </summary>
        public static string NugetExe => Path.Combine(CurrentDir, "NuGet.CommandLine", "tools", "NuGet.exe");

        /// <summary>
        /// Gets the ReportUnit executable path.
        /// </summary>
        public static string ReportUnitExe => Path.Combine(CurrentDir, "ReportUnit", "tools", "ReportUnit.exe");

        /// <summary>
        /// Gets the report generator executable.
        /// </summary>
        /// <value>
        /// The report generator executable.
        /// </value>
        public static string ReportGeneratorExe => Path.Combine(CurrentDir, "ReportGenerator", "tools", "net6.0", "ReportGenerator.exe");

        /// <summary>
        /// Gets the NUnit console executable path.
        /// </summary>
        public static string NUnitConsoleExe => Path.Combine(CurrentDir, "NUnit.ConsoleRunner", "tools", "nunit3-console.exe");

        /// <summary>
        /// Gets the OpenCover executable path.
        /// </summary>
        public static string OpenCoverExe => Path.Combine(CurrentDir, "OpenCover", "tools", "OpenCover.Console.exe");

        /// <summary>
        /// Gets the main project path.
        /// </summary>
        public static string BuildProjectFile => Path.Combine(CurrentDir, "Heleonix.Build.hxbproj");

        /// <summary>
        /// Generates the random file in current directory.
        /// </summary>
        /// <returns>The random file in current directory.</returns>
        public static string GenerateRandomFileInCurrentDir() => Path.Combine(CurrentDir, Path.GetRandomFileName());
    }
}
