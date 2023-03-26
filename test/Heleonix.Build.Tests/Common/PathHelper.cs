// <copyright file="PathHelper.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Common;

using System.Diagnostics;

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
    public static string NugetPackageFile => Path.Combine(CurrentDir, "NetSimulator.1.0.0.nupkg");

    /// <summary>
    /// Gets the OpenCover coverage results file.
    /// </summary>
    public static string CustomBuildProj => Path.Combine(CurrentDir, "Custom.hxbproj");

    /// <summary>
    /// Gets the Nuget executable path.
    /// </summary>
    public static string NugetExe => Path.Combine(CurrentDir, "NuGet.CommandLine.6.4.0", "tools", "NuGet.exe");

    /// <summary>
    /// Gets the Git executable path.
    /// </summary>
    public static string GitExe => Path.Combine(CurrentDir, "GitForWindows.2.39.2", "tools", "cmd", "git.exe");

    /// <summary>
    /// Gets the ReportUnit executable path.
    /// </summary>
    public static string ReportUnitExe => Path.Combine(CurrentDir, "ReportUnit.1.2.1", "tools", "ReportUnit.exe");

    /// <summary>
    /// Gets the report generator executable.
    /// </summary>
    /// <value>
    /// The report generator executable.
    /// </value>
    public static string ReportGeneratorExe => Path.Combine(CurrentDir, "ReportGenerator.5.1.19", "tools", "net6.0", "ReportGenerator.exe");

    /// <summary>
    /// Gets the NUnit console executable path.
    /// </summary>
    public static string NUnitConsoleExe => Path.Combine(CurrentDir, "NUnit.ConsoleRunner.3.16.3", "tools", "nunit3-console.exe");

    /// <summary>
    /// Gets the OpenCover executable path.
    /// </summary>
    public static string OpenCoverExe => Path.Combine(CurrentDir, "OpenCover.4.7.1221", "tools", "OpenCover.Console.exe");

    /// <summary>
    /// Gets the main project path.
    /// </summary>
    public static string BuildProjectFile => Path.Combine(
        CurrentDir,
        "Heleonix.Build." + FileVersionInfo.GetVersionInfo(typeof(PathHelper).Assembly.Location).ProductVersion,
        "Heleonix.Build.hxbproj");

    /// <summary>
    /// Gets the SNK file path to sign assemblies.
    /// </summary>
    public static string SnkPairFile => Path.Combine(CurrentDir, "SnkPair.snk");

    /// <summary>
    /// Gets the random file name in current directory.
    /// </summary>
    /// <returns>The random file name in current directory.</returns>
    public static string GetRandomFileNameInCurrentDir() => Path.Combine(CurrentDir, Path.GetRandomFileName());
}
