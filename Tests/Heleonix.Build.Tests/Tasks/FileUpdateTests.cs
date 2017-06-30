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

using System.IO;
using Heleonix.Build.Tasks;
using Heleonix.Build.Tests.Common;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace Heleonix.Build.Tests.Tasks
{
    /// <summary>
    /// Tests the <see cref="FileUpdate"/>.
    /// </summary>
    public static class FileUpdateTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="BaseTask.Execute"/>.
        /// </summary>
        [TestCase(true, "CultureInvariant", "Ver(0.0.0.0)", "(1.2.3.4)", "\\(.*\\)", ExpectedResult = "Ver(1.2.3.4)")]
        [TestCase(false, null, null, null, null, ExpectedResult = "")]
        [TestCase(true, null, "Ver(0.0.0.0)", "(1.2.3.4)", "\\(.*\\)", ExpectedResult = "Ver(1.2.3.4)")]
        [TestCase(true, "", "Ver(0.0.0.0)", "(1.2.3.4)", "\\(.*\\)", ExpectedResult = "Ver(1.2.3.4)")]
        [TestCase(true, null, "Ver(0.0.0.0)", null, "\\(.*\\)", ExpectedResult = "Ver")]
        [TestCase(true, "CultureInvariant", "Ver(0.0.0.0)", "(1.2.3.4)", "\\(.*\\)", ExpectedResult = "Ver(1.2.3.4)")]
        [TestCase(true, "CultureInvariant", "Ver(0.0.0.0)", null, "\\(.*\\)", ExpectedResult = "Ver")]
        public static string Execute(bool fileExists, string options, string text, string replacement, string regex)
        {
            var input = Path.Combine(SystemPath.CurrentDir, Path.GetRandomFileName());

            if (fileExists)
            {
                File.WriteAllText(input, text);
            }

            try
            {
                var task = new FileUpdate
                {
                    BuildEngine = new FakeBuildEngine(),
                    File = new TaskItem(input),
                    RegExp = regex,
                    Replacement = replacement,
                    RegExpOptions = options
                };

                var succeeded = task.Execute();

                Assert.That(succeeded, Is.True);

                return fileExists ? File.ReadAllText(input) : string.Empty;
            }
            finally
            {
                if (fileExists)
                {
                    File.Delete(input);
                }
            }
        }

        #endregion
    }
}