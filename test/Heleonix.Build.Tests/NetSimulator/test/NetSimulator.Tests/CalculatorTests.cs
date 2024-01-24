// <copyright file="CalculatorTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace NetSimulator.Tests
{
    using NetSimulator;
    using NUnit.Framework;

    /// <summary>
    /// Tests the <see cref="Calculator"/>.
    /// </summary>
    public static class CalculatorTests
    {
        /// <summary>
        /// Tests the <see cref="Calculator.Min"/>.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>Returns minimal value.</returns>
        [TestCase(1, 2, ExpectedResult = 1)]
#pragma warning disable S2699 // Tests should include assertions
        public static int Min(int left, int right)
#pragma warning restore S2699 // Tests should include assertions
        {
            return Calculator.Min(left, right);
        }

        /// <summary>
        /// Tests the <see cref="Calculator.Add"/>.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>The result.</returns>
        [TestCase(1, 2, ExpectedResult = 3)]
#pragma warning disable S2699 // Tests should include assertions
        public static int Add(int left, int right)
#pragma warning restore S2699 // Tests should include assertions
        {
            return Calculator.Add(left, right);
        }

        /// <summary>
        /// Tests ignorance.
        /// </summary>
        /// <param name="value">The value.</param>
        [Ignore("Tests ignorance")]
        public static void Add(int value)
        {
            Calculator.Add(value, value);
        }
    }
}
