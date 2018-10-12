// <copyright file="NetValidateTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Targets
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Heleonix.Build.Tests.Common;
    using Heleonix.Testing.NUnit.Aaa;
    using NUnit.Framework;
    using static Heleonix.Testing.NUnit.Aaa.AaaSpec;

    /// <summary>
    /// Tests the GitHubReleaseNet target.
    /// </summary>
    [ComponentTest(Type = typeof(NetValidateTests))]
    public static class NetValidateTests
    {
        /// <summary>
        /// Tests the <see cref="NetValidateTests"/>.
        /// </summary>
        [MemberTest(Name = nameof(NetValidateTests))]
        public static void Execute()
        {
            var succeeded = false;

            Act(() =>
            {
                succeeded = MSBuildHelper.RunTestTarget("Hx_Net_Validate", NetStandardSimulatorPathHelper.SolutionDir);
            });

            When("target is executed", () =>
            {
                Should("succeed", () =>
                {
                    Assert.That(succeeded, Is.True);
                });
            });
        }
    }
}
