// <copyright file="NetSimulatorHelper.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Common;

using System.Xml.Linq;

public class NetSimulatorHelper
{
    public NetSimulatorHelper()
    {
        this.CopyAll(
            new DirectoryInfo(Path.Combine(PathHelper.CurrentDir, "NetSimulator")),
            new DirectoryInfo(this.SolutionDir));
    }

    public string SolutionDir { get; } = PathHelper.GetRandomFileNameInCurrentDir();

    public string SolutionFile => Path.Combine(this.SolutionDir, "NetSimulator.sln");

    public string SourceProjectDir => Path.Combine(this.SolutionDir, "src", "NetSimulator");

    public string SourceProjectFile => Path.Combine(this.SourceProjectDir, "NetSimulator.csproj");

    public IEnumerable<string> SourceDirs =>
        Directory.GetDirectories(Path.Combine(this.SourceProjectDir, "bin", PathHelper.Configuration));

    public IEnumerable<string> SourceBinaries =>
        this.SourceDirs.Select(dir => Path.Combine(dir, "NetSimulator.dll"));

    public string TestProjectDir => Path.Combine(this.SolutionDir, "test", "NetSimulator.Tests");

    public string TestProjectFile => Path.Combine(this.TestProjectDir, "NetSimulator.Tests.csproj");

    public IEnumerable<string> TestDirs =>
        Directory.GetDirectories(Path.Combine(this.TestProjectDir, "bin", PathHelper.Configuration));

    public IEnumerable<string> TestBinaries =>
        this.TestDirs.Select(dir => Path.Combine(dir, "NetSimulator.Tests.dll"));

    public IEnumerable<string> SourceProjectTargetFrameworks =>
        XDocument.Load(this.SourceProjectFile)
        .Root
        .Element("PropertyGroup")
        .Element("TargetFrameworks")
        .Value.Split(';');

    public IEnumerable<string> TestProjectTargetFrameworks =>
        XDocument.Load(this.TestProjectFile)
        .Root
        .Element("PropertyGroup")
        .Element("TargetFrameworks")
        .Value.Split(';');

    public string GetArtifactsDir(string target) =>
        Path.Combine(this.SolutionDir, "Hx_Artifacts", target.Replace("Hx_", string.Empty));

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
