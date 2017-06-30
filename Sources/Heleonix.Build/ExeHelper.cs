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
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Heleonix.Build
{
    /// <summary>
    /// Provides functionality for working with executables.
    /// </summary>
    [ExcludeFromCodeCoverage]
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
        /// <param name="milliseconds">A number of millisecoonds to wait for process ending.
        /// Use <see cref="int.MaxValue"/> to wait infinitely.</param>
        /// <returns>An exit result.</returns>
        /// <exception cref="InvalidOperationException">
        /// No file name was specified in the <paramref name="exePath"/> property.
        /// </exception>
        /// <exception cref="Win32Exception">
        /// The <paramref name="exePath"/> could not be found.
        /// -or-
        /// An error occurred when opening the associated file.
        /// -or-
        /// The sum of the length of the arguments and the length of the full path to the process exceeds 2080.
        /// The error message associated with this exception can be one of the following:
        /// "The data area passed to a system call is too small." or "Access is denied."
        /// -or-
        /// The wait setting could not be accessed.
        /// </exception>
        /// <exception cref="IOException">An I/O error occurs during reading output or error streams.</exception>
        /// <exception cref="OutOfMemoryException">There is insufficient memory to allocate a buffer
        /// for the returned string during reading output or error streams.
        /// </exception>
        /// <exception cref="SystemException">No process <see cref="Process.Id"/> has been set, and a
        /// <see cref="Process.Handle"/> from which the <see cref="Process.Id"/> property can be determined does not exist.
        ///-or-
        /// There is no process associated with this <see cref="Process"/> object.
        ///-or-
        /// You are attempting to call <see cref="Process.WaitForExit(int)"/> for a process that is running on a remote computer.
        /// This method is available only for processes that are running on the local computer.
        /// </exception>
        public static ExeResult Execute(string exePath, string arguments, bool extractOutput, string workingDirectory,
            int milliseconds)
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
                if (process == null) return new ExeResult { ExitCode = int.MaxValue };

                var output = extractOutput ? process.StandardOutput.ReadToEnd() : null;

                var error = extractOutput ? process.StandardError.ReadToEnd() : null;

                var exited = process.WaitForExit(milliseconds);

                if (!exited) process.Kill();

                Console.WriteLine(output);

                return new ExeResult
                {
                    ExitCode = process.ExitCode,
                    Output = output,
                    Error = error
                };
            }
        }

        /// <summary>
        /// Executes an executable by the specified <paramref name="exePath"/>.
        /// </summary>
        /// <param name="exePath">The execute path.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="extractOutput">Defines whether to redirect and extract standard output and errors.</param>
        /// <returns>An exit result.</returns>
        /// <exception cref="InvalidOperationException">
        /// No file name was specified in the <paramref name="exePath"/> property.
        /// </exception>
        /// <exception cref="Win32Exception">
        /// The <paramref name="exePath"/> could not be found.
        /// -or-
        /// An error occurred when opening the associated file.
        /// -or-
        /// The sum of the length of the arguments and the length of the full path to the process exceeds 2080.
        /// The error message associated with this exception can be one of the following:
        /// "The data area passed to a system call is too small." or "Access is denied."
        /// -or-
        /// The wait setting could not be accessed.
        /// </exception>
        /// <exception cref="IOException">An I/O error occurs during reading output or error streams.</exception>
        /// <exception cref="OutOfMemoryException">
        /// There is insufficient memory to allocate a buffer
        /// for the returned string during reading output or error streams.
        /// </exception>
        /// <exception cref="SystemException">No process <see cref="Process.Id"/> has been set, and a
        /// <see cref="Process.Handle"/> from which the <see cref="Process.Id"/> property can be determined does not exist.
        ///-or-
        /// There is no process associated with this <see cref="Process"/> object.
        ///-or-
        /// You are attempting to call <see cref="Process.WaitForExit(int)"/> for a process that is running on a remote computer.
        /// This method is available only for processes that are running on the local computer.
        /// </exception>
        public static ExeResult Execute(string exePath, string arguments, bool extractOutput)
            => Execute(exePath, arguments, extractOutput, string.Empty, int.MaxValue);

        /// <summary>
        /// Executes an executable by the specified <paramref name="exePath"/>.
        /// </summary>
        /// <param name="exePath">The execute path.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="workingDirectory">The working directory.</param>
        /// <returns>An exit result.</returns>
        /// <exception cref="InvalidOperationException">
        /// No file name was specified in the <paramref name="exePath"/> property.
        /// </exception>
        /// <exception cref="Win32Exception">
        /// The <paramref name="exePath"/> could not be found.
        /// -or-
        /// An error occurred when opening the associated file.
        /// -or-
        /// The sum of the length of the arguments and the length of the full path to the process exceeds 2080.
        /// The error message associated with this exception can be one of the following:
        /// "The data area passed to a system call is too small." or "Access is denied."
        /// -or-
        /// The wait setting could not be accessed.
        /// </exception>
        /// <exception cref="SystemException">No process <see cref="Process.Id"/> has been set, and a
        /// <see cref="Process.Handle"/> from which the <see cref="Process.Id"/> property can be determined does not exist.
        ///-or-
        /// There is no process associated with this <see cref="Process"/> object.
        ///-or-
        /// You are attempting to call <see cref="Process.WaitForExit(int)"/> for a process that is running on a remote computer.
        /// This method is available only for processes that are running on the local computer.
        /// </exception>
        public static int Execute(string exePath, string arguments, string workingDirectory)
            => Execute(exePath, arguments, false, workingDirectory, int.MaxValue).ExitCode;

        /// <summary>
        /// Executes an executable by the specified <paramref name="exePath"/>.
        /// </summary>
        /// <param name="exePath">The execute path.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>An exit result.</returns>
        /// <exception cref="InvalidOperationException">
        /// No file name was specified in the <paramref name="exePath"/> property.
        /// </exception>
        /// <exception cref="Win32Exception">
        /// The <paramref name="exePath"/> could not be found.
        /// -or-
        /// An error occurred when opening the associated file.
        /// -or-
        /// The sum of the length of the arguments and the length of the full path to the process exceeds 2080.
        /// The error message associated with this exception can be one of the following:
        /// "The data area passed to a system call is too small." or "Access is denied."
        /// -or-
        /// The wait setting could not be accessed.
        /// </exception>
        /// <exception cref="SystemException">No process <see cref="Process.Id"/> has been set, and a
        /// <see cref="Process.Handle"/> from which the <see cref="Process.Id"/> property can be determined does not exist.
        ///-or-
        /// There is no process associated with this <see cref="Process"/> object.
        ///-or-
        /// You are attempting to call <see cref="Process.WaitForExit(int)"/> for a process that is running on a remote computer.
        /// This method is available only for processes that are running on the local computer.
        /// </exception>
        public static int Execute(string exePath, string arguments)
            => Execute(exePath, arguments, false, string.Empty, int.MaxValue).ExitCode;

        #endregion
    }
}