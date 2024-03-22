// <copyright file="ProgramTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests;

[ComponentTest(Type = typeof(Program))]

public static class ProgramTests
{
    [MemberTest(Name = nameof(Program.Main))]

    public static void MainTests()
    {
        string[] args = null;
        string outptut = null;
        string errors = null;
        var returnValue = 0;

        Act(() =>
        {
            using var outWriter = new StringWriter();
            using var errorWriter = new StringWriter();

            Console.SetOut(outWriter);
            Console.SetError(errorWriter);

            returnValue = Program.Main(args);

            outptut = outWriter.ToString();
            errors = errorWriter.ToString();
        });

        When("the help command is called", () =>
        {
            And("the short form is specified", () =>
            {
                args = new string[] { "-h" };

                Should("display information about the tool", () =>
                {
                    Assert.That(returnValue, Is.Zero);
                    Assert.That(outptut, Contains.Substring("Description:"));
                    Assert.That(outptut, Contains.Substring("Version:"));
                    Assert.That(outptut, Contains.Substring("More:"));
                    Assert.That(outptut, Contains.Substring("Options:"));
                    Assert.That(outptut, Contains.Substring("Examples:"));
                });
            });

            And("the long form is specified", () =>
            {
                args = new string[] { "--help" };

                Should("display information about the tool", () =>
                {
                    Assert.That(returnValue, Is.Zero);
                    Assert.That(outptut, Contains.Substring("Description:"));
                    Assert.That(outptut, Contains.Substring("Version:"));
                    Assert.That(outptut, Contains.Substring("More:"));
                    Assert.That(outptut, Contains.Substring("Options:"));
                    Assert.That(outptut, Contains.Substring("Examples:"));
                });
            });
        });

        When("the '--exe' value is not provided", () =>
        {
            args = new string[] { "--exe" };

            Should("fail", () =>
            {
                Assert.That(returnValue, Is.EqualTo(1));
                Assert.That(
                    errors,
                    Contains.Substring("The dotnet executable path is not defined for the specified argument '--exe'."));
            });
        });

        When("the --exe value is provided", () =>
        {
            args = new string[] { "--exe", "dotnet.exe", "-p:Hx_WS_RepositoryUrl=https://example.com" };

            Should("run the provided dotnet executable with passed command-line arguments", () =>
            {
                Assert.That(returnValue, Is.Zero);
                Assert.That(outptut, Contains.Substring("https://example.com"));
            });
        });

        When("the default dotnet.exe is used", () =>
        {
            args = new string[] { "-p:Hx_WS_RepositoryUrl=https://example.com" };

            Should("run the default dotnet executable with passed command-line arguments", () =>
            {
                Assert.That(returnValue, Is.Zero);
                Assert.That(outptut, Contains.Substring("https://example.com"));
            });
        });
    }
}
