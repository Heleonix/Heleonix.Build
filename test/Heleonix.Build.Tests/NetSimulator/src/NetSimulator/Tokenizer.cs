// <copyright file="Tokenizer.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace NetSimulator
{
    public static class Tokenizer
    {
        public static string[] Split(string source, char separator)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            return source.Split(separator);
#pragma warning restore CA1062 // Validate arguments of public methods
        }
    }
}
