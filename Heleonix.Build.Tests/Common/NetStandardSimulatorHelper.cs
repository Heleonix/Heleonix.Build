// <copyright file="NetStandardSimulatorHelper.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Common
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Provides paths for the NET Standard Simulator.
    /// </summary>
    public class NetStandardSimulatorHelper
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NetStandardSimulatorHelper"/> class.
        /// </summary>
        public NetStandardSimulatorHelper()
        {
            this.CopyAll(
                new DirectoryInfo(Path.Combine(PathHelper.CurrentDir, "NetStandardSimulator")),
                new DirectoryInfo(this.SolutionDir));

            var gitDir = Directory.CreateDirectory(Path.Combine(this.SolutionDir, ".git"));

            File.WriteAllText(
                Path.Combine(gitDir.FullName, "config"),
                "[remote \"origin\"] url = https://github.com/Heleonix/Heleonix.Build.git");
            File.WriteAllText(Path.Combine(gitDir.FullName, "HEAD"), "ref: refs/heads/develop");

            Directory.CreateDirectory(Path.Combine(gitDir.FullName, "objects"));
            Directory.CreateDirectory(Path.Combine(gitDir.FullName, "refs"));
        }

        /// <summary>
        /// Gets the solution dir.
        /// </summary>
        public string SolutionDir { get; } = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        /// <summary>
        /// Gets the solution file.
        /// </summary>
        public string SolutionFile => Path.Combine(this.SolutionDir, "NetStandardSimulator.sln");

        /// <summary>
        /// Gets the source project directory.
        /// </summary>
        public string SourceProjectDir => Path.Combine(this.SolutionDir, "NetStandardSimulator");

        /// <summary>
        /// Gets the source project file.
        /// </summary>
        public string SourceProjectFile => Path.Combine(this.SourceProjectDir, "NetStandardSimulator.csproj");

        /// <summary>
        /// Gets the source binary directories.
        /// </summary>
        public IEnumerable<string> SourcePublishDirs =>
            Directory
            .GetDirectories(Path.Combine(
                this.SourceProjectDir,
                "bin",
                PathHelper.Configuration))
            .Select(dir => Path.Combine(dir, "publish"));

        /// <summary>
        /// Gets the source binaries.
        /// </summary>
        public IEnumerable<string> SourceBinaries =>
            this.SourcePublishDirs.Select(dir => Path.Combine(dir, "NetStandardSimulator.dll"));

        /// <summary>
        /// Gets the path to the Nuget package file.
        /// </summary>
        public string NupkgFile =>
            Path.Combine(this.SourceProjectDir, "bin", PathHelper.Configuration, "NetStandardSimulator.1.0.0.nupkg");

        /// <summary>
        /// Gets the test project directory.
        /// </summary>
        public string TestProjectDir => Path.Combine(this.SolutionDir, "NetStandardSimulator.Tests");

        /// <summary>
        /// Gets the test project file.
        /// </summary>
        public string TestProjectFile => Path.Combine(this.TestProjectDir, "NetStandardSimulator.Tests.csproj");

        /// <summary>
        /// Gets the test binary directories.
        /// </summary>
        public IEnumerable<string> TestPublishDirs =>
            Directory
            .GetDirectories(Path.Combine(
                this.TestProjectDir,
                "bin",
                PathHelper.Configuration))
            .Select(dir => Path.Combine(dir, "publish"));

        /// <summary>
        /// Gets the test binaries.
        /// </summary>
        public IEnumerable<string> TestBinaries =>
            this.TestPublishDirs.Select(dir => Path.Combine(dir, "NetStandardSimulator.Tests.dll"));

        /// <summary>
        /// Gets the source project target frameworks.
        /// </summary>
        public IEnumerable<string> SourceProjectTargetFrameworks =>
            XDocument.Load(this.SourceProjectFile)
            .Root
            .Element("PropertyGroup")
            .Element("TargetFrameworks")
            .Value.Split(';');

        /// <summary>
        /// Gets the test project target frameworks.
        /// </summary>
        public IEnumerable<string> TestProjectTargetFrameworks =>
            XDocument.Load(this.TestProjectFile)
            .Root
            .Element("PropertyGroup")
            .Element("TargetFrameworks")
            .Value.Split(';');

        /// <summary>
        /// Gets an artifacts directory path for the specified target in the artifacts directory.
        /// </summary>
        /// <param name="target">A name of a target.</param>
        /// <returns>An artifacts directory path for the specified targed in the artifacts directory.</returns>
        public string GetArtifactDir(string target) => Path.Combine(this.SolutionDir, "Hx_Artifacts", target);

        /// <summary>
        /// Clears resources.
        /// </summary>
        public void Clear()
        {
            Directory.Delete(this.SolutionDir, true);
        }

        private void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);

                this.CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}
