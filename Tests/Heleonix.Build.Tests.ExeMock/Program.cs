using System;
using System.IO;

namespace Heleonix.Build.Tests.ExeMock
{
    /// <summary>
    /// The class of the entry point.
    /// </summary>
    internal class Program
    {
        #region Methods

        /// <summary>
        /// The entry point.
        /// </summary>
        private static int Main()
        {
            using (var cfg = File.OpenText(Path.ChangeExtension(typeof(Program).Assembly.Location, ".mock")))
            {
                var exitCode = Convert.ToInt32(cfg.ReadLine());
                var output = cfg.EndOfStream ? null : cfg.ReadLine();
                var errorOutput = cfg.EndOfStream ? null : cfg.ReadLine();

                if (!string.IsNullOrEmpty(output))
                {
                    Console.Write(output);
                }

                if (!string.IsNullOrEmpty(errorOutput))
                {
                    Console.Error.Write(errorOutput);
                }

                return exitCode;
            }
        }

        #endregion
    }
}