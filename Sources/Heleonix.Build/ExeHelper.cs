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
    public static class ExeHelper
    {
        #region Methods

        /// <summary>
        /// Executes an executable by the specified <paramref name="exePath"/>.
        /// </summary>
        /// <param name="exePath">The execute path.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="extractOutput">Defines whether to redirect and extract standard output and errors.</param>
        /// <param name="workingDirectory">The working directory.</param>
        /// <returns>An exit result.</returns>
        public static ExeResult Execute(string exePath, string arguments, bool extractOutput = false,
            string workingDirectory = "")
        {
            using (var process = Process.Start(new ProcessStartInfo
            {
                Arguments = arguments,
                FileName = exePath,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
                WorkingDirectory = workingDirectory,
                UseShellExecute = false,
                RedirectStandardOutput = extractOutput,
                RedirectStandardError = extractOutput
            }))
            {
                if (process == null)
                {
                    return new ExeResult
                    {
                        ExitCode = int.MaxValue
                    };
                }

                var output = extractOutput ? process.StandardOutput.ReadToEnd() : null;

                var error = extractOutput ? process.StandardError.ReadToEnd() : null;

                var exited = process.WaitForExit(int.MaxValue);

                if (!exited)
                {
                    process.Kill();
                }

                return new ExeResult
                {
                    ExitCode = process.ExitCode,
                    Output = output,
                    Error = error
                };
            }
        }

        #endregion
    }
}