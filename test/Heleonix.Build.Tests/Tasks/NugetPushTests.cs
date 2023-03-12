// <copyright file="NugetPushTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

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
        string packageFile = null;
        string sourceURL = null;
        string configFile = null;
        string sourceDir = null;

        Arrange(() =>
        {
            sourceDir = PathHelper.GetRandomFileNameInCurrentDir();

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
                NugetExe = PathHelper.NugetExe,
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
                packageFile = PathHelper.NugetPackageFile;
            });

            And("config file is specified and sourceURL is not specified", () =>
            {
                Arrange(() =>
                {
                    configFile = PathHelper.GetRandomFileNameInCurrentDir();

                    using (var file = File.CreateText(configFile))
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
                    File.Delete(configFile);
                });

                Should("succeed", () =>
                {
                    Assert.That(succeeded, Is.True);
                    Assert.That(
                        File.Exists(Path.Combine(sourceDir, Path.GetFileName(PathHelper.NugetPackageFile))),
                        Is.True);
                });
            });

            And("config file is not specified and sourceURL is specified", () =>
            {
                Arrange(() =>
                {
                    configFile = null;
                    sourceURL = sourceDir;
                });

                Should("succeed", () =>
                {
                    Assert.That(succeeded, Is.True);
                    Assert.That(
                        File.Exists(Path.Combine(sourceDir, Path.GetFileName(PathHelper.NugetPackageFile))),
                        Is.True);
                });
            });
        });

        When("package file is not provided", () =>
        {
            Arrange(() =>
            {
                packageFile = PathHelper.GetRandomFileNameInCurrentDir();
            });

            Should("fail", () =>
            {
                Assert.That(succeeded, Is.False);
                Assert.That(
                    File.Exists(Path.Combine(sourceDir, Path.GetFileName(PathHelper.NugetPackageFile))),
                    Is.False);
            });
        });
    }
}
