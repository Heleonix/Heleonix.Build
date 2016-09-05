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
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Build.Framework;

namespace Heleonix.Build.Tasks
{
    /// <summary>
    /// Filters items using regular expressions.
    /// </summary>
    public class ItemFilter : BaseTask
    {
        #region Properties

        /// <summary>
        /// Items to check.
        /// </summary>
        [Required]
        public ITaskItem[] Inputs { get; set; }

        /// <summary>
        /// The .NET regular expression to filter items by.
        /// </summary>
        [Required]
        public string RegEx { get; set; }

        /// <summary>
        /// The .NET regular expression options. Default is "IgnoreCase".
        /// </summary>
        public string RegExOptions { get; set; }

        /// <summary>
        /// The name of the metadata to apply filter to. Default is "FullPath".
        /// </summary>
        public string MetadataName { get; set; }

        /// <summary>
        /// Determines whether the regular expression is applied as negative expression.
        /// </summary>
        public bool Negative { get; set; }

        /// <summary>
        /// [Output] The filtered items.
        /// </summary>
        [Output]
        public ITaskItem[] Outputs { get; set; }

        #endregion

        #region BaseTask Members

        /// <summary>
        /// Filters items using regular expressions..
        /// </summary>
        protected override void ExecuteInternal()
        {
            var metadataName = string.IsNullOrEmpty(MetadataName) ? "FullPath" : MetadataName;
            var options = string.IsNullOrEmpty(RegExOptions)
                ? RegexOptions.IgnoreCase
                : (RegexOptions) Enum.Parse(typeof (RegexOptions), RegExOptions, true);

            Outputs = (from input in Inputs
                let isMatch =
                    Negative
                        ? !Regex.IsMatch(input.GetMetadata(metadataName), RegEx, options)
                        : Regex.IsMatch(input.GetMetadata(metadataName), RegEx, options)
                where isMatch
                select input).ToArray();
        }

        #endregion
    }
}