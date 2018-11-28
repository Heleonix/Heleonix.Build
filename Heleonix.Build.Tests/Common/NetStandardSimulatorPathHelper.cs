// <copyright file="NetStandardSimulatorPathHelper.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Common
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Provides paths for the NET Standard Simulator.
    /// </summary>
    public static class NetStandardSimulatorPathHelper
    {
        /// <summary>
        /// Gets the solution dir.
        /// </summary>
        public static string SolutionDir => Path.Combine(PathHelper.CurrentDir, "NetStandardSimulator");

        /// <summary>
        /// Gets the solution file.
        /// </summary>
        public static string SolutionFile => Path.Combine(SolutionDir, "NetStandardSimulator.sln");

        /// <summary>
        /// Gets the source project directory.
        /// </summary>
        public static string SourceProjectDir => Path.Combine(SolutionDir, "NetStandardSimulator");

        /// <summary>
        /// Gets the source project file.
        /// </summary>
        public static string SourceProjectFile => Path.Combine(SourceProjectDir, "NetStandardSimulator.csproj");

        /// <summary>
        /// Gets the source binary directories.
        /// </summary>
        public static IEnumerable<string> SourcePublishDirs =>
            Directory
            .GetDirectories(Path.Combine(
                SourceProjectDir,
                "bin",
                PathHelper.Configuration))
            .Select(dir => Path.Combine(dir, "publish"));

        /// <summary>
        /// Gets the source binaries.
        /// </summary>
        public static IEnumerable<string> SourceBinaries =>
            SourcePublishDirs.Select(dir => Path.Combine(dir, "NetStandardSimulator.dll"));

        /// <summary>
        /// Gets the path to the Nuget package file.
        /// </summary>
        public static string NupkgFile =>
            Path.Combine(SourceProjectDir, "bin", PathHelper.Configuration, "NetStandardSimulator.1.0.0.nupkg");

        /// <summary>
        /// Gets the test project directory.
        /// </summary>
        public static string TestProjectDir => Path.Combine(SolutionDir, "NetStandardSimulator.Tests");

        /// <summary>
        /// Gets the test project file.
        /// </summary>
        public static string TestProjectFile => Path.Combine(TestProjectDir, "NetStandardSimulator.Tests.csproj");

        /// <summary>
        /// Gets the test binary directories.
        /// </summary>
        public static IEnumerable<string> TestPublishDirs =>
            Directory
            .GetDirectories(Path.Combine(
                TestProjectDir,
                "bin",
                PathHelper.Configuration))
            .Select(dir => Path.Combine(dir, "publish"));

        /// <summary>
        /// Gets the test binaries.
        /// </summary>
        public static IEnumerable<string> TestBinaries =>
            TestPublishDirs.Select(dir => Path.Combine(dir, "NetStandardSimulator.Tests.dll"));

        /// <summary>
        /// Gets the source project target frameworks.
        /// </summary>
        public static IEnumerable<string> SourceProjectTargetFrameworks =>
                    Directory
                    .GetDirectories(Path.Combine(SourceProjectDir, "bin", PathHelper.Configuration))
                    .Select(dir => Path.GetFileName(dir));

        /// <summary>
        /// Gets the test project target frameworks.
        /// </summary>
        public static IEnumerable<string> TestProjectTargetFrameworks =>
                    Directory
                    .GetDirectories(Path.Combine(TestProjectDir, "bin", PathHelper.Configuration))
                    .Select(dir => Path.GetFileName(dir));

        /// <summary>
        /// Gets an artifacts directory path for the specified targed in the artifacts directory.
        /// </summary>
        /// <param name="target">A name of a target.</param>
        /// <returns>An artifacts directory path for the specified targed in the artifacts directory.</returns>
        public static string GetArtifactDir(string target) => Path.Combine(SolutionDir, "Hx_Artifacts", target);
    }
}
