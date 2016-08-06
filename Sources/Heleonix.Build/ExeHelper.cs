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

using System.Diagnostics;

namespace Heleonix.Build
{
    /// <summary>
    /// Provides functionality for working with executables.
    /// </summary>
    internal static class ExeHelper
    {
        #region Methods

        /// <summary>
        /// Executes an executable by the specified <paramref name="exePath"/>.
        /// </summary>
        /// <param name="exePath">The execute path.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="workingDirectory">The working directory.</param>
        /// <returns>An exit code.</returns>
        public static int Execute(string exePath, string arguments, string workingDirectory = "")
        {
            var process = Process.Start(new ProcessStartInfo
            {
                Arguments = arguments,
                FileName = exePath,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = workingDirectory
            });

            var exited = process.WaitForExit(int.MaxValue);

            if (!exited)
            {
                process.Kill();
            }

            return process.ExitCode;
        }

        /// <summary>
        /// Executes an executable by the specified <paramref name="exePath"/>.
        /// </summary>
        /// <param name="exePath">The execute path.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="output">Output string for the <see cref="Process.StandardOutput"/>.</param>
        /// <param name="workingDirectory">The working directory.</param>
        /// <returns>An exit code.</returns>
        public static int Execute(string exePath, string arguments, out string output, string workingDirectory = "")
        {
            var process = Process.Start(new ProcessStartInfo
            {
                Arguments = arguments,
                FileName = exePath,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = true
            });

            output = process.StandardOutput.ReadToEnd();

            var exited = process.WaitForExit(int.MaxValue);

            if (!exited)
            {
                process.Kill();
            }

            return process.ExitCode;
        }

        #endregion
    }
}