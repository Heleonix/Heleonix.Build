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
    public static string CurrentDir { get; } = Path.GetDirectoryName(typeof(PathHelper).Assembly.Location);

    /// <summary>
    /// Gets the installed Heleonix.Build package directory path.
    /// </summary>
    public static string HeleonixBuildDir { get; } = Path.Combine(CurrentDir, "Heleonix.Build");

    /// <summary>
    /// Gets the current configuration: Debug, Release.
    /// </summary>
    public static string Configuration { get; } = typeof(PathHelper).Assembly.GetCustomAttributes(false)
                .OfType<DebuggableAttribute>().Any(da => da.IsJITTrackingEnabled) ? "Debug" : "Release";

    /// <summary>
    /// Gets the OpenCover coverage results file.
    /// </summary>
    public static string OpenCoverResultFile { get; } = Path.Combine(CurrentDir, "OpenCover.xml");

    /// <summary>
    /// Gets the OpenCover coverage results file.
    /// </summary>
    public static string NUnitTestResultFile { get; } = Path.Combine(CurrentDir, "NUnitTestResult.xml");

    /// <summary>
    /// Gets the Nuget package file.
    /// </summary>
    public static string NugetPackageFile { get; } = Path.Combine(CurrentDir, "NetSimulator.1.0.0.nupkg");

    /// <summary>
    /// Gets the custom inpuy hxbproj file.
    /// </summary>
    public static string InputBuildProj { get; } = Path.Combine(CurrentDir, "Input.hxbproj");

    /// <summary>
    /// Gets the Nuget executable path.
    /// </summary>
    public static string NugetExe => Path.Combine(CurrentDir, "NuGet.CommandLine", "tools", "NuGet.exe");

    /// <summary>
    /// Gets the ReportUnit executable path.
    /// </summary>
    public static string ReportUnitExe { get; } = Path.Combine(HeleonixBuildDir, "ReportUnit.1.2.1", "tools", "ReportUnit.exe");

    /// <summary>
    /// Gets the report generator executable.
    /// </summary>
    /// <value>
    /// The report generator executable.
    /// </value>
    public static string ReportGeneratorExe { get; } = Path.Combine(HeleonixBuildDir, "ReportGenerator.5.1.19", "tools", "net6.0", "ReportGenerator.exe");

    /// <summary>
    /// Gets the NUnit console executable path.
    /// </summary>
    public static string NUnitConsoleExe { get; } = Path.Combine(HeleonixBuildDir, "NUnit.ConsoleRunner.3.16.3", "tools", "nunit3-console.exe");

    /// <summary>
    /// Gets the OpenCover executable path.
    /// </summary>
    public static string OpenCoverExe { get; } = Path.Combine(HeleonixBuildDir, "OpenCover.4.7.1221", "tools", "OpenCover.Console.exe");

    /// <summary>
    /// Gets the main project path.
    /// </summary>
    public static string BuildProjectFile { get; } = Path.Combine(
        HeleonixBuildDir,
        "Heleonix.Build." + FileVersionInfo.GetVersionInfo(typeof(PathHelper).Assembly.Location).ProductVersion,
        "Heleonix.Build.hxbproj");

    /// <summary>
    /// Gets the SNK file path to sign assemblies.
    /// </summary>
    public static string SnkPairFile { get; } = Path.Combine(CurrentDir, "SnkPair.snk");

    /// <summary>
    /// Gets the random file name in current directory.
    /// </summary>
    /// <returns>The random file name in current directory.</returns>
    public static string GetRandomFileNameInCurrentDir() => Path.Combine(CurrentDir, Path.GetRandomFileName());
}
