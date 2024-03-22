// <copyright file="TokenizerTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace NetSimulator.Tests
{
    using NUnit.Framework;

    public static class TokenizerTests
    {
        [TestCase("1,2,3", ExpectedResult = new[] { "1", "2", "3" })]
        public static string[] Split(string source) => Tokenizer.Split(source, ',');
    }
}
