// <copyright file="ToolHelper.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Common;

using NUnit.Framework.Internal;

public static class ToolHelper
{
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

        if (result.ExitCode != 0)
        {
            TestExecutionContext.CurrentContext.OutWriter.WriteLine(result.Output);

            return false;
        }

        return true;
    }
}
