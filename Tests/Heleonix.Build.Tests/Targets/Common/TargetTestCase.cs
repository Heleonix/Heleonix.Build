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

using System.Collections.Generic;
using Microsoft.Build.Framework;

namespace Heleonix.Build.Tests.Targets.Common
{
    /// <summary>
    /// Represents test cases for targets.
    /// </summary>
    public class TargetTestCase
    {
        #region Constructors

        public TargetTestCase(IDictionary<string, string> properties, IDictionary<string, ITaskItem[]> items,
            string dependsOnTargets, bool success)
        {
            Properties = properties;
            Items = items;
            DependsOnTargets = dependsOnTargets;
            Success = success;
        }

        public TargetTestCase(IDictionary<string, string> properties,
            IDictionary<string, ITaskItem[]> items, bool success)
            : this(properties, items, null, success)
        {
        }

        public TargetTestCase(IDictionary<string, string> properties, bool success)
            : this(properties, null, null, success)
        {
        }

        public TargetTestCase(IDictionary<string, ITaskItem[]> items, bool success)
            : this(null, items, null, success)
        {
        }

        public TargetTestCase(bool success)
            : this(null, null, null, success)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        public IDictionary<string, string> Properties { get; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public IDictionary<string, ITaskItem[]> Items { get; }

        /// <summary>
        /// Gets or sets the test case result.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the targets depends on.
        /// </summary>
        public string DependsOnTargets { get; set; }

        #endregion
    }
}