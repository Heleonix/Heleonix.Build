// <copyright file="MSBuildHelper.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using Heleonix.Execution;
    using Microsoft.Build.Framework;

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
                Execute(projectPath, nameof(Publish), $"TargetFramework={tf}", workingDirectory);
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
                customBuildProj = AppendCustomBuildProj(target, properties, items);

                var msBuildProperties = $"WORKSPACE={workspace};" +
                    "JENKINS_URL=http://localhot:1000;" +
                    $"Hx_Input_BuildProjFile={customBuildProj};" +
                    $"Hx_Input_Configuration={PathHelper.Configuration};" +
                    $"Hx_Input_Targets={target}";

                Execute(PathHelper.BuildProjectFile, null, msBuildProperties, workspace);

                return true;
            }
            catch (Exception ex)
            {
#pragma warning disable S1481 // Unused local variables should be removed
                var a = ex;
#pragma warning restore S1481 // Unused local variables should be removed
                return false;
            }
            finally
            {
                if (!string.IsNullOrEmpty(customBuildProj) && File.Exists(customBuildProj))
                {
#pragma warning disable SG0018 // Path traversal
                    File.Delete(customBuildProj);
#pragma warning restore SG0018 // Path traversal
                }
            }
        }

        /// <summary>
        /// Executes the ms build.
        /// </summary>
        /// <param name="projectPath">The project path.</param>
        /// <param name="target">The target.</param>
        /// <param name="properties">The properties.</param>
        /// <param name="workingDirectory">The working directory.</param>
        public static void Execute(string projectPath, string target, string properties, string workingDirectory)
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
                throw new InvalidOperationException($"MSBuild Failed: {result.Output}");
            }
        }

        /// <summary>
        /// Appends custom build file with properties and itesm.
        /// </summary>
        /// <param name="target">The name of the target.</param>
        /// <param name="properties">Properties of the target to override or define.</param>
        /// /// <param name="items">Items of the target to override or define.</param>
        /// <returns>The overrides file path.</returns>
        private static string AppendCustomBuildProj(
            string target,
            IDictionary<string, string> properties,
            IDictionary<string, ITaskItem[]> items)
        {
            var file = XDocument.Load(PathHelper.CustomBuildProj);

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
                new XAttribute("Name", target + "_B_Overrides"),
                new XAttribute("BeforeTargets", target),
                propertyGroupElement,
                itemGroupElement);

            projectElement.Add(targetElement);

            var filePath = Path.ChangeExtension(Path.Combine(PathHelper.CurrentDir, Path.GetRandomFileName()), ".hxbproj");

            file.Save(filePath);

            return filePath;
        }
    }
}
