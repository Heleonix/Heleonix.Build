// <copyright file="TestModule.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

using System.Diagnostics;

[assembly: Parallelizable(ParallelScope.Fixtures)]

/// <summary>
/// Sets up and cleans up the assembly.
/// </summary>
[SetUpFixture]
#pragma warning disable S3903 // Types should be defined in named namespaces
public static class TestModule
#pragma warning restore S3903 // Types should be defined in named namespaces
{
    /// <summary>
    /// Sets up the assembly.
    /// </summary>
    [OneTimeSetUp]
    public static void Setup()
    {
        Directory.CreateDirectory(PathHelper.HeleonixBuildDir);

        var args = ArgsBuilder.By("-", " ")
            .AddValue("install")
            .AddValue("Heleonix.Build")
            .AddArgument("Version", FileVersionInfo.GetVersionInfo(typeof(PathHelper).Assembly.Location).ProductVersion)
            .AddPath("Source", PathHelper.CurrentDir);

        ExeHelper.Execute(PathHelper.NugetExe, args, true, PathHelper.HeleonixBuildDir);
    }

    /// <summary>
    /// Cleans up the assembly.
    /// </summary>
    [OneTimeTearDown]
    public static void Cleanup()
    {
        Directory.Delete(PathHelper.HeleonixBuildDir, true);
    }
}
