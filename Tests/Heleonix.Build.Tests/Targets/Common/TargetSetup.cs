/*
The MIT License (MIT)

Copyright (c) 2015-present Heleonix - Hennadii Lutsyshyn

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Heleonix.Build.Tests.Common;
using Microsoft.Build.Framework;

namespace Heleonix.Build.Tests.Targets.Common
{
    /// <summary>
    /// Provides setup functionality for targets tests.
    /// </summary>
    public static class TargetSetup
    {
        #region Methods

        /// <summary>
        /// Sets up an overrides file.
        /// </summary>
        /// <param name="targetName">The name of the target.</param>
        /// <param name="testCase">The target test case.</param>
        /// <returns>The overrides file path.</returns>
        public static string Overrides(string targetName, TargetTestCase testCase)
        {
            if (testCase == null)
            {
                throw new ArgumentNullException(nameof(testCase));
            }

            var overrides = new XDocument(new XDeclaration("1.0", "UTF-8", null));

            var ns = XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");

            var project = new XElement(ns + "Project");

            overrides.Add(project);

            Action<XElement, IDictionary<string, string>> addProperties = delegate(XElement group,
                IDictionary<string, string> properties)
            {
                foreach (var property in properties)
                {
                    group.Add(new XElement(ns + property.Key, property.Value));
                }
            };

            Action<XElement, IDictionary<string, ITaskItem[]>> addItems = delegate(XElement group,
                IDictionary<string, ITaskItem[]> items)
            {
                foreach (var item in items)
                {
                    foreach (var value in item.Value)
                    {
                        var metadata = value.CloneCustomMetadata();
                        group.Add(new XElement(ns + item.Key, new XAttribute("Include", value.ItemSpec),
                            metadata.Keys.OfType<string>().Select(name => new XElement(ns + name, metadata[name]))));
                    }
                }
            };

            var propertyGroup = new XElement(ns + "PropertyGroup");

            if (testCase.Properties?.Count > 0)
            {
                addProperties(propertyGroup, testCase.Properties);
            }

            var itemGroup = new XElement(ns + "ItemGroup");

            if (testCase.Items?.Count > 0)
            {
                addItems(itemGroup, testCase.Items);
            }

            var target = new XElement(ns + "Target", new XAttribute("Name", targetName + "-B-Overrides"),
                new XAttribute("BeforeTargets", targetName), propertyGroup, itemGroup);

            project.Add(target);

            var overridesFilePath = Path.ChangeExtension(
                Path.Combine(SystemPath.CurrentDir, Path.GetRandomFileName()), ".proj");

            overrides.Save(overridesFilePath);

            return overridesFilePath;
        }

        /// <summary>
        /// Sets up properties for targets tests.
        /// </summary>
        /// <param name="targetName">The name of the target.</param>
        /// <param name="ciType">The continuous integration system type.</param>
        /// <param name="simulatorType">The type of the simulator.</param>
        /// <param name="overridesFilePath"></param>
        /// <param name="testCase">The target test case.</param>
        /// <returns>The properties.</returns>
        public static string InputProperties(string targetName, CIType ciType, SimulatorType simulatorType,
            string overridesFilePath, TargetTestCase testCase)
        {
            var props = ArgsBuilder.By(string.Empty, "=", string.Empty, "\"", ";")
                .AddPath("Hxb-In-Flow", string.Join(";", testCase?.DependsOnTargets, targetName).Trim(';'))
                .AddArgument("Hxb-In-Configuration", MSBuildHelper.CurrentConfiguration)
                .AddPath("Hxb-In-Overrides", overridesFilePath);

            switch (ciType)
            {
                case CIType.Jenkins:
                    props.AddArgument("JENKINS_URL", "http://localhost:8080")
                        .AddArgument("BRANCH_NAME", "1.2.3")
                        .AddArgument("BUILD_NUMBER", 123);
                    switch (simulatorType)
                    {
                        case SimulatorType.Library:
                            props.AddPath("WORKSPACE", LibSimulatorPath.SolutionDir);
                            break;
                    }
                    break;
                case CIType.TeamCity:
                    props.AddArgument("TEAMCITY_VERSION", "10.0")
                        .AddArgument("teamcity_build_branch", "1.2.3")
                        .AddArgument("BUILD_NUMBER", 123);
                    switch (simulatorType)
                    {
                        case SimulatorType.Library:
                            props.AddPath("system_agent_work_dir", LibSimulatorPath.SolutionDir);
                            break;
                    }
                    break;

                case CIType.GoCD:
                    props.AddArgument("GO_PIPELINE_COUNTER", "123")
                        .AddArgument("GO_SCM_CURRENT_BRANCH_SOURCE_NAME", "GO_SCM_HELEONIX_BUILD_CURRENT_BRANCH")
                        .AddArgument("GO_SCM_HELEONIX_BUILD_CURRENT_BRANCH", "1.2.3");
                    switch (simulatorType)
                    {
                        case SimulatorType.Library:
                            props.AddPath("system_agent_work_dir", LibSimulatorPath.SolutionDir);
                            break;
                    }
                    break;
            }

            return props;
        }

        #endregion
    }
}