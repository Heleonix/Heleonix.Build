﻿/*
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

using System.Linq;
using Heleonix.Build.Tasks;
using Heleonix.Build.Tests.Common;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;
using static System.FormattableString;

namespace Heleonix.Build.Tests.Tasks
{
    /// <summary>
    /// Tests the <see cref="ItemCombine"/>.
    /// </summary>
    public static class ItemCombineTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="BaseTask.Execute"/>.
        /// </summary>
        [TestCase("Pairing", "A,B,C", "M,N,D,B,C", null, ExpectedResult = "A-M,B-N,C-D")]
        [TestCase("Pairing", "A,B,C,D,F", "M,N,D", null, ExpectedResult = "A-M,B-N,C-D")]
        [TestCase("CrossProduct", "A,B,C", "M,N,D", "Identity", ExpectedResult = "A-M,A-N,A-D,B-M,B-N,B-D,C-M,C-N,C-D")]
        [TestCase("InvalidOperation", null, null, null, ExpectedResult = "")]
        public static string Execute(string operation, string left, string right, string sourceMetadataName)
        {
            var task = new ItemCombine
            {
                BuildEngine = new FakeBuildEngine(),
                Operation = operation,
                TargetMetadataName = "Right",
                SourceMetadataName = sourceMetadataName,
                Left = left?.Split(',').Select(l => new TaskItem(l) as ITaskItem).ToArray(),
                Right = right?.Split(',').Select(r => new TaskItem(r) as ITaskItem).ToArray()
            };

            var succeeded = task.Execute();

            Assert.That(succeeded, Is.EqualTo(operation != "InvalidOperation"));

            return string.Join(",", task.Result?.Select(r => Invariant($"{r.ItemSpec}-{r.GetMetadata("Right")}"))
                                    ?? new string[0]);
        }

        #endregion
    }
}