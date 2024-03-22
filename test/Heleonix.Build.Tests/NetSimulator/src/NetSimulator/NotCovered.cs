// <copyright file="NotCovered.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace NetSimulator
{
    public class NotCovered
    {
        private int original;

        public NotCovered(int original)
        {
            this.original = original;
        }

        public void Increase(int increment)
        {
            this.original = this.original + increment;
        }
    }
}
