// <copyright file="NotCovered.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace NetSimulator
{
    /// <summary>
    /// Represents the class, which is not covered by tests.
    /// </summary>
    public class NotCovered
    {
        /// <summary>
        /// The original value.
        /// </summary>
        private int original;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotCovered"/> class.
        /// </summary>
        /// <param name="original">An original value.</param>
        public NotCovered(int original)
        {
            this.original = original;
        }

        /// <summary>
        /// Increases the original value by the specified <paramref name="increment"/>.
        /// </summary>
        /// <param name="increment">The increment.</param>
        public void Increase(int increment)
        {
            this.original = this.original + increment;
        }
    }
}
