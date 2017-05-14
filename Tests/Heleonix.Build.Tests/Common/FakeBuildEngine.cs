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

using System.Collections;
using Microsoft.Build.Framework;

namespace Heleonix.Build.Tests.Common
{
    /// <summary>
    /// Fakes the <see cref="IBuildEngine"/>
    /// </summary>
    /// <seealso cref="IBuildEngine" />
    public class FakeBuildEngine : IBuildEngine
    {
        #region IBuildEngine Members

        /// <summary>
        /// Raises an error event to all registered loggers.
        /// </summary>
        /// <param name="e">The event data.</param>
        public void LogErrorEvent(BuildErrorEventArgs e)
        {
        }

        /// <summary>
        /// Raises a warning event to all registered loggers.
        /// </summary>
        /// <param name="e">The event data.</param>
        public void LogWarningEvent(BuildWarningEventArgs e)
        {
        }

        /// <summary>
        /// Raises a message event to all registered loggers.
        /// </summary>
        /// <param name="e">The event data.</param>
        public void LogMessageEvent(BuildMessageEventArgs e)
        {
        }

        /// <summary>
        /// Raises a custom event to all registered loggers.
        /// </summary>
        /// <param name="e">The event data.</param>
        public void LogCustomEvent(CustomBuildEventArgs e)
        {
        }

        /// <summary>
        /// Initiates a build of a project file. If the build is successful, the outputs,
        /// if any, of the specified targets are returned.
        /// </summary>
        /// <param name="projectFileName">The name of the project file to build.</param>
        /// <param name="targetNames">The names of the target in the project to build.
        /// Separate multiple targets with a semicolon (;).</param>
        /// <param name="globalProperties">An <see cref="T:System.Collections.IDictionary" /> of additional
        /// global properties to apply to the project. The key and value must be String data types.</param>
        /// <param name="targetOutputs">The outputs of each specified target.</param>
        /// <returns>
        /// true if the build was successful; otherwise, false.
        /// </returns>
        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties,
            IDictionary targetOutputs) => true;

        /// <summary>
        /// Returns true if the ContinueOnError flag was set to true for this particular task in the project file.
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

        #endregion
    }
}