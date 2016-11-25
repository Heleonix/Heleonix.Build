/*
The MIT License (MIT)

Copyright (c) 2015-2016 Heleonix - Hennadii Lutsyshyn

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
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace Heleonix.Build.Tests.Targets
{
    /// <summary>
    /// The base test class fro all targets tests.
    /// </summary>
    public abstract class TargetTests
    {
        #region Fields

        private readonly IDictionary<string, ITaskItem[]> _systemItems = new Dictionary<string, ITaskItem[]>
        {
            { "Hxb-System-NugetExe", new ITaskItem[] { new TaskItem(PathHelper.NugetExe) } },
            { "Hxb-System-NUnitConsoleExe", new ITaskItem[] { new TaskItem(PathHelper.NUnitConsoleExe) } },
            { "Hxb-System-OpenCoverConsoleExe", new ITaskItem[] { new TaskItem(PathHelper.OpenCoverExe) } },
            { "Hxb-System-ReportGeneratorExe", new ITaskItem[] { new TaskItem(PathHelper.ReportGeneratorExe) } },
            { "Hxb-System-ReportUnitExe", new ITaskItem[] { new TaskItem(PathHelper.ReportUnitExe) } }
        };

        #endregion

        #region Methods

        /// <summary>
        /// Executes a test.
        /// </summary>
        /// <param name="ciType">The continuous integration system type.</param>
        /// <param name="testCases">The test cases.</param>
        protected void ExecuteTest(CIType ciType, TargetTestCase testCases)
        {
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

            if (testCases.Properties?.Count > 0)
            {
                addProperties(propertyGroup, testCases.Properties);
            }

            var itemGroup = new XElement(ns + "ItemGroup");

            if (testCases.Items?.Count > 0)
            {
                addItems(itemGroup, testCases.Items);
            }

            var target = new XElement(ns + "Target", new XAttribute("Name", TargetName + "-Before-Overrides"),
                new XAttribute("BeforeTargets", TargetName), propertyGroup, itemGroup);

            project.Add(target);

            var systemItemGroup = new XElement(ns + "ItemGroup");

            addItems(systemItemGroup, _systemItems);

            var systemTarget = new XElement(ns + "Target", new XAttribute("Name", "Hxb-Initialize-Before-Overrides"),
                new XAttribute("BeforeTargets", "Hxb-Initialize"), systemItemGroup);

            project.Add(systemTarget);

            var overridesFilePath = Path.ChangeExtension(
                Path.Combine(PathHelper.CurrentDir, Path.GetRandomFileName()), ".proj");

            overrides.Save(overridesFilePath);

            var props = ArgsBuilder.By(';', '=')
                .Add("Hxb-In-Flow", testCases.DependsOnTargets + ";" + TargetName, true)
                .Add("Hxb-In-Configuration", MsBuildHelper.CurrentConfiguration)
                .Add("Hxb-In-Overrides", overridesFilePath, true)
                .Add("BUILD_NUMBER", "123");

            switch (ciType)
            {
                case CIType.Jenkins:
                    props.Add("JENKINS_URL", "http://localhost:8080")
                        .Add("BRANCH_NAME", "1.2");

                    switch (SimulatorType)
                    {
                        case SimulatorType.Library:
                            props.Add("WORKSPACE", LibSimulatorHelper.SolutionDir, true);
                            break;
                    }

                    break;
                case CIType.TeamCity:
                    props.Add("TEAMCITY_VERSION", "10.0")
                        .Add("teamcity_build_branch", "1.2");

                    switch (SimulatorType)
                    {
                        case SimulatorType.Library:
                            props.Add("system_agent_work_dir", LibSimulatorHelper.SolutionDir, true);
                            break;
                    }

                    break;
            }

            try
            {
                var succeeded = MsBuildHelper.ExecuteMsBuild(PathHelper.MainProject, null, props) == 0;

                Assert.That(succeeded, Is.EqualTo(testCases.Result));
            }
            finally
            {
                File.Delete(overridesFilePath);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the type of the simulator.
        /// </summary>
        protected abstract SimulatorType SimulatorType { get; }

        /// <summary>
        /// Gets the name of the target.
        /// </summary>
        protected abstract string TargetName { get; }

        #endregion

        #region Classes

        /// <summary>
        /// Represents test cases for targets.
        /// </summary>
        public class TargetTestCase
        {
            #region Properties

            /// <summary>
            /// Gets or sets the properties.
            /// </summary>
            public IDictionary<string, string> Properties { get; set; }

            /// <summary>
            /// Gets or sets the items.
            /// </summary>
            public IDictionary<string, ITaskItem[]> Items { get; set; }

            /// <summary>
            /// Gets or sets the test case result.
            /// </summary>
            public bool Result { get; set; }

            /// <summary>
            /// Gets or sets the targets depends on.
            /// </summary>
            public string DependsOnTargets { get; set; }

            #endregion
        }

        #endregion
    }
}