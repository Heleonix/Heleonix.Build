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
    /// Gets the installed Heleonix.Build executable file path.
    /// </summary>
    public static string HxBuildExe { get; } = Path.Combine(CurrentDir, "hxbuild.exe");

    /// <summary>
    /// Gets the current configuration: Debug, Release.
    /// </summary>
    public static string Configuration { get; } = typeof(PathHelper).Assembly.GetCustomAttributes(false)
                .OfType<DebuggableAttribute>().Any(da => da.IsJITTrackingEnabled) ? "Debug" : "Release";

    /// <summary>
    /// Gets the Coverlet coverage results file.
    /// </summary>
    public static string CoverletResultFile { get; } = Path.Combine(CurrentDir, "Coverlet.xml");

    /// <summary>
    /// Gets the Coverlet coverage results file.
    /// </summary>
    public static string NUnitTestResultFile { get; } = Path.Combine(CurrentDir, "NUnitTestResult.xml");

    /// <summary>
    /// Gets the custom inpuy hxbproj file.
    /// </summary>
    public static string RunBuildProj { get; } = Path.Combine(CurrentDir, "Run.hxbproj");

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
