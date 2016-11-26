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
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;

namespace Heleonix.Build.Tasks
{
    /// <summary>
    /// Updates a file with specified regular expression and content.
    /// </summary>
    public class FileUpdate : BaseTask
    {
        #region Properties

        /// <summary>
        /// The file path.
        /// </summary>
        [Required]
        public ITaskItem File { get; set; }

        /// <summary>
        /// The .NET regular expression to find content to replace.
        /// </summary>
        [Required]
        public string RegExp { get; set; }

        /// <summary>
        /// The .NET regular expression options.
        /// </summary>
        public string RegExpOptions { get; set; }

        /// <summary>
        /// Content to replace with.
        /// </summary>
        public string Replacement { get; set; }

        #endregion

        #region BaseTask Members

        /// <summary>
        /// Updates a file with specified regular expression and content.
        /// </summary>
        protected override void ExecuteInternal()
        {
            if (!System.IO.File.Exists(File.ItemSpec))
            {
                Log.LogMessage($"The file '{File.ItemSpec}' is not found. Stopping.");
            }

            var input = System.IO.File.ReadAllText(File.ItemSpec);

            string output;

            if (string.IsNullOrEmpty(RegExpOptions))
            {
                output = Regex.Replace(input, RegExp, Replacement ?? string.Empty);
            }
            else
            {
                output = Regex.Replace(input, RegExp, Replacement ?? string.Empty,
                    (RegexOptions) Enum.Parse(typeof(RegexOptions), RegExpOptions, true));
            }

            Log.LogMessage($"Updating file '{File.ItemSpec}'.");

            System.IO.File.WriteAllText(File.ItemSpec, output);
        }

        #endregion
    }
}