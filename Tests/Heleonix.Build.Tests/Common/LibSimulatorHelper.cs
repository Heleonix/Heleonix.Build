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

using System.IO;

namespace Heleonix.Build.Tests.Common
{
    /// <summary>
    /// The library simulator helper.
    /// </summary>
    public static class LibSimulatorHelper
    {
        #region Properties

        /// <summary>
        /// Gets the out file path.
        /// </summary>
        public static string OutFilePath => Path.Combine(OutDirectoryPath,
            Path.GetFileName(SolutionDirectoryPath) + ".dll");

        /// <summary>
        /// Gets the tests out file path.
        /// </summary>
        public static string TestsOutFilePath => Path.Combine(
            Path.GetDirectoryName(TestsProjectFilePath) ?? string.Empty, "bin", MsBuildHelper.CurrentConfiguration,
            Path.ChangeExtension(Path.GetFileName(TestsProjectFilePath), ".dll"));

        /// <summary>
        /// Gets the name of the solution.
        /// </summary>
        public static string SolutionName => Path.GetFileName(SolutionDirectoryPath);

        /// <summary>
        /// Gets the solution file path.
        /// </summary>
        public static string SolutionFilePath => Path.Combine(SolutionDirectoryPath,
            Path.GetFileName(SolutionDirectoryPath) + ".sln");

        /// <summary>
        /// Gets the project file path.
        /// </summary>
        public static string ProjectFilePath => Path.Combine(SolutionDirectoryPath,
            "Sources", Path.GetFileName(SolutionDirectoryPath) ?? string.Empty,
            Path.GetFileName(SolutionDirectoryPath) + ".csproj");

        /// <summary>
        /// Gets the tests project file path.
        /// </summary>
        public static string TestsProjectFilePath => Path.Combine(SolutionDirectoryPath,
            "Tests", Path.GetFileName(SolutionDirectoryPath) + ".Tests",
            Path.GetFileName(SolutionDirectoryPath) + ".Tests.csproj");

        /// <summary>
        /// Gets the out directory path.
        /// </summary>
        public static string OutDirectoryPath => Path.Combine(SolutionDirectoryPath, "Sources",
            Path.GetFileName(SolutionDirectoryPath), "bin",
            Path.GetFileNameWithoutExtension(PathHelper.CurrentDirectoryPath) ?? string.Empty);

        /// <summary>
        /// Gets the solution directory path.
        /// </summary>
        public static string SolutionDirectoryPath => Path.Combine(PathHelper.CurrentDirectoryPath,
            "..", "..", "..", "Heleonix.Build.Tests.LibSimulator");

        /// <summary>
        /// Gets the reports directory path.
        /// </summary>
        public static string ReportsDirectoryPath => Path.Combine(SolutionDirectoryPath, "Reports");

        #endregion
    }
}