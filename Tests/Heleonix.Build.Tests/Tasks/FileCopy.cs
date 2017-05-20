///*
//The MIT License (MIT)

//Copyright (c) 2015-present Heleonix - Hennadii Lutsyshyn

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//*/

//using System.IO;
//using System.Text.RegularExpressions;
//using Heleonix.Build.Tasks;
//using Heleonix.Build.Tests.Common;
//using Microsoft.Build.Utilities;
//using NUnit.Framework;

//namespace Heleonix.Build.Tests.Tasks
//{
//    /// <summary>
//    /// Tests the <see cref="FileCopy"/>.
//    /// </summary>
//    public static class FileCopyTests
//    {
//        #region Tests

//        /// <summary>
//        /// Tests the <see cref="BaseTask.Execute"/>.
//        /// </summary>
//        [Test]
//        public static void Execute()
//        {
//            var input = Path.Combine(SystemPath.CurrentDir, Path.GetRandomFileName());

//            File.WriteAllText(input,
//                @"[assembly: System.Reflection.AssemblyVersion(""1.0.0.0"")]
//[assembly: System.Reflection.AssemblyFileVersion(""1.0.0.0"")]
//[assembly: System.Reflection.AssemblyInformationalVersion(""1.0.0.0"")]");

//            try
//            {
//                var task = new FileUpdate
//                {
//                    BuildEngine = new FakeBuildEngine(),
//                    File = new TaskItem(input),
//                    RegExp = "\\(.*\\)",
//                    Replacement = "(1.2.3.4)"
//                };

//                var succeeded = task.Execute();

//                var result = File.ReadAllText(input);

//                Assert.That(succeeded, Is.True);
//                Assert.That(Regex.Matches(result, "\\(.*\\)"), Has.Count.EqualTo(3));
//            }
//            finally
//            {
//                File.Delete(input);
//            }
//        }

//        #endregion
//    }
//}