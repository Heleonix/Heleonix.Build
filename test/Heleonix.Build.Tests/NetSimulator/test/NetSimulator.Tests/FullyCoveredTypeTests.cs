// <copyright file="FullyCoveredTypeTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace NetSimulator
{
    using NUnit.Framework;

    public static class FullyCoveredTypeTests
    {
        [Test]
#pragma warning disable S2699 // Tests should include assertions
        public static void PlusOne()
#pragma warning restore S2699 // Tests should include assertions
        {
            FullyCoveredType.PlusOne(12);
        }
    }
}
