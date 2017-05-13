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

using System.Text.RegularExpressions;
using Heleonix.Build.Tasks;
using Heleonix.Build.Tests.Common;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace Heleonix.Build.Tests.Tasks
{
    /// <summary>
    /// Tests the <see cref="ItemFilter"/>.
    /// </summary>
    public static class ItemFilterTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="ItemFilter.Execute"/>.
        /// </summary>
        [TestCase(@"^.+\.Tests\.dll$", false)]
        [TestCase(@"^.+\.Tests\.dll$", true)]
        public static void Execute(string regex, bool negative)
        {
            var task = new ItemFilter
            {
                BuildEngine = new FakeBuildEngine(),
                Inputs = new[] { new TaskItem("Product.Tests.dll"), new TaskItem("Product.dll") as ITaskItem },
                RegExp = regex,
                MetadataName = "FullPath",
                Negative = negative,
                RegExpOptions = RegexOptions.IgnoreCase.ToString()
            };

            var succeeded = task.Execute();

            Assert.That(succeeded, Is.True);
            Assert.That(task.Outputs, Has.Length.EqualTo(1));
        }

        #endregion
    }
}