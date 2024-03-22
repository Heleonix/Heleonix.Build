// <copyright file="TestBuildEngine.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Common;

using System.Collections;

public class TestBuildEngine : IBuildEngine
{
    public List<string> ErrorMessages { get; } = new List<string>();

    public List<string> Messages { get; } = new List<string>();

    public bool ContinueOnError => true;

    public int LineNumberOfTaskNode => 0;

    public int ColumnNumberOfTaskNode => 0;

    public string ProjectFileOfTaskNode => string.Empty;

    public void LogErrorEvent(BuildErrorEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Message))
        {
            this.ErrorMessages.Add(e.Message);
        }
    }

    public void LogWarningEvent(BuildWarningEventArgs e)
    {
        // Dummy implementation.
    }

    public void LogMessageEvent(BuildMessageEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Message))
        {
            this.Messages.Add(e.Message);
        }
    }

    public void LogCustomEvent(CustomBuildEventArgs e)
    {
        // Dummy implementation.
    }

    public bool BuildProjectFile(
        string projectFileName,
        string[] targetNames,
        IDictionary globalProperties,
        IDictionary targetOutputs) => true;
}
