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

using System.Linq;
using Heleonix.Build.Tasks;
using Heleonix.Build.Tests.Common;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace Heleonix.Build.Tests.Tasks
{
    /// <summary>
    /// Tests the <see cref="ItemSet"/>.
    /// </summary>
    public static class ItemSetTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="BaseTask.Execute"/>.
        /// </summary>
        [TestCase("Union", null, "A,B,C,D,F", ExpectedResult = "A,B,C,D,F")]
        [TestCase("Union", "A,B,C,D,F", null, ExpectedResult = "A,B,C,D,F")]
        [TestCase("Union", "A,B,C,D,F", "M,N,D,B,C,E", ExpectedResult = "A,B,C,D,F,M,N,E")]
        [TestCase("Intersection", null, "A,B,C,D,F", ExpectedResult = "")]
        [TestCase("Intersection", "A,B,C,D,F", null, ExpectedResult = "")]
        [TestCase("Intersection", "A,B,C,D,F", "M,N,D,B,C,E", ExpectedResult = "B,C,D")]
        [TestCase("RelativeComplement", null, "A,B,C,D,F", ExpectedResult = "")]
        [TestCase("RelativeComplement", "A,B,C,D,F", null, ExpectedResult = "A,B,C,D,F")]
        [TestCase("RelativeComplement", "A,B,C,D,F", "M,N,D,B,C,E", ExpectedResult = "A,F")]
        [TestCase("SymmetricDifference", null, "A,B,C,D,F", ExpectedResult = "A,B,C,D,F")]
        [TestCase("SymmetricDifference", "A,B,C,D,F", null, ExpectedResult = "A,B,C,D,F")]
        [TestCase("SymmetricDifference", "A,B,C,D,F", "M,N,D,B,C,E", ExpectedResult = "A,F,M,N,E")]
        [TestCase("Intersection", null, "A,B,C,D,F", ExpectedResult = "")]
        [TestCase("Intersection", "A,B,C,D,F", null, ExpectedResult = "")]
        [TestCase("Intersection", "A,B,C,D,F", "M,N,D,B,C,E", ExpectedResult = "B,C,D")]
        public static string Execute(string operation, string left, string right)
        {
            var task = new ItemSet
            {
                BuildEngine = new FakeBuildEngine(),
                Operation = operation,
                Left = left?.Split(',').Select(l => new TaskItem(l) as ITaskItem).ToArray(),
                Right = right?.Split(',').Select(r => new TaskItem(r) as ITaskItem).ToArray()
            };

            var succeeded = task.Execute();

            Assert.That(succeeded, Is.True);

            return string.Join(",", task.Result.Select(r => r.ItemSpec));
        }

        #endregion
    }
}