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
using Heleonix.Build.Tasks;
using Heleonix.Build.Tests.Common;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace Heleonix.Build.Tests.Tasks
{
    /// <summary>
    /// Tests te <see cref="BaseTask"/>.
    /// </summary>
    public static class BaseTaskTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="BaseTask.Execute"/>.
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        public static void Execute(bool throwException)
        {
            var mock = new Mock<BaseTask> { CallBase = true };

            mock.Object.BuildEngine = new FakeBuildEngine();

            if (throwException)
            {
                mock.Protected().Setup("ExecuteInternal").Throws<InvalidOperationException>();
            }
            else
            {
                mock.Protected().Setup("ExecuteInternal");
            }

            var result = mock.Object.Execute();

            Assert.That(result, Is.Not.EqualTo(throwException));

            mock.Protected().Verify("ExecuteInternal", Times.Once());

            Assert.That(mock.Object.Log.HasLoggedErrors, Is.EqualTo(throwException));
        }

        #endregion
    }
}