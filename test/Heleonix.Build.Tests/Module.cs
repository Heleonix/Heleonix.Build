// <copyright file="Module.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Heleonix.Build.Tests.Common;

/// <summary>
/// Initializes the assembly.
/// </summary>
internal static class Module
{
    /// <summary>
    /// Initializes the assembly.
    /// </summary>
    [ModuleInitializer]
    internal static void Initialize()
    {
        Directory.CreateDirectory(PathHelper.HeleonixBuildDir);

        var args = ArgsBuilder.By("-", " ")
            .AddValue("install")
            .AddValue("Heleonix.Build")
            .AddArgument("Version", FileVersionInfo.GetVersionInfo(typeof(PathHelper).Assembly.Location).ProductVersion)
            .AddPath("Source", PathHelper.CurrentDir);

        ExeHelper.Execute(PathHelper.NugetExe, args, true, PathHelper.HeleonixBuildDir);
    }
}
