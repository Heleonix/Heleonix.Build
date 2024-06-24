// <copyright file="Hx_NetSetupToolTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

[ComponentTest(Type = typeof(Hx_NetSetupTool))]

public static class Hx_NetSetupToolTests
{
    [MemberTest(Name = nameof(Hx_NetSetupTool.Execute))]
    public static void Execute()
    {
        Hx_NetSetupTool task = null;
        string name = null;
        string packageName = null;
        string version = null;
        var isPackage = false;
        var succeeded = false;
        TestBuildEngine buildEngine = null;

        Act(() =>
        {
            buildEngine = new TestBuildEngine();

            task = new Hx_NetSetupTool
            {
                BuildEngine = buildEngine,
                Name = name,
                PackageName = packageName,
                IsPackage = isPackage,
                Version = version,
                DotnetExe = PathHelper.ExeMockFile,
            };

            succeeded = task.Execute();
        });

        When("the task is executed", () =>
        {
            And("the tool does not exist", () =>
            {
                And("git tool is requested", () =>
                {
                    name = "git";

                    Should("succeed", () =>
                    {
                        Assert.That(succeeded);
                        Assert.That(task.ToolPath, Is.EqualTo("git.exe"));
                    });
                });

                And("reportgenerator is requested", () =>
                {
                    name = "reportgenerator";

                    Should("succeed", () =>
                    {
                        Assert.That(succeeded);
                        Assert.That(
                            buildEngine.Messages,
                            Contains.Item(
                                "tool install dotnet-reportgenerator-globaltool" +
                                " --version 5.2.0 --tool-path " + Path.Combine(PathHelper.CurrentDir, "Tools")));
                        Assert.That(
                            task.ToolPath,
                            Is.EqualTo(Path.Combine(PathHelper.CurrentDir, "Tools", "reportgenerator.exe")));
                    });
                });

                And("docfx is requested", () =>
                {
                    name = "docfx";

                    Should("succeed", () =>
                    {
                        Assert.That(succeeded);
                        Assert.That(
                            buildEngine.Messages,
                            Contains.Item(
                                "tool install docfx" +
                                " --version 2.75.2 --tool-path " + Path.Combine(PathHelper.CurrentDir, "Tools")));
                        Assert.That(
                            task.ToolPath,
                            Is.EqualTo(Path.Combine(PathHelper.CurrentDir, "Tools", "docfx.exe")));
                    });
                });

                And("extent is requested", () =>
                {
                    name = "extent";

                    Should("succeed", () =>
                    {
                        Assert.That(succeeded);
                        Assert.That(
                            buildEngine.Messages,
                            Contains.Item(
                                "add package extent" +
                                " --version 0.0.3 --package-directory " + Path.Combine(PathHelper.CurrentDir, "Tools")));
                        Assert.That(
                            task.ToolPath,
                            Is.EqualTo(Path.Combine(PathHelper.CurrentDir, "Tools", "extent", "0.0.3")));
                    });
                });

                And("hxreport is requested", () =>
                {
                    name = "hxreport";

                    Should("succeed", () =>
                    {
                        Assert.That(succeeded);
                        Assert.That(
                            buildEngine.Messages,
                            Contains.Item(
                                "tool install Heleonix.Testing.Reporting" +
                                " --version 1.0.2 --tool-path " + Path.Combine(PathHelper.CurrentDir, "Tools")));
                        Assert.That(
                            task.ToolPath,
                            Is.EqualTo(Path.Combine(PathHelper.CurrentDir, "Tools", "hxreport.exe")));
                    });
                });

                And("NunitXml.TestLogger is requested", () =>
                {
                    name = "NunitXml.TestLogger";

                    Should("succeed", () =>
                    {
                        Assert.That(succeeded);
                        Assert.That(
                            buildEngine.Messages,
                            Contains.Item(
                                "add package NunitXml.TestLogger" +
                                " --version 3.1.15 --package-directory " + Path.Combine(PathHelper.CurrentDir, "Tools")));
                        Assert.That(
                            task.ToolPath,
                            Is.EqualTo(Path.Combine(PathHelper.CurrentDir, "Tools", "NunitXml.TestLogger", "3.1.15")));
                    });
                });
            });

            And("the package tool does not exist", () =>
            {
                name = "SomeTool";
                isPackage = true;

                And("no versions are installed", () =>
                {
                    Arrange(() =>
                    {
                        Directory.CreateDirectory(Path.Combine(PathHelper.CurrentDir, "Tools", name));
                    });

                    Teardown(() =>
                    {
                        Directory.Delete(Path.Combine(PathHelper.CurrentDir, "Tools", name), true);
                    });

                    Should("succeed and install the latest version", () =>
                    {
                        Assert.That(succeeded);
                        Assert.That(
                            buildEngine.Messages,
                            Contains.Item(
                                "add package SomeTool" +
                                " --package-directory " + Path.Combine(PathHelper.CurrentDir, "Tools")));
                        Assert.That(task.ToolPath, Is.Null);
                    });
                });

                And("there is no package folder", () =>
                {
                    Should("succeed and install the latest version", () =>
                    {
                        Assert.That(succeeded);
                        Assert.That(
                            buildEngine.Messages,
                            Contains.Item(
                                "add package SomeTool" +
                                " --package-directory " + Path.Combine(PathHelper.CurrentDir, "Tools")));
                        Assert.That(task.ToolPath, Is.Null);
                    });
                });
            });

            And("the dotnet tool exists", () =>
            {
                isPackage = false;

                Arrange(() =>
                {
                    name = "docfx";

                    File.WriteAllText(Path.Combine(PathHelper.CurrentDir, "Tools", "docfx.exe"), "dummy exe");
                });

                Teardown(() =>
                {
                    File.Delete(Path.Combine(PathHelper.CurrentDir, "Tools", "docfx.exe"));
                });

                Should("succeed", () =>
                {
                    Assert.That(succeeded);
                    Assert.That(buildEngine.Messages, Is.Empty);
                    Assert.That(task.ToolPath, Is.EqualTo(Path.Combine(PathHelper.CurrentDir, "Tools", "docfx.exe")));
                });
            });

            And("the package tool exists", () =>
            {
                isPackage = true;

                And("specific version is requested", () =>
                {
                    Arrange(() =>
                    {
                        name = "SomeTool";
                        version = "1.0.0";

                        Directory.CreateDirectory(Path.Combine(PathHelper.CurrentDir, "Tools", name, version));
                    });

                    Teardown(() =>
                    {
                        Directory.Delete(Path.Combine(PathHelper.CurrentDir, "Tools", name), true);
                    });

                    Should("succeed", () =>
                    {
                        Assert.That(succeeded);
                        Assert.That(buildEngine.Messages, Is.Empty);
                        Assert.That(task.ToolPath, Is.EqualTo(Path.Combine(PathHelper.CurrentDir, "Tools", name, version)));
                    });
                });

                And("version is not requested", () =>
                {
                    name = "SomeTool";
                    version = null;

                    And("several versions are installed", () =>
                    {
                        Arrange(() =>
                        {
                            Directory.CreateDirectory(Path.Combine(PathHelper.CurrentDir, "Tools", name, "1.0.0"));
                            Directory.CreateDirectory(Path.Combine(PathHelper.CurrentDir, "Tools", name, "1.0.1"));
                        });

                        Teardown(() =>
                        {
                            Directory.Delete(Path.Combine(PathHelper.CurrentDir, "Tools", name), true);
                        });

                        Should("succeed and provide the latest version", () =>
                        {
                            Assert.That(succeeded);
                            Assert.That(buildEngine.Messages, Is.Empty);
                            Assert.That(task.ToolPath, Is.EqualTo(Path.Combine(PathHelper.CurrentDir, "Tools", name, "1.0.1")));
                        });
                    });
                });
            });

            And("installation fails", () =>
            {
                name = "ERROR";
                isPackage = true;

                Should("fail", () =>
                {
                    Assert.That(!succeeded);
                    Assert.That(buildEngine.ErrorMessages, Contains.Item("ERROR details"));
                    Assert.That(buildEngine.Messages, Contains.Item("ERROR simulated"));
                    Assert.That(
                        buildEngine.Messages,
                        Contains.Item($"add package ERROR --package-directory \"{Path.Combine(PathHelper.CurrentDir, "Tools")}\""));
                });
            });
        });
    }
}
