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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Heleonix.Build.Tests.Common;
using Microsoft.Build.Framework;
using NUnit.Framework;

namespace Heleonix.Build.Tests.Targets
{
    /// <summary>
    /// The base test class fro all targets tests.
    /// </summary>
    public abstract class TargetTests
    {
        #region Methods

        /// <summary>
        /// Executes a test.
        /// </summary>
        /// <param name="ciType">The continuous integration system type.</param>
        /// <param name="testCases">The test cases.</param>
        protected void ExecuteTest(CIType ciType, TargetTestCase testCases)
        {
            var ns = XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");

            var propertyGroup = new XElement(ns + "PropertyGroup");

            if (testCases.Properties?.Count > 0)
            {
                foreach (var property in testCases.Properties)
                {
                    propertyGroup.Add(new XElement(ns + property.Key, property.Value));
                }
            }

            var itemGroup = new XElement(ns + "ItemGroup");

            if (testCases.Items?.Count > 0)
            {
                foreach (var item in testCases.Items)
                {
                    foreach (var value in item.Value)
                    {
                        var metadata = value.CloneCustomMetadata();
                        itemGroup.Add(new XElement(ns + item.Key, new XAttribute("Include", value.ItemSpec),
                            metadata.Keys.OfType<string>().Select(name => new XElement(ns + name, metadata[name]))));
                    }
                }
            }

            var overridesFile = new XDocument(new XDeclaration("1.0", "UTF-8", null));

            overridesFile.Add(new XElement(ns + "Project", propertyGroup, itemGroup));

            var overridesFilePath = Path.Combine(PathHelper.CurrentDirectoryPath, Path.GetRandomFileName());

            overridesFile.Save(overridesFilePath);

            var props = ArgsBuilder.By(';', '=')
                .Add("Hxb-In-Flow", TargetName)
                .Add("Hxb-In-Configuration", MsBuildHelper.CurrentConfiguration)
                .Add("Hxb-In-Overrides", overridesFilePath, true);

            switch (ciType)
            {
                case CIType.Jenkins:
                    props.Add("JENKINS_URL", "http://localhost:8080");

                    switch (SimulatorType)
                    {
                        case SimulatorType.Library:
                            props.Add("WORKSPACE", LibSimulatorHelper.SolutionDirectoryPath, true);
                            break;
                    }

                    break;
                case CIType.TeamCity:
                    props.Add("TEAMCITY_VERSION", "10.0");

                    switch (SimulatorType)
                    {
                        case SimulatorType.Library:
                            props.Add("system_agent_work_dir", LibSimulatorHelper.SolutionDirectoryPath, true);
                            break;
                    }

                    break;
            }

            try
            {
                var succeeded = MsBuildHelper.ExecuteMsBuild(PathHelper.MainProjectPath, null, props) == 0;

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

            #endregion
        }

        #endregion
    }
}