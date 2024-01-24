// <copyright file="Calculator.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace NetSimulator
{
    /// <summary>
    /// A class.
    /// </summary>
    public static class Calculator
    {
        /// <summary>
        /// Adds two numbers.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>Addition of two numbers.</returns>
        public static int Add(int left, int right) => left + right;

        /// <summary>
        /// Subtracts two numbers.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>Subtraction of two numbers.</returns>
        public static int Subtract(int left, int right) => left - right;

        /// <summary>
        /// Finds minimal of two numbers.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>Minimal of two numbers.</returns>
        public static int Min(int left, int right) => left < right ? left : right;
    }
}
