// <copyright file="MSBuildHelper.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Common;

using System.Xml.Linq;
using NUnit.Framework.Internal;

/// <summary>
/// Provides functionality to work with MSBuild.
/// </summary>
public static class MSBuildHelper
{
    /// <summary>
    /// Publishes the specified project.
    /// </summary>
    /// <param name="projectPath">The project path.</param>
    /// <param name="targetFrameworks">The target frameworks.</param>
    /// <param name="workingDirectory">The working directory.</param>
    public static void Publish(string projectPath, IEnumerable<string> targetFrameworks, string workingDirectory)
    {
        foreach (var tf in targetFrameworks)
        {
            RunTarget(projectPath, nameof(Publish), $"TargetFramework={tf}", workingDirectory);
        }
    }

    /// <summary>
    /// Runs a test target.
    /// </summary>
    /// <param name="target">A target to run.</param>
    /// <param name="workspace">A workspace to run the target in.</param>
    /// <param name="properties">Properties of the target to override or define.</param>
    /// /// <param name="items">Items of the target to override or define.</param>
    /// <returns><c>true</c> in case of success, otherwise <c>false</c>.</returns>
    public static bool RunTestTarget(
        string target,
        string workspace,
        IDictionary<string, string> properties = null,
        IDictionary<string, ITaskItem[]> items = null)
    {
        string customBuildProj = null;

        try
        {
            customBuildProj = AppendInputBuildProj(properties, items);

            var msBuildProperties = $"Hx_Input_BuildProjFile=\"{customBuildProj}\";" +
                $"Hx_Input_Configuration={PathHelper.Configuration};" +
                $"Hx_Input_Targets={target}";

            RunTarget(PathHelper.BuildProjectFile, null, msBuildProperties, workspace);

            return true;
        }
        catch (Exception ex)
        {
            TestExecutionContext.CurrentContext.OutWriter.WriteLine(ex);

            return false;
        }
        finally
        {
            if (!string.IsNullOrEmpty(customBuildProj) && File.Exists(customBuildProj))
            {
                File.Delete(customBuildProj);
            }
        }
    }

    /// <summary>
    /// Runs an MS Build target.
    /// </summary>
    /// <param name="projectPath">The project path.</param>
    /// <param name="target">The target.</param>
    /// <param name="properties">The properties.</param>
    /// <param name="workingDirectory">The working directory.</param>
    public static void RunTarget(string projectPath, string target, string properties, string workingDirectory)
    {
        var props = ArgsBuilder.By(string.Empty, "=", string.Empty, string.Empty, ";")
            .AddValue(properties)
            .AddArgument("Configuration", PathHelper.Configuration);

        var args = ArgsBuilder.By("/", ":")
            .AddPath(projectPath)
            .AddArgument("t", target)
            .AddArgument("p", props);

        var result = ExeHelper.Execute("MSBuild.exe", args, true, workingDirectory, int.MaxValue);

        if (result.ExitCode != 0)
        {
            TestExecutionContext.CurrentContext.OutWriter.WriteLine(result.Output);

            throw new InvalidOperationException(result.Output);
        }
    }

    /// <summary>
    /// Appends custom build file with properties and itesm.
    /// </summary>
    /// <param name="properties">Properties of the target to override or define.</param>
    /// /// <param name="items">Items of the target to override or define.</param>
    /// <returns>The overrides file path.</returns>
    private static string AppendInputBuildProj(
        IDictionary<string, string> properties,
        IDictionary<string, ITaskItem[]> items)
    {
        var file = XDocument.Load(PathHelper.InputBuildProj);

        var ns = XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");

        var projectElement = file.Element(ns + "Project");

        var propertyGroupElement = new XElement(ns + "PropertyGroup");

        if (properties != null)
        {
            foreach (var property in properties)
            {
                propertyGroupElement.Add(new XElement(ns + property.Key, property.Value));
            }
        }

        var itemGroupElement = new XElement(ns + "ItemGroup");

        if (items != null)
        {
            foreach (var item in items)
            {
                foreach (var value in item.Value)
                {
                    var metadata = value.CloneCustomMetadata();

                    itemGroupElement.Add(new XElement(
                        ns + item.Key,
                        new XAttribute("Include", value.ItemSpec),
                        metadata.Keys.OfType<string>().Select(name => new XElement(ns + name, metadata[name]))));
                }
            }
        }

        var targetElement = new XElement(
            ns + "Target",
            new XAttribute("Name", "Hx_Initialize_B_Overrides"),
            new XAttribute("BeforeTargets", "Hx_Initialize"),
            propertyGroupElement,
            itemGroupElement);

        projectElement.Add(targetElement);

        var filePath = Path.ChangeExtension(Path.Combine(PathHelper.CurrentDir, Path.GetRandomFileName()), ".hxbproj");

        file.Save(filePath);

        return filePath;
    }
}
