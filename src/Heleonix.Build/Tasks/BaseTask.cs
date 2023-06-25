// <copyright file="BaseTask.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks;

using Microsoft.Build.Utilities;

/// <summary>
/// The <c>base</c> task.
/// </summary>
/// <seealso cref="Task" />
public abstract class BaseTask : Task
{
    /// <summary>
    /// When overridden in a derived class, executes the task.
    /// </summary>
    /// <returns><c>true</c> if the task successfully executed; otherwise, <c>false</c>.</returns>
    public sealed override bool Execute()
    {
        try
        {
            this.ExecuteInternal();
        }
        catch (Exception ex)
        {
            this.Log.LogError(Resources.TaskFailed, this.GetType().FullName);

            this.Log.LogErrorFromException(ex, true, true, ex.Source);
        }

        return !this.Log.HasLoggedErrors;
    }

    /// <summary>
    /// When overridden in a derived class, executes the task.
    /// </summary>
    protected abstract void ExecuteInternal();
}
