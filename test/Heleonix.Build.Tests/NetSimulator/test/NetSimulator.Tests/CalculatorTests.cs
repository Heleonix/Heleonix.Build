// <copyright file="CalculatorTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace NetSimulator.Tests
{
    using NetSimulator;
    using NUnit.Framework;

    public static class CalculatorTests
    {
        [TestCase(1, 2, ExpectedResult = 1)]
#pragma warning disable S2699 // Tests should include assertions
        public static int Min(int left, int right)
#pragma warning restore S2699 // Tests should include assertions
        {
            return Calculator.Min(left, right);
        }

        [TestCase(1, 2, ExpectedResult = 3)]
#pragma warning disable S2699 // Tests should include assertions
        public static int Add(int left, int right)
#pragma warning restore S2699 // Tests should include assertions
        {
            return Calculator.Add(left, right);
        }

        [Ignore("Tests ignorance")]
        public static void Add(int value)
        {
            Calculator.Add(value, value);
        }
    }
}
