// <copyright file="ToolHelper.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Common;

using NUnit.Framework.Internal;

/// <summary>
/// Provides functionality to work with MSBuild.
/// </summary>
public static class ToolHelper
{
    /// <summary>
    /// Runs a test target.
    /// </summary>
    /// <param name="target">A target to run.</param>
    /// <param name="workspace">A workspace to run the target in.</param>
    /// <param name="properties">Properties of the target to override or define.</param>
    /// <returns><c>true</c> in case of success, otherwise <c>false</c>.</returns>
    public static bool RunTestTarget(string target, string workspace, IDictionary<string, string> properties)
    {
        var args = ArgsBuilder.By("-", ":")
            .AddArgument("t", target)
            .AddArgument("p", $"Hx_Run_BuildProjFile=\"{PathHelper.RunBuildProj}\"")
            .AddArgument("p", $"Hx_Run_Configuration={PathHelper.Configuration}");

        if (properties != null)
        {
            foreach (var prop in properties)
            {
                args.AddArgument("p", $"{prop.Key}={prop.Value}");
            }
        }

        var result = ExeHelper.Execute(PathHelper.HxBuildExe, args, true, workspace);

        TestExecutionContext.CurrentContext.OutWriter.WriteLine(result.Output);

        if (result.ExitCode != 0)
        {
            return false;
        }

        return true;
    }
}
