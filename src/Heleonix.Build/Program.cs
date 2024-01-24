// <copyright file="Program.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build;

using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

/// <summary>
/// Thr entry class of the tool.
/// </summary>
public static class Program
{
    /// <summary>
    /// The entry point of the tool.
    /// </summary>
    /// <param name="args">The command line arguments.</param>
    /// <returns>An exit code.</returns>
    public static int Main(string[] args)
    {
        if (args.Length == 1
            && ("--help".Equals(args[0], StringComparison.OrdinalIgnoreCase)
                || "-h".Equals(args[0], StringComparison.OrdinalIgnoreCase)))
        {
            Console.WriteLine("Description:");
            Console.WriteLine("  The MSBuild-based build framework to use in CI/CD systems" +
                              "  Version: " + Assembly.GetExecutingAssembly().GetName().Version);
            Console.WriteLine("  More: https://heleonix.github.io/docs/api/Heleonix.Build/intro.html");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --exe        Optionally specifies a custom installation path to the dotnet.exe to use.");
            Console.WriteLine("  -h|--help    Display help.");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  hxbuild -t:Hx_NetBuild");
            Console.WriteLine("  hxbuild --exe \"C:\\Program Files\\dotnet\\dotnet.exe\" -t:Hx_NetBuild -p:Hx_Run_Number=123");

            return 0;
        }

        var argsBuilder = new StringBuilder();

        string dotnetExePath = null;

        for (int i = 0; i < args.Length; i++)
        {
            if (string.Equals(args[i], "--exe", StringComparison.OrdinalIgnoreCase))
            {
                if ((i + 1) >= args.Length)
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.Error.WriteLine("The dotnet executable path is not defined for the specified argument '-exe'.");

                    return 1;
                }

                dotnetExePath = args[i + 1];

                i += 1;
            }
            else
            {
                argsBuilder.Append(args[i]).Append(' ');
            }
        }

        dotnetExePath ??= "dotnet.exe";

        argsBuilder.Append("-p:Hx_Sys_DotnetExe=").Append($"\"{dotnetExePath}\"");

        var mainProjFile = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
            "Heleonix.Build.hxbproj");

        using (var process = new Process())
        {
            process.StartInfo.FileName = dotnetExePath;
            process.StartInfo.Arguments = $"msbuild \"{mainProjFile}\" -noLogo -t:Hx_Initialize {argsBuilder}";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardOutput = true;

            process.OutputDataReceived += Process_OutputDataReceived;
            process.ErrorDataReceived += Process_ErrorDataReceived;

            process.Start();

            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            process.WaitForExit();

            return process.ExitCode;
        }
    }

    private static void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Red;

        Console.Error.WriteLine(e.Data);
    }

    private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        Console.ResetColor();

        Console.Out.WriteLine(e.Data);
    }
}