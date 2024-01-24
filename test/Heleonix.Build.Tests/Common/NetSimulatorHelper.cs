// <copyright file="NetSimulatorHelper.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Common;

using System.Xml.Linq;

/// <summary>
/// Provides paths for the NET Standard Simulator.
/// </summary>
public class NetSimulatorHelper
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NetSimulatorHelper"/> class.
    /// </summary>
    public NetSimulatorHelper()
    {
        this.CopyAll(
            new DirectoryInfo(Path.Combine(PathHelper.CurrentDir, "NetSimulator")),
            new DirectoryInfo(this.SolutionDir));
    }

    /// <summary>
    /// Gets the solution dir.
    /// </summary>
    public string SolutionDir { get; } = PathHelper.GetRandomFileNameInCurrentDir();

    /// <summary>
    /// Gets the solution file.
    /// </summary>
    public string SolutionFile => Path.Combine(this.SolutionDir, "NetSimulator.sln");

    /// <summary>
    /// Gets the source project directory.
    /// </summary>
    public string SourceProjectDir => Path.Combine(this.SolutionDir, "src", "NetSimulator");

    /// <summary>
    /// Gets the source project file.
    /// </summary>
    public string SourceProjectFile => Path.Combine(this.SourceProjectDir, "NetSimulator.csproj");

    /// <summary>
    /// Gets the source binary directories.
    /// </summary>
    public IEnumerable<string> SourceDirs =>
        Directory.GetDirectories(Path.Combine(this.SourceProjectDir, "bin", PathHelper.Configuration));

    /// <summary>
    /// Gets the source binaries.
    /// </summary>
    public IEnumerable<string> SourceBinaries =>
        this.SourceDirs.Select(dir => Path.Combine(dir, "NetSimulator.dll"));

    /// <summary>
    /// Gets the path to the Nuget package file.
    /// </summary>
    public string NupkgFile =>
        Path.Combine(this.SourceProjectDir, "bin", PathHelper.Configuration, "NetSimulator.1.0.0.nupkg");

    /// <summary>
    /// Gets the test project directory.
    /// </summary>
    public string TestProjectDir => Path.Combine(this.SolutionDir, "test", "NetSimulator.Tests");

    /// <summary>
    /// Gets the test project file.
    /// </summary>
    public string TestProjectFile => Path.Combine(this.TestProjectDir, "NetSimulator.Tests.csproj");

    /// <summary>
    /// Gets the test binary directories.
    /// </summary>
    public IEnumerable<string> TestDirs =>
        Directory.GetDirectories(Path.Combine(this.TestProjectDir, "bin", PathHelper.Configuration));

    /// <summary>
    /// Gets the test binaries.
    /// </summary>
    public IEnumerable<string> TestBinaries =>
        this.TestDirs.Select(dir => Path.Combine(dir, "NetSimulator.Tests.dll"));

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
    public string GetArtifactsDir(string target) =>
        Path.Combine(this.SolutionDir, "Hx_Artifacts", target.Replace("Hx_", string.Empty));

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
