// <copyright file="BaseTaskTests.cs" company="Heleonix - Hennadii Lutsyshyn">
// Copyright (c) Heleonix - Hennadii Lutsyshyn. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the repository root for full license information.
// </copyright>

namespace Heleonix.Build.Tests.Tasks;

[ComponentTest(Type = typeof(BaseTask))]
public static class BaseTaskTests
{
    [MemberTest(Name = nameof(BaseTask.Execute))]
    public static void Execute()
    {
        When("the Execute method throws an exception", () =>
        {
            FailedBaseTask task = null;

            var succeeded = false;

            Arrange(() =>
            {
                task = new FailedBaseTask { BuildEngine = new TestBuildEngine() };
            });

            Act(() =>
            {
                succeeded = task.Execute();
            });

            Should("fail", () =>
            {
                Assert.That(succeeded, Is.False);
            });
        });

        When("the Execute method does not throw an exception", () =>
        {
            SuccessBaseTask task = null;

            var succeeded = false;

            Arrange(() =>
            {
                task = new SuccessBaseTask();
            });

            Act(() =>
            {
                succeeded = task.Execute();
            });

            Should("succeed", () =>
            {
                Assert.That(succeeded, Is.True);
            });
        });
    }

    private class FailedBaseTask : BaseTask
    {
        protected override void ExecuteInternal()
        {
            throw new NotImplementedException();
        }
    }

    private class SuccessBaseTask : BaseTask
    {
        protected override void ExecuteInternal()
        {
        }
    }
}
