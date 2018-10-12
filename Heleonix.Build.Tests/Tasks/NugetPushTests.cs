// <copyright file="NugetPushTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks
{
    using System.IO;
    using Heleonix.Build.Tasks;
    using Heleonix.Build.Tests.Common;
    using Heleonix.Execution;
    using Heleonix.Testing.NUnit.Aaa;
    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;
    using NUnit.Framework;
    using static Heleonix.Testing.NUnit.Aaa.AaaSpec;

    /// <summary>
    /// Tests the <see cref="NugetPush"/>.
    /// </summary>
    [ComponentTest(Type = typeof(NugetPush))]
    public static class NugetPushTests
    {
        /// <summary>
        /// Tests the <see cref="NugetPush.ExecuteInternal"/>.
        /// </summary>
        [MemberTest(Name = nameof(NugetPush.Execute))]
        public static void Execute()
        {
            NugetPush task = null;
            var succeeded = false;
            ITaskItem packageFile = null;
            ITaskItem sourceURL = null;
            ITaskItem configFile = null;
            string sourceDir = null;

            Arrange(() =>
            {
                sourceDir = PathHelper.GetRandomFileInCurrentDir();

                Directory.CreateDirectory(sourceDir);

                ExeHelper.Execute(PathHelper.NugetExe, $"init \"{sourceDir}\"");
            });

            Act(() =>
            {
                task = new NugetPush
                {
                    BuildEngine = new TestBuildEngine(),
                    ConfigFile = configFile,
                    PackageFile = packageFile,
                    SourceURL = sourceURL,
                    NugetExe = new TaskItem(PathHelper.NugetExe)
                };

                succeeded = task.Execute();
            });

            Teardown(() =>
            {
                Directory.Delete(sourceDir, true);
            });

            When("package file is specified", () =>
            {
                Arrange(() =>
                {
                    packageFile = new TaskItem(NetStandardSimulatorPathHelper.NupkgFile);
                });

                And("config file is specified and sourceURL is not specified", () =>
                {
                    Arrange(() =>
                    {
                        configFile = new TaskItem(PathHelper.GetRandomFileInCurrentDir());

                        using (var file = File.CreateText(configFile.ItemSpec))
                        {
                            file.WriteLine(
                                "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
                                + $"<configuration>"
                                    + "<config>"
                                        + $"<add key=\"defaultPushSource\" value=\"{sourceDir}\"/>"
                                    + "</config>"
                                    + "<packageSources>"
                                        + $"<add key=\"Hxb-Test\" value=\"{sourceDir}\"/>"
                                    + "</packageSources>"
                                + "</configuration>");
                        }

                        sourceURL = null;
                    });

                    Teardown(() =>
                    {
                        File.Delete(configFile.ItemSpec);
                    });

                    Should("succeed", () =>
                    {
                        Assert.That(succeeded, Is.True);
                        Assert.That(
                            File.Exists(Path.Combine(sourceDir, Path.GetFileName(NetStandardSimulatorPathHelper.NupkgFile))),
                            Is.True);
                    });
                });

                And("config file is not specified and sourceURL is specified", () =>
                {
                    Arrange(() =>
                    {
                        configFile = null;
                        sourceURL = new TaskItem(sourceDir);
                    });

                    Should("succeed", () =>
                    {
                        Assert.That(succeeded, Is.True);
                        Assert.That(
                            File.Exists(Path.Combine(sourceDir, Path.GetFileName(NetStandardSimulatorPathHelper.NupkgFile))),
                            Is.True);
                    });
                });
            });

            When("package file is not provided", () =>
            {
                Arrange(() =>
                {
                    packageFile = new TaskItem(PathHelper.GetRandomFileInCurrentDir());
                });

                Should("fail", () =>
                {
                    Assert.That(succeeded, Is.False);
                    Assert.That(
                        File.Exists(Path.Combine(sourceDir, Path.GetFileName(NetStandardSimulatorPathHelper.NupkgFile))),
                        Is.False);
                });
            });
        }
    }
}
