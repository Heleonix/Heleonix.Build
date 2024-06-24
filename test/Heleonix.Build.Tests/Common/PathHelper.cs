// <copyright file="PathHelper.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Common;

using System.Diagnostics;

public static class PathHelper
{
    public static string CurrentDir { get; } = Path.GetDirectoryName(typeof(PathHelper).Assembly.Location);

    public static string HxBuildExe { get; } = Path.Combine(CurrentDir, "hxbuild.exe");

    public static string Configuration { get; } = typeof(PathHelper).Assembly.GetCustomAttributes(false)
                .OfType<DebuggableAttribute>().Any(da => da.IsJITTrackingEnabled) ? "Debug" : "Release";

    public static string CoverletResultFile { get; } = Path.Combine(CurrentDir, "Coverlet.xml");

    public static string NUnitTestResultFile { get; } = Path.Combine(CurrentDir, "NUnitTestResult.xml");

    public static string TrxTestResultFile { get; } = Path.Combine(CurrentDir, "TrxTestResult.trx");

    public static string RunBuildProj { get; } = Path.Combine(CurrentDir, "Run.hxbproj");

    public static string SnkPairFile { get; } = Path.Combine(CurrentDir, "SnkPair.snk");

    public static string ExeMockFile { get; } = Path.Combine(CurrentDir, "Heleonix.Build.Tests.ExeMock.exe");

    public static string GetRandomFileNameInCurrentDir() => Path.Combine(CurrentDir, Path.GetRandomFileName());
}
