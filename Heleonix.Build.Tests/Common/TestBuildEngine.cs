// <copyright file="TestBuildEngine.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Common
{
    using System.Collections;
    using Microsoft.Build.Framework;

    /// <summary>
    /// Fakes the <see cref="IBuildEngine"/>.
    /// </summary>
    /// <seealso cref="IBuildEngine" />
    public class TestBuildEngine : IBuildEngine
    {
        /// <summary>
        /// Gets a value indicating whether the ContinueOnError flag was set to true
        /// for this particular task in the project file.
        /// </summary>
        public bool ContinueOnError => true;

        /// <summary>
        /// Gets the line number of the task node within the project file that called it.
        /// </summary>
        public int LineNumberOfTaskNode => 0;

        /// <summary>
        /// Gets the line number of the task node within the project file that called it.
        /// </summary>
        public int ColumnNumberOfTaskNode => 0;

        /// <summary>
        /// Gets the full path to the project file that contained the call to this task.
        /// </summary>
        public string ProjectFileOfTaskNode => string.Empty;

        /// <summary>
        /// Raises an error event to all registered loggers.
        /// </summary>
        /// <param name="e">The event data.</param>
        public void LogErrorEvent(BuildErrorEventArgs e)
        {
            // Dummy implementation.
        }

        /// <summary>
        /// Raises a warning event to all registered loggers.
        /// </summary>
        /// <param name="e">The event data.</param>
        public void LogWarningEvent(BuildWarningEventArgs e)
        {
            // Dummy implementation.
        }

        /// <summary>
        /// Raises a message event to all registered loggers.
        /// </summary>
        /// <param name="e">The event data.</param>
        public void LogMessageEvent(BuildMessageEventArgs e)
        {
            // Dummy implementation.
        }

        /// <summary>
        /// Raises a custom event to all registered loggers.
        /// </summary>
        /// <param name="e">The event data.</param>
        public void LogCustomEvent(CustomBuildEventArgs e)
        {
            // Dummy implementation.
        }

        /// <summary>
        /// Initiates a build of a project file. If the build is successful, the outputs,
        /// if any, of the specified targets are returned.
        /// </summary>
        /// <param name="projectFileName">The name of the project file to build.</param>
        /// <param name="targetNames">The names of the target in the project to build.
        /// Separate multiple targets with a semicolon (;).</param>
        /// <param name="globalProperties">An <see cref="System.Collections.IDictionary" /> of additional
        /// global properties to apply to the project. The key and value must be String data types.</param>
        /// <param name="targetOutputs">The outputs of each specified target.</param>
        /// <returns>
        /// true if the build was successful; otherwise, false.
        /// </returns>
        public bool BuildProjectFile(
            string projectFileName,
            string[] targetNames,
            IDictionary globalProperties,
            IDictionary targetOutputs) => true;
    }
}
