// <copyright file="Tokenizer.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace NetStandardSimulator
{
    /// <summary>
    /// Represents simple .Net Standard class.
    /// </summary>
    public static class Tokenizer
    {
        /// <summary>
        /// Splits a <paramref name="source"/> by a <paramref name="separator"/>.
        /// </summary>
        /// <param name="source">String to split.</param>
        /// <param name="separator">Separator to split by.</param>
        /// <returns>Splitted array.</returns>
        public static string[] Split(string source, char separator)
        {
#pragma warning disable CA1062 // Validate arguments of public methods
            return source.Split(separator);
#pragma warning restore CA1062 // Validate arguments of public methods
        }
    }
}
