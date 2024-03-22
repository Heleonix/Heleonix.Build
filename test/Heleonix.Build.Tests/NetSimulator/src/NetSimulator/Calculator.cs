// <copyright file="Calculator.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace NetSimulator
{
    public static class Calculator
    {
        public static int Add(int left, int right) => left + right;

        public static int Subtract(int left, int right) => left - right;

        public static int Min(int left, int right) => left < right ? left : right;
    }
}
