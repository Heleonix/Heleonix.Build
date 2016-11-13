using System.IO;
using Heleonix.Build.Tasks;
using Heleonix.Build.Tests.Common;
using Microsoft.Build.Utilities;
using NUnit.Framework;

namespace Heleonix.Build.Tests.Tasks
{
    /// <summary>
    /// Tests the <see cref="NugetRestore"/>.
    /// </summary>
    public class NugetRestoreTests
    {
        #region Tests

        /// <summary>
        /// Tests the <see cref="NugetRestore.Execute"/>.
        /// </summary>
        [Test]
        public void Execute()
        {
            var packagesDir = Path.Combine(LibSimulatorHelper.SolutionDir, "packages");

            if (Directory.Exists(packagesDir))
            {
                Directory.Delete(packagesDir, true);
            }

            try
            {
                var task = new NugetRestore
                {
                    BuildEngine = new FakeBuildEngine(),
                    NugetExeFile = new TaskItem(PathHelper.NugetExe),
                    MsBuildVersion = MsBuildHelper.MsBuildVersion,
                    SolutionFile = new TaskItem(LibSimulatorHelper.Solution),
                    Verbosity = "detailed",
                    PackagesDir = new TaskItem(packagesDir)
                };

                var succeeded = task.Execute();

                Assert.That(succeeded, Is.True);

                Assert.That(Directory.Exists(packagesDir), Is.True);
                Assert.That(Directory.GetDirectories(packagesDir), Is.Not.Empty);
            }
            finally
            {
                if (Directory.Exists(packagesDir))
                {
                    Directory.Delete(packagesDir, true);
                }
            }
        }

        #endregion
    }
}