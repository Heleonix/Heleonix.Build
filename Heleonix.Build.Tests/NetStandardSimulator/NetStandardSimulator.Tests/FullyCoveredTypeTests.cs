// <copyright file="FullyCoveredTypeTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace NetStandardSimulator
{
    using NUnit.Framework;

    /// <summary>
    /// Tests the <see cref="FullyCoveredType"/>.
    /// </summary>
    public static class FullyCoveredTypeTests
    {
        /// <summary>
        /// Tests the <see cref="FullyCoveredType.PlusOne"/>.
        /// </summary>
        [Test]
        public static void PlusOne()
        {
            FullyCoveredType.PlusOne(12);
        }
    }
}
