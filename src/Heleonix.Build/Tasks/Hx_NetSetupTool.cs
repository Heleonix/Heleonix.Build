// <copyright file="Hx_NetSetupTool.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

using System.IO;
using System.Reflection;

public class Hx_NetSetupTool : BaseTask
{
    [Required]
    public string Name { get; set; }

    [Required]
    public string DotnetExe { get; set; }

    public string PackageName { get; set; }

    public string Version { get; set; }

    public bool IsPackage { get; set; } = false;

    [Output]
    public string ToolPath { get; set; }

    protected override void ExecuteInternal()
    {
        var toolsDir = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Tools");

        if ("git".Equals(this.Name, StringComparison.OrdinalIgnoreCase))
        {
            this.ToolPath = "git.exe";

            return;
        }

        if ("reportgenerator".Equals(this.Name, StringComparison.OrdinalIgnoreCase))
        {
            this.PackageName = "dotnet-reportgenerator-globaltool";
            this.Version = "5.2.0";
        }

        if ("docfx".Equals(this.Name, StringComparison.OrdinalIgnoreCase))
        {
            this.PackageName = "docfx";
            this.Version = "2.74.1";
        }

        if ("extent".Equals(this.Name, StringComparison.OrdinalIgnoreCase))
        {
            this.IsPackage = true;
            this.Version = "0.0.3";
        }

        if ("NunitXml.TestLogger".Equals(this.Name, StringComparison.OrdinalIgnoreCase))
        {
            this.IsPackage = true;
            this.Version = "3.1.15";
        }

        if (this.IsPackage)
        {
            this.PackageName = this.Name;
        }

        var path = Path.Combine(toolsDir, this.Name) + ".exe";

        if (!this.IsPackage && File.Exists(path))
        {
            this.ToolPath = path;

            return;
        }

        if (this.IsPackage && !string.IsNullOrEmpty(this.Version))
        {
            path = Path.Combine(toolsDir, this.PackageName, this.Version);

            if (Directory.Exists(path))
            {
                this.ToolPath = path;

                return;
            }
        }

        if (this.IsPackage && string.IsNullOrEmpty(this.Version))
        {
            path = Hx_NetSetupTool.GetLatestVersionPath(Path.Combine(toolsDir, this.PackageName));

            if (path != null)
            {
                this.ToolPath = path;

                return;
            }
        }

        var args = ArgsBuilder.By("--", " ")
            .AddValue("add", this.IsPackage)
            .AddValue("tool", !this.IsPackage)
            .AddValue("package", this.IsPackage)
            .AddValue("install", !this.IsPackage)
            .AddValue(this.PackageName)
            .AddArgument("version", this.Version, !string.IsNullOrEmpty(this.Version))
            .AddPath("package-directory", toolsDir, this.IsPackage)
            .AddPath("tool-path", toolsDir, !this.IsPackage);

        var result = ExeHelper.Execute(this.DotnetExe, args, true, toolsDir);

        this.Log.LogMessage(MessageImportance.High, result.Output);

        if (result.ExitCode != 0)
        {
            this.Log.LogError(
                Resources.NetSetupTool_InstallationFailed,
                this.Name,
                this.PackageName,
                this.Version,
                result.ExitCode);

            this.Log.LogMessage(MessageImportance.High, args);

            this.Log.LogError(result.Error);
        }

        if (!this.IsPackage)
        {
            this.ToolPath = Path.Combine(toolsDir, this.Name) + ".exe";

            return;
        }

        if (string.IsNullOrEmpty(this.Version))
        {
            this.ToolPath = Hx_NetSetupTool.GetLatestVersionPath(Path.Combine(toolsDir, this.PackageName));

            return;
        }

        this.ToolPath = Path.Combine(toolsDir, this.PackageName, this.Version);
    }

    private static string GetLatestVersionPath(string packagePath)
    {
        var dirs = Directory.Exists(packagePath) ? Directory.GetDirectories(packagePath) : Array.Empty<string>();

        Array.Sort(dirs, StringComparer.OrdinalIgnoreCase);

        return dirs.LastOrDefault();
    }
}
