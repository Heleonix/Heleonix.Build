// <copyright file="BaseTask.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) 2016-present Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tasks
{
    using System;
    using Heleonix.Build.Properties;
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
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                this.Log.LogErrorFromException(ex);

                this.Log.LogError(Resources.TaskFailed, this.GetType().Name);
            }

            return !this.Log.HasLoggedErrors;
        }

        /// <summary>
        /// When overridden in a derived class, executes the task.
        /// </summary>
        protected abstract void ExecuteInternal();
    }
}
