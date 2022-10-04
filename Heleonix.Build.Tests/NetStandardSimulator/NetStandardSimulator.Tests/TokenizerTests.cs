// <copyright file="TokenizerTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace NetStandardSimulator.Tests
{
    using NUnit.Framework;

    /// <summary>
    /// Tests the <see cref="Tokenizer"/>.
    /// </summary>
    public static class TokenizerTests
    {
        /// <summary>
        /// Tests the <see cref="Tokenizer.Split"/>.
        /// </summary>
        /// <param name="source">A source string to split.</param>
        /// <returns>Splitted strings.</returns>
        [TestCase("1,2,3", ExpectedResult = new[] { "1", "2", "3" })]
        public static string[] Split(string source) => Tokenizer.Split(source, ',');
    }
}
